using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
	private int maxInteractDistance = 1;

	public override string GetActionName()
	{
		return "Interact";
	}

	private void Update()
	{
		if (!isActive) {
			return;
		}
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction {
			gridPosition = gridPosition,
			actionValue = 0,
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();

		GridPosition unitGridPosition = unit.GetGridPosition();

		for (int x = -maxInteractDistance; x <= maxInteractDistance; x++) {
			for (int z = -maxInteractDistance; z <= maxInteractDistance; z++) {
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				if (unitGridPosition == testGridPosition) {
					continue;
				}

				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if (testDistance > maxInteractDistance) {
					continue;
				}

				IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
				if (interactable == null) {
					continue;
				}

				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
		interactable.Interact(OnInreractComplete);
		ActionStart(onActionComplete);
	}

	private void OnInreractComplete()
	{
		ActionComplete();
	}
}
