using Code.World.GUI.LoadingScreen;
using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_InterTeleportTemplate : MilMo_TeleportTemplate
{
	private MilMo_InterTeleportTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "InterTeleport")
	{
	}

	public new static MilMo_InterTeleportTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_InterTeleportTemplate(category, path, filePath);
	}

	public override void Activate(MilMo_Player player)
	{
		if (!MilMo_LoadingScreen.Instance.IsLoading)
		{
			MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
		}
	}
}
