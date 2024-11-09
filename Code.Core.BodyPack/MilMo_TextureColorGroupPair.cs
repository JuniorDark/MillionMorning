using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_TextureColorGroupPair
{
	private readonly string _textureName;

	private readonly ColorGroup _colorGroup;

	private readonly Vector2 _uvOffset;

	public string TextureName => _textureName;

	public Texture2D Texture { get; set; }

	public ColorGroup ColorGroup => _colorGroup;

	public Vector2 UVOffset => _uvOffset;

	public MilMo_TextureColorGroupPair(string textureName, ColorGroup colorGroup, Vector2 uvOffset)
	{
		_textureName = textureName;
		_colorGroup = colorGroup;
		_uvOffset = uvOffset;
	}
}
