using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_Box : MilMo_Volume
{
	private MilMo_BoxTemplate Template => VolumeTemplate as MilMo_BoxTemplate;

	public MilMo_Box(MilMo_VolumeTemplate template, Transform transform)
		: base(template, transform)
	{
	}

	public override bool IsInside(Vector3 position)
	{
		if (Transform == null)
		{
			return false;
		}
		MilMo_BoxTemplate template = Template;
		Vector3 vector = Transform.InverseTransformPoint(position);
		if (Mathf.Abs(vector.x) < template.HalfSize.x && Mathf.Abs(vector.y) < template.HalfSize.y)
		{
			return Mathf.Abs(vector.z) < template.HalfSize.z;
		}
		return false;
	}

	public override float GetSurface()
	{
		return Transform.position.y + Template.HalfSize.y;
	}

	public override void Debug_Draw()
	{
		if (!(Transform == null))
		{
			Vector3 halfSize = Template.HalfSize;
			Vector3 position = Transform.position;
			Vector3 start = position - new Vector3(halfSize.x, halfSize.y, halfSize.z);
			Vector3 vector = position - new Vector3(halfSize.x, halfSize.y, 0f - halfSize.z);
			Vector3 vector2 = position - new Vector3(0f - halfSize.x, halfSize.y, halfSize.z);
			Vector3 vector3 = position - new Vector3(0f - halfSize.x, halfSize.y, 0f - halfSize.z);
			Vector3 vector4 = position - new Vector3(halfSize.x, 0f - halfSize.y, halfSize.z);
			Vector3 vector5 = position - new Vector3(halfSize.x, 0f - halfSize.y, 0f - halfSize.z);
			Vector3 end = position - new Vector3(0f - halfSize.x, 0f - halfSize.y, halfSize.z);
			Vector3 vector6 = position - new Vector3(0f - halfSize.x, 0f - halfSize.y, 0f - halfSize.z);
			Debug.DrawLine(vector4, vector5, Color.red);
			Debug.DrawLine(vector4, end, Color.red);
			Debug.DrawLine(vector5, vector6, Color.red);
			Debug.DrawLine(vector6, end, Color.red);
			Debug.DrawLine(start, vector, Color.red);
			Debug.DrawLine(start, vector2, Color.red);
			Debug.DrawLine(vector, vector3, Color.red);
			Debug.DrawLine(vector3, vector2, Color.red);
			Debug.DrawLine(start, vector4, Color.red);
			Debug.DrawLine(vector2, end, Color.red);
			Debug.DrawLine(vector, vector5, Color.red);
			Debug.DrawLine(vector3, vector6, Color.red);
		}
	}
}
