using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObject
{
	private GridSystem<GridObject> gridSystem;
	private GridPosition gridPosition;
	private List<Unit> unitList;
	private IInteractable interactable;

	public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
	{
		this.gridSystem = gridSystem;
		this.gridPosition = gridPosition;
		unitList = new List<Unit>();
	}

	public void AddUnit(Unit unit)
	{
		unitList.Add(unit);
	}

	public void RemoveUnit(Unit unit)
	{
		unitList.Remove(unit);
	}

	public List<Unit> GetUnitList()
	{
		return unitList;
	}

	public bool HasAnyUnit()
	{
		return unitList.Count > 0;
	}

	public Unit GetUnit()
	{
		if (HasAnyUnit()) {
			return unitList[0];
		}
		return null;
	}

	public override string ToString()
	{
		string units = string.Join(",", unitList.ConvertAll(unit => $"{unit} \n"));
		return $"{gridPosition.ToString()} \n" + units;
	}

	public IInteractable GetInteractable()
	{
		return interactable;
	}

	public void SetInteractable(IInteractable interactable)
	{
		this.interactable = interactable;
	}
}
