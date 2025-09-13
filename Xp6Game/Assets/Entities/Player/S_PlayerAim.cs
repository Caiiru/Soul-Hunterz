using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerAim : MonoBehaviour
{
    public Transform aimTransform;

    InputAction _mousePositionAction;
    void Start()
    {
        _mousePositionAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mousePos = Input.mousePosition;


        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 100, LayerMask.GetMask("Ground")))
        {
            // aimTransform.position = hitInfo.point;
            aimTransform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);

            Vector3 rotatePosition = new Vector3(aimTransform.position.x, transform.position.y, aimTransform.position.z);
            transform.LookAt(rotatePosition);
            Debug.DrawLine(transform.position, aimTransform.position, Color.red);
        }

    }
}
