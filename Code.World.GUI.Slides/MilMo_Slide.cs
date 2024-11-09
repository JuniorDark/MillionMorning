using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.World.Slides;
using UnityEngine;

namespace Code.World.GUI.Slides;

public sealed class MilMo_Slide
{
	public delegate void LoadCompleteCallback(bool success);

	public class Sound
	{
		public readonly AudioClip MClip;

		public readonly float MDelay;

		public Sound(AudioClip clip, float delay)
		{
			MClip = clip;
			MDelay = delay;
		}
	}

	private readonly List<Sound> _mSounds = new List<Sound>();

	private bool _mIsDestroyed;

	private readonly MilMo_SlideData _mTemplate;

	public Texture2D Texture { get; private set; }

	public List<Sound> Sounds => _mSounds;

	public MilMo_SlideData Template => _mTemplate;

	public MilMo_Slide(MilMo_SlideData data)
	{
		_mTemplate = data;
	}

	public void Destroy()
	{
		_mIsDestroyed = true;
	}

	public async void Load(LoadCompleteCallback callback)
	{
		foreach (MilMo_SlideData.Sound sound in _mTemplate.Sounds)
		{
			string path = sound.Path;
			float delay = sound.Delay;
			AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(path);
			if (audioClip == null)
			{
				Debug.LogWarning("Error loading MilMo_Slide file: " + path);
				continue;
			}
			Sound item = new Sound(audioClip, delay);
			_mSounds.Add(item);
		}
		if (!string.IsNullOrEmpty(_mTemplate.MusicPath))
		{
			string path = "Content/Sounds/" + _mTemplate.MusicPath;
			if (await MilMo_ResourceManager.Instance.LoadAudioAsync(path) == null)
			{
				Debug.LogWarning("Error loading MilMo_Slide file: " + path);
			}
		}
		if (!string.IsNullOrEmpty(_mTemplate.ImagePath))
		{
			string path = "Content/GUI/" + _mTemplate.ImagePath;
			Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
			if (texture2D == null)
			{
				Debug.LogWarning("Error loading MilMo_Slide file: " + path);
			}
			Texture = texture2D;
		}
		if (_mIsDestroyed)
		{
			callback(success: false);
		}
		else
		{
			callback(success: true);
		}
	}
}
