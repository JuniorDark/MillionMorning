using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_Sphere : MilMo_Volume
{
	private MilMo_SphereTemplate Template => VolumeTemplate as MilMo_SphereTemplate;

	public MilMo_Sphere(MilMo_VolumeTemplate template, Transform transform)
		: base(template, transform)
	{
	}

	public override bool IsInside(Vector3 position)
	{
		if (Transform == null)
		{
			return false;
		}
		return (Transform.position - position).sqrMagnitude <= Template.SquareRadius;
	}

	public float GetRadius()
	{
		return Mathf.Sqrt(Template.SquareRadius);
	}

	public Vector3 GetPosition()
	{
		return Transform.position;
	}

	public override void Debug_Draw()
	{
		if (!(Transform == null))
		{
			float num = Mathf.Sqrt(Template.SquareRadius);
			Vector3 position = Transform.position;
			Vector3 end = position + Random.onUnitSphere * num;
			Vector3 end2 = position + Random.onUnitSphere * num;
			Vector3 end3 = position + Random.onUnitSphere * num;
			Vector3 end4 = position + Random.onUnitSphere * num;
			Vector3 end5 = position + Random.onUnitSphere * num;
			Vector3 end6 = position + Random.onUnitSphere * num;
			Vector3 end7 = position + Random.onUnitSphere * num;
			Vector3 end8 = position + Random.onUnitSphere * num;
			Vector3 end9 = position + Random.onUnitSphere * num;
			Debug.DrawLine(position, end, Color.red);
			Debug.DrawLine(position, end2, Color.red);
			Debug.DrawLine(position, end3, Color.red);
			Debug.DrawLine(position, end4, Color.red);
			Debug.DrawLine(position, end5, Color.red);
			Debug.DrawLine(position, end6, Color.red);
			Debug.DrawLine(position, end7, Color.red);
			Debug.DrawLine(position, end8, Color.red);
			Debug.DrawLine(position, end9, Color.red);
		}
	}
}
