using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.WearableHandlers;

public class HairStyleHandler : WearableHandler
{
	private readonly AvatarEditor _avatarEditor;

	public HairStyleHandler(AvatarEditor avatarEditor)
	{
		_avatarEditor = avatarEditor;
	}

	public void SetHairStyle(string bodypack)
	{
		IItem item = _avatarEditor.CurrentAvatarHandler.Defaults.FindHairStyle(bodypack);
		if (item == null)
		{
			Debug.LogWarning("Unable to find hairstyle");
		}
		else
		{
			SetHairStyle(item);
		}
	}

	public void SetHairStyle(IItem item)
	{
		MilMo_Wearable wearable = GetWearable(item);
		SetHairStyle(wearable);
	}

	private void SetHairStyle(MilMo_Wearable hairStyle)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeHair(hairStyle);
		_avatarEditor.AvatarUpdated();
	}

	public void SetHairColor(int hairColor)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeHairColor(hairColor);
		_avatarEditor.AvatarUpdated();
	}
}
