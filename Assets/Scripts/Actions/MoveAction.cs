using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAction : BaseAction
{
	[SerializeField] private float moveSpeed = 4f;
	[SerializeField] private float rotateSpeed = 10f;
	[SerializeField] private int maxMoveDistance = 2;

	public event EventHandler OnStartMoving;
	public event EventHandler OnStopMoving;

	private List<Vector3> positionList;
	private int currentPositionIndex;

	public override string GetActionName() => "Move";

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		List<GridPosition> gridPositonList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out _);
		positionList = gridPositonList.Select(g => LevelGrid.Instance.GetWorldPosition(g)).ToList();
		currentPositionIndex = 0;

		OnStartMoving?.Invoke(this, EventArgs.Empty);

		ActionStart(onActionComplete);
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{

		List<GridPosition> validGridPositionList = new List<GridPosition>();

		GridPosition unitGridPosition = unit.GetGridPosition();

		for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) {
			for (int z = -maxMoveDistance; z <= maxMoveDistance; z++) {
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				if (unitGridPosition == testGridPosition) {
					continue;
				}

				if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
					continue;
				}

				if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) {
					continue;
				}

				if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition)) {
					continue;
				}

				int pathfindingCostMultiplier = 10;
				if (Pathfinding.Instance.GetPathCost(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingCostMultiplier) {
					continue;
				}


				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;

	}

	private void Update()
	{
		if (!isActive) {
			return;
		}

		Vector3 targetPosition = positionList[currentPositionIndex];

		float stoppingDistance = .1f;

		Vector3 moveDirection = (targetPosition - transform.position).normalized;

		// Solution using the forward and Vector3 to rotate the character
		transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

		if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) {

			transform.position += moveDirection * Time.deltaTime * this.moveSpeed;

			// Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
			// transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
		} else {
			currentPositionIndex++;
			if (currentPositionIndex >= positionList.Count) {
				OnStopMoving?.Invoke(this, EventArgs.Empty);
				ActionComplete();
			}
		}
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		int targetCountAtGridposition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

		return new EnemyAIAction {
			gridPosition = gridPosition,
			actionValue = targetCountAtGridposition * 10,
		};
	}
}
