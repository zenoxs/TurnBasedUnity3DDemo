using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : Singleton<ScreenShake>
{

	private CinemachineImpulseSource cinemachineImpulseSource;

	protected override void Awake()
	{
		base.Awake();
		cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
	}

	public void Shake(float intensity = 1f)
	{
		cinemachineImpulseSource.GenerateImpulse(intensity);
	}
}
