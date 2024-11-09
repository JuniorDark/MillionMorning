using System.IO;
using Code.Core.Global;
using Code.Core.Visual.Effect;
using Code.Core.Visual.Materials;
using UnityEngine;

namespace Code.Core.Visual;

public class AreaEffect
{
	private readonly GameObject _gameObject;

	private const string EFFECT_NAME = "AreaHighlight";

	private GameObject _effectObj;

	private Texture2D _effectTexture;

	private readonly MilMo_Effect _particleEffect;

	private readonly string _assetPathToResources = "Assets" + Path.DirectorySeparatorChar + "Apps" + Path.DirectorySeparatorChar + "World" + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar;

	public AreaEffect(string fullAreaName, Vector3 center, float radiusSquared, float height)
	{
		LoadAssets();
		if (!_effectObj || !_effectTexture)
		{
			return;
		}
		_gameObject = Object.Instantiate(_effectObj);
		_gameObject.name = "FX:" + fullAreaName;
		_gameObject.transform.position = center;
		_gameObject.transform.localScale = new Vector3(radiusSquared, height, radiusSquared);
		Renderer componentInChildren = _gameObject.GetComponentInChildren<Renderer>();
		if (!componentInChildren)
		{
			Debug.LogWarning("Failed to get renderer for area effect");
			Destroy();
		}
		else if (MilMo_Material.GetMaterial("Junebug/Particles/AdditiveBlended") is MilMo_ParticleAdditiveBlend milMo_ParticleAdditiveBlend)
		{
			milMo_ParticleAdditiveBlend.Material.mainTexture = _effectTexture;
			componentInChildren.material = milMo_ParticleAdditiveBlend.Material;
			_particleEffect = MilMo_EffectContainer.GetEffect("AreaHighlight", _gameObject);
			if (_particleEffect == null)
			{
				return;
			}
			_particleEffect.Update();
			GameObject currentGameObject = _particleEffect.GetCurrentGameObject();
			if ((bool)currentGameObject)
			{
				ParticleSystem componentInChildren2 = currentGameObject.GetComponentInChildren<ParticleSystem>();
				if ((bool)componentInChildren2)
				{
					ParticleSystem.ShapeModule shape = componentInChildren2.shape;
					ParticleSystem.EmissionModule emission = componentInChildren2.emission;
					shape.radius = radiusSquared;
					emission.rateOverTime = new ParticleSystem.MinMaxCurve(radiusSquared * 7.5f, radiusSquared * 9f);
					componentInChildren2.Play();
				}
			}
		}
		else
		{
			Debug.LogWarning("Failed to setup material for area effect");
			Destroy();
		}
	}

	private void LoadAssets()
	{
		_effectObj = Resources.Load<GameObject>("ObjectEffects/AreaCircle");
		if (_effectObj == null)
		{
			Debug.LogWarning("Failed to load area FX object at ObjectEffects/AreaCircle");
		}
		_effectTexture = Resources.Load<Texture2D>("ObjectEffects/AreaCircleRamp");
		if (_effectTexture == null)
		{
			Debug.LogWarning("Failed to load area FX Texture at ObjectEffects/AreaCircleRamp");
		}
	}

	public void Update()
	{
		_particleEffect?.Update();
		if ((bool)_gameObject)
		{
			_gameObject.transform.Rotate(0f, 0.1f, 0f);
		}
	}

	public void Destroy()
	{
		_particleEffect?.Destroy();
		if ((bool)_gameObject)
		{
			MilMo_Global.Destroy(_gameObject);
		}
	}
}
