using Code.Core.Global;
using UnityEngine;

namespace Code.World.ImageEffects;

[RequireComponent(typeof(Camera))]
public class MilMo_ImageEffectBase : MonoBehaviour
{
	public Shader shader;

	private Material _material;

	protected Material Material
	{
		get
		{
			if (_material != null)
			{
				return _material;
			}
			_material = new Material(shader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			return _material;
		}
	}

	protected void Start()
	{
		if (!shader || !shader.isSupported)
		{
			base.enabled = false;
		}
	}

	protected void OnDisable()
	{
		if ((bool)_material)
		{
			MilMo_Global.Destroy(_material);
		}
	}
}
