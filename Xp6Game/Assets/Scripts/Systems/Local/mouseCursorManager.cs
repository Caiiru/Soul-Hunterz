using UnityEngine;

public class mouseCursorManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
