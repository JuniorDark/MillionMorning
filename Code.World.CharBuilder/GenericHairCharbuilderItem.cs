namespace Code.World.CharBuilder;

public class GenericHairCharbuilderItem : CharbuilderItem
{
	public GenericHairCharbuilderItem(string identifier, string gender, string category)
	{
		Identifier = identifier;
		FilePath = "Charbuilder/" + gender + "Clothing/HairStyles/" + identifier;
		Path = "Charbuilder." + gender + "Clothing.HairStyles." + identifier;
		Bodypack = "Batch01.Generic.Scripts.HairStyles." + identifier;
		Category = category;
	}
}
