using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public sealed class MilMo_MaterialTexture
{
	public string Property { get; private set; }

	public string Path { get; private set; }

	public Vector2 UVOffset { get; private set; }

	public Vector2 UVTiling { get; private set; }

	public MilMo_MaterialTexture(string property)
	{
		UVTiling = new Vector2(1f, 1f);
		UVOffset = new Vector2(0f, 0f);
		Property = property;
	}

	public void Read(MilMo_SFFile file)
	{
		Path = file.GetString();
		if (file.HasMoreTokens())
		{
			UVTiling = file.GetVector2();
		}
		if (file.HasMoreTokens())
		{
			UVOffset = file.GetVector2();
		}
	}
}
