using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerWalk : MonoBehaviour
{
    Rigidbody _rigibody;

    [SerializeField] float velocity;

    InputAction _moveAction;
    void Start()
    {
        _rigibody = GetComponent<Rigidbody>();

        _moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {

        Vector3 _moveValue = new Vector3(_moveAction.ReadValue<Vector2>().x, 0, _moveAction.ReadValue<Vector2>().y);
        // Debug.Log(_moveValue);
        this._rigibody.AddForce(_moveValue.normalized * velocity, ForceMode.Force);
        // this._rigibody.AddForce()
    }
}
