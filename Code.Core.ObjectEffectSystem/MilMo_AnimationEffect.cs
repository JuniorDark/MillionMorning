using Code.Core.Utility;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_AnimationEffect : MilMo_ObjectEffect
{
	private bool _initialized;

	private float _time;

	private readonly MilMo_MovableAnimationHandler _animationHandler;

	private bool _animateOnlyIfVisibleReset;

	private Animation _animation;

	public override float Duration
	{
		get
		{
			if (GameObject == null || GameObject.GetComponent<Animation>() == null || GameObject.GetComponent<Animation>()[Template.AnimationName] == null)
			{
				return 0f;
			}
			return GameObject.GetComponent<Animation>()[Template.AnimationName].length / Template.Speed - 0.3f;
		}
	}

	private MilMo_AnimationEffectTemplate Template => EffectTemplate as MilMo_AnimationEffectTemplate;

	public MilMo_AnimationEffect(GameObject gameObject, MilMo_AnimationEffectTemplate template)
		: base(gameObject, template)
	{
		_animation = GameObject.GetComponent<Animation>();
		if (!_animation)
		{
			Transform childWithAnimation = MilMo_Transform.GetChildWithAnimation(GameObject.transform);
			if (childWithAnimation != null)
			{
				GameObject = childWithAnimation.gameObject;
				_animation = GameObject.GetComponent<Animation>();
			}
		}
		_animationHandler = GameObject.GetComponent<MilMo_MovableAnimationHandler>();
	}

	public override bool Update()
	{
		if (GameObject == null || !_animation || !_animation[Template.AnimationName])
		{
			Debug.Log("Failed to play animation " + Template.AnimationName + " on game object '" + ((GameObject == null) ? "null" : GameObject.name) + "'.");
			Destroy();
			return false;
		}
		if (!_initialized)
		{
			Initialize();
		}
		_time += Time.deltaTime;
		if (_time < _animation[Template.AnimationName].length / Template.Speed)
		{
			return true;
		}
		if (Template.WrapMode != WrapMode.Once && Template.WrapMode != WrapMode.Once)
		{
			return true;
		}
		PlayFallbackAnimation();
		Destroy();
		return false;
	}

	public override void Destroy()
	{
		PlayFallbackAnimation();
		base.Destroy();
	}

	private void Initialize()
	{
		_animateOnlyIfVisibleReset = GameObject.GetComponent<Animation>().cullingType == AnimationCullingType.BasedOnRenderers;
		GameObject.GetComponent<Animation>().cullingType = (Template.AnimateOnlyIfVisible ? AnimationCullingType.BasedOnRenderers : AnimationCullingType.AlwaysAnimate);
		GameObject.GetComponent<Animation>().playAutomatically = Template.PlayAutomatically;
		if (_animationHandler != null)
		{
			_animationHandler.PlayAnimation(Template.AnimationName, Template.CrossFade, Template.WrapMode);
		}
		else
		{
			GameObject.GetComponent<Animation>()[Template.AnimationName].time = 0f;
			GameObject.GetComponent<Animation>()[Template.AnimationName].wrapMode = Template.WrapMode;
			GameObject.GetComponent<Animation>()[Template.AnimationName].speed = Template.Speed;
			if (Template.CrossFade > 0f)
			{
				GameObject.GetComponent<Animation>().CrossFade(Template.AnimationName, Template.CrossFade);
			}
			else
			{
				GameObject.GetComponent<Animation>().Play(Template.AnimationName);
			}
		}
		_initialized = true;
	}

	private void PlayFallbackAnimation()
	{
		GameObject.GetComponent<Animation>().cullingType = (_animateOnlyIfVisibleReset ? AnimationCullingType.BasedOnRenderers : AnimationCullingType.AlwaysAnimate);
		if (string.IsNullOrEmpty(Template.FallbackAnimation) || GameObject.GetComponent<Animation>()[Template.FallbackAnimation] == null)
		{
			return;
		}
		if (_animationHandler != null)
		{
			_animationHandler.PlayAnimation(Template.FallbackAnimation, Template.CrossFade, WrapMode.Loop);
			return;
		}
		GameObject.GetComponent<Animation>()[Template.FallbackAnimation].wrapMode = WrapMode.Loop;
		if (Template.CrossFade > 0f)
		{
			GameObject.GetComponent<Animation>().CrossFade(Template.FallbackAnimation, Template.CrossFade);
		}
		else
		{
			GameObject.GetComponent<Animation>().Play(Template.FallbackAnimation);
		}
	}
}
