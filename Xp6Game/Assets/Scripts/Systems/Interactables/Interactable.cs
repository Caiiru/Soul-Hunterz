using UnityEngine;

public interface Interactable
{
    InteractableType GetInteractableType();
    bool CanInteract();
    void Interact();
}

public enum InteractableType
{
    Collectable,Interactable
}
