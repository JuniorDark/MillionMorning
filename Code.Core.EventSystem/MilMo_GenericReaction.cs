namespace Code.Core.EventSystem;

public class MilMo_GenericReaction
{
	private readonly MilMo_EventSystem.MilMo_EventCallback _callback;

	public bool HighPriority { get; set; }

	public object CustomData { get; set; }

	public bool Repeating { get; set; }

	public bool Active { get; set; }

	public string Event { get; private set; }

	public MilMo_GenericReaction(string eventStr, MilMo_EventSystem.MilMo_EventCallback callback)
	{
		Active = true;
		Event = eventStr;
		_callback = callback;
	}

	public void Execute(object data)
	{
		_callback(data);
	}

	public override string ToString()
	{
		return ((_callback.Target != null) ? _callback.Target.ToString() : "") + "::" + _callback.Method.Name + "@" + Event + "(" + Repeating + ")";
	}
}
