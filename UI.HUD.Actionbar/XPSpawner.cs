using Core.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.HUD.Actionbar;

public class XPSpawner : MonoBehaviour
{
	[SerializeField]
	private AssetReference xpPrefab;

	public void Spawn(int xp)
	{
		if (xp > 0)
		{
			XPBubble xPBubble = Instantiator.Instantiate<XPBubble>(xpPrefab, base.transform);
			if (xPBubble != null)
			{
				xPBubble.SetXP(xp);
			}
		}
	}
}
