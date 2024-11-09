using System;
using Code.Core.Avatar;
using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public class MilMo_AvatarHandler : IAvatarChange
{
	private AvatarGender _currentGender;

	private AvatarGender _oldGender;

	private const float MIN_BOY_HEIGHT = 0.9f;

	private const float MAX_BOY_HEIGHT = 1.1f;

	private const float MIN_GIRL_HEIGHT = 0.85f;

	private const float MAX_GIRL_HEIGHT = 1.05f;

	private MilMo_Wearable _boyShirt;

	private MilMo_Wearable _girlShirt;

	private MilMo_Wearable _boyPants;

	private MilMo_Wearable _girlPants;

	private MilMo_Wearable _boyShoes;

	private MilMo_Wearable _girlShoes;

	private MilMo_Wearable _boyHair;

	private MilMo_Wearable _girlHair;

	private MilMo_Avatar Boy { get; set; }

	private MilMo_Avatar Girl { get; set; }

	public bool IsMale => _currentGender == AvatarGender.Male;

	public int CurrentGender => (int)_currentGender;

	public int OldGender => (int)_oldGender;

	public MilMo_Avatar CurrentAvatar => _currentGender switch
	{
		AvatarGender.Male => Boy, 
		AvatarGender.Female => Girl, 
		_ => null, 
	};

	public MilMo_Avatar OldAvatar => _oldGender switch
	{
		AvatarGender.Male => Boy, 
		AvatarGender.Female => Girl, 
		_ => null, 
	};

	public string CurrentSkinColor => CurrentAvatar.SkinColor;

	public int CurrentHairColor => CurrentAvatar.HairColor;

	public string CurrentEyeColor => CurrentAvatar.EyeColor;

	public string CurrentMouth => CurrentAvatar.EyeColor;

	public string CurrentEyes => CurrentAvatar.Eyes;

	public string CurrentEyeBrows => CurrentAvatar.EyeBrows;

	public MilMo_Wearable CurrentHair
	{
		get
		{
			if (!IsMale)
			{
				return _girlHair;
			}
			return _boyHair;
		}
	}

	public MilMo_Wearable CurrentShirt
	{
		get
		{
			if (!IsMale)
			{
				return _girlShirt;
			}
			return _boyShirt;
		}
	}

	public MilMo_Wearable CurrentPants
	{
		get
		{
			if (!IsMale)
			{
				return _girlPants;
			}
			return _boyPants;
		}
	}

	public MilMo_Wearable CurrentShoes
	{
		get
		{
			if (!IsMale)
			{
				return _girlShoes;
			}
			return _boyShoes;
		}
	}

	public int CurrentShirtColor
	{
		get
		{
			if (CurrentShirt?.BodyPack == null)
			{
				Debug.LogWarning("No shirt here");
				return -1;
			}
			string key = CurrentShirt.BodyPack.Path + ":" + CurrentShirt.BodyPack.ColorGroups[0].GroupName;
			return CurrentShirt.ColorIndices[key];
		}
	}

	public int CurrentShoesPrimaryColor
	{
		get
		{
			if (CurrentShoes?.BodyPack == null)
			{
				Debug.LogWarning("No shoes here");
				return -1;
			}
			string key = CurrentShoes.BodyPack.Path + ":" + CurrentShoes.BodyPack.ColorGroups[0].GroupName;
			return CurrentShoes.ColorIndices[key];
		}
	}

	public int CurrentShoesSecondaryColor
	{
		get
		{
			if (CurrentShoes?.BodyPack == null)
			{
				Debug.LogWarning("No shoes here");
				return -1;
			}
			string key = CurrentShoes.BodyPack.Path + ":" + CurrentShoes.BodyPack.ColorGroups[1].GroupName;
			return CurrentShoes.ColorIndices[key];
		}
	}

	public int CurrentPantsColor
	{
		get
		{
			if (CurrentPants?.BodyPack == null)
			{
				Debug.LogWarning("No pants here");
				return -1;
			}
			string key = CurrentPants.BodyPack.Path + ":" + CurrentPants.BodyPack.ColorGroups[1].GroupName;
			return CurrentPants.ColorIndices[key];
		}
	}

	public void SetBoyAndGirl(MilMo_Avatar boy, MilMo_Avatar girl)
	{
		if (Boy != null || Girl != null)
		{
			throw new InvalidOperationException("Boy and girl avatars can only be set once.");
		}
		Boy = boy;
		Girl = girl;
	}

	public void SetCurrentSelections(AvatarSelection boySelection, AvatarSelection girlSelection)
	{
		_boyShirt = boySelection.ShirtItem;
		_girlShirt = girlSelection.ShirtItem;
		_boyPants = boySelection.PantsItem;
		_girlPants = girlSelection.PantsItem;
		_boyShoes = boySelection.ShoesItem;
		_girlShoes = girlSelection.ShoesItem;
		_boyHair = boySelection.HairStyleItem;
		_girlHair = girlSelection.HairStyleItem;
	}

	public void Reset()
	{
		Boy = null;
		Girl = null;
		_boyShirt = null;
		_girlShirt = null;
		_boyPants = null;
		_girlPants = null;
		_boyShoes = null;
		_girlShoes = null;
		_boyHair = null;
		_girlHair = null;
		_currentGender = AvatarGender.Male;
		_oldGender = AvatarGender.Male;
	}

	public void SetCurrentGender(int value)
	{
		AvatarGender avatarGender = (AvatarGender)value;
		if (avatarGender != 0 && avatarGender != AvatarGender.Female)
		{
			throw new InvalidOperationException("Not a valid gender (" + value + ").");
		}
		_currentGender = avatarGender;
	}

	public void SetOldGender(int value)
	{
		AvatarGender avatarGender = (AvatarGender)value;
		if (avatarGender != 0 && avatarGender != AvatarGender.Female)
		{
			throw new InvalidOperationException("Not a valid gender (" + value + ").");
		}
		_oldGender = avatarGender;
	}

	public void ChangeHeight(float value)
	{
		float height = (IsMale ? Mathf.Clamp(value, 0.9f, 1.1f) : Mathf.Clamp(value, 0.85f, 1.05f));
		CurrentAvatar.ChangeHeight(height);
	}

	public void ChangeSkinColor(string value)
	{
		CurrentAvatar.ChangeSkinColor(value);
	}

	public void ChangeHairColor(int value)
	{
		CurrentAvatar.ChangeHairColor(value);
		if (CurrentHair?.BodyPack == null)
		{
			Debug.LogWarning("Got no hair on head, only brows are changed");
		}
		else
		{
			CurrentHair.UpdateColorIndex(CurrentHair.BodyPack.Path + ":Hair", value);
		}
	}

	public void ChangeEyeColor(string value)
	{
		CurrentAvatar.ChangeEyeColor(value);
	}

	public void ChangeMouth(string value)
	{
		CurrentAvatar.ChangeMouth(value);
	}

	public void ChangeEyes(string value)
	{
		CurrentAvatar.ChangeEyes(value);
	}

	public void ChangeEyeBrows(string value)
	{
		CurrentAvatar.ChangeEyeBrows(value);
	}

	public void ChangeHair(MilMo_Wearable value)
	{
		CurrentAvatar.UnequipLocal(CurrentHair);
		if (IsMale)
		{
			_boyHair = value;
		}
		else
		{
			_girlHair = value;
		}
		CurrentAvatar.EquipLocal(value);
	}

	public void ChangeShirt(string shirtIdentifier)
	{
	}

	public void ChangeShirt(MilMo_Wearable value)
	{
		CurrentAvatar.UnequipLocal(CurrentShirt);
		if (IsMale)
		{
			_boyShirt = value;
		}
		else
		{
			_girlShirt = value;
		}
		CurrentAvatar.EquipLocal(value);
	}

	public void ChangePants(MilMo_Wearable value)
	{
		CurrentAvatar.UnequipLocal(CurrentPants);
		if (IsMale)
		{
			_boyPants = value;
		}
		else
		{
			_girlPants = value;
		}
		CurrentAvatar.EquipLocal(value);
	}

	public void ChangeShoes(MilMo_Wearable value)
	{
		CurrentAvatar.UnequipLocal(CurrentShoes);
		if (IsMale)
		{
			_boyShoes = value;
		}
		else
		{
			_girlShoes = value;
		}
		CurrentAvatar.EquipLocal(value);
	}

	public void ChangeShirtColor(int value)
	{
		MilMo_Wearable currentShirt = CurrentShirt;
		if (currentShirt?.BodyPack == null)
		{
			Debug.LogWarning("No shirt to paint here");
			return;
		}
		CurrentAvatar.UnequipLocal(currentShirt);
		string colorGroup = currentShirt.BodyPack.Path + ":" + currentShirt.BodyPack.ColorGroups[0].GroupName;
		currentShirt.UpdateColorIndex(colorGroup, value);
		CurrentAvatar.EquipLocal(currentShirt);
	}

	public void ChangeShoesColor(int value, bool setPrimaryColor = true)
	{
		if (CurrentShoes?.BodyPack == null)
		{
			Debug.LogWarning("No shoes to paint here");
			return;
		}
		CurrentAvatar.UnequipLocal(CurrentShoes);
		int index = ((!setPrimaryColor) ? 1 : 0);
		string colorGroup = CurrentShoes.BodyPack.Path + ":" + CurrentShoes.BodyPack.ColorGroups[index].GroupName;
		CurrentShoes.UpdateColorIndex(colorGroup, value);
		CurrentAvatar.EquipLocal(CurrentShoes);
	}

	public void ChangePantsColor(int value)
	{
		if (CurrentPants?.BodyPack == null)
		{
			Debug.LogWarning("No pants to paint here");
			return;
		}
		CurrentAvatar.UnequipLocal(CurrentPants);
		string colorGroup = CurrentPants.BodyPack.Path + ":" + CurrentPants.BodyPack.ColorGroups[0].GroupName;
		CurrentPants.UpdateColorIndex(colorGroup, value);
		CurrentAvatar.EquipLocal(CurrentPants);
	}

	public void ChangeMood(string mood)
	{
		CurrentAvatar.SetMood(mood);
	}
}
