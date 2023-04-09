using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UnitActionSystemUI : MonoBehaviour
{

	[SerializeField] private Transform actionButtonPrefab;
	[SerializeField] private Transform actionButtonContainerTransform;
	[SerializeField] private TextMeshProUGUI actionPointsText;

	private List<ActionButonUI> actionButtonList;

	private void Awake()
	{
		actionButtonList = new List<ActionButonUI>();
	}

	private void OnEnable()
	{
		UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
		UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
		UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
		TurnSystem.Instance.OnTurnChanged += UnitActionSystem_OnTurnChanged;
		Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
	}


	private void OnDisable()
	{
		if (UnitActionSystem.Instance != null) {
			UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
			UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
			UnitActionSystem.Instance.OnActionStarted -= UnitActionSystem_OnActionStarted;
		}

		Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
	}

	private void Start()
	{
		UpdateActionPoints();
		CreateUnitActionsButton();
		UpdateSelectedVisual();
	}

	private void CreateUnitActionsButton()
	{
		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

		foreach (Transform buttonTransform in actionButtonContainerTransform) {
			Destroy(buttonTransform.gameObject);
		}

		actionButtonList.Clear();

		foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray()) {
			Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform.transform);
			ActionButonUI actionButonUI = actionButtonTransform.GetComponent<ActionButonUI>();
			actionButonUI.SetBaseAction(baseAction);
			actionButtonList.Add(actionButonUI);
		}
	}

	private void UnitActionSystem_OnActionStarted(object sender, BaseAction action)
	{
		UpdateActionPoints();
	}

	private void UnitActionSystem_OnSelectedUnitChanged(object semder, Unit unit)
	{
		CreateUnitActionsButton();
		UpdateSelectedVisual();
		UpdateActionPoints();
	}

	private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction action)
	{
		UpdateSelectedVisual();
	}

	private void UnitActionSystem_OnTurnChanged(object sender, int e)
	{
		UpdateActionPoints();
	}

	private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
	{
		UpdateActionPoints();
	}


	private void UpdateSelectedVisual()
	{
		foreach (ActionButonUI actionButonUI in actionButtonList) {
			actionButonUI.UpdateSelectedVisual();
		}
	}

	private void UpdateActionPoints()
	{
		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
		actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
	}
}
