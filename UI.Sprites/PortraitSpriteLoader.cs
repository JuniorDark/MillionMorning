using System.Threading.Tasks;
using Code.World.Player;
using UnityEngine;

namespace UI.Sprites;

public class PortraitSpriteLoader : IHaveSprite
{
	private readonly string _playerId;

	public PortraitSpriteLoader(string playerId)
	{
		_playerId = playerId;
	}

	public async Task<Sprite> GetSpriteAsync()
	{
		return Sprite.Create(await MilMo_ProfileManager.GetPlayerPortraitAsync(_playerId), new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f));
	}
}
