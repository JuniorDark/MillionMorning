using System;
using UI.FX;
using UnityEngine;

namespace UI.Bubbles;

public class ThinkBubble : TextBubble
{
	[Header("Thinking plops")]
	[SerializeField]
	private RectTransform plop1;

	[SerializeField]
	private RectTransform plop2;

	[SerializeField]
	private RectTransform plop3;

	[Header("Extra faders")]
	[SerializeField]
	private UIAlphaFX plop1Fader;

	[SerializeField]
	private UIAlphaFX plop2Fader;

	[SerializeField]
	private UIAlphaFX plop3Fader;

	private int _plop2Animation;

	private int _plop3Animation;

	private Vector3 _plop1Offset = Vector3.zero;

	private Vector3 _plop2Offset = Vector3.zero;

	private Vector3 _plop3Offset = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();
		if (plop1 == null || plop2 == null || plop3 == null)
		{
			Debug.LogWarning(base.gameObject.name + ": plops are missing");
			return;
		}
		_plop1Offset = plop1.localPosition;
		_plop2Offset = plop2.localPosition;
		_plop3Offset = plop3.localPosition;
	}

	public override void Show()
	{
		base.Show();
		if (plop2 != null)
		{
			_plop2Animation = LeanTween.rotateAroundLocal(plop2.gameObject, Vector3.forward, -360f, lifeTime + 3f).id;
		}
		if (plop3 != null)
		{
			_plop3Animation = LeanTween.rotateAroundLocal(plop3.gameObject, Vector3.forward, -360f, lifeTime + 4f).id;
		}
		if (plop1Fader == null || plop2Fader == null || plop3Fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing extra faders");
			return;
		}
		plop1Fader.FadeOutFast();
		plop2Fader.FadeOutFast();
		plop3Fader.FadeOutFast();
		plop1Fader.FadeIn();
		plop2Fader.FadeIn();
		plop3Fader.FadeIn();
	}

	public override void Hide()
	{
		base.Hide();
		LeanTween.cancel(_plop2Animation);
		LeanTween.cancel(_plop3Animation);
		if (plop1Fader == null || plop2Fader == null || plop3Fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing extra faders");
			return;
		}
		plop1Fader.FadeOut();
		plop2Fader.FadeOut();
		plop3Fader.FadeOut();
	}

	protected override void Follow(float deltaTime)
	{
		Transform obj = base.transform;
		Vector3 position = obj.position;
		Vector3 vector = (targetTransform ? targetTransform.position : position);
		Vector3 localScale = obj.localScale;
		if ((vector - position).magnitude > 0.1f)
		{
			base.transform.position = vector;
		}
		if (!(plop1 == null) && !(plop2 == null) && !(plop3 == null))
		{
			float num = Math.Clamp(deltaTime * 1.8f, 0.25f, 1f);
			float num2 = Math.Clamp(deltaTime * 1.2f, 0.25f, 1f);
			float num3 = Math.Clamp(deltaTime * 0.6f, 0.25f, 1f);
			Vector3 vector2 = Vector3.Scale(_plop1Offset, localScale);
			Vector3 vector3 = Vector3.Scale(_plop2Offset, localScale);
			Vector3 vector4 = Vector3.Scale(_plop3Offset, localScale);
			Vector3 vector5 = vector + vector2 - plop1.position;
			Vector3 vector6 = vector + vector3 - plop2.position;
			Vector3 vector7 = vector + vector4 - plop3.position;
			if (vector5.magnitude > 0.1f)
			{
				plop1.position += vector5 * num;
			}
			if (vector6.magnitude > 0.1f)
			{
				plop2.position += vector6 * num2;
			}
			if (vector7.magnitude > 0.1f)
			{
				plop3.position += vector7 * num3;
			}
		}
	}
}
