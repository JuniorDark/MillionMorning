using System.Collections.Generic;
using Core.Audio.AudioData;

namespace Core.Audio.SoundEmitters;

public class SoundEmitterVault
{
	private int _nextUniqueKey;

	private List<AudioCueKey> _emittersKey;

	private List<SoundEmitter[]> _emittersList;

	public SoundEmitterVault()
	{
		_emittersKey = new List<AudioCueKey>();
		_emittersList = new List<SoundEmitter[]>();
	}

	public AudioCueKey GetKey(AudioCueSO cue)
	{
		return new AudioCueKey(_nextUniqueKey++, cue);
	}

	public void Add(AudioCueKey key, SoundEmitter[] emitter)
	{
		_emittersKey.Add(key);
		_emittersList.Add(emitter);
	}

	public AudioCueKey Add(AudioCueSO cue, SoundEmitter[] emitter)
	{
		AudioCueKey key = GetKey(cue);
		_emittersKey.Add(key);
		_emittersList.Add(emitter);
		return key;
	}

	public bool Get(AudioCueKey key, out SoundEmitter[] emitter)
	{
		int num = _emittersKey.FindIndex((AudioCueKey x) => x == key);
		if (num < 0)
		{
			emitter = null;
			return false;
		}
		emitter = _emittersList[num];
		return true;
	}

	public bool Remove(AudioCueKey key)
	{
		int index = _emittersKey.FindIndex((AudioCueKey x) => x == key);
		return RemoveAt(index);
	}

	private bool RemoveAt(int index)
	{
		if (index < 0)
		{
			return false;
		}
		_emittersKey.RemoveAt(index);
		_emittersList.RemoveAt(index);
		return true;
	}
}
