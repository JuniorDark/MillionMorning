using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_LockBoxTemplate : MilMo_RandomBoxTemplate, IMilMo_OpenableBox
{
	public string IconPathClosed => IconPath;

	public string IconPathOpen => IconPath + "Open";

	public string KeyTemplateIdentifier { get; private set; }

	public static MilMo_LockBoxTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_LockBoxTemplate(category, path, filePath, "LockBox");
	}

	protected MilMo_LockBoxTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_LockBox(this, modifiers);
	}

	public void PostOpenEvent()
	{
		MilMo_EventSystem.Instance.PostEvent("tutorial_OpenLockBox", "");
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is LockBoxTemplate lockBoxTemplate))
		{
			return false;
		}
		KeyTemplateIdentifier = lockBoxTemplate.GetKeyTemplate().GetCategory() + ":" + lockBoxTemplate.GetKeyTemplate().GetPath();
		return true;
	}
}
