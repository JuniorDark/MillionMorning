using System.Collections.Generic;
using Code.Core.Collision;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerTemplate : MilMo_Template
{
	public const int CONTACT = 0;

	public const int USE = 1;

	public const int CLIENT = 0;

	public const int REQUEST = 1;

	public const int SERVER = 2;

	private MilMo_LocString _noKeyThink;

	private MilMo_LocString _noCaptureKeyThink;

	public Vector3 Offset { get; private set; }

	public Vector3 RotationOffset { get; private set; }

	public MilMo_VolumeTemplate VolumeTemplate { get; private set; }

	public int ActivationEvent { get; private set; }

	public int ActivationCheck { get; private set; }

	public MilMo_LocString ActivationVerb { get; private set; }

	public float CooldownPerPlayer { get; private set; }

	public float Cooldown { get; private set; }

	public List<GameplayTriggerReaction> Reactions { get; private set; }

	public MilMo_ItemTemplate Key { get; private set; }

	public MilMo_LocString NoKeyThink
	{
		get
		{
			if (Key == null || _noKeyThink == null)
			{
				return MilMo_LocString.Empty;
			}
			if (_noKeyThink.WantsFormatArgs)
			{
				MilMo_LocString copy = _noKeyThink.GetCopy();
				copy.SetFormatArgs(Key.DisplayName);
				return copy;
			}
			return _noKeyThink;
		}
	}

	public string CaptureKey { get; private set; }

	public MilMo_LocString NoCaptureKeyThink
	{
		get
		{
			if (string.IsNullOrEmpty(CaptureKey) || _noCaptureKeyThink == null)
			{
				return MilMo_LocString.Empty;
			}
			return _noCaptureKeyThink;
		}
	}

	public bool MembersOnly { get; private set; }

	public int RequiredAvatarLevel { get; private set; }

	public bool HasExitArrow { get; private set; }

	public string ExitArrowIdentifier { get; private set; }

	public float ExitArrowYOffset { get; private set; }

	private MilMo_GameplayTriggerTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "GameplayTrigger")
	{
		Reactions = new List<GameplayTriggerReaction>();
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!(t is GameplayTrigger gameplayTrigger))
		{
			Debug.LogWarning("Failed to create gameplay trigger " + Path + " from network message: Got null or invalid volume template.");
			return false;
		}
		VolumeTemplate = MilMo_VolumeTemplate.Create(gameplayTrigger.GetVolume());
		if (VolumeTemplate == null)
		{
			Debug.LogWarning("Failed to create gameplay trigger " + Path + " from network message: Got null or invalid volume template.");
			return false;
		}
		Offset = new Vector3(gameplayTrigger.GetOffset().GetX(), gameplayTrigger.GetOffset().GetY(), gameplayTrigger.GetOffset().GetZ());
		RotationOffset = new Vector3(gameplayTrigger.GetRotationOffset().GetX(), gameplayTrigger.GetRotationOffset().GetY(), gameplayTrigger.GetRotationOffset().GetZ());
		ActivationEvent = gameplayTrigger.GetActivationEvent();
		ActivationCheck = gameplayTrigger.GetActivationCheck();
		ActivationVerb = MilMo_Localization.GetLocString(gameplayTrigger.GetActivationVerb());
		CooldownPerPlayer = gameplayTrigger.GetCooldownPerPlayer();
		Cooldown = gameplayTrigger.GetCooldown();
		_noKeyThink = MilMo_Localization.GetLocString(gameplayTrigger.GetNoKeyText());
		MembersOnly = gameplayTrigger.GetMembersOnly() != 0;
		RequiredAvatarLevel = gameplayTrigger.GetRequiredAvatarLevel();
		if (gameplayTrigger.GetKey() != null)
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(gameplayTrigger.GetKey(), delegate(MilMo_Template keyTemplate, bool timeout)
			{
				if (timeout || keyTemplate == null)
				{
					Debug.LogWarning("Failed to load key item template for gameplay trigger " + Path);
				}
				else
				{
					Key = keyTemplate as MilMo_ItemTemplate;
					if (Key == null)
					{
						Debug.LogWarning("Got unexpected template when loading key item template for gameplay trigger " + Path);
					}
				}
			});
		}
		CaptureKey = gameplayTrigger.GetCaptureKey();
		_noCaptureKeyThink = MilMo_Localization.GetLocString(gameplayTrigger.GetNoCaptureKeyText());
		Reactions = (List<GameplayTriggerReaction>)gameplayTrigger.GetReactions();
		ExitArrow exitArrow = gameplayTrigger.GetExitArrow();
		if (exitArrow != null)
		{
			HasExitArrow = true;
			ExitArrowYOffset = exitArrow.GetHeightOffset();
			ExitArrowIdentifier = exitArrow.GetIdentifier();
		}
		return true;
	}

	public static MilMo_GameplayTriggerTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_GameplayTriggerTemplate(category, path, filePath);
	}
}
