namespace Code.World.CharBuilder;

public interface IItem
{
	const string PANTS = "Pants";

	const string SHIRTS = "Shirts";

	const string SHOES = "Shoes";

	const string HAIR = "Hair";

	const string MALE = "Boy";

	const string FEMALE = "Girl";

	const string CATEGORY_HAIR_FEMALE = "GirlHairCharBuilder";

	const string CATEGORY_HAIR_MALE = "BoyHairCharBuilder";

	const string CATEGORY_SHIRT_FEMALE = "GirlShirtsCharBuilder";

	const string CATEGORY_SHIRT_MALE = "BoyShirtsCharBuilder";

	const string CATEGORY_PANTS_FEMALE = "GirlsPantsCharBuilder";

	const string CATEGORY_PANTS_MALE = "BoyPantsCharBuilder";

	const string CATEGORY_SHOES_FEMALE = "GirlsShoesCharBuilder";

	const string CATEGORY_SHOES_MALE = "BoyShoesCharBuilder";

	string GetIdentifier();

	string GetPath();

	string GetFilePath();

	string GetBodyPack();

	string GetCategory();
}
