namespace Code.Core.Network.types;

public class ActionTime
{
	private readonly string _action;

	private readonly long _time;

	public ActionTime(MessageReader reader)
	{
		_action = reader.ReadString();
		_time = reader.ReadInt64();
	}

	public ActionTime(string action, long time)
	{
		_action = action;
		_time = time;
	}

	public string GetAction()
	{
		return _action;
	}

	public long GetTime()
	{
		return _time;
	}

	public int Size()
	{
		return 10 + MessageWriter.GetSize(_action);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_action);
		writer.WriteInt64(_time);
	}
}
