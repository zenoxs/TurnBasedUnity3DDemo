#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{

	private PlayerInputActions playerInputActions;

	protected override void Awake()
	{
		base.Awake();
#if USE_NEW_INPUT_SYSTEM
		playerInputActions = new PlayerInputActions();
		playerInputActions.Player.Enable();
#endif
	}

	public Vector2 GetMouseScreenPosition()
	{
		return Mouse.current.position.ReadValue();
	}

	public bool IsMouseButtonPressedThisFrame()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.Click.WasPressedThisFrame();
#else
		return Input.GetMouseButtonDown(0);
#endif
	}

	public Vector2 GetCameraMoveVector()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CamereMovement.ReadValue<Vector2>();
#else
		Vector2 inputMoveDir = new Vector2(0, 0);

		if (Input.GetKey(KeyCode.W)) {
			inputMoveDir.y = +1f;
		}
		if (Input.GetKey(KeyCode.S)) {
			inputMoveDir.y = -1f;
		}
		if (Input.GetKey(KeyCode.A)) {
			inputMoveDir.x = -1f;
		}
		if (Input.GetKey(KeyCode.D)) {
			inputMoveDir.x = +1f;
		}

		return inputMoveDir;
#endif
	}

	public float GetCameraRotateAmount()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
		float rotateAmount = 0f;

		if (Input.GetKey(KeyCode.Q)) {
			rotateAmount = +1f;
		}
		if (Input.GetKey(KeyCode.E)) {
			rotateAmount = -1f;
		}

		return rotateAmount;
#endif
	}

	public float GetCameraZoomAmount()
	{
#if USE_NEW_INPUT_SYSTEM
		return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
		float zoomAmount = 0f;
		if (Input.mouseScrollDelta.y != 0) {
			zoomAmount += Input.mouseScrollDelta.y;
		}

		return zoomAmount;
#endif
	}
}
