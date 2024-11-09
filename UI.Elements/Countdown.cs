using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Elements;

public class Countdown : MonoBehaviour
{
	[SerializeField]
	private int secondsLeft;

	[SerializeField]
	private UnityEvent<int> onTick;

	[SerializeField]
	private UnityEvent onTimesUp;

	private Coroutine _counter;

	private bool _paused;

	public void Setup(int seconds, UnityAction<int> tick, UnityAction timesUp)
	{
		secondsLeft = seconds;
		if (tick != null)
		{
			onTick.AddListener(tick);
		}
		if (timesUp != null)
		{
			onTimesUp.AddListener(timesUp);
		}
	}

	public void StartCountdown()
	{
		if (_counter == null)
		{
			_counter = StartCoroutine(CountdownToZero());
		}
	}

	public void StopCountdown()
	{
		if (_counter != null)
		{
			StopCoroutine(_counter);
		}
		_counter = null;
	}

	private IEnumerator CountdownToZero()
	{
		onTick?.Invoke(secondsLeft);
		while (secondsLeft > 0)
		{
			yield return new WaitForSeconds(1f);
			if (!_paused)
			{
				secondsLeft--;
				onTick?.Invoke(secondsLeft);
			}
		}
		onTimesUp?.Invoke();
	}

	public void Pause()
	{
		_paused = true;
	}

	public void Resume()
	{
		_paused = false;
	}
}
