using System;
using Code.World.Level.LevelObject;
using JetBrains.Annotations;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Marker.Combat;

public abstract class TargetMarker : WorldSpaceObject
{
	public const string ADDRESSABLE_ADDRESS = "TargetMarker";

	[Header("Elements")]
	[SerializeField]
	protected UIAlphaFX fader;

	[SerializeField]
	protected TMP_Text targetName;

	[SerializeField]
	protected TargetArrow arrow;

	[Header("Event")]
	[SerializeField]
	protected UnityEvent onTargeting;

	[Header("Target")]
	[CanBeNull]
	protected IMilMo_AttackTarget Target;

	[SerializeField]
	protected Transform targetTransform;

	protected bool Visible;

	public virtual void Initialize(IMilMo_AttackTarget target)
	{
		base.Initialize();
		if (target == null)
		{
			Debug.LogWarning(base.gameObject.name + ": target is null");
			return;
		}
		if (arrow == null)
		{
			Debug.LogWarning(base.gameObject.name + ": arrow is null");
			return;
		}
		Target = target;
		if (Target.GameObject != null)
		{
			targetTransform = Target.GameObject.transform;
		}
		SetMarkerText(target.Name);
		ShowTargetArrow(shouldShow: false);
		if (fader != null)
		{
			fader.FadeOutFast();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (fader == null)
		{
			Debug.LogError(base.gameObject.name + ": _fader is null");
		}
		if (targetName == null)
		{
			Debug.LogError(base.gameObject.name + ": targetName is null");
		}
		if (arrow == null)
		{
			Debug.LogError(base.gameObject.name + ": arrow is null");
		}
	}

	protected virtual void Update()
	{
		Follow(Time.deltaTime);
	}

	public virtual void Show(bool shouldShow)
	{
		Visible = shouldShow;
		if (!(fader == null))
		{
			if (shouldShow)
			{
				fader.FadeIn();
			}
			else
			{
				fader.FadeOut();
			}
		}
	}

	protected void ShowName(bool shouldShow)
	{
		if (!(targetName == null) && targetName.gameObject.activeSelf != shouldShow)
		{
			targetName.gameObject.SetActive(shouldShow);
		}
	}

	public void ShowTargetArrow(bool shouldShow)
	{
		if (!(arrow == null))
		{
			if (shouldShow)
			{
				onTargeting?.Invoke();
			}
			arrow.Show(shouldShow);
		}
	}

	private void SetMarkerText(string tName)
	{
		if (!string.IsNullOrEmpty(tName) && targetName != null)
		{
			targetName.text = tName;
		}
	}

	protected void SetMarkerTextColor(Color color)
	{
		if (targetName != null)
		{
			targetName.color = color;
		}
	}

	private void Follow(float deltaTime)
	{
		if ((bool)targetTransform)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = targetTransform.position;
			if (Target != null)
			{
				position2.y += Target.MarkerYOffset;
			}
			Vector3 vector = position2 - position;
			if (!(vector.magnitude < 0.001f))
			{
				float num = Math.Clamp(deltaTime * 100f, 0.25f, 1f);
				base.transform.position += vector * num;
			}
		}
	}
}
