namespace Code.Core.Network.types;

public class AnimationPoint
{
	private readonly float _time;

	private readonly string _animation;

	private readonly sbyte _alignAxis;

	public AnimationPoint(MessageReader reader)
	{
		_time = reader.ReadFloat();
		_animation = reader.ReadString();
		_alignAxis = reader.ReadInt8();
	}

	public AnimationPoint(float time, string animation, sbyte alignAxis)
	{
		_time = time;
		_animation = animation;
		_alignAxis = alignAxis;
	}

	public float GetTime()
	{
		return _time;
	}

	public string GetAnimation()
	{
		return _animation;
	}

	public sbyte GetAlignAxis()
	{
		return _alignAxis;
	}

	public int Size()
	{
		return 7 + MessageWriter.GetSize(_animation);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteFloat(_time);
		writer.WriteString(_animation);
		writer.WriteInt8(_alignAxis);
	}
}
