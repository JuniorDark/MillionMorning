using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.World.Player;
using Player.Moods;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectMood : MilMo_PlayerStateEffect
{
	private readonly MilMo_MoodHandler _moodHandler;

	private readonly string _fxMood = "";

	private Mood _oldMood;

	public MilMo_PlayerStateEffectMood(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		_moodHandler = avatar?.MoodHandler;
		if (effectData.GetParameters().Count > 0)
		{
			_fxMood = effectData.GetParameters()[0];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"Mood\" player state effect");
		}
	}

	public override void Activate()
	{
		if (_moodHandler != null)
		{
			_oldMood = _moodHandler.GetMoodByKey(Avatar.Mood);
			Mood moodByKey = _moodHandler.GetMoodByKey(_fxMood);
			_moodHandler.SetMood(moodByKey, send: false, persist: false);
			_moodHandler.MoodLocked = true;
		}
	}

	public override void Deactivate()
	{
		if (_moodHandler != null)
		{
			_moodHandler.MoodLocked = false;
			_moodHandler.SetMood(_oldMood, send: false, persist: false);
		}
	}
}
