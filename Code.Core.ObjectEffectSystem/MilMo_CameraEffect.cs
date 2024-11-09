using Code.World;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_CameraEffect : MilMo_ObjectEffect
{
	private readonly string _cameraEvent;

	public override float Duration => 0.5f;

	public MilMo_CameraEffect(GameObject gameObject, MilMo_CameraEffectTemplate template)
		: base(gameObject, template)
	{
		_cameraEvent = template.Event;
	}

	public override bool Update()
	{
		MilMo_World.Instance.Camera.CameraEvent(_cameraEvent);
		Destroy();
		return false;
	}
}
