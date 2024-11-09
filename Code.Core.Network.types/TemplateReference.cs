namespace Code.Core.Network.types;

public class TemplateReference
{
	private readonly string _category;

	private readonly string _path;

	public TemplateReference(MessageReader reader)
	{
		_category = reader.ReadString();
		_path = reader.ReadString();
	}

	public TemplateReference(string category, string path)
	{
		_category = category;
		_path = path;
	}

	public string GetIdentifier()
	{
		return _category + ":" + _path;
	}

	public string GetCategory()
	{
		return _category;
	}

	public string GetPath()
	{
		return _path;
	}

	public int Size()
	{
		return 4 + MessageWriter.GetSize(_category) + MessageWriter.GetSize(_path);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_category);
		writer.WriteString(_path);
	}
}
