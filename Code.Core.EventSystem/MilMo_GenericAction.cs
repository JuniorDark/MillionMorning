namespace Code.Core.EventSystem;

public class MilMo_GenericAction : MilMo_EventAction
{
	public delegate void Callback(object o);

	private readonly Callback _callback;

	private readonly object _arg;

	public MilMo_GenericAction(Callback callback, object arg)
	{
		_callback = callback;
		_arg = arg;
	}

	public override void Execute()
	{
		_callback?.Invoke(_arg);
	}

	public override string ToString()
	{
		if (_callback == null)
		{
			return "null";
		}
		return _callback.Target?.ToString() + "::" + _callback.Method.Name;
	}
}
