using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_TeleportTemplate : MilMo_Template
{
	protected string ArriveSound { get; private set; }

	protected MilMo_TeleportTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		Teleport teleport = (Teleport)t;
		if (teleport == null)
		{
			return false;
		}
		ArriveSound = teleport.GetArriveSound();
		return true;
	}

	public static MilMo_TeleportTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_TeleportTemplate(category, path, filePath, "Teleport");
	}

	public virtual void Activate(MilMo_Player player)
	{
		MilMo_Player.Instance.Teleporting = true;
	}
}
