using UnityEngine;
using UnityEngine.UI;

namespace Core.Colors;

public class ColoredIcon : MonoBehaviour
{
	[SerializeField]
	private Image target;

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int Color1 = Shader.PropertyToID("_Color1");

	private static readonly int Color2 = Shader.PropertyToID("_Color2");

	private static readonly int Color3 = Shader.PropertyToID("_Color3");

	private Material _material;

	private Color _primaryColor = Color.clear;

	private Color _secondaryColor = Color.clear;

	private void Awake()
	{
		if (target == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing target.");
			base.enabled = false;
		}
		target.RegisterDirtyMaterialCallback(SetIconTexture);
	}

	private void Start()
	{
		SetIconTexture();
	}

	private void InitMaterial()
	{
		Shader shader = Resources.Load<Shader>("Shaders/IconBuilder/ColoredIcon");
		_material = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		target.material = _material;
	}

	public void UpdatePrimaryColor(Color color)
	{
		_primaryColor = color;
		UpdateIconColor();
	}

	public void UpdateSecondaryColor(Color color)
	{
		_secondaryColor = color;
		UpdateIconColor();
	}

	private void UpdateIconColor()
	{
		if (_material == null)
		{
			InitMaterial();
		}
		target.enabled = false;
		_material.SetColor(Color1, _primaryColor);
		_material.SetColor(Color2, _secondaryColor);
		_material.SetColor(Color3, Color.clear);
		target.enabled = true;
	}

	private void SetIconTexture()
	{
		Texture2D texture = target.sprite.texture;
		if (_material == null)
		{
			InitMaterial();
		}
		_material.SetTexture(MainTex, texture);
	}
}
