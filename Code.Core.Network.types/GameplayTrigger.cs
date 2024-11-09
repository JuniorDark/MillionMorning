using System.Collections.Generic;

namespace Code.Core.Network.types;

public class GameplayTrigger : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new GameplayTrigger(reader);
		}
	}

	private readonly vector3 _offset;

	private readonly vector3 _rotationOffset;

	private readonly VolumeTemplate _volume;

	private readonly sbyte _activationEvent;

	private readonly string _activationVerb;

	private readonly sbyte _activationCheck;

	private readonly float _cooldown;

	private readonly float _cooldownPerPlayer;

	private readonly TemplateReference _key;

	private readonly string _noKeyText;

	private readonly string _captureKey;

	private readonly string _noCaptureKeyText;

	private readonly sbyte _membersOnly;

	private readonly sbyte _requiredAvatarLevel;

	private readonly IList<GameplayTriggerReaction> _reactions;

	private readonly ExitArrow _exitArrow;

	private const int TYPE_ID = 9;

	public override int GetTypeId()
	{
		return 9;
	}

	public GameplayTrigger(MessageReader reader)
		: base(reader)
	{
		_offset = new vector3(reader);
		_rotationOffset = new vector3(reader);
		_volume = VolumeTemplate.Create(reader.ReadTypeCode(), reader);
		_activationEvent = reader.ReadInt8();
		_activationVerb = reader.ReadString();
		_activationCheck = reader.ReadInt8();
		_cooldown = reader.ReadFloat();
		_cooldownPerPlayer = reader.ReadFloat();
		if (reader.ReadInt8() == 1)
		{
			_key = new TemplateReference(reader);
		}
		_noKeyText = reader.ReadString();
		_captureKey = reader.ReadString();
		_noCaptureKeyText = reader.ReadString();
		_membersOnly = reader.ReadInt8();
		_requiredAvatarLevel = reader.ReadInt8();
		_reactions = new List<GameplayTriggerReaction>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_reactions.Add(new GameplayTriggerReaction(reader));
		}
		if (reader.ReadInt8() == 1)
		{
			_exitArrow = new ExitArrow(reader);
		}
	}

	public GameplayTrigger(vector3 offset, vector3 rotationOffset, VolumeTemplate volume, sbyte activationEvent, string activationVerb, sbyte activationCheck, float cooldown, float cooldownPerPlayer, TemplateReference key, string noKeyText, string captureKey, string noCaptureKeyText, sbyte membersOnly, sbyte requiredAvatarLevel, IList<GameplayTriggerReaction> reactions, ExitArrow exitArrow, string type, TemplateReference reference)
		: base(type, reference)
	{
		_offset = offset;
		_rotationOffset = rotationOffset;
		_volume = volume;
		_activationEvent = activationEvent;
		_activationVerb = activationVerb;
		_activationCheck = activationCheck;
		_cooldown = cooldown;
		_cooldownPerPlayer = cooldownPerPlayer;
		_key = key;
		_noKeyText = noKeyText;
		_captureKey = captureKey;
		_noCaptureKeyText = noCaptureKeyText;
		_membersOnly = membersOnly;
		_requiredAvatarLevel = requiredAvatarLevel;
		_reactions = reactions;
		_exitArrow = exitArrow;
	}

	public vector3 GetOffset()
	{
		return _offset;
	}

	public vector3 GetRotationOffset()
	{
		return _rotationOffset;
	}

	public VolumeTemplate GetVolume()
	{
		return _volume;
	}

	public sbyte GetActivationEvent()
	{
		return _activationEvent;
	}

	public string GetActivationVerb()
	{
		return _activationVerb;
	}

	public sbyte GetActivationCheck()
	{
		return _activationCheck;
	}

	public float GetCooldown()
	{
		return _cooldown;
	}

	public float GetCooldownPerPlayer()
	{
		return _cooldownPerPlayer;
	}

	public TemplateReference GetKey()
	{
		return _key;
	}

	public string GetNoKeyText()
	{
		return _noKeyText;
	}

	public string GetCaptureKey()
	{
		return _captureKey;
	}

	public string GetNoCaptureKeyText()
	{
		return _noCaptureKeyText;
	}

	public sbyte GetMembersOnly()
	{
		return _membersOnly;
	}

	public sbyte GetRequiredAvatarLevel()
	{
		return _requiredAvatarLevel;
	}

	public IList<GameplayTriggerReaction> GetReactions()
	{
		return _reactions;
	}

	public ExitArrow GetExitArrow()
	{
		return _exitArrow;
	}

	public override int Size()
	{
		int num = 50 + base.Size();
		num += _volume.Size();
		num += MessageWriter.GetSize(_activationVerb);
		if (_key != null)
		{
			num += _key.Size();
		}
		num += MessageWriter.GetSize(_noKeyText);
		num += MessageWriter.GetSize(_captureKey);
		num += MessageWriter.GetSize(_noCaptureKeyText);
		foreach (GameplayTriggerReaction reaction in _reactions)
		{
			num += reaction.Size();
		}
		if (_exitArrow != null)
		{
			num += _exitArrow.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_offset.Write(writer);
		_rotationOffset.Write(writer);
		writer.WriteTypeCode(_volume.GetTypeId());
		_volume.Write(writer);
		writer.WriteInt8(_activationEvent);
		writer.WriteString(_activationVerb);
		writer.WriteInt8(_activationCheck);
		writer.WriteFloat(_cooldown);
		writer.WriteFloat(_cooldownPerPlayer);
		if (_key == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			_key.Write(writer);
		}
		writer.WriteString(_noKeyText);
		writer.WriteString(_captureKey);
		writer.WriteString(_noCaptureKeyText);
		writer.WriteInt8(_membersOnly);
		writer.WriteInt8(_requiredAvatarLevel);
		writer.WriteInt16((short)_reactions.Count);
		foreach (GameplayTriggerReaction reaction in _reactions)
		{
			reaction.Write(writer);
		}
		if (_exitArrow == null)
		{
			writer.WriteInt8(0);
			return;
		}
		writer.WriteInt8(1);
		_exitArrow.Write(writer);
	}
}
