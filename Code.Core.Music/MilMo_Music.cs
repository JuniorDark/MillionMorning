using System;
using System.Collections.Generic;
using Code.Core.Command;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Core.Audio;
using Core.GameEvent;
using Core.Settings;
using UnityEngine;
using UnityEngine.Audio;

namespace Code.Core.Music;

public class MilMo_Music
{
	private class MusicSettings
	{
		public readonly int LODSampleOffset;

		public MusicSettings(int lodSampleOffset)
		{
			LODSampleOffset = lodSampleOffset;
		}
	}

	public delegate void MusicStartedCallback(float clipLength);

	private const string MUSIC_SETTINGS_PATH = "Sound/Music/MusicSettings";

	private static bool _debugMode = MilMo_Config.Instance.IsTrue("Debug.Audio", defaultValue: false);

	private static float _defaultVolume = 0.5f;

	private MilMo_MusicTrack _currentMusicTrack;

	private AudioSource _currentTrack;

	private AudioSource _backTrack;

	private float _wantedVolume = _defaultVolume;

	private float _fadeInSpeed = 2f;

	private float _fadeOutSpeed = 2f;

	private string _currentPath = "";

	private static MilMo_Music _theInstance;

	private bool _enabled = true;

	private string _nextToPlay;

	private readonly Dictionary<string, MilMo_MusicTrack> _loadedTracks = new Dictionary<string, MilMo_MusicTrack>();

	private readonly Dictionary<string, MilMo_MusicTrack> _loadingTracks = new Dictionary<string, MilMo_MusicTrack>();

	private static readonly Dictionary<string, MusicSettings> TheMusicSettings = new Dictionary<string, MusicSettings>();

	private bool _playingLOD;

	private bool _playingMain;

	public static MilMo_Music Instance
	{
		get
		{
			if (_theInstance != null)
			{
				return _theInstance;
			}
			_theInstance = new MilMo_Music();
			_theInstance.Create();
			MilMo_Command.Instance.RegisterCommand("NoSound", Debug_NoSound);
			MilMo_Command.Instance.RegisterCommand("Music.DebugMode", Debug_DebugMode);
			return _theInstance;
		}
	}

	public float Volume
	{
		set
		{
			_defaultVolume = ((value > 0f) ? value : 0f);
			_wantedVolume = ((value > 0f) ? value : 0f);
			float num = _fadeInSpeed * _wantedVolume;
			_fadeInSpeed = ((num > 0f) ? (1f / num) : 0.5f);
			_fadeOutSpeed = ((num > 0f) ? (1f / num) : 0.5f);
		}
	}

	private MilMo_Music()
	{
	}

	private void Create()
	{
		if (MilMo_Global.MainGameObject == null)
		{
			Debug.LogWarning("Trying to create MilMo_Music when main object is null");
			return;
		}
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("Sound/Music/MusicSettings");
		if (milMo_SFFile != null)
		{
			while (milMo_SFFile.NextRow())
			{
				string @string = milMo_SFFile.GetString();
				int @int = milMo_SFFile.GetInt();
				TheMusicSettings.Add(@string, new MusicSettings(@int));
			}
		}
		Volume = Settings.MusicVolume;
		GameEvent.MusicVolumeEvent.RegisterAction(UpdateVolume);
		_enabled = MilMo_Config.Instance.IsTrue("Sound.Music", defaultValue: true);
		_currentTrack = CreateMusicAudioSource();
		_backTrack = CreateMusicAudioSource();
		MilMo_EventSystem.RegisterUpdate(Update);
	}

	private void SetOutputMixer(AudioSource source)
	{
		AudioManager audioManager = UnityEngine.Object.FindObjectOfType<AudioManager>();
		if (audioManager == null)
		{
			Debug.LogWarning("Unable to find audio manager");
			return;
		}
		AudioMixer mixer = audioManager.GetMixer();
		string subPath = "MusicVolume";
		source.outputAudioMixerGroup = mixer.FindMatchingGroups(subPath)[0];
	}

	private void UpdateVolume(float f)
	{
		Volume = f;
	}

