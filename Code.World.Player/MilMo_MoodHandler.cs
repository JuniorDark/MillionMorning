using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;
using Player.Moods;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.World.Player;

public class MilMo_MoodHandler
{
	private readonly MilMo_Avatar _avatar;

	private readonly List<Mood> _moods;

	private Mood _currentMood;

	private Mood _persistedMood;

	private MilMo_GenericReaction _updateMoodReaction;

	public bool MoodLocked { private get; set; }

	public MilMo_MoodHandler(MilMo_Avatar avatar)
	{
		_avatar = avatar;
		MoodConfiguration moodConfiguration = Addressables.LoadAssetAsync<MoodConfiguration>("MoodConfiguration").WaitForCompletion();
		_moods = moodConfiguration.GetMoods();
		_updateMoodReaction = MilMo_EventSystem.Listen("update_mood", OnServerUpdateMood);
		_updateMoodReaction.Repeating = true;
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_updateMoodReaction);
		_updateMoodReaction = null;
	}

	public void RestorePersistedMood()
	{
		SetMood(_persistedMood, send: false, persist: false);
	}

	public void SetMood(Mood mood, bool send, bool persist)
	{
		if (mood == null)
		{
			Debug.LogWarning("Trying to set mood == null");
			return;
		}
		if (MoodLocked)
		{
			Debug.LogWarning("Trying to set mood but mood is locked");
			return;
		}
		_currentMood = mood;
		if (persist)
		{
			_persistedMood = mood;
		}
		_avatar.SetMood(mood);
		if (send)
		{
			SendMoodToGameServer(persist);
		}
	}

	private void SendMoodToGameServer(bool persist)
	{
		if (!(_currentMood == null))
		{
			Singleton<GameNetwork>.Instance.SendMoodUpdate(_currentMood.GetKey(), persist);
		}
	}

	private void OnServerUpdateMood(object msgAsObj)
	{
		if (msgAsObj is ServerUpdateMood serverUpdateMood)
		{
			string playerID = serverUpdateMood.getPlayerID();
			Mood moodByKey = GetMoodByKey(serverUpdateMood.getMoodName());
			if (!moodByKey)
			{
				Debug.LogWarning("MilMo_MoodHandler: Got unknown mood from ServerUpdateMood");
			}
			else if (!_avatar.IsTheLocalPlayer && !(_avatar.Id != playerID))
			{
				SetMood(moodByKey, send: false, persist: false);
			}
		}
	}

	public Mood GetMoodByKey(string moodKey)
	{
		return _moods.FirstOrDefault((Mood mood) => mood.GetKey() == moodKey);
	}

	public void SetInitialMood(string mood)
	{
		Mood mood2 = GetMoodByKey(mood);
		if (mood2 == null)
		{
			mood2 = _moods.First();
		}
		if (mood2 == null)
		{
			Debug.LogWarning("Unable to set initial mood: " + mood);
		}
		else
		{
			SetMood(mood2, send: false, persist: true);
		}
	}
}
