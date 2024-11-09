namespace Core.Audio.AudioData;

public struct AudioCueKey
{
	public static AudioCueKey Invalid = new AudioCueKey(-1, null);

	internal int Value;

	internal AudioCueSO AudioCue;

	internal AudioCueKey(int value, AudioCueSO audioCue)
	{
		Value = value;
		AudioCue = audioCue;
	}

	public override bool Equals(object obj)
	{
		if (obj is AudioCueKey audioCueKey && Value == audioCueKey.Value)
		{
			return AudioCue == audioCueKey.AudioCue;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode() ^ AudioCue.GetHashCode();
	}

	public static bool operator ==(AudioCueKey x, AudioCueKey y)
	{
		if (x.Value == y.Value)
		{
			return x.AudioCue == y.AudioCue;
		}
		return false;
	}

	public static bool operator !=(AudioCueKey x, AudioCueKey y)
	{
		return !(x == y);
	}
}
