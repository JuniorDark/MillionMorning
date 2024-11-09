using System.Collections.Generic;

namespace Code.Core.Sound;

public class MilMo_AudioClipLibrary
{
	public static Dictionary<string, MilMo_SoundType> Paths = new Dictionary<string, MilMo_SoundType>
	{
		{
			"Content/Sounds/Batch01/GUI/Generic/Select",
			MilMo_SoundType.Select
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/SelectMainCat",
			MilMo_SoundType.SelectMainCat
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/SelectSubCat",
			MilMo_SoundType.SelectSubCat
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Back",
			MilMo_SoundType.Back
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Info",
			MilMo_SoundType.Info
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Confirm",
			MilMo_SoundType.Confirm
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Pick",
			MilMo_SoundType.Pick
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Cancel",
			MilMo_SoundType.Cancel
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Wrong",
			MilMo_SoundType.Wrong
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Swap",
			MilMo_SoundType.Swap
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Tick",
			MilMo_SoundType.Tick
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Attention",
			MilMo_SoundType.Attention
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Message",
			MilMo_SoundType.Message
		},
		{
			"Content/Sounds/Batch01/GUI/Generic/Request",
			MilMo_SoundType.Request
		}
	};
}
