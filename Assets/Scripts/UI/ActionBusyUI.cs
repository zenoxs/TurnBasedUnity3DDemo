using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{

	private void Start()
	{
		Hide();
	}

	private void OnEnable()
	{
		UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
	}

	private void OnDisable()
	{
		UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
	}

	private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
	{
		if (isBusy) {
			Show();
		} else {
			Hide();
		}
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
}
