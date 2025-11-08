using System; 
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool inventory;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;


		[Header("Actions")]
		[SerializeField] private InputActionReference interactActionReference;
		[SerializeField] private InputActionReference inventoryActionReference;

		#region Events
		public delegate void ChangeWeaponHandler(int slot);
		public static event ChangeWeaponHandler OnChangeWeapon;

		public delegate void PlayerInventoryHandler(bool isOpen);
		public static event PlayerInventoryHandler OnPlayerInventoryToggle;



		#endregion



#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}
		public void OnInventory(InputValue value)
		{
			InventoryInput(value.isPressed);
		}

		public void OnFirstWeapon(InputValue value)
		{
			ChangeWeapon(0);
		}
		public void OnSecondWeapon(InputValue value)
		{

			ChangeWeapon(1);
		}
		public void OnThirdWeapon(InputValue value)
		{

		}


#endif

		public void InventoryInput(bool newInventoryState)
		{
			if (newInventoryState == true)
			{
				inventory = !inventory;
				OnPlayerInventoryToggle?.Invoke(inventory);
			}
		}
		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;

		}
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void ChangeWeapon(int slot)
		{
			OnChangeWeapon?.Invoke(slot);
		}


		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		public InputActionReference GetInteractAction()
		{
			return interactActionReference;
		}


		#region Singleton

		public static StarterAssetsInputs Instance;
		void Awake()
		{
			Instance = this;

		}
		#endregion




	}

}