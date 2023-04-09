using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : Singleton<LevelGrid>
{
	public class OnAnyUnitMovedGridPositionArgs : EventArgs
	{
		public Unit unit;
		public GridPosition gridPosition;
	}

	[SerializeField] private Transform gridDebugObjectPrefab;
	[SerializeField] private int width = 20;
	[SerializeField] private int height = 20;
	[SerializeField] private int cellSize = 2;



	public event EventHandler<OnAnyUnitMovedGridPositionArgs> OnAnyUnitMovedGridPosition;
	private GridSystem<GridObject> gridSystem;



	protected override void Awake()
	{
		gridSystem = new GridSystem<GridObject>(width, height, cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
		// gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

		base.Awake();
	}

	private void Start()
	{
		Pathfinding.Instance.Setup(width, height, cellSize);
	}

	public int GetWidth() => gridSystem.GetWidth();
	public int GetHeight() => gridSystem.GetHeight();
	public float GetCellSize() => gridSystem.GetCellSize();

	public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.AddUnit(unit);
	}

	public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetUnitList();
	}

	public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.RemoveUnit(unit);
	}

	public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

	public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

	public void UnitMovedGridPostion(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
	{
		RemoveUnitAtGridPosition(fromGridPosition, unit);
		AddUnitAtGridPosition(toGridPosition, unit);

		OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionArgs {
			gridPosition = toGridPosition,
			unit = unit,
		});
	}

	public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

	public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.HasAnyUnit();
	}

	public Unit GetUnitAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetUnit();
	}

	public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		return gridObject.GetInteractable();
	}

	public void SetInteratableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
	{
		GridObject gridObject = gridSystem.GetGridObject(gridPosition);
		gridObject.SetInteractable(interactable);
	}


}
