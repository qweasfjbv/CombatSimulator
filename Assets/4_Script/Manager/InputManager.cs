using Defense.Controller;
using Defense.Debugger;
using Defense.InputActions;
using Defense.Props;
using Defense.Utils;
using System.Net;
using UnityEngine;

namespace Defense.Manager
{
	public class InputManager
	{
		private PlayerControl control;
		private CameraController cameraController;

		public void Init()
		{
			control = new();
			control.Enable();

			cameraController = Camera.main.GetComponent<CameraController>();
		}
		private GameInputState currentInputState = GameInputState.Main;
		public void OnUpdate()
		{
			SetInputMode();

			switch (currentInputState) {
				case GameInputState.PrePlay:
					HandleSetupInput();
					break;
				case GameInputState.Playing:
					HandlePlayingInput();
					break;
			}
		}

		private PlacementSlot selectedSlot = null;
		private PlacementSlot prevHoverSlot = null;

		// HACK - 임시 테스트용 함수
		private void SetInputMode()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1)) currentInputState = GameInputState.PrePlay;
			else if(Input.GetKeyDown(KeyCode.Alpha2)) currentInputState = GameInputState.Playing;
		}

		private void HandleSetupInput()
		{
			PlacementSlot slot = null;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, 100f, Constants.LAYER_SLOT))
			{
				if (control.Game.LeftClick.WasPressedThisFrame())
				{
					slot = hit.collider.GetComponent<PlacementSlot>();
					if (slot != null)
					{
						SelectSlot(slot);
					}
				}
				else if (control.Game.LeftClick.IsPressed())
				{
					if (selectedSlot == null) return;
					slot = hit.collider.GetComponent<PlacementSlot>();
					if (prevHoverSlot != slot && slot != null)
					{
						prevHoverSlot?.OnUnhover();
						slot.OnHover();

						PoolingManager.Instance.SetDottedLine(selectedSlot.transform.position,
							slot.transform.position);
						prevHoverSlot = slot;
					}
				}
			}
			if (control.Game.LeftClick.WasReleasedThisFrame())
			{
				if (prevHoverSlot != null)
				{
					ChangeSlot(prevHoverSlot);
				}

				PoolingManager.Instance.UnsetDottedLine();
				UnselectSlot();
				prevHoverSlot = null;
			}
		}

		private void SelectSlot(PlacementSlot slot)
		{
			if (selectedSlot != null)
				selectedSlot.OnRelease();

			selectedSlot = slot;
			selectedSlot.OnSelect();
		}

		private void UnselectSlot()
		{
			if (selectedSlot == null) return;

			selectedSlot.OnRelease();
			selectedSlot = null;
		}

		private void ChangeSlot(PlacementSlot slot)
		{
			if (selectedSlot == null) return;

			selectedSlot.ChangeSlot(slot);
			selectedSlot = null;
		}

		private void HandlePlayingInput()
		{
			cameraController.SetInputs(control.Camera.LeftClick.IsPressed(),
				control.Camera.RightClick.IsPressed(),
				control.Camera.Init.WasPressedThisFrame(),
				control.Camera.MouseMove.ReadValue<Vector2>(),
				control.Camera.Scroll.ReadValue<float>()
				);
		}

		private void HandleMainInput()
		{

		}
	}
}
