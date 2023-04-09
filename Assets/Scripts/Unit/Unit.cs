using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
	private const int ACTION_POINTS_MAX = 3;

	public static event EventHandler OnAnyActionPointsChanged;
	public static event EventHandler<Unit> OnAnyUnitSpawned;
	public static event EventHandler<Unit> OnAnyUnitDead;

	[SerializeField] private bool isEnemy;

	private GridPosition gridPosition;
	private HealthSystem healthSystem;

	private BaseAction[] baseActionArray;
	private int actionPoints = ACTION_POINTS_MAX;

	public GridPosition GetGridPosition() => gridPosition;

	public BaseAction[] GetBaseActionArray() => baseActionArray;

	public int GetActionPoints() => actionPoints;

	public bool IsEnemy() => isEnemy;

	private void Awake()
	{
		baseActionArray = GetComponents<BaseAction>();
		healthSystem = GetComponent<HealthSystem>();
	}

	private void OnEnable()
	{
		TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
		healthSystem.OnDead += HealthSystem_OnDead;
	}

	private void HealthSystem_OnDead(object sender, EventArgs e)
	{
		LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
		Destroy(gameObject);
		OnAnyUnitDead?.Invoke(this, this);
	}

	private void OnDisable()
	{
		if (TurnSystem.Instance != null) {
			TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
		}
		healthSystem.OnDead -= HealthSystem_OnDead;
	}

	private void Start()
	{
		this.gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

		OnAnyUnitSpawned?.Invoke(this, this);
	}

	private void Update()
	{
		GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		if (gridPosition != newGridPosition) {

			GridPosition oldGridPosition = gridPosition;
			gridPosition = newGridPosition;

			LevelGrid.Instance.UnitMovedGridPostion(this, oldGridPosition, newGridPosition);
		}

	}

	public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
	{
		if (CanSpendActionPointsToTakeAction(baseAction)) {
			SpendActionPoints(baseAction.GetActionPointsCost());
			return true;
		}

		return false;
	}

	public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
	{
		return actionPoints >= baseAction.GetActionPointsCost();
	}

	private void SpendActionPoints(int amount)
	{
		actionPoints -= amount;
		OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
	}

	private void TurnSystem_OnTurnChanged(object sender, int turnNumber)
	{
		if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
			(!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())) {
			actionPoints = ACTION_POINTS_MAX;
			OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
		}

	}

	public void Damage(int damageAmount, Vector3? damageSource)
	{
		healthSystem.Damage(damageAmount, damageSource);
	}

	public Vector3 GetWorldPosition()
	{
		return transform.position;
	}

	public float GetHealthNormalized()
	{
		return healthSystem.GetHealthNormalized();
	}

	public T GetAction<T>() where T : BaseAction
	{
		return (T)Array.Find(baseActionArray, (baseAction) => baseAction is T);

	}
}
