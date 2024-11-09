namespace Code.World.CharBuilder;

public class GenericCharbuilderItem : CharbuilderItem
{
	public GenericCharbuilderItem(string identifier, string gender, string shape, string category)
	{
		Identifier = identifier;
		FilePath = "CharBuilder/" + gender + "Clothing/" + shape + "/" + identifier;
		Path = "CharBuilder." + gender + "Clothing." + shape + "." + identifier;
		Bodypack = "Batch01." + gender + ".Scripts." + shape + "." + identifier;
		Category = category;
	}
}
