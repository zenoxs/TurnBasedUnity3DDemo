using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

	[SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

	private float moveSpeed = 10f;
	private float rotationSpeed = 100f;
	private float zoomIncreaseAmount = 1f;
	private float zoomSpeed = 3f;

	private const float MIN_FOLLOW_Y_OFFSET = 1f;
	private const float MAX_FOLLOW_Y_OFFSET = 12f;

	private Vector3 targetFollowOffset;
	private CinemachineTransposer cinemachineTransposer;

	private void Start()
	{
		cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
		targetFollowOffset = cinemachineTransposer.m_FollowOffset;
	}


	private void Update()
	{

		HandleMovement();
		HandleRotation();
		HandleZoom();

	}

	private void HandleMovement()
	{
		Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

		Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
		transform.position += moveVector * moveSpeed * Time.deltaTime;
	}

	private void HandleRotation()
	{
		Vector3 rotationVector = new Vector3(0, 0, 0);

		rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

		transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
	}

	private void HandleZoom()
	{
		targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
		targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

		cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
	}
}
