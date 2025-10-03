using UnityEngine;
using UnityEngine.InputSystem;

public class Aim : MonoBehaviour
{
    public Transform aimTransform;
    private Camera playerCamera;
    [SerializeField]
    private Transform playerMesh;
    void Start()
    {
        // playerCamera = transform.GetComponentInChildren<Camera>();
        playerCamera = Camera.main;
        // _mousePositionAction = InputSystem.actions.FindAction("Look");
        if (playerMesh == null)
            playerMesh = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 mousePos = Input.mousePosition;


        Ray mouseRay = playerCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, 100, LayerMask.GetMask("Ground")))
        {
            // aimTransform.position = hitInfo.point;
            aimTransform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);

            Vector3 rotatePosition = new Vector3(aimTransform.position.x, transform.position.y, aimTransform.position.z);
            // transform.LookAt(rotatePosition);
            playerMesh.LookAt(rotatePosition);
            Debug.DrawLine(transform.position, aimTransform.position, Color.red);
        }

    }
}
