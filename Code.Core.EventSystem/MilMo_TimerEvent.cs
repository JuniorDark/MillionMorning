using Code.Core.Utility;

namespace Code.Core.EventSystem;

public class MilMo_TimerEvent
{
	public readonly float Time;

	public readonly MilMo_EventAction Action;

	public float TriggerTime;

	public MilMo_TimerEvent Next;

	private bool _paused;

	private float _timeLeftWhenPaused;

	public MilMo_TimerEvent(float aTime, MilMo_EventAction aAction)
	{
		Time = aTime;
		Action = aAction;
		TriggerTime = Time + MilMo_Time.GetCurrentTime();
	}

	public bool Test()
	{
		if (_paused || !(TriggerTime <= MilMo_Time.GetCurrentTime()))
		{
			return false;
		}
		Action.Execute();
		return true;
	}

	public void Pause()
	{
		_paused = true;
		_timeLeftWhenPaused = TriggerTime - MilMo_Time.GetCurrentTime();
	}

	public void Unpause()
	{
		_paused = false;
		TriggerTime = _timeLeftWhenPaused + MilMo_Time.GetCurrentTime();
	}

	private bool Equals(MilMo_TimerEvent other)
	{
		if (Time.Equals(other.Time))
		{
			return object.Equals(Action, other.Action);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((MilMo_TimerEvent)obj);
	}

	public override int GetHashCode()
	{
		return (Time.GetHashCode() * 397) ^ ((Action != null) ? Action.GetHashCode() : 0);
	}

	public MilMo_TimerEvent Copy()
	{
		return new MilMo_TimerEvent(Time, Action);
	}
}
