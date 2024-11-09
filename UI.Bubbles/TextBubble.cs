using System;
using System.Threading.Tasks;
using Core.Audio.AudioData;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Bubbles;

public abstract class TextBubble : MonoBehaviour
{
	[Tooltip("Life time in seconds")]
	[SerializeField]
	protected float lifeTime = 4f;

	[SerializeField]
	private UIAudioCueSO showBubbleSound;

	[Header("Elements")]
	[SerializeField]
	protected TMP_Text text;

	[SerializeField]
	private UIAlphaFX fader;

	private UnityAction _done;

	[SerializeField]
	protected Transform targetTransform;

	protected virtual void Awake()
	{
		if (text == null)
		{
			Debug.LogWarning(base.gameObject.name + ": text is missing");
		}
		else if (fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": fader is missing");
		}
	}

	protected virtual void Update()
	{
		Follow(Time.deltaTime);
		RefreshSize();
	}

	public virtual async void Show()
	{
		float num = (((double)lifeTime > 0.0) ? lifeTime : 0f);
		base.transform.localScale = Vector3.zero;
		if (showBubbleSound != null)
		{
			showBubbleSound.PlayAudioCue();
		}
		if (fader != null)
		{
			fader.FadeOutFast();
			fader.FadeIn();
			num += fader.GetFadeInDuration(1f);
		}
		await Task.Delay((int)num * 1000);
		if ((bool)base.gameObject)
		{
			Hide();
		}
	}

	public virtual void Hide()
	{
		int num = 10;
		_done?.Invoke();
		if (fader != null)
		{
			fader.FadeOut();
			num = (int)fader.GetFadeOutDuration(1f) * 1000;
		}
		UnityEngine.Object.Destroy(base.gameObject, num);
	}

	public void SetCallback(UnityAction done)
	{
		_done = done;
	}

	public void SetText(string newText)
	{
		text.SetText(newText);
		RefreshSize();
	}

	public void SetTarget(Transform newTargetTransform)
	{
		targetTransform = newTargetTransform;
		base.transform.localPosition = newTargetTransform.position;
	}

	protected int GetTextLength()
	{
		if (!text)
		{
			return 0;
		}
		return text.text.Length;
	}

	protected virtual void Follow(float deltaTime)
	{
		if ((bool)targetTransform)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = targetTransform.position - position;
			if (!(vector.magnitude < 0.1f))
			{
				float num = Math.Clamp(deltaTime * 2f, 0.25f, 1f);
				base.transform.position += vector * num;
			}
		}
	}

	protected virtual void RefreshSize()
	{
	}
}
