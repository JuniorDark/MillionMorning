using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.WearableHandlers;

public class ShirtHandler : WearableHandler
{
	private readonly AvatarEditor _avatarEditor;

	public ShirtHandler(AvatarEditor avatarEditor)
	{
		_avatarEditor = avatarEditor;
	}

	public void SetShirt(string bodypack)
	{
		IItem item = _avatarEditor.CurrentAvatarHandler.Defaults.FindShirt(bodypack);
		if (item == null)
		{
			Debug.LogWarning("Unable to find shirt");
		}
		else
		{
			SetShirt(item);
		}
	}

	public void SetShirt(IItem item)
	{
		MilMo_Wearable wearable = GetWearable(item);
		SetShirt(wearable);
	}

	private void SetShirt(MilMo_Wearable shirt)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeShirt(shirt);
		_avatarEditor.AvatarUpdated();
	}

	public void SetShirtColor(int colorIndex)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeShirtColor(colorIndex);
		_avatarEditor.AvatarUpdated();
	}
}
