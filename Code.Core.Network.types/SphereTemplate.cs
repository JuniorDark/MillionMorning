namespace Code.Core.Network.types;

public class SphereTemplate : VolumeTemplate
{
	public new class Factory : VolumeTemplate.Factory
	{
		public override VolumeTemplate Create(MessageReader reader)
		{
			return new SphereTemplate(reader);
		}
	}

	private readonly float _sqrRadius;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public SphereTemplate(MessageReader reader)
		: base(reader)
	{
		_sqrRadius = reader.ReadFloat();
	}

	public SphereTemplate(float sqrRadius)
	{
		_sqrRadius = sqrRadius;
	}

	public float GetSqrRadius()
	{
		return _sqrRadius;
	}

	public override int Size()
	{
		return 4;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_sqrRadius);
	}
}
