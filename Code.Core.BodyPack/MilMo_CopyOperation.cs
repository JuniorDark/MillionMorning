using System;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_CopyOperation
{
	private const string THE_COPY_OPERATION_PATH = "Avatar/";

	private const string THE_SHADER = "Shaders/BodyBuilder/MilMoColor";

	private float _xOffset;

	private float _yOffset;

	private string _textureName;

	private Texture2D _texture;

	private float _width;

	private float _height;

	private bool _additive;

	private static readonly int Stitch = Shader.PropertyToID("_Stitch");

	private static readonly int BlendTex1 = Shader.PropertyToID("_BlendTex1");

	public bool Load(MilMo_SFFile file)
	{
		try
		{
			_textureName = file.GetString();
			_xOffset = (float)file.GetInt() / 1024f;
			_yOffset = (1024f - (float)file.GetInt()) / 1024f;
			if (file.HasMoreTokens())
			{
				_additive = file.GetString().Equals("Additive", StringComparison.InvariantCultureIgnoreCase);
			}
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to load copy operation at line " + file.GetLineNumber());
			return false;
		}
		Texture2D texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal("Avatar/" + _textureName);
		if (texture2D == null)
		{
			Debug.LogWarning("Failed to load texture Avatar/" + _textureName + " for copy operation at line " + file.GetLineNumber());
			return false;
		}
		_texture = texture2D;
		_width = (float)texture2D.width / 1024f;
		_height = (float)texture2D.height / 1024f;
		_yOffset -= _height;
		return true;
	}

	public void Apply(Texture inTex)
	{
		Material material = new Material(MilMo_ResourceManager.LoadShaderLocal("Shaders/BodyBuilder/MilMoColor"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		material.EnableKeyword("COPY");
		material.SetTexture(Stitch, inTex);
		material.SetTexture(BlendTex1, _texture);
		material.SetPass((_additive && material.passCount > 1) ? 1 : 0);
		MilMo_ColorShaderUtil.DrawQuad(new Rect(_xOffset, _yOffset, _width, _height));
	}
}
