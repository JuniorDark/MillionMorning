using System.Collections.Generic;

namespace Code.Core.Network.types;

public class LevelObject
{
	public class Factory
	{
		public virtual LevelObject Create(MessageReader reader)
		{
			return new LevelObject(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly string _type;

	private readonly int _id;

	private readonly string _fullLevelName;

	private readonly string _visualRep;

	private readonly vector3 _position;

	private readonly vector3 _rotation;

	private readonly IList<string> _spawnTypes;

	private readonly IList<string> _removalEffects;

	private readonly float _lifespan;

	private const int TYPE_ID = 0;

	static LevelObject()
	{
		ChildFactories = new Factory[6];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new Npc.Factory();
		ChildFactories[2] = new Creature.Factory();
		ChildFactories[3] = new LevelItem.Factory();
		ChildFactories[4] = new GameplayObject.Factory();
		ChildFactories[5] = new Boss.Factory();
	}

	public static LevelObject Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public LevelObject(MessageReader reader)
	{
		_type = reader.ReadString();
		_id = reader.ReadInt32();
		_fullLevelName = reader.ReadString();
		_visualRep = reader.ReadString();
		_position = new vector3(reader);
		_rotation = new vector3(reader);
		_spawnTypes = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_spawnTypes.Add(reader.ReadString());
		}
		_removalEffects = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_removalEffects.Add(reader.ReadString());
		}
		_lifespan = reader.ReadFloat();
	}

	public LevelObject(string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
	{
		_type = type;
		_id = id;
		_fullLevelName = fullLevelName;
		_visualRep = visualRep;
		_position = position;
		_rotation = rotation;
		_spawnTypes = spawnTypes;
		_removalEffects = removalEffects;
		_lifespan = lifespan;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public int GetId()
	{
		return _id;
	}

	public string GetFullLevelName()
	{
		return _fullLevelName;
	}

	public string GetVisualRep()
	{
		return _visualRep;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public vector3 GetRotation()
	{
		return _rotation;
	}

	public IList<string> GetSpawnTypes()
	{
		return _spawnTypes;
	}

	public IList<string> GetRemovalEffects()
	{
		return _removalEffects;
	}

	public float GetLifespan()
	{
		return _lifespan;
	}

	public virtual int Size()
	{
		int num = 42;
		num += MessageWriter.GetSize(_type);
		num += MessageWriter.GetSize(_fullLevelName);
		num += MessageWriter.GetSize(_visualRep);
		num += (short)(2 * _spawnTypes.Count);
		foreach (string spawnType in _spawnTypes)
		{
			num += MessageWriter.GetSize(spawnType);
		}
		num += (short)(2 * _removalEffects.Count);
		foreach (string removalEffect in _removalEffects)
		{
			num += MessageWriter.GetSize(removalEffect);
		}
		return num;
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteInt32(_id);
		writer.WriteString(_fullLevelName);
		writer.WriteString(_visualRep);
		_position.Write(writer);
		_rotation.Write(writer);
		writer.WriteInt16((short)_spawnTypes.Count);
		foreach (string spawnType in _spawnTypes)
		{
			writer.WriteString(spawnType);
		}
		writer.WriteInt16((short)_removalEffects.Count);
		foreach (string removalEffect in _removalEffects)
		{
			writer.WriteString(removalEffect);
		}
		writer.WriteFloat(_lifespan);
	}
}
