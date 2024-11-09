namespace Code.Core.Network.types;

public class InvertedCircleTemplate : VolumeTemplate
{
	public new class Factory : VolumeTemplate.Factory
	{
		public override VolumeTemplate Create(MessageReader reader)
		{
			return new InvertedCircleTemplate(reader);
		}
	}

	private readonly float _radius;

	private readonly sbyte _dynamicRadius;

	private const int TYPE_ID = 3;

	public override int GetTypeId()
	{
		return 3;
	}

	public InvertedCircleTemplate(MessageReader reader)
		: base(reader)
	{
		_radius = reader.ReadFloat();
		_dynamicRadius = reader.ReadInt8();
	}

	public InvertedCircleTemplate(float radius, bool dynamicRadius)
	{
		_radius = radius;
		_dynamicRadius = (sbyte)(dynamicRadius ? 1 : 0);
	}

	public float GetRadius()
	{
		return _radius;
	}

	public bool IsDynamicRadius()
	{
		return _dynamicRadius == 1;
	}

	public override int Size()
	{
		return 5;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_radius);
		writer.WriteInt8(_dynamicRadius);
	}
}
