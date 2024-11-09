namespace Code.Core.Network.types;

public class CreatureUpdate : LevelObjectUpdate
{
	public new class Factory : LevelObjectUpdate.Factory
	{
		public override LevelObjectUpdate Create(MessageReader reader)
		{
			return new CreatureUpdate(reader);
		}
	}

	private readonly vector3 _position;

	private readonly vector3 _target;

	private readonly float _speed;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public CreatureUpdate(MessageReader reader)
		: base(reader)
	{
		_position = new vector3(reader);
		_target = new vector3(reader);
		_speed = reader.ReadFloat();
	}

	public CreatureUpdate(vector3 position, vector3 target, float speed, int id)
		: base(id)
	{
		_position = position;
		_target = target;
		_speed = speed;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public vector3 GetTarget()
	{
		return _target;
	}

	public float GetSpeed()
	{
		return _speed;
	}

	public override int Size()
	{
		return 32;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_position.Write(writer);
		_target.Write(writer);
		writer.WriteFloat(_speed);
	}
}
