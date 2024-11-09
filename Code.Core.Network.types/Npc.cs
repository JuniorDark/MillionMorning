using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Npc : LevelObject
{
	public new class Factory : LevelObject.Factory
	{
		public override LevelObject Create(MessageReader reader)
		{
			return new Npc(reader);
		}
	}

	private readonly string _templateIdentifier;

	private readonly string _name;

	private readonly sbyte _nrOfQuests;

	private readonly float _sqrInteractionRange;

	private readonly sbyte _useTurnToPlayer;

	private readonly sbyte _interactionState;

	private readonly string _interactionVerb;

	private readonly ExitArrow _exitArrow;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public Npc(MessageReader reader)
		: base(reader)
	{
		_templateIdentifier = reader.ReadString();
		_name = reader.ReadString();
		_nrOfQuests = reader.ReadInt8();
		_sqrInteractionRange = reader.ReadFloat();
		_useTurnToPlayer = reader.ReadInt8();
		_interactionState = reader.ReadInt8();
		_interactionVerb = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_exitArrow = new ExitArrow(reader);
		}
	}

	public Npc(string templateIdentifier, string name, sbyte nrOfQuests, float sqrInteractionRange, sbyte useTurnToPlayer, sbyte interactionState, string interactionVerb, ExitArrow exitArrow, string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
		: base(type, id, fullLevelName, visualRep, position, rotation, spawnTypes, removalEffects, lifespan)
	{
		_templateIdentifier = templateIdentifier;
		_name = name;
		_nrOfQuests = nrOfQuests;
		_sqrInteractionRange = sqrInteractionRange;
		_useTurnToPlayer = useTurnToPlayer;
		_interactionState = interactionState;
		_interactionVerb = interactionVerb;
		_exitArrow = exitArrow;
	}

	public string GetTemplateIdentifier()
	{
		return _templateIdentifier;
	}

	public string GetName()
	{
		return _name;
	}

	public sbyte GetNrOfQuests()
	{
		return _nrOfQuests;
	}

	public float GetSqrInteractionRange()
	{
		return _sqrInteractionRange;
	}

	public sbyte GetUseTurnToPlayer()
	{
		return _useTurnToPlayer;
	}

	public sbyte GetInteractionState()
	{
		return _interactionState;
	}

	public string GetInteractionVerb()
	{
		return _interactionVerb;
	}

	public ExitArrow GetExitArrow()
	{
		return _exitArrow;
	}

	public override int Size()
	{
		int num = 14 + base.Size();
		num += MessageWriter.GetSize(_templateIdentifier);
		num += MessageWriter.GetSize(_name);
		num += MessageWriter.GetSize(_interactionVerb);
		if (_exitArrow != null)
		{
			num += _exitArrow.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_templateIdentifier);
		writer.WriteString(_name);
		writer.WriteInt8(_nrOfQuests);
		writer.WriteFloat(_sqrInteractionRange);
		writer.WriteInt8(_useTurnToPlayer);
		writer.WriteInt8(_interactionState);
		writer.WriteString(_interactionVerb);
		if (_exitArrow == null)
		{
			writer.WriteInt8(0);
			return;
		}
		writer.WriteInt8(1);
		_exitArrow.Write(writer);
	}
}
