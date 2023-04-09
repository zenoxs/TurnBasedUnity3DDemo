using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : Singleton<Pathfinding>
{
	private const int MOVE_STRAIGHT_COST = 10;
	private const int MOVE_DIAGONAL_COST = 20;

	private class PathResult
	{
		public List<GridPosition> listGridPosition;
		public int pathCost;
	}

	[SerializeField] private Transform gridDebugObjectPrefab;
	[SerializeField] private LayerMask obstaclesLayerMask;

	private GridSystem<PathNode> gridSystem;
	private Dictionary<string, PathResult> pathCache;

	protected override void Awake()
	{
		base.Awake();
	}

	public void Setup(int width, int height, float cellSize)
	{
		gridSystem = new GridSystem<PathNode>(width, height, cellSize,
			(GridSystem<PathNode> gameObject, GridPosition gridPosition) => new PathNode(gridPosition));

#if UNITY_EDITOR
		gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
#endif

		pathCache = new Dictionary<string, PathResult>();

		for (int x = 0; x < gridSystem.GetWidth(); x++) {
			for (int z = 0; z < gridSystem.GetHeight(); z++) {
				GridPosition gridPosition = new GridPosition(x, z);
				Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
				float raycastOffsetDistance = 5f;
				if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask)) {
					// Debug.Log("Find obstacle " + gridPosition);
					GetNode(x, z).SetIsWalkable(false);
				}
			}
		}
	}

	public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathCost)
	{
		string pathHashCode = GetHashCodeFromPath(startGridPosition, endGridPosition);

		//if (pathCache.ContainsKey(pathHashCode)) {
		//	PathResult pathResult = pathCache[pathHashCode];
		//	Debug.Log("Path already calculated " + startGridPosition + " -> " + endGridPosition);

		//	pathCost = pathResult.pathCost;
		//	return pathResult.listGridPosition;
		//}

		List<PathNode> openList = new List<PathNode>();
		List<PathNode> closedList = new List<PathNode>();

		PathNode startNode = gridSystem.GetGridObject(startGridPosition);
		PathNode endNode = gridSystem.GetGridObject(endGridPosition);
		openList.Add(startNode);

		for (int x = 0; x < gridSystem.GetWidth(); x++) {
			for (int z = 0; z < gridSystem.GetHeight(); z++) {
				GridPosition gridPosition = new GridPosition(x, z);
				PathNode pathNode = gridSystem.GetGridObject(gridPosition);

				pathNode.SetGCost(int.MaxValue);
				pathNode.SetHCost(0);
				pathNode.ResetCameFromPathNode();
			}
		}

		startNode.SetGCost(0);
		startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));

		while (openList.Count > 0) {
			PathNode currentNode = GetLowestFCostPAthNode(openList);

			if (currentNode == endNode) {
				pathCost = endNode.GetFCost();
				List<GridPosition> path = CalculatePath(endNode);
				//pathCache.Add(pathHashCode, new PathResult {
				//	listGridPosition = path,
				//	pathCost = pathCost
				//});

				return path;
			}

			openList.Remove(currentNode);
			closedList.Add(currentNode);

			foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
				if (closedList.Contains(neighbourNode)) {
					continue;
				}

				if (!neighbourNode.IsWalkable()) {
					closedList.Add(neighbourNode);
					continue;
				}

				int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

				if (tentativeGCost < neighbourNode.GetGCost()) {
					neighbourNode.SetCameFromPathNode(currentNode);
					neighbourNode.SetGCost(tentativeGCost);
					neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
				}

				if (!openList.Contains(neighbourNode)) {
					openList.Add(neighbourNode);
				}
			}
		}

		// No path found
		//pathCache.Add(pathHashCode, new PathResult {
		//	listGridPosition = null,
		//	pathCost = 0
		//});
		pathCost = 0;
		return null;
	}

	private List<GridPosition> CalculatePath(PathNode endNode)
	{
		List<PathNode> pathNodeList = new List<PathNode>();
		pathNodeList.Add(endNode);
		PathNode currentNode = endNode;

		while (currentNode.GetCameFromPathNode() != null) {
			pathNodeList.Add(currentNode.GetCameFromPathNode());
			currentNode = currentNode.GetCameFromPathNode();
		}

		pathNodeList.Reverse();

		return pathNodeList.Select(pathNode => pathNode.GetGridPosition()).ToList();

	}

	public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
	{
		GridPosition gridPositionDistance = gridPositionA - gridPositionB;
		int totalDistance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
		int xDistance = Mathf.Abs(gridPositionDistance.x);
		int zDistance = Mathf.Abs(gridPositionDistance.z);
		int remaining = Mathf.Abs(xDistance - zDistance);

		return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
	}

	private List<PathNode> GetNeighbourList(PathNode currentNode)
	{
		List<PathNode> neightbourList = new List<PathNode>();
		GridPosition gridPosition = currentNode.GetGridPosition();

		if (gridPosition.x - 1 >= 0) {
			// Left
			neightbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));

			if (gridPosition.z - 1 >= 0) {
				// Left Down
				neightbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
			}
			if (gridPosition.z + 1 < gridSystem.GetHeight()) {
				// Left Up
				neightbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
			}
		}

		if (gridPosition.x + 1 < gridSystem.GetWidth()) {
			// Right
			neightbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));

			if (gridPosition.z - 1 >= 0) {
				// Right Down
				neightbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
			}

			if (gridPosition.z + 1 < gridSystem.GetHeight()) {
				// Right Up
				neightbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
			}
		}

		if (gridPosition.z - 1 >= 0) {
			// Down
			neightbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
		}

		if (gridPosition.z + 1 < gridSystem.GetHeight()) {
			// Up
			neightbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
		}

		return neightbourList;

	}

	private PathNode GetNode(int x, int z)
	{
		return gridSystem.GetGridObject(new GridPosition(x, z));
	}

	private PathNode GetLowestFCostPAthNode(List<PathNode> pathNodeList)
	{
		return pathNodeList.OrderBy(pathNode => pathNode.GetFCost()).First();
	}

	public bool IsWalkableGridPosition(GridPosition gridPosition)
	{
		return gridSystem.GetGridObject(gridPosition).IsWalkable();
	}

	public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
	{
		return FindPath(startGridPosition, endGridPosition, out _) != null;
	}

	public int GetPathCost(GridPosition startGridPosition, GridPosition endGridPosition)
	{
		FindPath(startGridPosition, endGridPosition, out int pathCost);
		return pathCost;
	}

	private string GetHashCodeFromPath(GridPosition startGridPosition, GridPosition endGridPosition)
	{
		return $"{startGridPosition.GetHashCode()}-{endGridPosition.GetHashCode()}";
	}

	public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
	{
		gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
	}
}
