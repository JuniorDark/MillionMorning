using Code.Core.Items;
using UnityEngine;

namespace Code.World.CharBuilder.WearableHandlers;

public class PantsHandler : WearableHandler
{
	private readonly AvatarEditor _avatarEditor;

	public PantsHandler(AvatarEditor avatarEditor)
	{
		_avatarEditor = avatarEditor;
	}

	public void SetPants(string bodypack)
	{
		IItem item = _avatarEditor.CurrentAvatarHandler.Defaults.FindPants(bodypack);
		if (item == null)
		{
			Debug.LogWarning("Unable to find pants");
		}
		else
		{
			SetPants(item);
		}
	}

	public void SetPants(IItem item)
	{
		MilMo_Wearable wearable = GetWearable(item);
		SetPants(wearable);
	}

	private void SetPants(MilMo_Wearable pants)
	{
		_avatarEditor.CurrentAvatarHandler.ChangePants(pants);
		_avatarEditor.AvatarUpdated();
	}

	public void SetPantsColor(int colorIndex)
	{
		_avatarEditor.CurrentAvatarHandler.ChangePantsColor(colorIndex);
		_avatarEditor.AvatarUpdated();
	}
}
