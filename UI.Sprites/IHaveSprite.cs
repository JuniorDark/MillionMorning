using System.Threading.Tasks;
using UnityEngine;

namespace UI.Sprites;

public interface IHaveSprite
{
	Task<Sprite> GetSpriteAsync();
}
