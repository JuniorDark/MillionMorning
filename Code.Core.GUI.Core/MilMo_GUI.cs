using UnityEngine;

namespace Code.Core.GUI.Core;

public static class MilMo_GUI
{
	public enum Align
	{
		TopLeft,
		TopCenter,
		TopRight,
		CenterLeft,
		CenterCenter,
		CenterRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	public enum HoverBehaviour
	{
		None,
		Fade,
		Impulse,
		PosImpulse,
		Snap
	}

	public enum DrawFunction
	{
		Normal,
		Hover
	}

	public enum CropMode
	{
		Stretch,
		Crop,
		Cropadelic
	}

	public enum FontPreset
	{
		Normal,
		DropShadow,
		Outline,
		OutlineDropShadow
	}

	public enum Font
	{
		ArialRounded,
		ArialRoundedMedium,
		EborgSmall,
		EborgMedium,
		EborgLarge,
		EborgXL,
		GothamSmall,
		GothamMedium,
		GothamLarge
	}

	public const string GUI_CONTENT_PATH = "Content/GUI/";

	public const float CENTER = 0.5f;

	public const float LEFT = 0f;

	public const float RIGHT = 1f;

	public const float TOP = 0f;

	public const float BOTTOM = 1f;

	public static Vector2 InvalidVector2 = new Vector2(-99999f, -99999f);

	public const string INVISIBLE_TEXTURE_PATH = "Batch01/Textures/Core/Invisible";

	public static float GlobalFade = 1f;

	public const int JUNEBUG = 0;

	public const int CHAT = 1;

	public const int JUNEBUG_DARKER = 2;

	public static Vector2 Align2Float(Align align)
	{
		return align switch
		{
			Align.TopLeft => new Vector2(0f, 0f), 
			Align.TopCenter => new Vector2(0.5f, 0f), 
			Align.TopRight => new Vector2(1f, 0f), 
			Align.CenterLeft => new Vector2(0f, 0.5f), 
			Align.CenterCenter => new Vector2(0.5f, 0.5f), 
			Align.CenterRight => new Vector2(1f, 0.5f), 
			Align.BottomLeft => new Vector2(0f, 1f), 
			Align.BottomCenter => new Vector2(0.5f, 1f), 
			Align.BottomRight => new Vector2(1f, 1f), 
			_ => new Vector2(0f, 0f), 
		};
	}
}
