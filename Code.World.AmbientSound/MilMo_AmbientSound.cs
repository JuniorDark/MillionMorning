using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Core.Utilities;
using UnityEngine;

namespace Code.World.AmbientSound;

public abstract class MilMo_AmbientSound
{
	private const float VOLUME_INTERPOLATION_SPEED = 10f;

	private const int DIRECTION_BUFFER_SIZE = 16;

	private Vector2 _heightRange = new Vector2(0f, 35f);

	private readonly LinkedList<Vector2> _directionBuffer = new LinkedList<Vector2>();

	private GameObject _source;

	public Vector2 HeightRange => _heightRange;

	public int Channel { get; }

	protected AudioSourceWrapper Wrapper { get; private set; }

	public Vector3 Position
	{
		set
		{
			_source.transform.position = value;
		}
	}

	public float Volume { get; set; }

	protected MilMo_AmbientSound(int channel)
	{
		Channel = channel;
		for (int i = 0; i < 16; i++)
		{
			_directionBuffer.AddLast(new Vector2(0f, 1f));
		}
	}

	public void SetSource(GameObject source)
	{
		_source = source;
		_source.transform.parent = MilMo_Global.AudioListener.transform;
		Wrapper = _source.AddComponent<AudioSourceWrapper>();
		Sound.SetOutputMixer(Wrapper.Source, "AmbienceVolume");
		Wrapper.Volume = 0f;
	}

	public void Update()
	{
		if (!(Wrapper == null) && Math.Abs(Wrapper.Volume - Volume) > 0.01f)
		{
			Wrapper.Volume = Mathf.Lerp(Wrapper.Volume, Volume, 10f * Time.deltaTime);
		}
	}

	public Vector2 SmoothDirection(Vector2 direction)
	{
		if (_directionBuffer.Count == 0)
		{
			return Vector2.zero;
		}
		_directionBuffer.AddLast(direction);
		_directionBuffer.RemoveFirst();
		return _directionBuffer.Aggregate(Vector2.zero, (Vector2 current, Vector2 d) => current + d) / _directionBuffer.Count;
	}

	public void Mute()
	{
		if (!(Wrapper == null))
		{
			Wrapper.enabled = false;
		}
	}

	public void Unmute()
	{
		if (!(Wrapper == null))
		{
			Wrapper.enabled = true;
		}
	}

	public bool Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("</AMBIENTSOUND>"))
			{
				FinishReading();
				return true;
			}
			if (!ReadLine(file))
			{
				return false;
			}
		}
		return false;
	}

	protected virtual bool ReadLine(MilMo_SFFile file)
	{
		if (!file.IsNext("Height"))
		{
			return true;
		}
		_heightRange.x = file.GetFloat();
		_heightRange.y = (file.HasMoreTokens() ? file.GetFloat() : _heightRange.x);
		return true;
	}

	protected virtual void FinishReading()
	{
	}

	public abstract void Play();

	public void Destroy()
	{
		MilMo_Global.Destroy(_source);
		_source = null;
		Wrapper = null;
	}
}
