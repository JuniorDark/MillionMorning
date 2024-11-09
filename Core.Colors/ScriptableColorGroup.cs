using UnityEngine;

namespace Core.Colors;

[CreateAssetMenu(menuName = "Color/ColorGroup", fileName = "new ColorGroup")]
public class ScriptableColorGroup : ScriptableObject
{
	[SerializeField]
	private string identifier;

	public ScriptableColor[] colors;
}
