using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour
{
	[Header("Area 1")]
	[SerializeField] private GameObject hider1;
	[SerializeField] private List<GameObject> enemy1List;
	[SerializeField] private int zPositionTrigger1;

	[Header("Area 2")]
	[SerializeField] private GameObject hider2;
	[SerializeField] private List<GameObject> enemy2List;
	[SerializeField] private Door door2;

	[Header("Area 3")]
	[SerializeField] private GameObject hider3;
	[SerializeField] private List<GameObject> enemy3List;
	[SerializeField] private Door door3;


	[Header("Area 4")]
	[SerializeField] private GameObject hider4;
	[SerializeField] private List<GameObject> enemy4List;
	[SerializeField] private Door door4;

	private void OnEnable()
	{
		LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
	}

	private void Start()
	{
		door2.OnDoorOpened += (object sender, EventArgs e) => {
			HideHider(hider2);
			SetActiveGameObjectList(enemy2List, true);
		};

		door3.OnDoorOpened += (object sender, EventArgs e) => {
			HideHider(hider3);
			SetActiveGameObjectList(enemy3List, true);
		};

		door4.OnDoorOpened += (object sender, EventArgs e) => {
			HideHider(hider4);
			SetActiveGameObjectList(enemy4List, true);
		};
	}

	private void OnDisable()
	{
		if (LevelGrid.Instance != null) {
			LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
		}
	}

	private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionArgs e)
	{
		if (e.gridPosition.z == zPositionTrigger1 && hider1.activeInHierarchy) {
			HideHider(hider1);
			SetActiveGameObjectList(enemy1List, true);
		}
	}

	private void SetActiveGameObjectList(List<GameObject> objectList, bool isActive)
	{
		foreach (GameObject obj in objectList) {
			obj.SetActive(isActive);
		}
	}

	private void HideHider(GameObject hider)
	{
		hider.SetActive(false);
	}
}
