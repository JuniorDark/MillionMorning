namespace Code.Core.Network.types;

public class InvertedCylinderTemplate : VolumeTemplate
{
	public new class Factory : VolumeTemplate.Factory
	{
		public override VolumeTemplate Create(MessageReader reader)
		{
			return new InvertedCylinderTemplate(reader);
		}
	}

	private readonly float _radius;

	private readonly sbyte _dynamicRadius;

	private readonly float _height;

	private const int TYPE_ID = 4;

	public override int GetTypeId()
	{
		return 4;
	}

	public InvertedCylinderTemplate(MessageReader reader)
		: base(reader)
	{
		_radius = reader.ReadFloat();
		_dynamicRadius = reader.ReadInt8();
		_height = reader.ReadFloat();
	}

	public InvertedCylinderTemplate(float radius, bool dynamicRadius, float height)
	{
		_radius = radius;
		_dynamicRadius = (sbyte)(dynamicRadius ? 1 : 0);
		_height = height;
	}

	public float GetRadius()
	{
		return _radius;
	}

	public bool IsDynamicRadius()
	{
		return _dynamicRadius == 1;
	}

	public float GetHeight()
	{
		return _height;
	}

	public override int Size()
	{
		return 9;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_radius);
		writer.WriteInt8(_dynamicRadius);
		writer.WriteFloat(_height);
	}
}
