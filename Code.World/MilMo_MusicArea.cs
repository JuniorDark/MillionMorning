using Code.Core.Collision;
using Code.Core.Music;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World;

public sealed class MilMo_MusicArea
{
	private MilMo_Cylinder _cylinder;

	private string MusicName { get; set; }

	public bool Load(MilMo_SFFile file)
	{
		MusicName = file.GetString();
		Vector3 vector = file.GetVector3();
		float @float = file.GetFloat();
		float float2 = file.GetFloat();
		Transform transform = new GameObject().transform;
		transform.position = vector;
		MilMo_CylinderTemplate milMo_CylinderTemplate = new MilMo_CylinderTemplate(@float, float2 * 2f);
		_cylinder = milMo_CylinderTemplate.Instantiate(transform) as MilMo_Cylinder;
		return file.CheckEventTags();
	}

	public bool Inside(Vector3 pos)
	{
		return _cylinder.IsInside(pos);
	}

	public void Play()
	{
		MilMo_Music.Instance.FadeIn(MusicName);
	}
}