	private void Update(object obj)
	{
		if (!_enabled)
		{
			return;
		}
		if (_debugMode)
		{
			Debug.Log(_playingLOD ? "Playing LOD" : (_playingMain ? "Playing MAIN" : "Playing NONE"));
		}
		if (_currentTrack != null)
		{
			if (_currentTrack.volume < _wantedVolume)
			{
				_currentTrack.volume = Mathf.Min(_wantedVolume, _currentTrack.volume + _fadeInSpeed * Time.deltaTime);
			}
			else if (_currentTrack.volume > _wantedVolume)
			{
				_currentTrack.volume = Mathf.Max(_wantedVolume, _currentTrack.volume - _fadeOutSpeed * Time.deltaTime);
			}
		}
		if (_backTrack != null && _backTrack.volume > 0f)
		{
			_backTrack.volume = Mathf.Max(0f, _backTrack.volume - _fadeOutSpeed * Time.deltaTime);
		}
	}

	private void PlayLOD(MilMo_MusicTrack musicTrack)
	{
		if (_debugMode)
		{
			Debug.Log("MUSIC: PlayMusicLOD " + musicTrack.Path);
		}
		if (musicTrack.Path == _currentPath)
		{
			return;
		}
		if (musicTrack.Path == _nextToPlay)
		{
			AudioSource backTrack = _backTrack;
			_backTrack = CreateMusicAudioSource();
			if (_backTrack != null)
			{
				_backTrack.clip = _currentTrack.clip;
				_backTrack.volume = _currentTrack.volume;
				_backTrack.loop = true;
				_backTrack.Play();
				_backTrack.time = _currentTrack.time;
			}
			UnityEngine.Object.Destroy(backTrack);
			AudioSource currentTrack = _currentTrack;
			_currentTrack = CreateMusicAudioSource();
			if (_currentTrack != null)
			{
				_currentTrack.clip = musicTrack.LOD;
				_currentTrack.volume = 0f;
				_currentTrack.loop = true;
				_currentTrack.Play();
			}
			UnityEngine.Object.Destroy(currentTrack);
			_currentMusicTrack = musicTrack;
			_currentPath = musicTrack.Path;
			_playingLOD = true;
			_playingMain = false;
		}
		else if (_debugMode)
		{
			Debug.Log("Music streaming was done for audio clip (LOD) that isn't the next to be played.");
		}
	}

	private void PlayMusic(MilMo_MusicTrack musicTrack)
	{
		if (_debugMode)
		{
			Debug.Log("MUSIC: PlayMusic " + musicTrack.Path);
		}
		if (musicTrack.Path == _nextToPlay)
		{
			if (musicTrack.Path == _currentPath)
			{
				int timeSamples = _currentTrack.timeSamples;
				float volume = _currentTrack.volume;
				_currentTrack.clip = musicTrack.MainClip;
				_currentTrack.volume = volume;
				_currentTrack.loop = true;
				_currentTrack.Play();
				_currentTrack.timeSamples = musicTrack.LODSampleOffset + timeSamples + 1;
			}
			else
			{
				AudioSource backTrack = _backTrack;
				_backTrack = CreateMusicAudioSource();
				if (_backTrack != null)
				{
					_backTrack.clip = _currentTrack.clip;
					_backTrack.volume = _currentTrack.volume;
					_backTrack.loop = true;
					_backTrack.Play();
					_backTrack.time = _currentTrack.time;
				}
				UnityEngine.Object.Destroy(backTrack);
				AudioSource currentTrack = _currentTrack;
				_currentTrack = CreateMusicAudioSource();
				if (_currentTrack != null)
				{
					_currentTrack.clip = musicTrack.MainClip;
					_currentTrack.volume = 0f;
					_currentTrack.loop = true;
					_currentTrack.Play();
				}
				UnityEngine.Object.Destroy(currentTrack);
				_currentMusicTrack = musicTrack;
				_currentPath = musicTrack.Path;
			}
			_playingMain = true;
			_playingLOD = false;
		}
		else if (_debugMode)
		{
			Debug.Log("Music streaming was done for audio clip that isn't the next to be played");
		}
	}

	private AudioSource CreateMusicAudioSource()
	{
		AudioSource audioSource = MilMo_Global.AudioListener.AddComponent<AudioSource>();
		if (audioSource != null)
		{
			SetOutputMixer(audioSource);
		}
		return audioSource;
	}

