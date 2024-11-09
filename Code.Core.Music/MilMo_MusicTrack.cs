using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Music;

public class MilMo_MusicTrack
{
	private const string MUSIC_BASE_PATH = "Content/Sounds/";

	private readonly string _mFullPath;

	private readonly string _mFullLODPath;

	private bool _mDestroyed;

	public string Path { get; private set; }

	public string AssetTag { get; private set; }

	public AudioClip MainClip { get; private set; }

	public AudioClip LOD { get; private set; }

	public int LODSampleOffset { get; private set; }

	public MilMo_MusicTrack(string path, string assetTag, int lodSampleOffset)
	{
		Path = path;
		AssetTag = assetTag;
		_mFullPath = "Content/Sounds/" + path;
		_mFullLODPath = _mFullPath + "LOD";
		LODSampleOffset = lodSampleOffset;
	}

	public async Task<bool> LoadLODAsync()
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_mFullLODPath);
		if (audioClip == null)
		{
			return false;
		}
		if (_mDestroyed)
		{
			MilMo_ResourceManager.Instance.UnloadAsset(_mFullLODPath);
			return false;
		}
		LOD = audioClip;
		return true;
	}

	public async Task<bool> LoadMusicAsync()
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_mFullPath, "Generic", MilMo_ResourceManager.Priority.Low);
		if (audioClip == null)
		{
			return false;
		}
		if (_mDestroyed)
		{
			MilMo_ResourceManager.Instance.UnloadAsset(_mFullPath);
			return false;
		}
		MainClip = audioClip;
		return true;
	}

	public void Destroy()
	{
		_mDestroyed = true;
		MainClip = null;
		LOD = null;
		MilMo_ResourceManager.Instance.UnloadAsset(_mFullLODPath);
		MilMo_ResourceManager.Instance.UnloadAsset(_mFullPath);
	}
}
