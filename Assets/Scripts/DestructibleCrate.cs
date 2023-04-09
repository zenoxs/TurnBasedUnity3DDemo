using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour, IHitable
{
	public static event EventHandler OnAnyDestroyed;

	[SerializeField] private Transform crateDestroyedPrefab;
	private GridPosition gridPosition;

	public GridPosition GetGridPosition() => gridPosition;

	private void Start()
	{
		gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
	}

	public void Damage(int damage, Vector3? damageSource)
	{
		Vector3 hitDirection = damageSource.HasValue ? (damageSource.Value - transform.position).normalized : Vector3.zero;

		Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);

		Vector3 randomModifierDir = new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, UnityEngine.Random.Range(-0.3f, +0.3f));
		ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position + hitDirection + randomModifierDir, 10f);

		Destroy(gameObject);

		OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
	}

	private void ApplyExplosionToChildren(
		Transform root,
		float explosionForce,
		Vector3 explosionPosition,
		float explosionRange)
	{
		foreach (Transform child in root) {
			if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody)) {
				childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
			}

			ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
		}
	}
}
