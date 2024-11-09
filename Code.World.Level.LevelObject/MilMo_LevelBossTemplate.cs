using Code.Core.Network.types;
using Code.Core.Template;
using Core;

namespace Code.World.Level.LevelObject;

public class MilMo_LevelBossTemplate : MilMo_MovableObjectTemplate
{
	public bool NoHealthBar { get; private set; }

	private MilMo_LevelBossTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Boss")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is BossTemplate bossTemplate))
		{
			return false;
		}
		NoHealthBar = bossTemplate.GetNoHeathBar() != 0;
		foreach (TemplateReference aggroMode in bossTemplate.GetAggroModes())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(aggroMode, delegate
			{
			});
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(bossTemplate.GetIdleMode(), delegate
		{
		});
		return true;
	}

	public static MilMo_LevelBossTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_LevelBossTemplate(category, path, filePath);
	}
}
