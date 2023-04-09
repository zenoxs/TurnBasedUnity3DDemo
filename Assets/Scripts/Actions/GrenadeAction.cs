using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{

	[SerializeField] private int maxThrowDistance = 4;
	[SerializeField] private int explosionRange = 1;
	[SerializeField] private LayerMask obstaclesLayerMask;
	[SerializeField] private Transform grenadeProjectilePrefab;

	private void Update()
	{
		if (!isActive) {
			return;
		}
	}

	public int GetExplosionRange()
	{
		return explosionRange;
	}

	public override string GetActionName()
	{
		return "Grenade";
	}

	public override int GetActionPointsCost()
	{
		return 1;
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction {
			gridPosition = gridPosition,
			actionValue = 0
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();

		return GetValidActionGridPositionList(unitGridPosition);
	}

	public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();

		for (int x = -maxThrowDistance; x <= maxThrowDistance; x++) {
			for (int z = -maxThrowDistance; z <= maxThrowDistance; z++) {
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if (testDistance > maxThrowDistance) {
					continue;
				}

				//Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
				//Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
				//Vector3 shootDir = (targetWorldPosition - unitWorldPosition).normalized;
				//float unitShoulderHeight = 1.7f;
				//if (Physics.Raycast(
				//	unitWorldPosition + Vector3.up * unitShoulderHeight,
				//	shootDir,
				//	Vector3.Distance(unitWorldPosition, targetWorldPosition),
				//	obstaclesLayerMask)) {
				//	// Blocked by an obstacle
				//	continue;
				//}

				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
		GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
		grenadeProjectile.Setup(gridPosition, explosionRange, OnGrenadeBehaviourComplete);

		ActionStart(onActionComplete);

	}

	private void OnGrenadeBehaviourComplete()
	{
		ActionComplete();
	}
}
