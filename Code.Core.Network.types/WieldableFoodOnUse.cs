namespace Code.Core.Network.types;

public class WieldableFoodOnUse
{
	private readonly sbyte _useNumber;

	private readonly TemplateReference _playerState;

	private readonly float _duration;

	public WieldableFoodOnUse(MessageReader reader)
	{
		_useNumber = reader.ReadInt8();
		_playerState = new TemplateReference(reader);
		_duration = reader.ReadFloat();
	}

	public WieldableFoodOnUse(sbyte useNumber, TemplateReference playerState, float duration)
	{
		_useNumber = useNumber;
		_playerState = playerState;
		_duration = duration;
	}

	public sbyte GetUseNumber()
	{
		return _useNumber;
	}

	public TemplateReference GetPlayerState()
	{
		return _playerState;
	}

	public float GetDuration()
	{
		return _duration;
	}

	public int Size()
	{
		return 5 + _playerState.Size();
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt8(_useNumber);
		_playerState.Write(writer);
		writer.WriteFloat(_duration);
	}
}
