using CMF;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] bool isInventoryOpen = false;

    [SerializeField] Canvas inventoryCanvas;

    private CharacterInput _characterInput;

    public IModifier[] modifiers = new IModifier[10];

    #region Events

    public delegate void PlayerInventoryHandler(bool isOpen);
    public static event PlayerInventoryHandler OnPlayerInventoryToggle;


    #endregion
    void Start()
    {

        // inventoryCanvas = GetComponent<Canvas>();
        _characterInput = GetComponent<CharacterInput>();
        isInventoryOpen = true;
        ToggleInventory();

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (_characterInput.IsInventoryKeyPressed())
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        // Update UI accordingly

        inventoryCanvas.enabled = isInventoryOpen;
        OnPlayerInventoryToggle?.Invoke(isInventoryOpen);
    }
}
