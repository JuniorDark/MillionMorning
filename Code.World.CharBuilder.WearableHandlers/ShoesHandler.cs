using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.WearableHandlers;

public class ShoesHandler : WearableHandler
{
	private readonly AvatarEditor _avatarEditor;

	public ShoesHandler(AvatarEditor avatarEditor)
	{
		_avatarEditor = avatarEditor;
	}

	public void SetShoes(string bodypack)
	{
		IItem item = _avatarEditor.CurrentAvatarHandler.Defaults.FindShoe(bodypack);
		if (item == null)
		{
			Debug.LogWarning("Unable to find shoes");
		}
		else
		{
			SetShoes(item);
		}
	}

	public void SetShoes(IItem item)
	{
		MilMo_Wearable wearable = GetWearable(item);
		SetShoes(wearable);
	}

	private void SetShoes(MilMo_Wearable shoes)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeShoes(shoes);
		_avatarEditor.AvatarUpdated();
	}

	public void SetShoesPrimaryColor(int colorIndex)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeShoesColor(colorIndex);
		_avatarEditor.AvatarUpdated();
	}

	public void SetShoesSecondaryColor(int colorIndex)
	{
		_avatarEditor.CurrentAvatarHandler.ChangeShoesColor(colorIndex, setPrimaryColor: false);
		_avatarEditor.AvatarUpdated();
	}
}
