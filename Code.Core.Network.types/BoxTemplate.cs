namespace Code.Core.Network.types;

public class BoxTemplate : VolumeTemplate
{
	public new class Factory : VolumeTemplate.Factory
	{
		public override VolumeTemplate Create(MessageReader reader)
		{
			return new BoxTemplate(reader);
		}
	}

	private readonly vector3 _halfSize;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public BoxTemplate(MessageReader reader)
		: base(reader)
	{
		_halfSize = new vector3(reader);
	}

	public BoxTemplate(vector3 halfSize)
	{
		_halfSize = halfSize;
	}

	public vector3 GetHalfSize()
	{
		return _halfSize;
	}

	public override int Size()
	{
		return 12;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_halfSize.Write(writer);
	}
}
