using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAction : BaseAction
{

	public event EventHandler<OnShootEventArgs> OnShoot;
	public static event EventHandler<OnShootEventArgs> OnAnyShoot;

	public class OnShootEventArgs : EventArgs
	{
		public Unit targetUnit;
		public Unit shootingUnit;
	}

	private enum State
	{
		Aiming,
		Shooting,
		Cooloff,
	}

	private const float UNIT_SHOULDER_HEIGHT = 1.7f;

	[SerializeField] private int maxShootDistance = 4;
	[SerializeField] private int shootDamage = 40;
	[SerializeField] private LayerMask obstaclesLayerMask;


	private State state;
	private float stateTimer;
	private Unit targetUnit;
	private bool canShootBullet;

	public int GetMaxShootDistance() => maxShootDistance;

	public override string GetActionName() => "Shoot";


	private void Update()
	{
		if (!isActive) {
			return;
		}

		stateTimer -= Time.deltaTime;


		switch (state) {
			case State.Aiming:
				// rotate to the enemy
				Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
				float rotateSpeed = 10f;
				transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
				break;
			case State.Shooting:
				if (canShootBullet) {
					Shoot();
					canShootBullet = false;
				}
				break;
			case State.Cooloff:

				break;
		}

		if (stateTimer <= 0f) {
			NextState();
		}
	}

	private void Shoot()
	{
		OnShootEventArgs onShootEventArgs = new OnShootEventArgs {
			targetUnit = targetUnit,
			shootingUnit = unit,
		};

		OnAnyShoot?.Invoke(this, onShootEventArgs);
		OnShoot?.Invoke(this, onShootEventArgs);

		targetUnit.Damage(shootDamage, unit.GetWorldPosition() + Vector3.up * UNIT_SHOULDER_HEIGHT);
	}

	private void NextState()
	{
		switch (state) {
			case State.Aiming:
				state = State.Shooting;
				float shootingStateTime = 0.3f;
				stateTimer = shootingStateTime;

				break;
			case State.Shooting:
				state = State.Cooloff;
				float coolOffStateTime = 0.5f;
				stateTimer = coolOffStateTime;
				break;
			case State.Cooloff:
				ActionComplete();
				break;
		}
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		GridPosition unitGridPosition = unit.GetGridPosition();

		return GetValidActionGridPositionList(unitGridPosition);
	}

	public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();

		for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
			for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if (testDistance > maxShootDistance) {
					continue;
				}

				if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
					continue;
				}

				Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

				// Both units on same 'team'
				if (targetUnit.IsEnemy() == unit.IsEnemy()) {
					continue;
				}

				Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
				Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

				if (Physics.Raycast(
					unitWorldPosition + Vector3.up * UNIT_SHOULDER_HEIGHT,
					shootDir,
					Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
					obstaclesLayerMask)) {
					// Blocked by an obstacle
					continue;
				}

				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

		state = State.Aiming;
		canShootBullet = true;
		float aimingStateTime = 1f;
		stateTimer = aimingStateTime;

		ActionStart(onActionComplete);
	}

	public Unit GetTargetUnit()
	{
		return targetUnit;
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
		return new EnemyAIAction {
			gridPosition = gridPosition,
			// Look for the lowest life enemy
			actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f)
		};
	}

	public int GetTargetCountAtPosition(GridPosition gridPosition)
	{
		return GetValidActionGridPositionList(gridPosition).Count;
	}

}
