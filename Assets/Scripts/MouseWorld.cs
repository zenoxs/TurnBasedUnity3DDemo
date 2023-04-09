using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
	private static MouseWorld instance;

	public static EventHandler<GridPosition> OnMouseGridPositionChanged;


	[SerializeField] private LayerMask mousePlaneLayerMask;
	private GridPosition currentMouseGridPosition;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		GridPosition newMouseGridPosition = LevelGrid.Instance.GetGridPosition(GetPosition());
		if (currentMouseGridPosition != newMouseGridPosition) {
			currentMouseGridPosition = newMouseGridPosition;
			OnMouseGridPositionChanged?.Invoke(this, currentMouseGridPosition);
		}
	}

	public static Vector3 GetPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
		Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
		return raycastHit.point;
	}
}
