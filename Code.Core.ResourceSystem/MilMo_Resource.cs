namespace Code.Core.ResourceSystem;

public abstract class MilMo_Resource
{
	protected string Path { get; set; }

	protected bool IsLoading { get; set; }

	protected MilMo_Resource()
	{
		IsLoading = false;
		Path = "";
	}

	public abstract void LoadLocal(string path);

	public abstract void LoadLocal();

	public abstract void AsyncLoad(string path);

	public abstract void AsyncLoad();
}
