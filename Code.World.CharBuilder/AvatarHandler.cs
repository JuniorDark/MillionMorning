using System;
using System.Threading.Tasks;
using Code.Core.Avatar;
using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder;

public class AvatarHandler : IAvatarChange
{
	public readonly AvatarCreatorDefaults Defaults;

	private MilMo_Avatar _avatar;

	private MilMo_Wearable _shirt;

	private MilMo_Wearable _pants;

	private MilMo_Wearable _shoes;

	private MilMo_Wearable _hair;

	private AvatarSelection _selection;

	public event Action<MilMo_Avatar> AvatarLoaded;

	public AvatarHandler(AvatarGender gender)
	{
		if (gender == AvatarGender.Male)
		{
			Defaults = new AvatarCreatorMaleDefaults();
		}
		else
		{
			Defaults = new AvatarCreatorFemaleDefaults();
		}
		_selection = Defaults.GetRandomAvatarSelection();
	}

	public void Destroy()
	{
		_avatar?.Destroy();
	}

	public async Task InitAvatar()
	{
		_avatar = await CreateAvatarAsync(_selection);
		this.AvatarLoaded?.Invoke(_avatar);
	}

	public MilMo_Avatar GetAvatar()
	{
		return _avatar;
	}

	public AvatarGender GetGender()
	{
		return Defaults.GetGender();
	}

	public AvatarSelection GetSelection()
	{
		return _selection;
	}

	public void SetSelection(AvatarSelection newSelection)
	{
		_selection = newSelection;
	}

	public float[] GetHeightRange()
	{
		return new float[2]
		{
			Defaults.GetMinHeight(),
			Defaults.GetMaxHeight()
		};
	}

	public void Hide()
	{
		_avatar.GameObject.SetActive(value: false);
	}

	public void Show()
	{
		_avatar.GameObject.SetActive(value: true);
	}

	private async Task<MilMo_Avatar> CreateAvatarAsync(AvatarSelection selection, string userTag = "")
	{
		string text = ((selection.Gender == 0) ? "Boy" : "Girl");
		Debug.Log("Creating Avatar... " + text);
		TaskCompletionSource<MilMo_Avatar> tcs = new TaskCompletionSource<MilMo_Avatar>();
		MilMo_Avatar milMo_Avatar = new MilMo_Avatar(thumbnailMode: false);
		milMo_Avatar.BodyPackManager.SetMainTextureSize(MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize);
		milMo_Avatar.SetInitializedCallback(delegate(MilMo_Avatar initializedAvatar, string initializedTag)
		{
			if (initializedAvatar == null)
			{
				Debug.LogError("Initialized avatar is null");
				tcs.TrySetResult(null);
			}
			else
			{
				Debug.Log("Initialized avatar is ready");
				initializedAvatar.EquipLocal(selection.ShirtItem);
				initializedAvatar.EquipLocal(selection.PantsItem);
				initializedAvatar.EquipLocal(selection.ShoesItem);
				initializedAvatar.AsyncApply();
				tcs.TrySetResult(initializedAvatar);
			}
		}, userTag);
		milMo_Avatar.InitLocal(selection.AvatarName, selection.Gender, selection.SkinColor, selection.EyeColor, selection.HairColor, selection.Mouth, selection.Eyes, selection.EyeBrows, selection.HairStyleItem, selection.Height);
		return await tcs.Task;
	}

	public void ChangeHeight(float value)
	{
		float height = Mathf.Clamp(value, Defaults.GetMinHeight(), Defaults.GetMaxHeight());
		_avatar.ChangeHeight(height);
		_selection.SetHeight(value);
	}

	public void ChangeSkinColor(string value)
	{
		_avatar.ChangeSkinColor(value);
		_selection.SetSkinColor(value);
	}

	public void ChangeHair(MilMo_Wearable value)
	{
		_avatar.UnequipLocal(_hair);
		_hair = value;
		_selection.SetHairStyle(Defaults.FindHairStyle(value.BodyPack.Path));
		UpdateColor(ref _selection.HairColor, _hair);
		_avatar.EquipLocal(_hair);
		_selection.HairStyleItem = _hair;
	}

	public void ChangeHairColor(int value)
	{
		_avatar.ChangeHairColor(value);
		if (_hair?.BodyPack == null)
		{
			Debug.LogWarning("Got no hair on head, only brows are changed");
			return;
		}
		_hair.UpdateColorIndex(_hair.BodyPack.Path + ":Hair", value);
		_selection.HairColor = value;
		_selection.HairStyleItem = _hair;
	}

	public void ChangeEyeBrows(string value)
	{
		_avatar.ChangeEyeBrows(value);
		_selection.SetEyeBrows(value);
	}

