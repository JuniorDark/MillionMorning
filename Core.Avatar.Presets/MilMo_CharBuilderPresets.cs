namespace Core.Avatar.Presets;

public static class MilMo_CharBuilderPresets
{
	public const int NAME = 0;

	public const int SKIN_COLOR = 1;

	public const int EYE_COLOR = 2;

	public const int HAIR_COLOR = 3;

	public const int EYES = 4;

	public const int EYEBROWS = 5;

	public const int MOUTH = 6;

	public const int HAIR = 7;

	public const int HAIR_BODY_PACK = 8;

	public const int SHIRT = 9;

	public const int SHIRT_COLOR = 10;

	public const int PANTS = 11;

	public const int PANTS_COLOR = 12;

	public const int SHOES = 13;

	public const int SHOES_COLOR1 = 14;

	public const int SHOES_COLOR2 = 15;

	public const int MOOD = 16;

	public static readonly string[][] BoyPresets = new string[4][]
	{
		new string[17]
		{
			"Boy01", "Skin0003", "BrownLight", "71", "BoyEyes05", "BoyEyeBrows04", "BoyMouth05", "BoyHairCharBuilder:HairStyle0010", "BoyHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0010", "BoyShirtsCharBuilder:Top0004",
			"ColorGroup:Top#106", "BoyPantsCharBuilder:Shorts0003", "ColorGroup:Pants#115", "BoyShoesCharBuilder:Shoes0016", "ColorGroup:Laces#146", "ColorGroup:Shoes#116", "Normal"
		},
		new string[17]
		{
			"Boy02", "Skin0008", "Brown", "57", "BoyEyes05", "BoyEyeBrows01", "BoyMouth01", "BoyHairCharBuilder:HairStyle0004", "BoyHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0004", "BoyShirtsCharBuilder:Shirt0008",
			"ColorGroup:Shirt#121", "BoyPantsCharBuilder:Jeans0005", "ColorGroup:Pants#108", "BoyShoesCharBuilder:Shoes0019", "ColorGroup:Laces#146", "ColorGroup:Shoes#113", "Normal"
		},
		new string[17]
		{
			"Boy03", "Skin0002", "Amber", "63", "BoyEyes04", "BoyEyeBrows01", "BoyMouth04", "BoyHairCharBuilder:HairStyle0008", "BoyHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0008", "BoyShirtsCharBuilder:Tshirt0017",
			"ColorGroup:Shirt#102", "BoyPantsCharBuilder:Pants0014", "ColorGroup:Pants#132", "BoyShoesCharBuilder:Shoes0018", "ColorGroup:Laces#114", "ColorGroup:Shoes#116", "Happy"
		},
		new string[17]
		{
			"Boy04", "Skin0004", "BrownLight", "57", "BoyEyes05", "BoyEyeBrows03", "BoyMouth03", "BoyHairCharBuilder:HairStyle0006", "BoyHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0006", "BoyShirtsCharBuilder:SleeveShirt0011",
			"ColorGroup:Shirt#104", "BoyPantsCharBuilder:Pants0015", "ColorGroup:Pants#147", "BoyShoesCharBuilder:Shoes0017", "ColorGroup:Shoes#116", "ColorGroup:Soles#146", "Angry"
		}
	};

	public static readonly string[][] GirlPresets = new string[4][]
	{
		new string[17]
		{
			"Girl01", "Skin0003", "GreenLight", "73", "GirlEyes06", "GirlEyeBrows02", "GirlMouth01", "GirlHairCharBuilder:HairStyle0001", "GirlHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0001", "GirlShirtsCharBuilder:Top0009",
			"ColorGroup:Top#102", "GirlsPantsCharBuilder:Pants0010", "ColorGroup:Pants#147", "GirlsShoesCharBuilder:Shoes0020", "ColorGroup:Laces#146", "ColorGroup:Shoes#116", "Normal"
		},
		new string[17]
		{
			"Girl02", "Skin0004", "BrownLight", "57", "GirlEyes06", "GirlEyeBrows02", "GirlMouth03", "GirlHairCharBuilder:HairStyle0014", "GirlHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0014", "GirlShirtsCharBuilder:Shirt0006",
			"ColorGroup:Shirt#118", "GirlsPantsCharBuilder:Pants0011", "ColorGroup:Pants#140", "GirlsShoesCharBuilder:Shoes0017", "ColorGroup:Laces#117", "ColorGroup:Shoes#116", "Normal"
		},
		new string[17]
		{
			"Girl03", "Skin0004", "Amber", "63", "GirlEyes03", "GirlEyeBrows02", "GirlMouth02", "GirlHairCharBuilder:HairStyle0013", "GirlHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0013", "GirlShirtsCharBuilder:SleeveShirt0008",
			"ColorGroup:Shirt#124", "GirlsPantsCharBuilder:Skirt0005", "ColorGroup:Skirt#147", "GirlsShoesCharBuilder:Shoes0018", "ColorGroup:Laces#114", "ColorGroup:Shoes#125", "Happy"
		},
		new string[17]
		{
			"Girl04", "Skin0008", "Orange", "71", "GirlEyes06", "GirlEyeBrows01", "GirlMouth04", "GirlHairCharBuilder:HairStyle0009", "GirlHairCharBuilder:Bodypacks.Batch01.Generic.Scripts.HairStyles.HairStyle0009", "GirlShirtsCharBuilder:Top0009",
			"ColorGroup:Top#121", "GirlsPantsCharBuilder:Jeans0005", "ColorGroup:Pants#115", "GirlsShoesCharBuilder:Shoes0020", "ColorGroup:Laces#114", "ColorGroup:Shoes#145", "Happy"
		}
	};

	public static string GetColorIndexFromModifier(string modifier)
	{
		return modifier.Split("#".ToCharArray())[1];
	}

	public static string GetColorGroupNameFromModifier(string modifier)
	{
		return modifier.Split("#".ToCharArray())[0].Split(':')[1];
	}
}
