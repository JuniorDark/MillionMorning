using System.Collections.Generic;

namespace Code.World.CharBuilder;

public class AvatarCreatorMaleDefaults : AvatarCreatorDefaults
{
	private const float MIN_HEIGHT = 0.9f;

	private const float MAX_HEIGHT = 1.1f;

	private readonly List<string> _mouths = new List<string> { "BoyMouth01", "BoyMouth02", "BoyMouth03", "BoyMouth04", "BoyMouth05", "BoyMouth06" };

	private readonly List<string> _eyes = new List<string> { "BoyEyes01", "BoyEyes02", "BoyEyes03", "BoyEyes04", "BoyEyes05", "BoyEyes06" };

	private readonly List<string> _eyeBrows = new List<string> { "BoyEyeBrows01", "BoyEyeBrows02", "BoyEyeBrows03", "BoyEyeBrows04" };

	private readonly List<IItem> _shirts = new List<IItem>
	{
		new GenericCharbuilderItem("Shirt0008", "Boy", "Shirts", "BoyShirtsCharBuilder"),
		new GenericCharbuilderItem("SleeveShirt0011", "Boy", "Shirts", "BoyShirtsCharBuilder"),
		new GenericCharbuilderItem("Top0004", "Boy", "Shirts", "BoyShirtsCharBuilder"),
		new GenericCharbuilderItem("Tshirt0017", "Boy", "Shirts", "BoyShirtsCharBuilder")
	};

	private readonly List<IItem> _pants = new List<IItem>
	{
		new GenericCharbuilderItem("Jeans0005", "Boy", "Pants", "BoyPantsCharBuilder"),
		new GenericCharbuilderItem("Pants0014", "Boy", "Pants", "BoyPantsCharBuilder"),
		new GenericCharbuilderItem("Pants0015", "Boy", "Pants", "BoyPantsCharBuilder"),
		new GenericCharbuilderItem("Shorts0003", "Boy", "Pants", "BoyPantsCharBuilder")
	};

	private readonly List<IItem> _shoes = new List<IItem>
	{
		new GenericCharbuilderItem("Shoes0017", "Boy", "Shoes", "BoyShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0018", "Boy", "Shoes", "BoyShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0019", "Boy", "Shoes", "BoyShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0016", "Boy", "Shoes", "BoyShoesCharBuilder")
	};

	private readonly List<IItem> _hairs = new List<IItem>
	{
		new GenericHairCharbuilderItem("HairStyle0009", "Boy", "BoyHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0008", "Boy", "BoyHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0006", "Boy", "BoyHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0010", "Boy", "BoyHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0004", "Boy", "BoyHairCharBuilder")
	};

	public override AvatarGender GetGender()
	{
		return AvatarGender.Male;
	}

	public override float GetMinHeight()
	{
		return 0.9f;
	}

	public override float GetMaxHeight()
	{
		return 1.1f;
	}

	public override List<IItem> GetHairStyles()
	{
		return _hairs;
	}

	public override List<string> GetEyeBrows()
	{
		return _eyeBrows;
	}

	public override List<string> GetEyes()
	{
		return _eyes;
	}

	public override List<string> GetMouths()
	{
		return _mouths;
	}

	public override List<IItem> GetShirts()
	{
		return _shirts;
	}

	public override List<IItem> GetPants()
	{
		return _pants;
	}

	public override List<IItem> GetShoes()
	{
		return _shoes;
	}
}
