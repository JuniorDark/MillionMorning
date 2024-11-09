using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Code.Core.Avatar.HappyPickup;

public class HappyPickupTemplate : MilMo_Template
{
	private string _startSoundFile = "";

	private string _showItemSoundFile = "";

	public string Animation;

	public string Emote;

	public float Duration;

	public string ItemNode;

	public MilMo_LocString Think;

	public Vector3 ItemOffset;

	public float ItemAppearanceTime;

	public AudioClip StartSound;

	public AudioClip ShowItemSound;

	private HappyPickupTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "HappyPickup")
	{
		ItemOffset = Vector3.zero;
		ItemNode = "";
		Emote = "";
		Animation = "";
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Animation"))
		{
			Animation = file.GetString();
		}
		else if (file.IsNext("Emote"))
		{
			Emote = file.GetString();
		}
		else if (file.IsNext("Duration"))
		{
			Duration = file.GetFloat();
		}
		else if (file.IsNext("ItemNode"))
		{
			ItemNode = file.GetString();
		}
		else if (file.IsNext("ItemOffset"))
		{
			ItemOffset = file.GetVector3();
		}
		else if (file.IsNext("ItemAppearanceTime"))
		{
			ItemAppearanceTime = file.GetFloat();
		}
		else if (file.IsNext("StartSound"))
		{
			_startSoundFile = file.GetString();
		}
		else if (file.IsNext("ShowItemSound"))
		{
			_showItemSoundFile = file.GetString();
		}
		else
		{
			if (!file.IsNext("Think"))
			{
				return base.ReadLine(file);
			}
			Think = MilMo_Localization.GetLocString(file.GetString());
		}
		return true;
	}

	public override bool FinishLoading()
	{
		if (!base.FinishLoading())
		{
			return false;
		}
		if (string.IsNullOrEmpty(Animation))
		{
			Debug.LogWarning("Animation is missing in happy pickup template " + Path);
			return false;
		}
		if (string.IsNullOrEmpty(ItemNode))
		{
			Debug.LogWarning("ItemNode is missing in happy pickup template " + Path);
			return false;
		}
		if (Duration <= 0f)
		{
			Debug.LogWarning("Missing, zero or negative duration in happy pickup template " + Path);
			return false;
		}
		LoadSoundsAsync();
		return true;
	}

	private async void LoadSoundsAsync()
	{
		if (!string.IsNullOrEmpty(_startSoundFile))
		{
			AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(_startSoundFile);
			if (audioClip == null)
			{
				Debug.LogWarning("Failed to load sound " + _startSoundFile + " for happy pickup template " + Path);
			}
			else
			{
				StartSound = audioClip;
			}
		}
		if (!string.IsNullOrEmpty(_showItemSoundFile))
		{
			AudioClip audioClip2 = await MilMo_ResourceManager.Instance.LoadAudioAsync(_showItemSoundFile);
			if (audioClip2 == null)
			{
				Debug.LogWarning("Failed to load sound " + _showItemSoundFile + " for happy pickup template " + Path);
			}
			else
			{
				ShowItemSound = audioClip2;
			}
		}
	}

	public static HappyPickupTemplate Create(string category, string path, string filePath)
	{
		return new HappyPickupTemplate(category, path, filePath);
	}
}