	public void StopCurrent()
	{
		if (_debugMode)
		{
			Debug.Log("MUSIC: stop current");
		}
		AudioSource backTrack = _backTrack;
		_backTrack = CreateMusicAudioSource();
		if (_backTrack != null)
		{
			_backTrack.clip = _currentTrack.clip;
			_backTrack.volume = _currentTrack.volume;
			_backTrack.loop = true;
			_backTrack.Play();
			_backTrack.time = _currentTrack.time;
		}
		UnityEngine.Object.Destroy(backTrack);
		_currentTrack.Stop();
		_currentTrack.clip = null;
		_currentTrack.volume = 0f;
		_currentMusicTrack = null;
		_currentPath = "";
		_nextToPlay = "";
		_playingLOD = false;
		_playingMain = false;
	}

	public void FadeIn(string path, MusicStartedCallback callback = null)
	{
		FadeIn(path, "Generic", callback);
	}

	private void FadeIn(string path, string assetTag = "Generic", MusicStartedCallback callback = null)
	{
		if (!_enabled)
		{
			return;
		}
		if (_debugMode)
		{
			Debug.Log("MUSIC: FadeIn " + path);
		}
		if (path.Equals(_currentPath, StringComparison.InvariantCultureIgnoreCase))
		{
			float time = _currentTrack.time;
			callback?.Invoke(_currentTrack.clip.length - time);
			return;
		}
		_nextToPlay = path;
		if (!_loadingTracks.ContainsKey(path))
		{
			if (_loadedTracks.TryGetValue(path, out var value))
			{
				PlayMusic(value);
				callback?.Invoke(value.MainClip.length);
				return;
			}
			TheMusicSettings.TryGetValue(path, out var value2);
			MilMo_MusicTrack musicTrack = new MilMo_MusicTrack(path, assetTag, value2?.LODSampleOffset ?? 0);
			LoadAndPlayMusicTrackAsync(musicTrack, callback);
			LoadAndPlayLODAsync(musicTrack);
		}
	}

	private async void LoadAndPlayMusicTrackAsync(MilMo_MusicTrack musicTrack, MusicStartedCallback callback)
	{
		_loadingTracks.Add(musicTrack.Path, musicTrack);
		bool num = await musicTrack.LoadMusicAsync();
		_loadingTracks.Remove(musicTrack.Path);
		if (num)
		{
			_loadedTracks.Add(musicTrack.Path, musicTrack);
			PlayMusic(musicTrack);
			callback?.Invoke(musicTrack.MainClip.length);
		}
	}

	private async void LoadAndPlayLODAsync(MilMo_MusicTrack musicTrack)
	{
		if (await musicTrack.LoadLODAsync())
		{
			PlayLOD(musicTrack);
		}
	}

	public void UnloadAllByTag(string assetTag)
	{
		if (_currentMusicTrack != null && _currentMusicTrack.AssetTag == assetTag)
		{
			StopCurrent();
		}
		List<string> list = new List<string>();
		foreach (MilMo_MusicTrack value in _loadedTracks.Values)
		{
			if (value.AssetTag == assetTag)
			{
				list.Add(value.Path);
				value.Destroy();
			}
		}
		foreach (string item in list)
		{
			_loadedTracks.Remove(item);
		}
		list.Clear();
		foreach (MilMo_MusicTrack value2 in _loadingTracks.Values)
		{
			if (value2.AssetTag == assetTag)
			{
				list.Add(value2.Path);
				value2.Destroy();
			}
		}
		foreach (string item2 in list)
		{
			_loadingTracks.Remove(item2);
		}
		list.Clear();
	}

	public void FadeToVolume(float volumeMultiplier, float speedInSeconds)
	{
		_fadeInSpeed = speedInSeconds;
		_fadeOutSpeed = speedInSeconds;
		_wantedVolume = _defaultVolume * volumeMultiplier;
	}

	public void ResetVolume(float speedInSeconds)
	{
		_fadeInSpeed = speedInSeconds;
		_fadeOutSpeed = speedInSeconds;
		_wantedVolume = _defaultVolume;
	}

	private static string Debug_NoSound(string[] args)
	{
		AudioListener.volume = 0f;
		return "Audio volume set to zero";
	}

	private static string Debug_DebugMode(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Music.Debug 1|0";
		}
		_debugMode = MilMo_Utility.StringToInt(args[1]) != 0;
		return "Debug mode " + (_debugMode ? "activated" : "deactivated");
	}
}
