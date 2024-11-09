using UnityEngine;

namespace Code.Core.ResourceSystem;

public sealed class MilMo_Texture : MilMo_Resource
{
	public Texture Texture { get; private set; }

	public MilMo_Texture(string path)
	{
		base.Path = path;
	}

	public MilMo_Texture(Texture texture)
	{
		Texture = texture;
	}

	public MilMo_Texture(string path, bool loadLocal)
	{
		base.Path = path;
		if (loadLocal)
		{
			LoadLocal();
		}
	}

	public override void LoadLocal(string path)
	{
		base.Path = path;
		LoadLocal();
	}

	public override void LoadLocal()
	{
		if (Texture != null)
		{
			Debug.Log("Destroying texture " + Texture.name + " id = " + Texture.GetInstanceID());
			Object.Destroy(Texture);
		}
		Texture = MilMo_ResourceManager.Instance.LoadTextureLocal(base.Path);
	}

	public override void AsyncLoad(string path)
	{
		if (!base.IsLoading)
		{
			base.Path = path;
			AsyncLoad();
		}
	}

	public override void AsyncLoad()
	{
		if (!base.IsLoading)
		{
			base.IsLoading = true;
			StartLoading(base.Path);
		}
	}

	private async void StartLoading(string path)
	{
		OnLoad(await MilMo_ResourceManager.Instance.LoadTextureAsync(path));
	}

	private void OnLoad(Texture2D texture)
	{
		if (Texture != null)
		{
			Debug.Log("Destroying texture " + Texture.name + " id = " + Texture.GetInstanceID());
			Object.Destroy(Texture);
		}
		Texture = texture;
		base.IsLoading = false;
	}
}
