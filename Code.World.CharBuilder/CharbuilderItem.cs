namespace Code.World.CharBuilder;

public abstract class CharbuilderItem : IItem
{
	protected string Identifier;

	protected string FilePath;

	protected string Path;

	protected string Bodypack;

	protected string Category;

	public string GetIdentifier()
	{
		return Identifier;
	}

	public string GetFilePath()
	{
		return FilePath;
	}

	public string GetPath()
	{
		return Path;
	}

	public string GetBodyPack()
	{
		return Bodypack;
	}

	public string GetCategory()
	{
		return Category;
	}
}
