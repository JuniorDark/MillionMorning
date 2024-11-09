using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual.Audio;
using UnityEngine;

namespace Code.World;

public sealed class MilMo_SoundArea
{
	public const float VISUALIZATION_VOLUME_THRESHOLD = 0.0048638275f;

	private GameObject _mGameObject;

	public MilMo_SoundArea(GameObject gameObject)
	{
		_mGameObject = gameObject;
	}

	public void Load(MilMo_SFFile file)
	{
		if (_mGameObject == null)
		{
			_mGameObject = new GameObject("Area_Sound");
		}
		MilMo_Audio milMo_Audio = null;
		while (file.NextRow() && !file.IsNext("</SOUNDAREA>"))
		{
			if (file.IsNext("Position"))
			{
				_mGameObject.transform.position = file.GetVector3();
			}
			else if (file.IsNext("<AudioSource>"))
			{
				milMo_Audio = new MilMo_Audio();
				milMo_Audio.Load(file);
			}
		}
		if (milMo_Audio != null)
		{
			milMo_Audio.Create(_mGameObject);
			_mGameObject.name = ((_mGameObject.GetComponent<AudioSourceWrapper>().Clip != null) ? ("Area_Sound" + _mGameObject.GetComponent<AudioSourceWrapper>().Clip.name) : "Area_Sound");
			_mGameObject.transform.localScale = Vector3.one * (0.9951362f / (MilMo_AudioUtils.GetRollOffFactorFromDistance(_mGameObject.GetComponent<AudioSourceWrapper>().MaxDistance) * 0.0048638275f) + 1f);
		}
		else
		{
			Debug.LogWarning("Failed to load audio source for sound area at position " + _mGameObject.transform.position.ToString());
		}
	}

	public void Start()
	{
		if (_mGameObject != null && _mGameObject.GetComponent<AudioSourceWrapper>() != null)
		{
			_mGameObject.GetComponent<AudioSourceWrapper>().Play();
		}
	}

	public void Pause()
	{
		if (_mGameObject != null && _mGameObject.GetComponent<AudioSourceWrapper>() != null)
		{
			_mGameObject.GetComponent<AudioSourceWrapper>().Pause();
		}
	}

	public void Resume()
	{
		if (_mGameObject != null && _mGameObject.GetComponent<AudioSourceWrapper>() != null)
		{
			_mGameObject.GetComponent<AudioSourceWrapper>().Play();
		}
	}

	public void Unload()
	{
		Object.Destroy(_mGameObject);
	}
}
