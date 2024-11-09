using UnityEngine;

namespace Core.Analytics;

public class FPS
{
	private bool _samplingLevelFps;

	private int _frames;

	private int _fixedFrames;

	private float _lastInterval;

	private float _lastFixedInterval;

	private const float UPDATE_INTERVAL = 0.5f;

	private const float MAX_FPS = 60f;

	private float _fps;

	private float _fixedFps;

	private readonly SampleValueCollector _levelFpsSamples = new SampleValueCollector();

	private readonly SampleValueCollector _accumulatedFpsSamples = new SampleValueCollector();

	public float MedianFps => _accumulatedFpsSamples.Median;

	public float MedianLevelFps => _levelFpsSamples.Median;

	public bool IsSampling => _samplingLevelFps;

	public FPS()
	{
		_lastInterval = Time.realtimeSinceStartup;
		_lastFixedInterval = Time.realtimeSinceStartup;
	}

	public float GetFPS()
	{
		return _fps;
	}

	public float GetFixedFPS()
	{
		return _fixedFps;
	}

	public void StartSampling()
	{
		_samplingLevelFps = true;
	}

	public void StopSampling()
	{
		_samplingLevelFps = false;
		_levelFpsSamples.Clear();
	}

	public void Update()
	{
		_frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(realtimeSinceStartup < _lastInterval + 0.5f))
		{
			_fps = Mathf.Min(60f, (float)_frames / (realtimeSinceStartup - _lastInterval));
			_frames = 0;
			_lastInterval = realtimeSinceStartup;
			_accumulatedFpsSamples.AddSample(_fps);
			if (_samplingLevelFps)
			{
				_levelFpsSamples.AddSample(_fps);
			}
		}
	}

	public void FixedUpdate()
	{
		_fixedFrames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(realtimeSinceStartup < _lastFixedInterval + 0.5f))
		{
			_fixedFps = (float)_fixedFrames / (realtimeSinceStartup - _lastFixedInterval);
			_fixedFrames = 0;
			_lastFixedInterval = realtimeSinceStartup;
		}
	}
}