	public void ChangeEyes(string value)
	{
		_avatar.ChangeEyes(value);
		if (_selection.EyeColor != null)
		{
			_avatar.ChangeEyeColor(_selection.EyeColor);
		}
		_selection.SetEyes(value);
	}

	public void ChangeEyeColor(string value)
	{
		_avatar.ChangeEyeColor(value);
		_selection.SetEyeColor(value);
	}

	public void ChangeMouth(string value)
	{
		_avatar.ChangeMouth(value);
		_selection.SetMouth(value);
	}

	public void ChangeShirt(MilMo_Wearable value)
	{
		_avatar.UnequipLocal(_shirt);
		_shirt = value;
		_selection.SetShirt(Defaults.FindShirt(_shirt.BodyPack.Path));
		UpdateColor(ref _selection.ShirtColor, _shirt);
		_avatar.EquipLocal(_shirt);
		_selection.ShirtItem = _shirt;
	}

	public void ChangeShirtColor(int value)
	{
		if (_shirt?.BodyPack == null)
		{
			Debug.LogWarning("No shirt to paint here");
			return;
		}
		_avatar.UnequipLocal(_shirt);
		string colorGroup = _shirt.GetColorGroup();
		_shirt.UpdateColorIndex(colorGroup, value);
		_avatar.EquipLocal(_shirt);
		_selection.ShirtItem = _shirt;
		_selection.ShirtColor = value.ToString();
	}

	public void ChangePants(MilMo_Wearable value)
	{
		_avatar.UnequipLocal(_pants);
		_pants = value;
		_selection.PantsItem = _pants;
		_selection.SetPants(Defaults.FindPants(_pants.BodyPack.Path));
		UpdateColor(ref _selection.PantsColor, _pants);
		_avatar.EquipLocal(_pants);
	}

	private void ChangeColor(MilMo_Wearable wearable, string colorGroup, int value)
	{
		if (wearable?.BodyPack == null)
		{
			Debug.LogWarning("No wearable to paint here");
		}
		else
		{
			wearable.UpdateColorIndex(colorGroup, value);
		}
	}

	public void ChangePantsColor(int value)
	{
		if (_pants?.BodyPack == null)
		{
			Debug.LogWarning("No pants to paint here");
			return;
		}
		_avatar.UnequipLocal(_pants);
		string colorGroup = _pants.GetColorGroup();
		_pants.UpdateColorIndex(colorGroup, value);
		_avatar.EquipLocal(_pants);
		_selection.PantsItem = _pants;
		_selection.PantsColor = value.ToString();
	}

	public void ChangeShoes(MilMo_Wearable value)
	{
		_avatar.UnequipLocal(_shoes);
		_shoes = value;
		UpdateColor(ref _selection.ShoesColor, _shoes);
		UpdateColor(ref _selection.LacesColor, _shoes, 1);
		_avatar.EquipLocal(_shoes);
		_selection.ShoesItem = _shoes;
		_selection.SetShoes(Defaults.FindShoe(_shoes.BodyPack.Path));
	}

	private void UpdateColor(ref string selection, MilMo_Wearable item, int colorGroup = 0)
	{
		if (selection != null)
		{
			ChangeColor(item, item.GetColorGroup(colorGroup), int.Parse(selection));
		}
		else
		{
			selection = item.GetColor(item.GetColorGroup(colorGroup)).ToString();
		}
	}

	private void UpdateColor(ref int selection, MilMo_Wearable item, int colorGroup = 0)
	{
		if (selection != -1)
		{
			ChangeColor(item, item.GetColorGroup(colorGroup), selection);
		}
		else
		{
			selection = item.GetColor(item.GetColorGroup(colorGroup));
		}
	}

	public void ChangeShoesColor(int value, bool setPrimaryColor = true)
	{
		if (_shoes?.BodyPack == null)
		{
			Debug.LogWarning("No shoes to paint here");
			return;
		}
		_avatar.UnequipLocal(_shoes);
		int num = ((!setPrimaryColor) ? 1 : 0);
		string colorGroup = _shoes.GetColorGroup(num);
		_shoes.UpdateColorIndex(colorGroup, value);
		_avatar.EquipLocal(_shoes);
		_selection.ShoesItem = _shoes;
		if (num == 0)
		{
			_selection.ShoesColor = value.ToString();
		}
		else
		{
			_selection.LacesColor = value.ToString();
		}
	}

	public void ChangeMood(string mood)
	{
		_avatar.SetMood(mood);
		_selection.SetMood(mood);
	}

	public void ChangeName(string name)
	{
		_selection.SetAvatarName(name);
	}
}
