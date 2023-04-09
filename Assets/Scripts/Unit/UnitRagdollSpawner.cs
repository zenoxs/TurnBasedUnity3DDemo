using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
	[SerializeField] private Transform ragdollPrefab;
	[SerializeField] private Transform originalRootBone;

	private HealthSystem healthSystem;

	private void Awake()
	{
		healthSystem = GetComponent<HealthSystem>();
	}

	private void OnEnable()
	{
		healthSystem.OnDead += HealthSystem_OnDead;
	}

	private void HealthSystem_OnDead(object sender, HealthSystem.OnDeadEventArgs e)
	{
		Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
		UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();

		Vector3 hitDirection = e.damageSource.HasValue ? (e.damageSource.Value - transform.position).normalized : Vector3.zero;

		unitRagdoll.Setup(originalRootBone, hitDirection);
	}
}
