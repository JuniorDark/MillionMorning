using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Tutorial;

public sealed class MilMo_TutorialArea
{
	private readonly string _world;

	private readonly string _level;

	private MilMo_Cylinder _cylinder;

	private string Name { get; set; }

	private string FullLevelName => _world + ":" + _level;

	public string FullName => FullLevelName + ":" + Name;

	public bool Active { get; set; }

	public MilMo_TutorialArea(string world, string level)
	{
		_world = world;
		_level = level;
	}

	public bool Load(MilMo_SFFile file)
	{
		Name = file.GetString();
		Vector3 vector = file.GetVector3();
		float @float = file.GetFloat();
		float float2 = file.GetFloat();
		Transform transform = new GameObject().transform;
		transform.position = vector;
		MilMo_CylinderTemplate milMo_CylinderTemplate = new MilMo_CylinderTemplate(@float, float2 * 2f);
		_cylinder = milMo_CylinderTemplate.Instantiate(transform) as MilMo_Cylinder;
		return true;
	}

	public void Update()
	{
		if (Active && MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null)
		{
			Vector3 position = MilMo_Player.Instance.Avatar.Position;
			if (_cylinder.IsInside(position))
			{
				MilMo_EventSystem.Instance.PostEvent("tutorial_Area", this);
			}
		}
	}
}
