using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

	[SerializeField] private Unit unit;

	void Start()
	{

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.T)) {
			GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
			GridPosition startGridPosition = new GridPosition(0, 0);

			List<GridPosition> gridPositionlist = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition, out _);

			for (int i = 0; i < gridPositionlist.Count - 1; i++) {
				Debug.DrawLine(
					LevelGrid.Instance.GetWorldPosition(gridPositionlist[i]),
					LevelGrid.Instance.GetWorldPosition(gridPositionlist[i + 1]),
					Color.white,
					10f
					);
			}
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			ScreenShake.Instance.Shake();
		}
	}

}
