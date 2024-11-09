using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_StaticGem : MilMo_LevelToken
{
	private MilMo_GemTemplate _template;

	private readonly sbyte _index;

	public MilMo_StaticGem(StaticGem token, sbyte index)
		: base(token)
	{
		MilMo_StaticGem milMo_StaticGem = this;
		_index = index;
		TokenFoundEffectName = "GemPickup";
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(token.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (template == null || timeOut)
			{
				Debug.LogWarning("Failed to get template " + token.GetTemplate().GetCategory() + ":" + token.GetTemplate().GetPath());
			}
			else
			{
				milMo_StaticGem._template = template as MilMo_GemTemplate;
				if (milMo_StaticGem._template == null)
				{
					Debug.LogWarning("Wrong template type in template " + template.Identifier);
				}
				else
				{
					milMo_StaticGem.UnFoundMeshVisualRepPath = "Content/Items/" + milMo_StaticGem._template.VisualRep;
					milMo_StaticGem.SqrFindRadius = milMo_StaticGem._template.PickupRadiusSquared;
					if (!milMo_StaticGem.IsFound)
					{
						milMo_StaticGem.Load();
					}
				}
			}
		});
	}

	protected override void SendFindRequest(Vector3 position)
	{
		Singleton<GameNetwork>.Instance.RequestFindStaticGem(_index, position);
		MilMo_Player.Instance.SetHasPlayed();
	}

	public override void SetAsFound()
	{
		base.SetAsFound();
		if (_template.PickupSound != "none")
		{
			MilMo_Player.Instance.Avatar.PlaySoundEffect(_template.PickupSound);
		}
	}
}
