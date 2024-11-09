using UnityEngine;

namespace Core.BodyShapes;

[CreateAssetMenu(menuName = "Shapes/SkinPartShape", fileName = "new shape")]
public class ScriptableShape : ScriptableObject
{
	[SerializeField]
	private string identifier;

	[SerializeField]
	private Texture2D textureMap;

	[SerializeField]
	private Sprite icon;

	public Sprite GetIcon()
	{
		return icon;
	}

	public string GetIdentifier()
	{
		return identifier;
	}

	public Texture2D GetTexture()
	{
		return textureMap;
	}
}
