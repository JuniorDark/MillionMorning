using System.Collections.Generic;
using System.Linq;
using Code.Core.ResourceSystem;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

internal class MilMo_GibEffect : MilMo_ObjectEffect
{
	private const float FADE_DURATION = 1f;

	private MilMo_VisualRep _visualRep;

	private bool _firstSetup = true;

	private bool _firstFade = true;

	private float _startTime;

	private readonly List<Vector3> _offsets = new List<Vector3>();

	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	private static readonly int MainColor = Shader.PropertyToID("_Color");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private MilMo_GibEffectTemplate Template => EffectTemplate as MilMo_GibEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_GibEffect(GameObject gameObject, MilMo_GibEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (_firstSetup)
		{
			_startTime = Time.realtimeSinceStartup;
			_firstSetup = false;
			Setup();
		}
		foreach (Vector3 offset in _offsets)
		{
			Debug.DrawLine(offset, offset + new Vector3(0.1f, 0.1f, 0.1f), Color.red);
		}
		float num = Template.Duration - (Time.realtimeSinceStartup - _startTime);
		if (num < 1f)
		{
			if (_visualRep == null)
			{
				Destroy();
				return false;
			}
			if (_firstFade)
			{
				SetFadeMaterial();
				_firstFade = false;
			}
			foreach (MilMo_Gib gib in _visualRep.Gibs)
			{
				if ((bool)gib.GameObjectRef && (bool)gib.Renderer)
				{
					Color color = gib.Renderer.sharedMaterial.color;
					color.a = Mathf.Max(color.a - Time.deltaTime / 1f, 0f);
					gib.Renderer.sharedMaterial.color = color;
				}
			}
		}
		if (num > 0f)
		{
			return true;
		}
		Destroy();
		return false;
	}

	private void Setup()
	{
		_visualRep = MilMo_VisualRepContainer.GetVisualRep(GameObject);
		if (_visualRep == null)
		{
			Debug.LogWarning("Failed to fetch visual rep when running gib effect. Game object is " + ((GameObject == null) ? "null" : GameObject.name));
			return;
		}
		_visualRep.CurrentLod.ParentVisualRep.Renderer.enabled = false;
		_visualRep.CurrentLod.ParentVisualRep.GameObject.layer = LayerMask.NameToLayer("Gibs");
		Collider component = _visualRep.CurrentLod.ParentVisualRep.GameObject.GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		foreach (MilMo_Gib gib in _visualRep.Gibs)
		{
			GameObject gameObjectRef = gib.GameObjectRef;
			if (gameObjectRef == null || gib.Renderer == null)
			{
				continue;
			}
			gameObjectRef.transform.position += Template.GibOffset;
			gameObjectRef.SetActive(value: true);
			gib.Renderer.enabled = true;
			switch (Template.GibCollisionType)
			{
			case MilMo_GibEffectTemplate.CollisionType.Box:
				gib.Collider = gameObjectRef.AddComponent<BoxCollider>();
				break;
			case MilMo_GibEffectTemplate.CollisionType.Capsule:
				gib.Collider = gameObjectRef.AddComponent<CapsuleCollider>();
				break;
			case MilMo_GibEffectTemplate.CollisionType.Sphere:
				gib.Collider = gameObjectRef.AddComponent<SphereCollider>();
				break;
			default:
				continue;
			}
			if (gib.Collider == null)
			{
				continue;
			}
			gib.Rigidbody = gameObjectRef.AddComponent<Rigidbody>();
			if (!gib.Rigidbody)
			{
				continue;
			}
			gib.Rigidbody.mass = Template.Mass;
			gib.Rigidbody.drag = Template.Drag;
			gib.Rigidbody.angularDrag = Template.AngularDrag;
			gib.Rigidbody.useGravity = Template.UseGravity;
			gib.Rigidbody.freezeRotation = Template.FreezeRotation;
			switch (Template.ImpactForceType)
			{
			case MilMo_GibEffectTemplate.ImpactType.Random:
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				gib.Rigidbody.AddForceAtPosition(onUnitSphere * Template.ImpactForce, gib.Collider.bounds.center + Template.ImpactOffset);
				break;
			}
			case MilMo_GibEffectTemplate.ImpactType.Center:
			{
				Vector3 vector = ((GameObject.GetComponent<Collider>() == null) ? GameObject.transform.position : GameObject.GetComponent<Collider>().bounds.center);
				Vector3 center = gib.Collider.bounds.center;
				Vector3 vector2 = center - vector;
				vector2.y *= 0.1f;
				vector2.Normalize();
				_offsets.Add(center + Template.ImpactOffset);
				gib.Rigidbody.AddForceAtPosition(vector2 * Template.ImpactForce, center + Template.ImpactOffset);
				break;
			}
			}
			if (!Template.GibCollision)
			{
				foreach (Collider item in _visualRep.Gibs.Select((MilMo_Gib g) => g.Collider))
				{
					if ((bool)item)
					{
						Physics.IgnoreCollision(item, gib.Collider);
					}
				}
			}
			gib.GameObjectRef.layer = LayerMask.NameToLayer("Gibs");
		}
	}

	private void SetFadeMaterial()
	{
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/Junebug/DiffuseTrans");
		if (shader == null)
		{
			Debug.LogWarning("Failed to load shader Shaders/Junebug/DiffuseTrans, gib fade effect will not work");
			return;
		}
		if (_visualRep == null)
		{
			Debug.LogWarning("Visual rep for gib parent is null, gib fade effect will not work");
			return;
		}
		foreach (MilMo_Gib gib in _visualRep.Gibs)
		{
			if (gib.GameObjectRef == null)
			{
				Debug.LogWarning("Got invalid game object for gib when starting fade effect");
				continue;
			}
			if (!gib.Renderer)
			{
				Debug.LogWarning("Got no renderer for gib when starting fade effect");
				continue;
			}
			Material sharedMaterial = gib.Renderer.sharedMaterial;
			Material material = new Material(shader);
			if (material.HasProperty("_MainTex") && sharedMaterial.HasProperty("_MainTex"))
			{
				material.SetTexture(MainTex, sharedMaterial.GetTexture(MainTex));
				material.SetTextureOffset(MainTex, sharedMaterial.GetTextureOffset(MainTex));
				material.SetTextureScale(MainTex, sharedMaterial.GetTextureScale(MainTex));
			}
			if (material.HasProperty("_Color") && sharedMaterial.HasProperty("_Color"))
			{
				material.SetColor(MainColor, sharedMaterial.GetColor(MainColor));
			}
			if (material.HasProperty("_VelvetColor") && sharedMaterial.HasProperty("_VelvetColor"))
			{
				material.SetColor(VelvetColor, sharedMaterial.GetColor(VelvetColor));
			}
			if (material.HasProperty("_VelvetChannel") && sharedMaterial.HasProperty("_VelvetChannel"))
			{
				material.SetVector(VelvetChannel, sharedMaterial.GetVector(VelvetChannel));
			}
			if (material.HasProperty("_Ramp"))
			{
				material.SetTexture(Ramp, sharedMaterial.HasProperty("_Ramp") ? sharedMaterial.GetTexture(Ramp) : MilMo_Material.RampTexture);
			}
			gib.Renderer.sharedMaterial = material;
		}
	}
}
