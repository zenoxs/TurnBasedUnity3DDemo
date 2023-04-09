using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
	private GridPosition gridPosition;
	private int gCost;
	private int hCost;
	private bool isWalkable = true;

	private int fCost { get { return gCost + hCost; } }

	private PathNode camFromPathNode;

	public int GetGCost() => gCost;
	public int GetHCost() => hCost;
	public int GetFCost() => fCost;

	public PathNode(GridPosition gridPosition)
	{
		this.gridPosition = gridPosition;

	}

	public override string ToString()
	{
		return $"{gridPosition.ToString()} \n";
	}

	public void SetGCost(int gCost)
	{
		this.gCost = gCost;
	}

	public void SetHCost(int hCost)
	{
		this.hCost = hCost;
	}

	public void ResetCameFromPathNode()
	{
		camFromPathNode = null;
	}

	public void SetCameFromPathNode(PathNode pathNode)
	{
		camFromPathNode = pathNode;
	}

	public PathNode GetCameFromPathNode()
	{
		return camFromPathNode;
	}

	public GridPosition GetGridPosition()
	{
		return gridPosition;
	}

	public bool IsWalkable()
	{
		return isWalkable;
	}

	public void SetIsWalkable(bool isWalkable)
	{
		this.isWalkable = isWalkable;
	}
}
