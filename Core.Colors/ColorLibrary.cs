using System.Linq;
using UnityEngine;

namespace Core.Colors;

public class ColorLibrary : MonoBehaviour
{
	[SerializeField]
	private ScriptableColor[] colors;

	public ScriptableColor GetColorByIdentifier(string colorIdentifier)
	{
		return colors.FirstOrDefault((ScriptableColor c) => c.GetIdentifier() == colorIdentifier);
	}
}
