using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IHitable
{
	[SerializeField] private int healthMax = 100;
	private int health;

	public EventHandler<OnDeadEventArgs> OnDead;
	public EventHandler OnHealthChanged;


	public class OnDeadEventArgs : EventArgs
	{
		public Vector3? damageSource;
	}

	private void Awake()
	{
		health = healthMax;
	}

	public void Damage(int damageAmount, Vector3? damageSource)
	{
		health -= damageAmount;

		if (health < 0) {
			health = 0;
		}

		OnHealthChanged?.Invoke(this, EventArgs.Empty);

		if (health == 0) {
			Die(damageSource);
		}
	}

	private void Die(Vector3? damageSource)
	{
		OnDead?.Invoke(this, new OnDeadEventArgs { damageSource = damageSource });
	}

	public float GetHealthNormalized()
	{
		return (float)health / healthMax;
	}
}
