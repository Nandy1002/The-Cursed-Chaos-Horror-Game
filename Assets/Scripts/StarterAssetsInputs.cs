using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public static StarterAssetsInputs Instance { get; private set; }
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool pickUp;
		public bool click;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
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
		public void OnPickUp(InputValue value)
		{
			PickUpInput(value.isPressed);
		}
		public void OnClick(InputValue value)
		{
			ClickInput(value.isPressed);
		}
		
#endif

		public void ClickInput(bool newClickState)
		{
			click = newClickState;
			// Implement the logic for interact input here
		}
		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
			// Implement the logic for interact input here
		}
		public void PickUpInput(bool newPickUpState)
		{
			pickUp = newPickUpState;
			// Implement the logic for interact input here
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

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	
	}
	
}