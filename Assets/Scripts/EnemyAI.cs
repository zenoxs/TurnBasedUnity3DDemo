using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

	public enum State
	{
		WaitingForEnemyTurn,
		TakingTurn,
		Busy,
	}

	private State state;
	private float timer;

	private void Awake()
	{
		state = State.WaitingForEnemyTurn;
	}

	private void Start()
	{
		TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
	}

	void Update()
	{
		if (TurnSystem.Instance.IsPlayerTurn()) {
			return;
		}

		switch (state) {
			case State.WaitingForEnemyTurn:
				break;
			case State.TakingTurn:
				timer -= Time.deltaTime;
				if (timer <= 0f) {
					if (TryTakeEnemyAIAction(SetStateTakingTurn)) {
						state = State.Busy;
					} else {
						// No more enemies have actions they can take, end enemy turn
						TurnSystem.Instance.NextTurn();
					}
				}
				break;
			case State.Busy:
				break;
		}


	}

	private void SetStateTakingTurn()
	{
		timer = 0.5f;
		state = State.TakingTurn;
	}

	private void TurnSystem_OnTurnChanged(object sender, int turnNumber)
	{
		if (!TurnSystem.Instance.IsPlayerTurn()) {
			state = State.TakingTurn;
			timer = 2f;
		}

	}

	private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
	{
		foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) {
			if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {
				return true;
			}
		}
		return false;
	}

	private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
	{
		EnemyAIAction bestEnemyAIAction = null;
		BaseAction bestBaseAction = null;
		foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray()) {
			if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) {
				// Enemy cannot afford this action
				continue;
			}

			EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();

			if (bestEnemyAIAction == null) {
				bestEnemyAIAction = testEnemyAIAction;
				bestBaseAction = baseAction;
			} else {
				if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) {
					bestEnemyAIAction = testEnemyAIAction;
					bestBaseAction = baseAction;
				}
			}

		}

		if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)) {
			bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
			return true;
		} else {
			return false;
		}

	}
}
