using UnityEngine;

namespace Code.Core.Collision;

public class MilMo_Cylinder : MilMo_Volume
{
	private MilMo_CylinderTemplate Template => VolumeTemplate as MilMo_CylinderTemplate;

	public MilMo_Cylinder(MilMo_VolumeTemplate template, Transform transform)
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
		if (Mathf.Sqrt(Mathf.Pow(vector.x, 2f) + Mathf.Pow(vector.z, 2f)) < Template.Radius && vector.y < Template.Height / 2f)
		{
			return vector.y > (0f - Template.Height) / 2f;
		}
		return false;
	}

	public override float GetSurface()
	{
		return Transform.position.y + Template.Height;
	}

	public override void Debug_Draw()
	{
		if (!(Transform == null))
		{
			float radius = Template.Radius;
			Vector3 position = Transform.position;
			float height = Template.Height;
			Vector3 start = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start2 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start3 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start4 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start5 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start6 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start7 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start8 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Vector3 start9 = new Vector3(position.x + Random.onUnitSphere.x * radius, position.y, position.z + Random.onUnitSphere.z * radius);
			Debug.DrawLine(start, new Vector3(start.x, start.y + height, start.z), Color.red);
			Debug.DrawLine(start2, new Vector3(start2.x, start2.y + height, start2.z), Color.red);
			Debug.DrawLine(start3, new Vector3(start3.x, start3.y + height, start3.z), Color.red);
			Debug.DrawLine(start4, new Vector3(start4.x, start4.y + height, start4.z), Color.red);
			Debug.DrawLine(start5, new Vector3(start5.x, start5.y + height, start5.z), Color.red);
			Debug.DrawLine(start6, new Vector3(start6.x, start6.y + height, start6.z), Color.red);
			Debug.DrawLine(start7, new Vector3(start7.x, start7.y + height, start7.z), Color.red);
			Debug.DrawLine(start8, new Vector3(start8.x, start8.y + height, start8.z), Color.red);
			Debug.DrawLine(start9, new Vector3(start9.x, start9.y + height, start9.z), Color.red);
		}
	}
}
