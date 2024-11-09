namespace Code.Core.Network.types;

public class Timespan
{
	private readonly long _startTime;

	private readonly long _endTime;

	public Timespan(MessageReader reader)
	{
		_startTime = reader.ReadInt64();
		_endTime = reader.ReadInt64();
	}

	public Timespan(long startTime, long endTime)
	{
		_startTime = startTime;
		_endTime = endTime;
	}

	public long GetStartTime()
	{
		return _startTime;
	}

	public long GetEndTime()
	{
		return _endTime;
	}

	public int Size()
	{
		return 16;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt64(_startTime);
		writer.WriteInt64(_endTime);
	}
}
