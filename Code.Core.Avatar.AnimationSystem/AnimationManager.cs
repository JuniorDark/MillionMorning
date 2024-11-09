using System.Collections.Generic;
using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Avatar.AnimationSystem;

public class AnimationManager
{
	public const float DEFAULT_CROSS_FADE_TIME = 0.3f;

	private AnimationGroups _groups;

	private GameObject _gameObject;

	private Animation _animations;

	private string _curAnimGroup;

	private string _curNonLoopingAnim;

	private string _curNonLoopingAnimFullName;

	private string _currentLoopingAnimation;

	private string _currentVisibleLoopingAnim = "";

	private MilMo_TimerEvent _fallbackTime;

	private MilMo_TimerEvent _resetNonLoopingAnimationEvent;

	private static readonly IDictionary<string, AnimationGroup.PartialBone> MixingTransforms = new Dictionary<string, AnimationGroup.PartialBone>();

	public string GetCurrentLoopingAnimation()
	{
		return _currentLoopingAnimation;
	}

	public string GetCurrentVisibleLoopingAnimation()
	{
		return _currentVisibleLoopingAnim;
	}

	public void Init(GameObject gameObject, bool thumbnailMode)
	{
		_groups = new AnimationGroups();
		_gameObject = gameObject;
		if (_gameObject == null)
		{
			Debug.LogError("AnimationManager: gameObject is null");
			return;
		}
		_animations = _gameObject.GetComponent<Animation>();
		_animations.playAutomatically = false;
		if (!thumbnailMode)
		{
			SetupAnimations(storeMixingTransforms: true);
			SetupWrapModes();
			SetupMixTransforms();
			_currentLoopingAnimation = "Idle";
			_currentVisibleLoopingAnim = "Idle";
			_curNonLoopingAnim = "";
		}
	}

	private void SetupWrapModes()
	{
		foreach (AnimationGroup group in _groups.GetGroups())
		{
			int layer = group.Layer;
			foreach (AvatarAnimation animation in group.Animations)
			{
				if (!(_animations[animation.Name] == null))
				{
					_animations[animation.Name].layer = layer;
					_animations[animation.Name].wrapMode = ((!group.Loop) ? WrapMode.Once : WrapMode.Loop);
				}
			}
			foreach (string key in group.Partials.Keys)
			{
				foreach (AvatarAnimation animation2 in group.Animations)
				{
					string name = animation2.Name + ":" + key;
					if (!(_animations[name] == null))
					{
						_animations[name].layer = layer;
						_animations[name].wrapMode = ((!group.Loop) ? WrapMode.Once : WrapMode.Loop);
					}
				}
			}
		}
	}

	private void SetupMixTransforms()
	{
		foreach (KeyValuePair<string, AnimationGroup.PartialBone> mixingTransform in MixingTransforms)
		{
			if (_animations[mixingTransform.Key] == null)
			{
				Debug.LogWarning("Got none existing animation " + mixingTransform.Key + " when applying mixing transforms");
				continue;
			}
			Transform transform = _gameObject.transform.Find(mixingTransform.Value.Bone);
			if (transform == null)
			{
				Debug.LogWarning("Got null transform " + mixingTransform.Value.Bone + " when applying mixing transform for animation " + mixingTransform.Key);
			}
			else
			{
				_animations[mixingTransform.Key].AddMixingTransform(transform, recursive: true);
			}
		}
	}

	private void SetupAnimations(bool storeMixingTransforms)
	{
		if (_animations == null)
		{
			return;
		}
		if (storeMixingTransforms)
		{
			MixingTransforms.Clear();
		}
		foreach (AnimationGroup group in _groups.GetGroups())
		{
			foreach (KeyValuePair<string, List<AnimationGroup.PartialBone>> partial in group.Partials)
			{
				foreach (AnimationGroup.PartialBone item in partial.Value)
				{
					foreach (AvatarAnimation animation in group.Animations)
					{
						string text = animation.Name;
						if (_animations[animation.Name + item.Postfix] != null)
						{
							text += item.Postfix;
						}
						string text2 = animation.Name + ":" + partial.Key;
						if (_animations[text2] == null)
						{
							AnimationState animationState = _animations[text];
							if (animationState == null)
							{
								Debug.LogWarning("MilMo_AnimationManager: Animation " + text + " does not exist but are being used in Mix in animation group " + group.Name);
								continue;
							}
							if (animationState.clip == null)
							{
								Debug.LogWarning("MilMo_AnimationManager: Animation " + text + " has no default animation clip when setting up Mix in animation group " + group.Name);
								continue;
							}
							_animations.AddClip(animationState.clip, text2);
						}
						if (_animations[text2] == null)
						{
							Debug.LogWarning("MilMo_AnimationManager: Failed to add animation clip " + text2 + " for Mixing in animation group " + group.Name);
							continue;
						}
						_animations[text2].wrapMode = ((!group.Loop) ? WrapMode.Once : WrapMode.Loop);
						if (storeMixingTransforms)
						{
							if (_animations.transform.Find(item.Bone) != null)
							{
								MixingTransforms.Add(text2, item);
							}
							else
							{
								Debug.LogWarning("MilMo_AnimationManager: Using non existing transform " + item.Bone + " in Mix for animation group " + group.Name);
							}
						}
					}
				}
			}
		}
	}

