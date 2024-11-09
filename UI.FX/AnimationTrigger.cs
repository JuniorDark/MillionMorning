using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class AnimationTrigger : MonoBehaviour
{
	[SerializeField]
	private UnityEvent onEnable;

	private Animation _animation;

	private void Awake()
	{
		_animation = GetComponent<Animation>();
	}

	private void OnEnable()
	{
		onEnable?.Invoke();
	}

	public void Play()
	{
		if ((bool)_animation)
		{
			_animation.Play();
		}
	}

	public void Play(string animationName)
	{
		if ((bool)_animation && (bool)_animation.GetClip(animationName))
		{
			_animation.Play(animationName);
		}
	}

	public void Play(AnimationClip animationClip)
	{
		if ((bool)_animation && (bool)animationClip && (bool)_animation.GetClip(animationClip.name))
		{
			_animation.Play(animationClip.name);
		}
	}
}
