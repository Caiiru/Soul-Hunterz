using UnityEngine;

public class S_CameraController : MonoBehaviour
{

    [SerializeField, Range(0.05f, 0.15f)]
    float _cameraTime = 0.2f;

    [SerializeField]
    GameObject _targetObject;

    [SerializeField]
    Vector3 _offset;


    Transform _localTransform;
    void Start()
    {
        _localTransform = this.transform;
    }

    void FixedUpdate()
    {
        if (!_targetObject)
        {
            return;
        }

        Vector3 targetPosition = _targetObject.transform.position;

        this.transform.LookAt(Vector3.Lerp(transform.position, targetPosition, _cameraTime));

        this.transform.position = Vector3.Lerp(this.transform.position, _targetObject.transform.position + _offset, _cameraTime);

    }

    public void RotateTowardPosition(Vector3 _position, float _lookSpeed)
    {
        Vector3 _direction = _position - _localTransform.position;

        RotateTowardDirection(_direction, _lookSpeed);
    }

    public void RotateTowardDirection(Vector3 _direction, float _lookSpeed)
    {

        _direction.Normalize();

        _direction = _localTransform.parent.InverseTransformDirection(_direction);


        Vector3 _currentLookVector = _localTransform.parent.InverseTransformDirection(Input.mousePosition);
    }
}
