using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{

	public static event EventHandler OnAnySwordHit;

	private enum State
	{
		SwingingSwordBeforeHit,
		SwingingSwordAfterHit
	}

	public event EventHandler OnSwordActionStarted;
	public event EventHandler OnSwordActionCompleted;

	[SerializeField] private int maxSwordDistance = 1;

	private State state;
	private float stateTimer;
	private Unit targetUnit;

	public int GetMaxSwordDistance() => maxSwordDistance;

	public override string GetActionName()
	{
		return "Sword";
	}

	private void Update()
	{
		if (!isActive) {
			return;
		}

		stateTimer -= Time.deltaTime;


		switch (state) {
			case State.SwingingSwordBeforeHit:
				// rotate to the enemy
				Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
				float rotateSpeed = 10f;
				transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
				break;
			case State.SwingingSwordAfterHit:
				break;
		}

		if (stateTimer <= 0f) {
			NextState();
		}
	}

	private void NextState()
	{
		switch (state) {
			case State.SwingingSwordBeforeHit:
				state = State.SwingingSwordAfterHit;
				float afterHitStateTime = 0.2f;
				stateTimer = afterHitStateTime;
				targetUnit.Damage(100, unit.GetWorldPosition() + Vector3.up * 1.5f);
				OnAnySwordHit?.Invoke(this, EventArgs.Empty);
				break;
			case State.SwingingSwordAfterHit:
				OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
				ActionComplete();
				break;
		}
	}

	public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
	{
		return new EnemyAIAction {
			gridPosition = gridPosition,
			actionValue = 200,
		};
	}

	public override List<GridPosition> GetValidActionGridPositionList()
	{
		List<GridPosition> validGridPositionList = new List<GridPosition>();

		GridPosition unitGridPosition = unit.GetGridPosition();

		for (int x = -maxSwordDistance; x <= maxSwordDistance; x++) {
			for (int z = -maxSwordDistance; z <= maxSwordDistance; z++) {
				GridPosition offsetGridPosition = new GridPosition(x, z);
				GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if (testDistance > maxSwordDistance) {
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


				validGridPositionList.Add(testGridPosition);
			}
		}

		return validGridPositionList;
	}

	public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
	{
		targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
		state = State.SwingingSwordBeforeHit;
		float beforeHitStateTime = 1f;
		stateTimer = beforeHitStateTime;

		OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

		ActionStart(onActionComplete);
	}
}
