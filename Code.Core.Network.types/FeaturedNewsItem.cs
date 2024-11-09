namespace Code.Core.Network.types;

public class FeaturedNewsItem
{
	private readonly string _headline;

	private readonly string _message;

	private readonly string _iconPath;

	public FeaturedNewsItem(MessageReader reader)
	{
		_headline = reader.ReadString();
		_message = reader.ReadString();
		_iconPath = reader.ReadString();
	}

	public FeaturedNewsItem(string headline, string message, string iconPath)
	{
		_headline = headline;
		_message = message;
		_iconPath = iconPath;
	}

	public string GetHeadline()
	{
		return _headline;
	}

	public string GetMessage()
	{
		return _message;
	}

	public string GetIconPath()
	{
		return _iconPath;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_headline) + MessageWriter.GetSize(_message) + MessageWriter.GetSize(_iconPath);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_headline);
		writer.WriteString(_message);
		writer.WriteString(_iconPath);
	}
}
