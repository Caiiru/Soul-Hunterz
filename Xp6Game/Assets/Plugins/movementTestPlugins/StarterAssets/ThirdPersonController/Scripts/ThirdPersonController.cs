using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        public bool isAlwaysRunning = true;
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Header("Player Aim")]
        public Transform aimTransform;
        public Transform playerMesh;

        public Vector3 aimDirection;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        // [Space(10)]
        // [Tooltip("The height the player can jump")]
        // float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Player Dash")]
        public bool CanDash;
        public float DashForce = 1;

        [Tooltip("Time required to dash again")]
        public float DashTimeout = 1f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _dashTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDDash;
        private int _animIDMotionSpeed;
        private int _animIDIsMoving;
        // private int _animIDSpeedX;
        // private int _animIDSpeedZ;
        private int _animIDVelocity;
        private int _animIDDead;

        private const int k_DashReduction = 100;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;


        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        //Events
        public delegate void OnDashEvent();
        public static OnDashEvent onPlayerDash;

        

        #region Start
        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _animator = GetComponentInChildren<Animator>();
            if (_animator != null)
            {
                _hasAnimator = true;
            }

            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs(); 

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _dashTimeoutDelta = DashTimeout;
        }
        #endregion

        private void Update()
        {
            // _hasAnimator = TryGetComponent(out _animator);

            // JumpAndGravity();
            HandleGravity();
            Dash();
            LookAtMouse();
            GroundedCheck();
            Move();

        }

        private void LateUpdate()
        {
            CameraRotation();
        } 

        #region Anim Bind
        private void AssignAnimationIDs()
        {
            // _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            // _animIDSpeedX = Animator.StringToHash("SpeedX");
            // _animIDSpeedZ = Animator.StringToHash("SpeedZ");
            // _animIDIsMoving = Animator.StringToHash("isMoving");
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDDash = Animator.StringToHash("isDashing");
            _animIDVelocity = Animator.StringToHash("Velocity");
            _animIDDead = Animator.StringToHash("isDead");
        }

        #endregion

        #region Ground Check
        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                // _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        #endregion
        #region Camera Rotation
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
        #endregion
        #region Move

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed;
            if (isAlwaysRunning)
                targetSpeed = SprintSpeed;
            else
            {

                targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                // update animator if using character
                if (_hasAnimator)
                {
                    // _animator.SetBool(_animIDIsMoving, false);
                }

            }
            else
            {
                // if (_hasAnimator)
                //     _animator.SetBool(_animIDIsMoving, true);
            }


            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;


            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude

            Vector3 forwardDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 leftDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.left;
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                // // _targetRotation = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg +
                // //     _mainCamera.transform.eulerAngles.y;
                // // float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                // //     RotationSmoothTime);

                float rot = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, rot, ref _rotationVelocity,
                                  RotationSmoothTime);
                // // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            // Vector3 moveDirection = (forwardDirection.normalized * inputDirection.z) + (rightDirection.normalized * inputDirection.x);

            Vector3 moveDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 animMoveDirection = (forwardDirection.normalized * inputDirection.z) + (leftDirection.normalized * inputDirection.x);

            //Debug.DrawRay(transform.position, animMoveDirection * 5, Color.rebeccaPurple);
            // Quaternion animRotation = transform.rotation;
            // if (inputDirection.x > 0f)
            // {
            //     animRotation = Quaternion.Euler(0, 90, 0);
            // }
            // else if (inputDirection.x < 0f)
            // {
            //     animRotation = Quaternion.Euler(0, -90, 0);

            // }
            // if (inputDirection.z > 0f)
            // {
            //     animRotation = Quaternion.Euler(0, 0, 0);
            // }
            // else if (inputDirection.z < 0f)
            // {
            //     animRotation = Quaternion.Euler(0, 180, 0);

            // }
            // transform.rotation = Quaternion.Slerp(transform.rotation, animRotation , Time.deltaTime * 10f);
            // move the player
            _controller.Move(moveDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                // _animator.SetFloat(_animIDSpeed, _animationBlend);

                // _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                // _animator.SetFloat(_animIDSpeedX, animMoveDirection.normalized.x);
                // _animator.SetFloat(_animIDSpeedZ, animMoveDirection.normalized.z);
                _animator.SetFloat(_animIDSpeed, _speed);
                // _animator.SetFloat(_animIDVelocity, _animationBlend);
            }


        }
        #endregion
        #region Dash
        private void Dash()
        {
            if (_input.jump && _dashTimeoutDelta <= 0.0f)
            {

                Vector3 dashDirection = Vector3.zero;
                // Grounded = false;
                if (_input.move != Vector2.zero)
                {
                    dashDirection = new Vector3(_input.move.x, 0f, _input.move.y);
                }
                else
                {
                    dashDirection = transform.forward * -1f; // do back dash
                }

                CanDash = false;
                _dashTimeoutDelta = DashTimeout;


                if (_hasAnimator)
                {
                    _animator.SetTrigger(_animIDDash);
                }

                //Call Event
                onPlayerDash?.Invoke();

                //Dash 
                _controller.Move(dashDirection.normalized * (DashForce / k_DashReduction));



            }
            else if (_dashTimeoutDelta <= 0.0f)
            {
                CanDash = true;
            }
            else
            {
                _dashTimeoutDelta -= Time.deltaTime;
            }
        }
        #endregion
        #region Look At Mouse

        private void LookAtMouse()
        {
            // return; 
            Vector2 mousePos = Input.mousePosition;

            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 100, GroundLayers))
            {
                // aimTransform.position = hitInfo.point;
                aimDirection = (hitInfo.point - transform.position).normalized;
                aimTransform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);

                // Vector3 rotatePosition = new Vector3(aimTransform.position.x, transform.position.y, aimTransform.position.z);
                // transform.LookAt(rotatePosition);
                // playerMesh.LookAt(rotatePosition);

                _targetRotation = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                // _targetRotation = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg +
                //     _mainCamera.transform.eulerAngles.y;
                // float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                //     RotationSmoothTime);

                float rot = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, rot, ref _rotationVelocity,
                                  RotationSmoothTime);
                // // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

        }
        #endregion
        #region Gravity
        private void HandleGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDDash, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }


            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        // _animator.SetBool(_animIDFreeFall, true);
                    }
                }

            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        #endregion

        #region Aux

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }
        #endregion
        #region Sounds

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        #endregion
    }
}