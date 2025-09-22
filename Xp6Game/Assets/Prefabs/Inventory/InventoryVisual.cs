using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    public Transform weaponsPanel;
    public Transform componentsPanel;

    private Canvas _canvas;

    void Start()
    {
        _canvas = GetComponent<Canvas>();
        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {

    }
    void OnDisable()
    {

        PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;
    }
    private void HandleInventoryToggle(bool isOpen)
    {
        _canvas.enabled = isOpen;
    }
}
