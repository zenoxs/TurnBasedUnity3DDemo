using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
	[SerializeField] private Unit unit;

	private MeshRenderer meshRenderer;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSysten_OnSelectedUnitChanged;
		UpdateVisual();
	}

	private void UnitActionSysten_OnSelectedUnitChanged(object sender, Unit unit)
	{
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		if (UnitActionSystem.Instance.GetSelectedUnit() == unit) {
			meshRenderer.enabled = true;
		} else {
			meshRenderer.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (UnitActionSystem.Instance != null) {
			UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSysten_OnSelectedUnitChanged;
		}
	}
}
