using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_EmoteManager
{
	public delegate void PlayAnimCallback(string anim);

	public delegate void DisableWeaponCallback();

	public delegate void EnableWeaponCallback();

	private PlayAnimCallback _playAnim;

	private DisableWeaponCallback _disableWeapon;

	private EnableWeaponCallback _enableWeapon;

	private bool _weaponWasDisabled;

	private readonly List<MilMo_EventSystem.MilMo_Callback> _emoteDoneCallbacks = new List<MilMo_EventSystem.MilMo_Callback>();

	private MilMo_Emote _currentEmote;

	private readonly Dictionary<string, MilMo_EmoteMover> _emoteMovers = new Dictionary<string, MilMo_EmoteMover>();

	private string _mood;

	private MilMo_Face _currentMood;

	private MilMo_EmoteMover _leftEyeMover;

	private MilMo_EmoteMover _rightEyeMover;

	private readonly Vector2 _eyeLookDrag = new Vector2(0.6f, 0.6f);

	private readonly Vector2 _eyeLookPull = new Vector2(0.05f, 0.05f);

	private MilMo_EmoteMover _eyeSpecMover;

	private MilMo_GenericReaction _update;

	private MilMo_GenericReaction _fixedUpdate;

	private MilMo_GenericReaction _lateUpdate;

	public SkinnedMeshRenderer Renderer { get; private set; }

	public bool Enable { get; set; }

	public bool IsPlaying { get; private set; }

	private MilMo_SuperAlivenessManager SuperAliveness { get; set; }

	public MilMo_EmoteManager(MilMo_SuperAlivenessManager superAliveness)
	{
		Enable = true;
		SuperAliveness = superAliveness;
	}

	public void Init(SkinnedMeshRenderer meshRenderer, PlayAnimCallback animCallback)
	{
		Renderer = meshRenderer;
		_playAnim = animCallback;
		_fixedUpdate = MilMo_EventSystem.RegisterFixedUpdate(FixedUpdate);
		_update = MilMo_EventSystem.RegisterUpdate(Update);
		_lateUpdate = MilMo_EventSystem.RegisterLateUpdate(LateUpdate);
		_leftEyeMover = GetUVMover("_EyeL");
		_rightEyeMover = GetUVMover("_EyeR");
		_eyeSpecMover = GetUVMover("_EyeSpec");
	}

	public void Destroy()
	{
		MilMo_EventSystem.UnregisterFixedUpdate(_fixedUpdate);
		MilMo_EventSystem.UnregisterUpdate(_update);
		MilMo_EventSystem.UnregisterLateUpdate(_lateUpdate);
	}

	public void RegisterWieldCallbacks(EnableWeaponCallback wield, DisableWeaponCallback unwield)
	{
		_enableWeapon = wield;
		_disableWeapon = unwield;
	}

	public void WieldWeapon()
	{
		if (_enableWeapon != null)
		{
			_enableWeapon();
			_weaponWasDisabled = false;
		}
	}

	public void UnwieldWeapon()
	{
		if (_disableWeapon != null)
		{
			_disableWeapon();
			_weaponWasDisabled = true;
		}
	}

	public void SetMoodByName(string moodName)
	{
		MilMo_Face mood = MilMo_Face.GetMood(moodName);
		foreach (MilMo_EmoteMover value in _emoteMovers.Values)
		{
			value.ResetDefaultValues();
		}
		_currentMood = mood;
		if (_currentMood == null)
		{
			return;
		}
		foreach (MilMo_Emote.FrameAction action in _currentMood.Actions)
		{
			action.Action.UpdateDefaultValues(this);
		}
	}

	public void SetFaceByName(string faceName)
	{
		MilMo_Face face = MilMo_Face.GetFace(faceName);
		foreach (MilMo_EmoteMover value in _emoteMovers.Values)
		{
			value.ResetDefaultValues();
		}
		_currentMood = face;
		if (_currentMood == null)
		{
			return;
		}
		foreach (MilMo_Emote.FrameAction action in _currentMood.Actions)
		{
			action.Action.UpdateDefaultValues(this);
		}
	}

	public void ForceMood()
	{
		if (_currentMood == null)
		{
			return;
		}
		foreach (MilMo_Emote.FrameAction action in _currentMood.Actions)
		{
			action.Action.UpdateDefaultValues(this);
		}
	}

	public void ForceOneUpdate()
	{
		FixedUpdate(null);
		Update(null);
		LateUpdate(null);
	}

	public void EnableSuperAliveness()
	{
		if (SuperAliveness != null)
		{
			if (_leftEyeMover != null)
			{
				_leftEyeMover.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				_leftEyeMover.Mover.Pull = _eyeLookPull;
				_leftEyeMover.Mover.Drag = _eyeLookDrag;
			}
			if (_rightEyeMover != null)
			{
				_rightEyeMover.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				_rightEyeMover.Mover.Pull = _eyeLookPull;
				_rightEyeMover.Mover.Drag = _eyeLookDrag;
			}
			SuperAliveness.Enable();
			SuperAliveness.UseEyeLook = true;
		}
	}

	public void DisableSuperAliveness()
	{
		if (SuperAliveness != null)
		{
			SuperAliveness.Disable();
		}
	}

	public void EnableEyeLook()
	{
		if (SuperAliveness != null)
		{
			SuperAliveness.UseEyeLook = true;
		}
	}

	public void DisableEyeLook()
	{
		if (SuperAliveness != null)
		{
			SuperAliveness.UseEyeLook = false;
		}
	}

	public MilMo_EmoteMover GetBoneMover(string name)
	{
		string key = "bone:" + name;
		if (!_emoteMovers.TryGetValue(key, out var value))
		{
			value = new MilMo_BoneMover(name, this);
			_emoteMovers.Add(key, value);
		}
		return value;
	}

	public MilMo_EmoteMover GetBoneScaleMover(string name)
	{
		string key = "bonescale:" + name;
		if (!_emoteMovers.TryGetValue(key, out var value))
		{
			value = new MilMo_BoneScaleMover(name, this);
			_emoteMovers.Add(key, value);
		}
		return value;
	}

	public MilMo_EmoteMover GetUVMover(string name)
	{
		string key = "uv:" + name;
		if (!_emoteMovers.TryGetValue(key, out var value))
		{
			value = new MilMo_UVMover(name);
			_emoteMovers.Add(key, value);
		}
		return value;
	}

	public MilMo_EmoteMover GetRotationMover(string name)
	{
		string key = "rotate:" + name;
		if (!_emoteMovers.TryGetValue(key, out var value))
		{
			value = new MilMo_RotationMover(name);
			_emoteMovers.Add(key, value);
		}
		return value;
	}

	public void PlayEmote(MilMo_Emote emote)
	{
		AbortEmote();
		_currentEmote = emote;
		if (_currentEmote != null)
		{
			DisableSuperAliveness();
			_currentEmote.Execute(this);
			IsPlaying = true;
		}
	}

	public void AbortEmote()
	{
		foreach (KeyValuePair<string, MilMo_EmoteMover> emoteMover in _emoteMovers)
		{
			emoteMover.Value.Abort(this);
		}
		if (_currentEmote != null)
		{
			_currentEmote.Abort(this);
			IsPlaying = false;
		}
		if (_weaponWasDisabled)
		{
			WieldWeapon();
		}
		EnableSuperAliveness();
		MilMo_EventSystem.MilMo_Callback[] array = _emoteDoneCallbacks.ToArray();
		_emoteDoneCallbacks.Clear();
		MilMo_EventSystem.MilMo_Callback[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i]();
		}
	}

	public void RegisterEmoteDoneCallback(MilMo_EventSystem.MilMo_Callback callback)
	{
		if (callback != null)
		{
			if (!IsPlaying)
			{
				callback();
			}
			else
			{
				_emoteDoneCallbacks.Add(callback);
			}
		}
	}

	private void FixedUpdate(object o)
	{
		if (!Enable)
		{
			return;
		}
		foreach (KeyValuePair<string, MilMo_EmoteMover> emoteMover in _emoteMovers)
		{
			emoteMover.Value.FixedUpdate(this);
		}
	}

	private void Update(object o)
	{
		if (!Enable)
		{
			return;
		}
		foreach (KeyValuePair<string, MilMo_EmoteMover> emoteMover in _emoteMovers)
		{
			emoteMover.Value.Update(this);
		}
	}

	private void LateUpdate(object o)
	{
		if (!Enable)
		{
			return;
		}
		foreach (KeyValuePair<string, MilMo_EmoteMover> emoteMover in _emoteMovers)
		{
			emoteMover.Value.LateUpdate(this);
		}
	}

	public void PlayAnimation(string anim)
	{
		if (_playAnim != null)
		{
			_playAnim(anim);
		}
	}

	public void SetEyeLook(Vector2 uvOffsetRight, Vector2 uvOffsetLeft)
	{
		if (_rightEyeMover != null)
		{
			_rightEyeMover.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
			_rightEyeMover.Mover.Target = uvOffsetRight;
		}
		if (_leftEyeMover != null)
		{
			_leftEyeMover.Mover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
			_leftEyeMover.Mover.Target = uvOffsetLeft;
		}
	}

	public void SetEyeSpec(Vector2 uvOffset)
	{
		if (_eyeSpecMover != null)
		{
			_eyeSpecMover.Mover.Val = uvOffset;
			_eyeSpecMover.Mover.Target = uvOffset;
		}
	}
}
