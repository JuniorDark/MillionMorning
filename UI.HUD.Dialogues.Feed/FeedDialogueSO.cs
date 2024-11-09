using System.Threading.Tasks;
using Core.Utilities;
using UnityEngine;

namespace UI.HUD.Dialogues.Feed;

public abstract class FeedDialogueSO : DialogueSO
{
	public async Task<Texture2D> GetIconAsync(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		return await Core.Utilities.UI.GetIcon(path);
	}

	public virtual Transform GetObjectDestination()
	{
		return null;
	}
}
