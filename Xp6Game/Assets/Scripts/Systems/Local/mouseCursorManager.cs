using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class CursorController : MonoBehaviour
{

     public Texture2D cursor;

     public Texture2D cursorClicked;

     public InputActionReference Click;


     private void Awake() {
    
         ChangeCursor(cursor);
         Cursor.lockState = CursorLockMode.Confined;
   }



    private void Start() {
        Click.action.performed += _ => StartedClick();
        Click.action.canceled += _ => EndedClick();
    }

    private void StartedClick() {
        ChangeCursor(cursorClicked);
    }

    private void EndedClick() {
        ChangeCursor(cursor);
        // Debug.Log("Cursor Released");
    }



    private void ChangeCursor(Texture2D cursorType) {
        Vector2 hotSpot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotSpot, CursorMode.Auto);
   }
}