namespace Code.Core.EventSystem;

public class MilMo_SimpleAction : MilMo_EventAction
{
	private readonly MilMo_EventSystem.MilMo_Callback _callback;

	public MilMo_SimpleAction(MilMo_EventSystem.MilMo_Callback aCallback)
	{
		_callback = aCallback;
	}

	public override void Execute()
	{
		_callback?.Invoke();
	}

	public override string ToString()
	{
		if (_callback == null || _callback.Target == null)
		{
			return "null";
		}
		return _callback.Target?.ToString() + "::" + _callback.Method.Name;
	}
}
