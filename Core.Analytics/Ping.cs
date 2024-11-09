using System.Collections;
using UnityEngine;

namespace Core.Analytics;

public class Ping
{
	private readonly Queue _packetSentTimes = new Queue();

	private readonly Queue _pingTimes = new Queue();

	private readonly SampleValueCollector _accumulatedPingSamples = new SampleValueCollector();

	public float MedianPing => _accumulatedPingSamples.Median;

	public IEnumerable GetPingTimes()
	{
		return _pingTimes.ToArray();
	}

	public void PositionUpdateSent()
	{
		_packetSentTimes.Enqueue(Time.realtimeSinceStartup);
	}

	public void PositionUpdateReceived()
	{
		if (_packetSentTimes.Count > 0)
		{
			float num = (float)_packetSentTimes.Dequeue();
			float num2 = Time.realtimeSinceStartup - num;
			_accumulatedPingSamples.AddSample(num2);
			_pingTimes.Enqueue(num2);
			if (_pingTimes.Count > 10)
			{
				_pingTimes.Dequeue();
			}
		}
	}
}