	public float GetAnimationTime()
	{
		return Mathf.Repeat(_animations[_currentVisibleLoopingAnim].time, _animations[_currentVisibleLoopingAnim].length);
	}

	public float GetAnimationDuration(string animationName)
	{
		return _animations[animationName].length * Mathf.Abs(_animations[animationName].speed);
	}

	public bool HasAnimation(string animationName)
	{
		if (_animations != null)
		{
			return _animations[animationName] != null;
		}
		return false;
	}

	public bool IsEnabled()
	{
		if (_animations != null)
		{
			return _animations.enabled;
		}
		return false;
	}

	public bool EnableAnimations()
	{
		if (_animations == null || _animations.enabled)
		{
			return false;
		}
		return _animations.enabled = true;
	}

	public void DisableAnimations()
	{
		if (!(_animations == null) && _animations.enabled)
		{
			_animations.enabled = false;
		}
	}

	public void Play(string animation)
	{
		if (!_gameObject)
		{
			Debug.LogWarning("AnimationManager: Trying to play animation '" + animation + "' when game object is null");
			return;
		}
		if (!_animations || !_animations[animation])
		{
			Debug.LogWarning("AnimationManager: Trying to play animation '" + animation + "' when animation is null or missing");
			return;
		}
		string curNonLoopingAnim = animation;
		float crossFadeTime = 0.3f;
		AnimationGroup group = _groups.GetGroup(animation);
		if (group != null)
		{
			crossFadeTime = group.GetBlendTime(animation);
			if (group.Loop)
			{
				_currentLoopingAnimation = animation;
				_curAnimGroup = group.Name;
				_currentVisibleLoopingAnim = animation;
				MilMo_EventSystem.RemoveTimerEvent(_fallbackTime);
			}
			if (_curAnimGroup != null)
			{
				if (group.TryGetPartial(_curAnimGroup, out var bones))
				{
					animation += ":";
					animation += _curAnimGroup;
				}
				else if (!group.Loop)
				{
					_currentVisibleLoopingAnim = "";
				}
				else if (!string.IsNullOrEmpty(_curNonLoopingAnim))
				{
					AnimationGroup group2 = _groups.GetGroup(_curNonLoopingAnim);
					if (group2 != null && group2.TryGetPartial(_curAnimGroup, out bones))
					{
						string text = _curNonLoopingAnim + ":" + _curAnimGroup;
						if (text != _curNonLoopingAnimFullName && _animations[text] != null && _animations[_curNonLoopingAnimFullName] != null)
						{
							_animations[text].time = _animations[_curNonLoopingAnimFullName].time;
							_animations.Play(text);
							_animations.Stop(_curNonLoopingAnimFullName);
							_animations.CrossFade(text, 0.1f, PlayMode.StopSameLayer);
							_curNonLoopingAnimFullName = text;
						}
					}
				}
			}
		}
		else
		{
			_currentVisibleLoopingAnim = "";
		}
		if (_animations[animation].wrapMode != WrapMode.Loop)
		{
			_animations[animation].time = 0f;
			_curNonLoopingAnim = curNonLoopingAnim;
			_curNonLoopingAnimFullName = animation;
			if (_resetNonLoopingAnimationEvent != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_resetNonLoopingAnimationEvent);
			}
			_resetNonLoopingAnimationEvent = MilMo_EventSystem.At(_animations[animation].length - 0.3f, delegate
			{
				_curNonLoopingAnim = "";
				_curNonLoopingAnimFullName = "";
			});
		}
		_animations[animation].speed = 1f;
		_animations.CrossFade(animation, crossFadeTime);
		if (_animations[animation].wrapMode == WrapMode.Loop || _currentLoopingAnimation == null || _animations[animation].layer != _animations[_currentLoopingAnimation].layer)
		{
			return;
		}
		MilMo_EventSystem.RemoveTimerEvent(_fallbackTime);
		_fallbackTime = MilMo_EventSystem.At(_animations[animation].length - 0.3f, delegate
		{
			if (!(_gameObject == null) && !(_animations == null) && !(_animations[_currentLoopingAnimation] == null))
			{
				_animations.CrossFade(_currentLoopingAnimation, crossFadeTime);
				_currentVisibleLoopingAnim = _currentLoopingAnimation;
			}
		});
	}
}
