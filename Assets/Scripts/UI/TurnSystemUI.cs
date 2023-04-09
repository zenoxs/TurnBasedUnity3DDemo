using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
	[SerializeField] private Button endTurnButton;
	[SerializeField] private TextMeshProUGUI turnNumberText;
	[SerializeField] private GameObject enemyTurnVisualGameObject;

	private void Start()
	{
		endTurnButton.onClick.AddListener(() => {
			TurnSystem.Instance.NextTurn();
		});

		UpdateTurnText(TurnSystem.Instance.GetTurnNumber());
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
	}

	private void OnEnable()
	{
		TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChange;
	}

	private void OnDisable()
	{
		if (TurnSystem.Instance != null) {
			TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChange;
		}
	}

	private void TurnSystem_OnTurnChange(object sender, int turnNumber)
	{
		UpdateTurnText(turnNumber);
		UpdateEnemyTurnVisual();
		UpdateEndTurnButtonVisibility();
	}

	private void UpdateTurnText(int turnNumber)
	{
		turnNumberText.text = "TURN " + turnNumber;
	}

	private void UpdateEnemyTurnVisual()
	{
		enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
	}

	private void UpdateEndTurnButtonVisibility()
	{
		endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
	}
}
