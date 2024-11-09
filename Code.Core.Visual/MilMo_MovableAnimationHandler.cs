using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_MovableAnimationHandler : MonoBehaviour
{
	public delegate bool IsMainAnimation(string anim);

	private string _lastPlayedAnimation = "";

	private string _fallbackAnimation;

	private MilMo_TimerEvent _playFallbackAnimEvent;

	private Animation _animation;

	public MilMo_VisualRep VisualRep { get; set; }

	public IsMainAnimation IsMainAnimationCallback { get; set; }

	public string CurrentMainAnimation { get; private set; }

	public MilMo_MovableAnimationHandler()
	{
		CurrentMainAnimation = "Idle";
	}

	public void PlayAnimation(string anim, float crossFade, WrapMode wrapMode, bool startAtRandomTime = false)
	{
		if ((wrapMode == WrapMode.Loop && anim == _lastPlayedAnimation) || VisualRep == null || VisualRep.GameObject == null)
		{
			return;
		}
		if (_animation == null)
		{
			_animation = VisualRep.GameObject.GetComponent<Animation>();
		}
		if (_animation == null || _animation[anim] == null)
		{
			return;
		}
		float time = (startAtRandomTime ? Random.Range(0f, _animation[anim].length) : 0f);
		_animation[anim].time = time;
		bool flag = IsMainAnimationCallback != null && IsMainAnimationCallback(anim);
		if (wrapMode == WrapMode.Loop && _playFallbackAnimEvent != null && flag)
		{
			CurrentMainAnimation = anim;
			_fallbackAnimation = CurrentMainAnimation;
			return;
		}
		MilMo_EventSystem.RemoveTimerEvent(_playFallbackAnimEvent);
		_playFallbackAnimEvent = null;
		VisualRep.PlayAnimation(anim, crossFade, wrapMode);
		if (wrapMode == WrapMode.Once)
		{
			_fallbackAnimation = CurrentMainAnimation;
			_playFallbackAnimEvent = MilMo_EventSystem.At(_animation[anim].length - crossFade, PlayFallbackAnimation);
		}
		if (flag)
		{
			CurrentMainAnimation = anim;
		}
		_lastPlayedAnimation = anim;
	}

	private void PlayFallbackAnimation()
	{
		if (VisualRep != null && !(_animation == null) && !(_animation[_fallbackAnimation] == null))
		{
			VisualRep.PlayAnimation(_fallbackAnimation, WrapMode.Loop);
			_playFallbackAnimEvent = null;
			_lastPlayedAnimation = _fallbackAnimation;
		}
	}
}
