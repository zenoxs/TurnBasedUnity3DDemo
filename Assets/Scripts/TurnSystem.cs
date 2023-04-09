using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : Singleton<TurnSystem>
{
	private int turnNumber = 1;

	public event EventHandler<int> OnTurnChanged;

	public int GetTurnNumber() => turnNumber;

	private bool isPlayerTurn = true;

	public bool IsPlayerTurn() => isPlayerTurn;

	public void NextTurn()
	{
		turnNumber++;
		isPlayerTurn = !isPlayerTurn;
		OnTurnChanged?.Invoke(this, turnNumber);
	}
}
