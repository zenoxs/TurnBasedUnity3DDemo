using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI actionPointsText;
	[SerializeField] private Unit unit;
	[SerializeField] private Image HealthBarImage;
	[SerializeField] private HealthSystem healthSystem;

	private void OnEnable()
	{
		Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
		healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
	}

	private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
	{
		UpdateHealthBar();
	}

	private void OnDisable()
	{
		Unit.OnAnyActionPointsChanged -= Unit_OnAnyActionPointsChanged;
	}

	private void Start()
	{
		UpdateActionPointsText();
		UpdateHealthBar();
	}

	private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
	{
		UpdateActionPointsText();
	}

	private void UpdateActionPointsText()
	{
		actionPointsText.text = unit.GetActionPoints().ToString();
	}

	private void UpdateHealthBar()
	{
		HealthBarImage.fillAmount = healthSystem.GetHealthNormalized();
	}
}
