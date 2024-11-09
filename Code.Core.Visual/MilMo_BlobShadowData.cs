using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_BlobShadowData
{
	public readonly string Material;

	public readonly string Bone;

	public readonly float Fov;

	public readonly float ActivateHeight;

	public readonly float DeactivationHeight;

	public readonly bool Orthographic;

	public readonly float OrthographicSize;

	public readonly bool AutoFarPlaneMode;

	public readonly float FarPlane;

	public readonly Vector3 Offset;

	public MilMo_BlobShadowData(string material, string bone, float fov, float activateHeight, float deactivationHeight, bool orthographic, float orthographicSize, Vector3 offset = default(Vector3), bool autoFarPlaneMode = true, float farPlane = 10f)
	{
		Material = material;
		Bone = ((bone.Length == 0) ? "Root" : ("Root/" + bone));
		Fov = fov;
		ActivateHeight = activateHeight;
		DeactivationHeight = deactivationHeight;
		Orthographic = orthographic;
		OrthographicSize = orthographicSize;
		Offset = offset;
		AutoFarPlaneMode = autoFarPlaneMode;
		FarPlane = farPlane;
	}
}
