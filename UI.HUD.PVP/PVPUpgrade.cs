using System.Threading.Tasks;
using Core.Utilities;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.PVP;

public class PVPUpgrade : MonoBehaviour
{
	[SerializeField]
	private UIAlphaFX fader;

	[SerializeField]
	private GameObject newPlus;

	[SerializeField]
	private GameObject levelUp;

	[SerializeField]
	private Image weapon;

	[SerializeField]
	private TMP_Text level;

	private void Awake()
	{
		if (newPlus == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing newPlus");
		}
		if (levelUp == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing levelUp");
		}
		if (weapon == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing weapon");
		}
		if (level == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing level");
		}
		if (fader == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing fader");
		}
		if (fader != null)
		{
			fader.FadeOutFast();
		}
	}

	public void SetWeapon(Texture2D texture2D, int currentLevel)
	{
		if (weapon != null)
		{
			Core.Utilities.UI.SetIcon(weapon, texture2D);
		}
		if (level != null)
		{
			level.text = ((currentLevel > 0) ? currentLevel.ToString() : "");
		}
	}

	public void NewWeapon()
	{
		Trigger(newPlus);
	}

	public void LevelUpWeapon()
	{
		Trigger(levelUp);
	}

	private async void Trigger(GameObject toActivate)
	{
		if (!(toActivate == null))
		{
			if (levelUp != null)
			{
				levelUp.SetActive(value: false);
			}
			if (newPlus != null)
			{
				newPlus.SetActive(value: false);
			}
			toActivate.SetActive(value: true);
			if (!(fader == null) && !(weapon == null))
			{
				LeanTween.moveLocalY(toActivate, 0f, 0f);
				LeanTween.moveLocalY(weapon.gameObject, 0f, 0f);
				LeanTween.moveLocalY(toActivate, 320f, 1f).setEase(LeanTweenType.easeInOutQuart);
				LeanTween.moveLocalY(weapon.gameObject, 320f, 0.7f).setEase(LeanTweenType.easeInOutQuart);
				fader.FadeIn();
				await Task.Delay(300);
				fader.FadeOut();
			}
		}
	}
}
