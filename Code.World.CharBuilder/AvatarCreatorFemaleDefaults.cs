using System.Collections.Generic;

namespace Code.World.CharBuilder;

public class AvatarCreatorFemaleDefaults : AvatarCreatorDefaults
{
	private const float MIN_HEIGHT = 0.85f;

	private const float MAX_HEIGHT = 1.05f;

	private readonly List<string> _mouths = new List<string> { "GirlMouth01", "GirlMouth02", "GirlMouth03", "GirlMouth04", "GirlMouth05", "GirlMouth06" };

	private readonly List<string> _eyes = new List<string> { "GirlEyes01", "GirlEyes02", "GirlEyes03", "GirlEyes04", "GirlEyes05", "GirlEyes06" };

	private readonly List<string> _eyeBrows = new List<string> { "GirlEyeBrows01", "GirlEyeBrows02", "GirlEyeBrows03", "GirlEyeBrows04" };

	private readonly List<IItem> _shirts = new List<IItem>
	{
		new GenericCharbuilderItem("Shirt0006", "Girl", "Shirts", "GirlShirtsCharBuilder"),
		new GenericCharbuilderItem("SleeveShirt0008", "Girl", "Shirts", "GirlShirtsCharBuilder"),
		new GenericCharbuilderItem("Top0009", "Girl", "Shirts", "GirlShirtsCharBuilder"),
		new GenericCharbuilderItem("Tshirt0017", "Girl", "Shirts", "GirlShirtsCharBuilder")
	};

	private readonly List<IItem> _pants = new List<IItem>
	{
		new GenericCharbuilderItem("Jeans0005", "Girl", "Pants", "GirlsPantsCharBuilder"),
		new GenericCharbuilderItem("Pants0010", "Girl", "Pants", "GirlsPantsCharBuilder"),
		new GenericCharbuilderItem("Pants0011", "Girl", "Pants", "GirlsPantsCharBuilder"),
		new GenericCharbuilderItem("Skirt0005", "Girl", "Pants", "GirlsPantsCharBuilder")
	};

	private readonly List<IItem> _shoes = new List<IItem>
	{
		new GenericCharbuilderItem("Shoes0018", "Girl", "Shoes", "GirlsShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0019", "Girl", "Shoes", "GirlsShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0020", "Girl", "Shoes", "GirlsShoesCharBuilder"),
		new GenericCharbuilderItem("Shoes0017", "Girl", "Shoes", "GirlsShoesCharBuilder")
	};

	private readonly List<IItem> _hairs = new List<IItem>
	{
		new GenericHairCharbuilderItem("HairStyle0009", "Girl", "GirlHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0001", "Girl", "GirlHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0007", "Girl", "GirlHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0013", "Girl", "GirlHairCharBuilder"),
		new GenericHairCharbuilderItem("HairStyle0014", "Girl", "GirlHairCharBuilder")
	};

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

	public override float GetMaxHeight()
	{
		return 1.05f;
	}

	public override float GetMinHeight()
	{
		return 0.85f;
	}

	public override AvatarGender GetGender()
	{
		return AvatarGender.Female;
	}
}
