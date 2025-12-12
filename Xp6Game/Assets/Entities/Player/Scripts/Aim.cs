using UnityEngine; 

public class Aim : MonoBehaviour
{
    public Transform aimTransform;
    [SerializeField]private Camera playerCamera;
    [SerializeField]private Transform playerMesh;
    void Start()
    {
        // playerCamera = transform.GetComponentInChildren<Camera>();
        playerCamera = Camera.main; 
        // _mousePositionAction = InputSystem.actions.FindAction("Look");
        if (playerMesh == null)
            playerMesh = transform.GetChild(0);
    }

    // Update is called once per frame
     
}
