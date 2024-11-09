using UnityEngine;

namespace Core.BodyShapes;

public readonly struct ShapePart
{
	public readonly Rect Region;

	public readonly Texture Texture;

	public ShapePart(Rect region, Texture texture)
	{
		Region = region;
		Texture = texture;
	}
}
