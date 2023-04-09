using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : Singleton<UnitActionSystem>
{
	public event EventHandler<Unit> OnSelectedUnitChanged;
	public event EventHandler<BaseAction> OnSelectedActionChanged;
	public event EventHandler<BaseAction> OnActionStarted;
	public event EventHandler<bool> OnBusyChanged;

	[SerializeField] private Unit selectedUnit;
	[SerializeField] private LayerMask unitLayerMask;

	private BaseAction selectedAction;
	private bool isBusy;

	private void Start()
	{
		SetSelectedUnit(selectedUnit);
	}


	private void Update()
	{
		if (isBusy) {
			return;
		}

		if (!TurnSystem.Instance.IsPlayerTurn()) {
			return;
		}

		// if the pointer is over a UI element then ignore it
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}

		if (TryHandleUnitSelection()) {
			return;
		}

		HandleSelectedAction();
	}

	private void HandleSelectedAction()
	{

		if (InputManager.Instance.IsMouseButtonPressedThisFrame()) {
			GridPosition mouseGridPostion = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

			if (!selectedAction.IsValidActionGridPosition(mouseGridPostion)) {
				return;
			}
			if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) {
				return;
			}

			SetBusy();
			selectedAction.TakeAction(mouseGridPostion, ClearBusy);


			OnActionStarted?.Invoke(this, selectedAction);
		}
	}

	private void SetBusy()
	{
		isBusy = true;
		OnBusyChanged?.Invoke(this, isBusy);
	}

	private void ClearBusy()
	{
		isBusy = false;
		OnBusyChanged?.Invoke(this, isBusy);
	}

	private bool TryHandleUnitSelection()
	{
		if (InputManager.Instance.IsMouseButtonPressedThisFrame()) {

			Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

			if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) {
				if (raycastHit.collider.TryGetComponent<Unit>(out Unit unit)) {
					if (unit == selectedUnit) {
						return false;
					}

					if (unit.IsEnemy()) {
						return false;
					}

					SetSelectedUnit(unit);
					return true;
				}
			}
		}

		return false;
	}

	private void SetSelectedUnit(Unit unit)
	{
		selectedUnit = unit;

		SetSelectedAction(unit.GetAction<MoveAction>());

		OnSelectedUnitChanged?.Invoke(this, unit);
	}

	public void SetSelectedAction(BaseAction baseAction)
	{
		selectedAction = baseAction;

		OnSelectedActionChanged?.Invoke(this, baseAction);
	}

	public Unit GetSelectedUnit() => selectedUnit;

	public BaseAction GetSelectedAction() => selectedAction;

	public bool GetIsBusy() => isBusy;
}
