using System.Collections;
using UI.FX;
using UnityEngine;

namespace UI.HUD.WeaponSwap;

public class WeaponSwap : HudElement
{
	[SerializeField]
	private WeaponSlot previousPreviousSlot;

	[SerializeField]
	private WeaponSlot previousSlot;

	[SerializeField]
	private WeaponSlot currentSlot;

	[SerializeField]
	private WeaponSlot nextSlot;

	[SerializeField]
	private WeaponSlot nextNextSlot;

	[SerializeField]
	private int fadeTimer;

	private Coroutine _fadeCoroutine;

	private UIAlphaFX _fader;

	protected void Awake()
	{
		_fader = GetComponent<UIAlphaFX>();
		if (_fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing UIAlphaFX");
		}
	}

	private void Start()
	{
		if ((bool)_fader)
		{
			_fader.FadeOutFast();
		}
	}

	public void OnToggleLeft()
	{
		ResetCoroutine();
		if (currentSlot.CanToggleSlots())
		{
			if ((bool)_fader)
			{
				_fader.FadeIn();
			}
			previousPreviousSlot.AnimateFrom(previousSlot);
			previousSlot.AnimateFrom(currentSlot);
			currentSlot.AnimateFrom(nextSlot);
			nextSlot.AnimateFrom(nextNextSlot);
			if (base.gameObject.activeInHierarchy)
			{
				_fadeCoroutine = StartCoroutine(FadeTimer());
			}
		}
	}

	public void OnToggleRight()
	{
		ResetCoroutine();
		if (currentSlot.CanToggleSlots())
		{
			if ((bool)_fader)
			{
				_fader.FadeIn();
			}
			previousSlot.AnimateFrom(previousPreviousSlot);
			currentSlot.AnimateFrom(previousSlot);
			nextSlot.AnimateFrom(currentSlot);
			nextNextSlot.AnimateFrom(nextSlot);
			if (base.gameObject.activeInHierarchy)
			{
				_fadeCoroutine = StartCoroutine(FadeTimer());
			}
		}
	}

	private void ResetCoroutine()
	{
		if (_fadeCoroutine != null)
		{
			StopCoroutine(_fadeCoroutine);
			_fadeCoroutine = null;
		}
	}

	private IEnumerator FadeTimer()
	{
		yield return new WaitForSeconds(fadeTimer);
		if ((bool)_fader)
		{
			_fader.FadeOut();
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
