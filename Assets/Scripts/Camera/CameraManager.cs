using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[SerializeField] private GameObject actionCameraObject;

	private void Start()
	{

		BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
		BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

		HideActionCamera();
	}

	private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
	{
		switch (sender) {
			case ShootAction:
				HideActionCamera();
				break;
		}
	}

	private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
	{
		switch (sender) {
			case ShootAction shootAction:
				Unit shooterUnit = shootAction.GetUnit();
				Unit targetUnit = shootAction.GetTargetUnit();
				Vector3 cameraCharacterHeight = Vector3.up * 1.7f;

				Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

				float shoulderOffsetAmount = 0.5f;
				Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * shoulderOffsetAmount;

				Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);

				actionCameraObject.transform.position = actionCameraPosition;
				actionCameraObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);

				ShootActionCamera();
				break;
		}
	}

	private void ShootActionCamera()
	{
		actionCameraObject.SetActive(true);
	}

	private void HideActionCamera()
	{
		actionCameraObject.SetActive(false);
	}
}
