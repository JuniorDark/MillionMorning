using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Sprites;

public class AddressableSpriteLoader : IHaveSprite
{
	private readonly string _addressableKey;

	public AddressableSpriteLoader(string addressableKey)
	{
		_addressableKey = addressableKey;
	}

	public Task<Sprite> GetSpriteAsync()
	{
		return Addressables.LoadAssetAsync<Sprite>(_addressableKey).Task;
	}
}
