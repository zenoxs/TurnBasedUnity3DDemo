using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
	private List<Unit> unitList;
	private List<Unit> friendlyUnitList;
	private List<Unit> enemyUnitList;

	protected override void Awake()
	{
		base.Awake();
		unitList = new List<Unit>();
		friendlyUnitList = new List<Unit>();
		enemyUnitList = new List<Unit>();
	}

	private void OnEnable()
	{
		Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
		Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
	}

	private void Unit_OnAnyUnitDead(object sender, Unit unit)
	{
		unitList.Remove(unit);

		if (unit.IsEnemy()) {
			enemyUnitList.Remove(unit);
		} else {
			friendlyUnitList.Remove(unit);
		}
	}

	private void Unit_OnAnyUnitSpawned(object sender, Unit unit)
	{
		unitList.Add(unit);

		if (unit.IsEnemy()) {
			enemyUnitList.Add(unit);
		} else {
			friendlyUnitList.Add(unit);
		}
	}

	public List<Unit> GetUnitList()
	{
		return unitList;
	}

	public List<Unit> GetEnemyUnitList()
	{
		return enemyUnitList;
	}

	public List<Unit> GetFriendlyUnitList()
	{
		return friendlyUnitList;
	}
}
