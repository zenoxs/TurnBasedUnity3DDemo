using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{

	public static event EventHandler OnAnyGrenadeExploded;

	[SerializeField] private Transform grenadeExplosionVfxPrefab;
	[SerializeField] private TrailRenderer trailRenderer;
	[SerializeField] private AnimationCurve arcYAnimationCurve;

	private Vector3 targetPosition;
	private Action onGrenadeBehaviourComplete;
	private int explosionRange;
	private float totalDistance;
	private Vector3 positionXZ;

	private void Update()
	{
		Vector3 moveDir = (targetPosition - positionXZ).normalized;
		float moveSpeed = 15f;
		positionXZ += moveDir * moveSpeed * Time.deltaTime;

		float distance = Vector3.Distance(positionXZ, targetPosition);
		float distanceNormalized = 1 - distance / totalDistance;

		float maxHeight = totalDistance / 4f;
		float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
		transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

		float reachedTargetDistance = .2f;
		if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance) {
			float damageRadius = 2 * LevelGrid.Instance.GetCellSize(); // Multiply by 2 to get one cell (worldPosition * 2 = gridPosition)
			Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

			foreach (Collider collider in colliderArray) {
				if (collider.TryGetComponent<IHitable>(out IHitable target)) {
					target.Damage(30, transform.position);
				}
			}

			OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

			trailRenderer.transform.parent = null;

			Instantiate(grenadeExplosionVfxPrefab, targetPosition + Vector3.up * 1, Quaternion.identity);

			Destroy(gameObject);

			onGrenadeBehaviourComplete();
		}
	}

	public void Setup(GridPosition targetGridPositon, int explosionRange, Action onGrenadeBehaviourComplete)
	{
		this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
		this.explosionRange = explosionRange;
		targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPositon);

		positionXZ = transform.position;
		positionXZ.y = 0;

		totalDistance = Vector3.Distance(transform.position, targetPosition);
	}
}
