using Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.OptionsMenu.Audio;

public class AudioTabContent : MonoBehaviour
{
	[Header("Volume Sliders")]
	[SerializeField]
	private Slider masterSlider;

	[SerializeField]
	private Slider sfxSlider;

	[SerializeField]
	private Slider musicSlider;

	[SerializeField]
	private Slider ambienceSlider;

	private void Awake()
	{
		if (masterSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find masterSlider");
		}
		else if (sfxSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find sfxSlider");
		}
		else if (musicSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find musicSlider");
		}
		else if (ambienceSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find ambienceSlider");
		}
	}

	private void OnEnable()
	{
		SetupSliders();
	}

	private void OnDestroy()
	{
		ClearListeners();
	}

	private void SetupSliders()
	{
		masterSlider.value = Settings.MasterVolume;
		masterSlider.onValueChanged.AddListener(Settings.SetMasterVolume);
		sfxSlider.value = Settings.SFXVolume;
		sfxSlider.onValueChanged.AddListener(Settings.SetSFXVolume);
		musicSlider.value = Settings.MusicVolume;
		musicSlider.onValueChanged.AddListener(Settings.SetMusicVolume);
		ambienceSlider.value = Settings.AmbienceVolume;
		ambienceSlider.onValueChanged.AddListener(Settings.SetAmbienceVolume);
	}

	private void ClearListeners()
	{
		masterSlider.onValueChanged.RemoveAllListeners();
		sfxSlider.onValueChanged.RemoveAllListeners();
		musicSlider.onValueChanged.RemoveAllListeners();
		ambienceSlider.onValueChanged.RemoveAllListeners();
	}
}
