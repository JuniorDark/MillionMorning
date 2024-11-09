using System.Threading.Tasks;
using TMPro;
using UI.FX;
using UnityEngine;

namespace UI.HUD.Actionbar;

public class XPBubble : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private UIAlphaFX fader;

	[Header("Settings")]
	[SerializeField]
	private float lifeTime = 1.5f;

	[SerializeField]
	private float yAmount = 50f;

	[SerializeField]
	private float xAmount = 2f;

	[SerializeField]
	private float yTime = 1.5f;

	[SerializeField]
	private float xTime = 0.3f;

	[SerializeField]
	private LeanTweenType xEase = LeanTweenType.easeInOutSine;

	[SerializeField]
	private LeanTweenType yEase = LeanTweenType.easeOutSine;

	[SerializeField]
	private float impulseTime = 0.5f;

	[SerializeField]
	private LeanTweenType impulseEase = LeanTweenType.easeOutQuad;

	private void Start()
	{
		Impulse();
		StartFade();
		StartMoving();
	}

	private void Impulse()
	{
		LeanTween.scale(base.gameObject, Vector3.zero, 0f);
		LeanTween.scale(base.gameObject, Vector3.one, impulseTime).setEase(impulseEase);
	}

	private void StartMoving()
	{
		LeanTween.moveLocalY(base.gameObject, yAmount, yTime).setEase(yEase).setLoopCount(Mathf.CeilToInt(lifeTime / yTime));
		LeanTween.moveLocalX(base.gameObject, xAmount, xTime).setEase(xEase).setLoopPingPong();
	}

	private async void StartFade()
	{
		fader.FadeOutFast();
		fader.FadeIn();
		await Task.Delay((int)lifeTime * 1000);
		fader.FadeOut();
		Object.Destroy(base.gameObject, fader.GetFadeOutDuration(1f));
	}

	public void SetXP(int xp)
	{
		if (text != null)
		{
			text.text = $"+{xp}";
		}
	}
}
