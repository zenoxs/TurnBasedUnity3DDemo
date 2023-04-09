using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
	void Damage(int damage, Vector3? damageSource);
}
