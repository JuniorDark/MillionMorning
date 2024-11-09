using Code.Core.Network.types;
using Code.Core.ResourceSystem;

namespace Code.Core.Template;

public abstract class MilMo_Template
{
	public string Category;

	public string Path;

	protected string FilePath;

	public string Type;

	public string Identifier => Category + ":" + Path;

	public string Name { get; protected set; }

	public TemplateReference TemplateReferenceStruct => new TemplateReference(Category, Path);

	protected MilMo_Template(string category, string path, string filePath, string type)
	{
		Category = category;
		Path = path;
		FilePath = filePath;
		Type = type;
		int num = Path.LastIndexOf('.');
		if (num == -1)
		{
			num = Path.LastIndexOf('/');
		}
		Name = ((num == -1) ? Path : Path.Substring(num + 1));
	}

	public virtual bool ReadLine(MilMo_SFFile file)
	{
		return true;
	}

	public virtual bool FinishLoading()
	{
		return true;
	}

	public virtual bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		return true;
	}
}
