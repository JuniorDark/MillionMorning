using Code.World.Gameplay;
using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_InvertedCircle : MilMo_Volume
{
	private MilMo_InvertedCircleTemplate Template => VolumeTemplate as MilMo_InvertedCircleTemplate;

	public MilMo_InvertedCircle(MilMo_VolumeTemplate template, Transform transform)
		: base(template, transform)
	{
	}

	public override bool IsInside(Vector3 position)
	{
		if (Transform == null)
		{
			return false;
		}
		Vector3 vector = Transform.InverseTransformPoint(position);
		return Mathf.Sqrt(Mathf.Pow(vector.x, 2f) + Mathf.Pow(vector.z, 2f)) > Template.Radius;
	}

	public override bool IsInsideDynamic(Vector3 position, MilMo_GameplayObject gameplayObject)
	{
		if (Transform == null)
		{
			return false;
		}
		Vector3 vector = Transform.InverseTransformPoint(position);
		Vector3 localScale = gameplayObject.GameObject.transform.localScale;
		float num = localScale.x / localScale.z;
		return Mathf.Sqrt(Mathf.Pow(vector.z * num, 2f) + Mathf.Pow(vector.x, 2f)) > localScale.x;
	}

	public override bool IsDynamicVolume()
	{
		return Template.DynamicRadius;
	}

	public override void Debug_Draw()
	{
		if (!(Transform == null))
		{
			float radius = Template.Radius;
			Vector3 position = Transform.position;
			Vector3 vector = position + Random.onUnitSphere * radius;
			Vector3 vector2 = position + Random.onUnitSphere * radius;
			Vector3 vector3 = position + Random.onUnitSphere * radius;
			Vector3 vector4 = position + Random.onUnitSphere * radius;
			Vector3 vector5 = position + Random.onUnitSphere * radius;
			Vector3 vector6 = position + Random.onUnitSphere * radius;
			Vector3 vector7 = position + Random.onUnitSphere * radius;
			Vector3 vector8 = position + Random.onUnitSphere * radius;
			Vector3 vector9 = position + Random.onUnitSphere * radius;
			Debug.DrawLine(vector, vector * 2f, Color.red);
			Debug.DrawLine(vector2, vector2 * 2f, Color.red);
			Debug.DrawLine(vector3, vector3 * 2f, Color.red);
			Debug.DrawLine(vector4, vector4 * 2f, Color.red);
			Debug.DrawLine(vector5, vector5 * 2f, Color.red);
			Debug.DrawLine(vector6, vector6 * 2f, Color.red);
			Debug.DrawLine(vector7, vector7 * 2f, Color.red);
			Debug.DrawLine(vector8, vector8 * 2f, Color.red);
			Debug.DrawLine(vector9, vector9 * 2f, Color.red);
		}
	}
}
