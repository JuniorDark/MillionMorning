using Code.Core.Network.types;

namespace Code.World.Gameplay;

public class MilMo_PlayerSplineTemplate : MilMo_GameObjectSplineTemplate
{
	public float FOV { get; private set; }

	public float CameraDistance { get; private set; }

	private MilMo_PlayerSplineTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "PlayerSpline")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		base.LoadFromNetwork(t);
		PlayerSpline playerSpline = (PlayerSpline)t;
		if (playerSpline == null)
		{
			return false;
		}
		FOV = playerSpline.GetFov();
		CameraDistance = playerSpline.GetCameraDistance();
		return true;
	}

	public new static MilMo_PlayerSplineTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_PlayerSplineTemplate(category, path, filePath);
	}
}
