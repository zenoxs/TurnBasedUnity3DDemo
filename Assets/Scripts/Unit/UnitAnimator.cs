using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] Transform bulletProjectilePrefab;
	[SerializeField] Transform shootPointTransform;
	[SerializeField] Transform rifleTransform;
	[SerializeField] Transform swordTransform;

	private readonly int IS_WALKING_KEY = Animator.StringToHash("IsWalking");
	private readonly int SHOOT_KEY = Animator.StringToHash("Shoot");
	private readonly int STAB_KEY = Animator.StringToHash("Stab");

	private void Awake()
	{
		if (TryGetComponent<MoveAction>(out MoveAction moveAction)) {
			moveAction.OnStartMoving += MoveAction_OnStartMoving;
			moveAction.OnStopMoving += MoveAction_OnStopMoving;
		}

		if (TryGetComponent<ShootAction>(out ShootAction shootAction)) {
			shootAction.OnShoot += ShootAction_OnShoot;
		}

		if (TryGetComponent<SwordAction>(out SwordAction swordAction)) {
			swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
			swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
		}
	}

	private void Start()
	{
		EquipRifle();
	}

	private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
	{
		EquipRifle();
	}

	private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
	{
		EquipSword();
		animator.SetTrigger(STAB_KEY);
	}

	private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
	{
		animator.SetTrigger(SHOOT_KEY);

		Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
		BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
		Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
		targetUnitShootAtPosition.y = shootPointTransform.position.y;

		bulletProjectile.Setup(targetUnitShootAtPosition);
	}

	private void MoveAction_OnStopMoving(object sender, EventArgs e)
	{
		animator.SetBool(IS_WALKING_KEY, false);
	}

	private void MoveAction_OnStartMoving(object sender, EventArgs e)
	{
		animator.SetBool(IS_WALKING_KEY, true);
	}

	private void EquipSword()
	{
		swordTransform.gameObject.SetActive(true);
		rifleTransform.gameObject.SetActive(false);
	}

	private void EquipRifle()
	{
		swordTransform.gameObject.SetActive(false);
		rifleTransform.gameObject.SetActive(true);
	}
}
