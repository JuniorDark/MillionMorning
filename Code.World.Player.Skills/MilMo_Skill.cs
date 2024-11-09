using System;
using System.Threading.Tasks;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Core;
using UI.Elements.Slot;
using UnityEngine;

namespace Code.World.Player.Skills;

public class MilMo_Skill : IEntryItem, IUsable, IHaveCooldown, IAssignable, IEquatable<MilMo_Skill>
{
	private Action _onUsed;

	private Action _onFail;

	private float _activeCooldown;

	private float _activationTime;

	public MilMo_LocString Name { get; private set; }

	public MilMo_LocString Description { get; private set; }

	public string Icon { get; private set; }

	private Texture2D IconTexture { get; set; }

	public float Cooldown { get; private set; }

	public string ClassName { get; private set; }

	public sbyte Level { get; private set; }

	public sbyte ModeId { get; private set; }

	public MilMo_SkillTemplate Template { get; set; }

	public Texture2D GetItemIcon()
	{
		return IconTexture;
	}

	public MilMo_LocString GetDisplayName()
	{
		return Name;
	}

	public MilMo_LocString GetDescription()
	{
		return Description;
	}

	public async Task<Texture2D> AsyncGetIcon()
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(Icon);
		if (!texture2D)
		{
			Debug.LogWarning("could not load icon for: " + Name);
		}
		IconTexture = texture2D;
		return texture2D;
	}

	public float GetTimeRemaining()
	{
		return Math.Max(_activationTime + _activeCooldown - Time.time, 0f);
	}

	public bool TestCooldownExpired()
	{
		return Time.time - _activationTime > _activeCooldown;
	}

	public float GetCooldownProgress()
	{
		if (_activeCooldown != 0f)
		{
			return Mathf.Clamp01((Time.time - _activationTime) / _activeCooldown);
		}
		return 1f;
	}

	public float GetCooldown()
	{
		return Cooldown;
	}

	public void RegisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Combine(_onUsed, onUsed);
	}

	public void UnregisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Remove(_onUsed, onUsed);
	}

	public void RegisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Combine(_onFail, onFail);
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Combine(_onFail, onFail);
	}

	public bool Use(int entryId)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		if (!TestCooldownExpired())
		{
			return false;
		}
		Vector3 position = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		Vector3 eulerAngles = MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles;
		IMilMo_AttackTarget milMo_AttackTarget = ((MilMo_Player.Instance.Avatar.WieldedItem is MilMo_Weapon milMo_Weapon) ? MilMo_Level.CurrentLevel.GetClosestTarget(position, MilMo_Player.Instance.Avatar.GameObject.transform.forward, milMo_Weapon.Template, useHitRange: true) : null);
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientSkillActivate(ClassName, Level, ModeId, milMo_AttackTarget?.AsNetworkAttackTarget(), new vector3(position.x, position.y, position.z), new vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z)));
		return true;
	}

	public string GetSaveString(int entryId = 0)
	{
		return "Skill:" + ClassName + ":" + Level + ":" + ModeId;
	}

	public MilMo_Skill(string name, string description, float cooldown, string icon, string className, sbyte level, sbyte modeId)
	{
		Name = MilMo_Localization.GetLocString(name);
		Description = MilMo_Localization.GetLocString(description);
		Cooldown = cooldown;
		Icon = "Content/GUI/Batch01/Textures/Abilities/" + icon;
		ClassName = className;
		Level = level;
		ModeId = modeId;
		_activationTime = 0f;
	}

	public void WasActivated(float cooldownToUse)
	{
		Debug.LogWarning("MilMo_Skill: Activated");
		_activeCooldown = cooldownToUse;
		_activationTime = Time.time;
		_onUsed?.Invoke();
	}

	public void FailedToActivate()
	{
		_onFail?.Invoke();
	}

	public bool Equals(MilMo_Skill other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (ClassName == other.ClassName && Level == other.Level)
		{
			return ModeId == other.ModeId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((MilMo_Skill)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(ClassName, Level, ModeId);
	}
}
