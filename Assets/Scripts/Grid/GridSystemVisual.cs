using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemVisual : Singleton<GridSystemVisual>
{

	[Serializable]
	public struct GridVisualTypeMaterial
	{
		public GridVisualType gridVisualType;
		public Material material;
	}

	public enum GridVisualType
	{
		White,
		Blue,
		Red,
		RedSoft,
		Yellow,
	}

	[SerializeField] private Transform gridSystemVisualSinglePrefabs;
	[SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

	private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

	private void OnEnable()
	{
		MouseWorld.OnMouseGridPositionChanged += MouseWorld_OnMouseGridPositionChanged;
	}

	private void OnDisable()
	{
		MouseWorld.OnMouseGridPositionChanged -= MouseWorld_OnMouseGridPositionChanged;
	}

	private void Start()
	{
		gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
		for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
			for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
				GridPosition gridPosition = new GridPosition(x, z);

				Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefabs, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
				gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
			}
		}

		UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
		LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

		UpdateGridVisual();
	}

	private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
	{
		UpdateGridVisual();
	}

	private void UnitActionSystem_OnSelectedActionChanged(object sender, BaseAction e)
	{
		UpdateGridVisual();
	}

	public void HideAllGridPosition()
	{
		for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++) {
			for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++) {
				gridSystemVisualSingleArray[x, z].Hide();
			}
		}
	}

	private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
	{
		List<GridPosition> gridPositionList = new List<GridPosition>();
		for (int x = -range; x <= range; x++) {
			for (int z = -range; z <= range; z++) {

				GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

				int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
				if (testDistance > range) {
					continue;
				}

				if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
					continue;
				}

				gridPositionList.Add(testGridPosition);

			}
		}

		ShowGridPositionList(gridPositionList, gridVisualType);
	}

	public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
	{
		foreach (GridPosition gridPosition in gridPositionList) {
			gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
		}
	}

	private void UpdateGridVisual()
	{
		HideAllGridPosition();

		Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
		BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

		GridVisualType gridVisualType = GridVisualType.White;
		switch (selectedAction) {
			case SwordAction swordAction:
				gridVisualType = GridVisualType.Red;
				ShowGridPositionRange(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
				ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
				break;
			case ShootAction shootAction:
				gridVisualType = GridVisualType.Red;
				ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
				ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
				break;
			case GrenadeAction grenadeAction:
				gridVisualType = GridVisualType.RedSoft;
				GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
				ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
				ShowGridPositionRange(mouseGridPosition, grenadeAction.GetExplosionRange(), GridVisualType.Red);
				break;
			case InteractAction:
			case SpinAction:
				gridVisualType = GridVisualType.Blue;
				ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
				break;
			default:
				ShowGridPositionList(selectedAction.GetValidActionGridPositionList(), gridVisualType);
				break;

		}

	}

	private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
	{
		return gridVisualTypeMaterialList.Find(item => item.gridVisualType == gridVisualType).material;
	}

	private void MouseWorld_OnMouseGridPositionChanged(object sender, GridPosition mouseGridPosition)
	{
		BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
		if (selectedAction is GrenadeAction) {
			if (selectedAction.GetValidActionGridPositionList().Contains(mouseGridPosition)) {
				UpdateGridVisual();
			}
		}
	}
}
