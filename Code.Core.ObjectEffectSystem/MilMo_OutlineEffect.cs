using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_OutlineEffect : MilMo_ObjectEffect
{
	private float _time;

	private Material _oldMaterial;

	private bool _isSetup;

	private Renderer _renderer;

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbColor = Shader.PropertyToID("_AmbColor");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private static readonly int Width = Shader.PropertyToID("_Width");

	private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

	private MilMo_OutlineEffectTemplate Template => EffectTemplate as MilMo_OutlineEffectTemplate;

	public override float Duration => float.MaxValue;

	public MilMo_OutlineEffect(GameObject gameObject, MilMo_OutlineEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		if (!_isSetup)
		{
			Setup();
			if (!_isSetup)
			{
				Destroy();
				return false;
			}
		}
		if (!_renderer)
		{
			Destroy();
			return false;
		}
		return true;
	}

	private void Setup()
	{
		MilMo_VisualRepComponent componentInChildren = GameObject.GetComponentInChildren<MilMo_VisualRepComponent>();
		if (componentInChildren == null)
		{
			return;
		}
		MilMo_VisualRep visualRep = componentInChildren.GetVisualRep();
		if (visualRep == null)
		{
			return;
		}
		_renderer = visualRep.Renderer;
		if (_renderer == null)
		{
			return;
		}
		_oldMaterial = new Material(_renderer.material);
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/ImageEffects/Outline");
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader Shaders/ImageEffects/Outline, outline effect will not work");
			return;
		}
		Material material = new Material(shader);
		if (material.HasProperty(MainTex) && _oldMaterial.HasProperty(MainTex))
		{
			material.SetTexture(MainTex, _oldMaterial.GetTexture(MainTex));
			material.SetTextureOffset(MainTex, _oldMaterial.GetTextureOffset(MainTex));
			material.SetTextureScale(MainTex, _oldMaterial.GetTextureScale(MainTex));
		}
		if (material.HasProperty(Color) && _oldMaterial.HasProperty(Color))
		{
			Color color = _oldMaterial.GetColor(Color);
			if (_oldMaterial.HasProperty(AmbColor))
			{
				Color color2 = _oldMaterial.GetColor(AmbColor);
				color.r = Mathf.Max(color.r, color2.r);
				color.g = Mathf.Max(color.g, color2.g);
				color.b = Mathf.Max(color.b, color2.b);
			}
			material.SetColor(Color, color);
		}
		if (material.HasProperty(VelvetColor) && _oldMaterial.HasProperty(VelvetColor))
		{
			material.SetColor(VelvetColor, _oldMaterial.GetColor(VelvetColor));
		}
		if (material.HasProperty(VelvetChannel) && _oldMaterial.HasProperty(VelvetChannel))
		{
			material.SetVector(VelvetChannel, _oldMaterial.GetVector(VelvetChannel));
		}
		if (material.HasProperty(Ramp))
		{
			material.SetTexture(Ramp, MilMo_Material.RampTexture);
		}
		material.SetFloat(Width, Template.Width);
		material.SetColor(OutlineColor, Template.Color);
		_renderer.material = material;
		_isSetup = true;
	}

	private void ResetShader()
	{
		if (_renderer != null)
		{
			_renderer.material = _oldMaterial;
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		ResetShader();
	}
}
