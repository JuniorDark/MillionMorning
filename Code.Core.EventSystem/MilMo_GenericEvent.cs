namespace Code.Core.EventSystem;

internal class MilMo_GenericEvent
{
	public readonly string Event;

	public readonly object Data;

	public MilMo_GenericEvent(string aEvent, object aData)
	{
		Event = aEvent;
		Data = aData;
	}
}
