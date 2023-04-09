using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Door : MonoBehaviour, IInteractable
{

	[SerializeField] private bool isOpen;
	private Animator animator;
	private Action onInteractComplete;
	private float timer;
	private bool isActive;

	private GridPosition gridPosition;

	public EventHandler OnDoorOpened;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
		LevelGrid.Instance.SetInteratableAtGridPosition(gridPosition, this);

		animator.speed = 1000;

		if (isOpen) {
			OpenDoor();
		} else {
			CloseDoor();
		}
	}
	private void Update()
	{
		if (!isActive) {
			return;
		}

		timer -= Time.deltaTime;

		if (timer <= 0) {
			isActive = false;
			onInteractComplete();
		}
	}

	public void Interact(Action onInteractComplete)
	{
		this.onInteractComplete = onInteractComplete;
		isActive = true;
		timer = 1f;
		animator.speed = 1;
		if (isOpen) {
			CloseDoor();
		} else {
			OpenDoor();
		}
	}

	private void OpenDoor()
	{
		isOpen = true;
		animator.SetBool("IsOpen", true);
		OnDoorOpened?.Invoke(this, EventArgs.Empty);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
	}

	private void CloseDoor()
	{
		isOpen = false;
		animator.SetBool("IsOpen", false);
		Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
	}
}
