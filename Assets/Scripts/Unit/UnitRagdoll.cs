using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
	[SerializeField] private Transform ragdollRootBone;

	public void Setup(Transform originalRootBone, Vector3 hitDirection)
	{
		MatchAllChildTransforms(originalRootBone, ragdollRootBone);

		Vector3 randomModifierDir = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, +0.3f));
		ApplyExplosionToRagdoll(ragdollRootBone, 400f, transform.position + hitDirection * 2 + randomModifierDir, 10f);
	}

	private void MatchAllChildTransforms(Transform root, Transform clone)
	{
		foreach (Transform child in root) {
			Transform cloneChild = clone.Find(child.name);
			if (cloneChild != null) {
				cloneChild.position = child.position;
				cloneChild.rotation = child.rotation;

				MatchAllChildTransforms(child, cloneChild);
			}

		}
	}

	private void ApplyExplosionToRagdoll(
		Transform root,
		float explosionForce,
		Vector3 explosionPosition,
		float explosionRange)
	{
		foreach (Transform child in root) {
			if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody)) {
				childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
			}

			ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
		}
	}
}
