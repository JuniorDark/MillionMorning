using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Core.Visual;

public class MilMo_Particle
{
	private static ParticleSystem _particleSystem;

	private static MilMo_TrailRenderer _trail;

	private static ParticleSystem.MainModule _main;

	private static ParticleSystem.EmissionModule _emission;

	private static ParticleSystem.ShapeModule _shape;

	private static ParticleSystemRenderer _renderer;

	private static ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;

	private static ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;

	private static ParticleSystem.TextureSheetAnimationModule _textureSheetAnimation;

	private static ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;

	private static ParticleSystem.ForceOverLifetimeModule _forceOverLifetime;

	public MilMo_ParticleObject ParticleObject { get; private set; }

	private MilMo_Particle(MilMo_ParticleObject particleObject)
	{
		ParticleObject = particleObject;
		ParticleObject.SetActive(state: false);
		ApplyTemplate(ParticleObject.Template);
	}

	public static void LoadFromDisk(string path)
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadFromDisk(path);
		if (milMo_SFFile != null)
		{
			milMo_SFFile.SetName(MilMo_Utility.ExtractNameNoExtensionFromPath(path));
			Load(milMo_SFFile);
		}
	}

	private static ParticleSystem.MinMaxCurve ApplyDampen(ParticleSystem.MinMaxCurve velocityCurve, float dampen)
	{
		if (velocityCurve.mode == ParticleSystemCurveMode.Constant)
		{
			if (Mathf.Approximately(velocityCurve.constant, 0f) || Mathf.Approximately(dampen, 0f))
			{
				return new ParticleSystem.MinMaxCurve(0f, AnimationCurve.Linear(0f, 1f, 1f, 1f));
			}
			AnimationCurve animationCurve = new AnimationCurve();
			for (float num = 0f; num <= 1f; num += 0.2f)
			{
				animationCurve.AddKey(num, Mathf.Pow(dampen, num));
			}
			while (animationCurve.keys.Length > 2)
			{
				animationCurve.RemoveKey(1);
			}
			return new ParticleSystem.MinMaxCurve(velocityCurve.constant, animationCurve);
		}
		AnimationCurve animationCurve2 = new AnimationCurve();
		AnimationCurve animationCurve3 = new AnimationCurve();
		if (Mathf.Approximately(velocityCurve.constantMin, velocityCurve.constantMax) || Mathf.Approximately(dampen, 0f))
		{
			return new ParticleSystem.MinMaxCurve(0f, AnimationCurve.Linear(0f, 1f, 1f, 1f), AnimationCurve.Linear(0f, 1f, 1f, 1f));
		}
		float num2 = Math.Abs(velocityCurve.constantMin);
		float num3 = Math.Abs(velocityCurve.constantMax);
		float num4 = num2 / num3;
		float num5 = ((!(velocityCurve.constantMin < 0f)) ? 1 : (-1));
		float num6 = ((!(velocityCurve.constantMax < 0f)) ? 1 : (-1));
		for (float num7 = 0f; num7 <= 1f; num7 += 0.2f)
		{
			animationCurve2.AddKey(num7, num5 * num4 * Mathf.Pow(dampen, num7));
			animationCurve3.AddKey(num7, num6 * Mathf.Pow(dampen, num7));
		}
		while (animationCurve2.keys.Length > 2)
		{
			animationCurve2.RemoveKey(1);
			animationCurve3.RemoveKey(1);
		}
		return new ParticleSystem.MinMaxCurve(num3, animationCurve2, animationCurve3);
	}

	private static ParticleSystem.MinMaxCurve GenerateSizeGrowCurve(float sizeGrow, float minTime, float maxTime)
	{
		if (sizeGrow > 0f)
		{
			if (Mathf.Approximately(minTime, maxTime))
			{
				float num = Mathf.Pow(1f + sizeGrow, minTime);
				AnimationCurve animationCurve = new AnimationCurve();
				for (float num2 = 0f; num2 <= 1f; num2 += 0.2f)
				{
					animationCurve.AddKey(num2, Mathf.Pow(1f + sizeGrow, num2 * maxTime) / num);
				}
				while (animationCurve.keys.Length > 2)
				{
					animationCurve.RemoveKey(1);
				}
				return new ParticleSystem.MinMaxCurve(num, animationCurve);
			}
			float num3 = Mathf.Pow(1f + sizeGrow, maxTime);
			AnimationCurve animationCurve2 = new AnimationCurve();
			AnimationCurve animationCurve3 = new AnimationCurve();
			for (float num4 = 0f; num4 <= 1f; num4 += 0.2f)
			{
				animationCurve2.AddKey(num4, Mathf.Pow(1f + sizeGrow, num4 * minTime) / num3);
				animationCurve3.AddKey(num4, Mathf.Pow(1f + sizeGrow, num4 * maxTime) / num3);
			}
			while (animationCurve2.keys.Length > 2)
			{
				animationCurve2.RemoveKey(1);
				animationCurve3.RemoveKey(1);
			}
			return new ParticleSystem.MinMaxCurve(num3, animationCurve2, animationCurve3);
		}
		if (Mathf.Approximately(minTime, maxTime))
		{
			AnimationCurve animationCurve4 = new AnimationCurve();
			for (float num5 = 0f; num5 <= 1f; num5 += 0.2f)
			{
				animationCurve4.AddKey(num5, Mathf.Pow(0f - sizeGrow, num5));
			}
			while (animationCurve4.keys.Length > 2)
			{
				animationCurve4.RemoveKey(1);
			}
			return new ParticleSystem.MinMaxCurve(1f, animationCurve4);
		}
		AnimationCurve animationCurve5 = new AnimationCurve();
		AnimationCurve animationCurve6 = new AnimationCurve();
		float num6 = minTime / maxTime;
		for (float num7 = 0f; num7 <= 1f; num7 += 0.2f)
		{
			animationCurve5.AddKey(num7, Mathf.Pow(0f - sizeGrow, num7 * num6));
			animationCurve6.AddKey(num7, Mathf.Pow(0f - sizeGrow, num7));
		}
		while (animationCurve5.keys.Length > 2)
		{
			animationCurve5.RemoveKey(1);
			animationCurve6.RemoveKey(1);
		}
		return new ParticleSystem.MinMaxCurve(1f, animationCurve5, animationCurve6);
	}

	private void ApplyTemplate(MilMo_ParticleTemplate template)
	{
		_main.startSpeed = 0f;
		_main.maxParticles = 10000;
		_shape.enabled = true;
		_main.loop = true;
		_main.scalingMode = ParticleSystemScalingMode.Local;
		_renderer.minParticleSize = 0f;
		_renderer.maxParticleSize = 10000f;
		_renderer.shadowCastingMode = ShadowCastingMode.On;
		_renderer.receiveShadows = true;
		_renderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
		string type = template.type;
		if (!(type == "EllipsoidParticleEmitter"))
		{
			if (type == "MeshParticleEmitter")
			{
				_shape.shapeType = ParticleSystemShapeType.Mesh;
				_shape.mesh = template.mesh;
				_shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
			}
			else
			{
				Debug.LogWarning("MilMo_Particle: emitter of type: " + template.type + " is not supported in " + ParticleObject.GetGameObject().name);
				ParticleObject.Template.type = "MeshParticleEmitter";
			}
		}
		else
		{
			_shape.shapeType = ParticleSystemShapeType.Circle;
			_shape.radius = template.radius;
			_shape.radiusThickness = template.radiusThickness;
			_shape.rotation = template.circleRotation;
		}
		ParticleObject.GetGameObject().transform.localScale = template.scale;
		_main.playOnAwake = template.emit;
		_main.startSize = new ParticleSystem.MinMaxCurve(template.minSize, template.maxSize);
		_main.startLifetime = new ParticleSystem.MinMaxCurve(template.minEnergy, template.maxEnergy);
		_main.duration = template.maxEnergy;
		_emission.rateOverTime = new ParticleSystem.MinMaxCurve(template.minEmission, template.maxEmission);
		Vector3 worldVelocity = template.worldVelocity;
		Vector3 localVelocity = template.localVelocity;
		Vector3 rndVelocity = template.rndVelocity;
		float emitterVelocityScale = template.emitterVelocityScale;
		Vector3? vector = null;
		Vector3? vector2 = null;
		if ((double)Math.Abs(localVelocity.x) > 0.0 || (double)Math.Abs(localVelocity.y) > 0.0 || (double)Math.Abs(localVelocity.z) > 0.0)
		{
			_velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
			vector = localVelocity - rndVelocity;
			vector2 = localVelocity + rndVelocity;
		}
		else if ((double)Math.Abs(worldVelocity.x) > 0.0 || (double)Math.Abs(worldVelocity.y) > 0.0 || (double)Math.Abs(worldVelocity.z) > 0.0)
		{
			_velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
			vector = worldVelocity - rndVelocity;
			vector2 = worldVelocity + rndVelocity;
		}
		else if ((double)Math.Abs(rndVelocity.x) > 0.0 || (double)Math.Abs(rndVelocity.y) > 0.0 || (double)Math.Abs(rndVelocity.z) > 0.0)
		{
			_velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
			vector = -rndVelocity;
			vector2 = rndVelocity;
		}
		if (vector.HasValue)
		{
			_velocityOverLifetime.enabled = true;
			_velocityOverLifetime.xMultiplier = emitterVelocityScale;
			_velocityOverLifetime.yMultiplier = emitterVelocityScale;
			_velocityOverLifetime.zMultiplier = emitterVelocityScale;
			Vector3 value = vector.Value;
			Vector3 value2 = vector2.Value;
			if (value == value2)
			{
				_velocityOverLifetime.x = value.x;
				_velocityOverLifetime.y = value.y;
				_velocityOverLifetime.z = value.z;
			}
			else
			{
				_velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(value.x, value2.x);
				_velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(value.y, value2.y);
				_velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(value.z, value2.z);
			}
		}
		_main.simulationSpace = (template.useWorldSpace ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local);
		if (template.oneShot)
		{
			_main.loop = false;
			_emission.rateOverTime = 0f;
			ParticleSystem.Burst burst = default(ParticleSystem.Burst);
			burst.cycleCount = int.MaxValue;
			burst.repeatInterval = _main.startLifetime.constantMax;
			burst.maxCount = (short)template.maxEmission;
			burst.minCount = (short)template.minEmission;
			ParticleSystem.Burst burst2 = burst;
			_emission.SetBursts(new ParticleSystem.Burst[1] { burst2 });
		}
		_colorOverLifetime.enabled = template.doesAnimateColor;
		_colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient
		{
			colorKeys = new GradientColorKey[5]
			{
				new GradientColorKey(template.color[0], 0f),
				new GradientColorKey(template.color[1], 0.25f),
				new GradientColorKey(template.color[2], 0.5f),
				new GradientColorKey(template.color[3], 0.75f),
				new GradientColorKey(template.color[4], 1f)
			},
			alphaKeys = new GradientAlphaKey[5]
			{
				new GradientAlphaKey(template.color[0].a, 0f),
				new GradientAlphaKey(template.color[1].a, 0.25f),
				new GradientAlphaKey(template.color[2].a, 0.5f),
				new GradientAlphaKey(template.color[3].a, 0.75f),
				new GradientAlphaKey(template.color[4].a, 1f)
			}
		});
		Vector3 worldRotationAxis = template.worldRotationAxis;
		Vector3 localRotationAxis = template.localRotationAxis;
		ParticleObject.GetGameObject().transform.localRotation = Quaternion.Euler(localRotationAxis);
		ParticleObject.GetGameObject().transform.rotation = Quaternion.Euler(worldRotationAxis);
		if (template.sizeGrow != 0f)
		{
			_sizeOverLifetime.enabled = true;
			_sizeOverLifetime.size = GenerateSizeGrowCurve(template.sizeGrow, template.minEnergy, template.maxEnergy);
		}
		Vector3 rndForce = template.rndForce;
		Vector3 force = template.force;
		if ((double)Math.Abs(rndForce.x) > 0.0 || (double)Math.Abs(rndForce.y) > 0.0 || (double)Math.Abs(rndForce.z) > 0.0)
		{
			_forceOverLifetime.randomized = true;
		}
		if ((double)Math.Abs(force.x) > 0.0 || (double)Math.Abs(force.y) > 0.0 || (double)Math.Abs(force.z) > 0.0 || (double)Math.Abs(rndForce.x) > 0.0 || (double)Math.Abs(rndForce.y) > 0.0 || (double)Math.Abs(rndForce.z) > 0.0)
		{
			_forceOverLifetime.enabled = true;
			_forceOverLifetime.space = ParticleSystemSimulationSpace.World;
			_forceOverLifetime.x = new ParticleSystem.MinMaxCurve(force.x - rndForce.x * 0.5f, force.x + rndForce.x * 0.5f);
			_forceOverLifetime.y = new ParticleSystem.MinMaxCurve(force.y - rndForce.y * 0.5f, force.y + rndForce.y * 0.5f);
			_forceOverLifetime.z = new ParticleSystem.MinMaxCurve(force.z - rndForce.z * 0.5f, force.z + rndForce.z * 0.5f);
		}
		if (template.damping < 1f)
		{
			_velocityOverLifetime.x = ApplyDampen(_velocityOverLifetime.x, template.damping);
			_velocityOverLifetime.y = ApplyDampen(_velocityOverLifetime.y, template.damping);
			_velocityOverLifetime.z = ApplyDampen(_velocityOverLifetime.z, template.damping);
		}
		_renderer.sharedMaterials = template.sharedMaterials;
		_renderer.cameraVelocityScale = template.cameraVelocityScale;
		string particleRenderMode = template.particleRenderMode;
		if (particleRenderMode.Equals("Billboard", StringComparison.InvariantCultureIgnoreCase))
		{
			_renderer.renderMode = ParticleSystemRenderMode.Billboard;
		}
		else if (particleRenderMode.Equals("HorizontalBillboard", StringComparison.InvariantCultureIgnoreCase))
		{
			_renderer.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
			_renderer.alignment = (template.useWorldSpace ? ParticleSystemRenderSpace.Facing : ParticleSystemRenderSpace.Local);
		}
		else if (particleRenderMode.Equals("StoredBillboard", StringComparison.InvariantCultureIgnoreCase))
		{
			_renderer.renderMode = ParticleSystemRenderMode.Billboard;
			_renderer.sortMode = ParticleSystemSortMode.Distance;
		}
		else if (particleRenderMode.Equals("Stretch", StringComparison.InvariantCultureIgnoreCase))
		{
			_renderer.renderMode = ParticleSystemRenderMode.Stretch;
		}
		else if (particleRenderMode.Equals("VerticalBillboard", StringComparison.InvariantCultureIgnoreCase))
		{
			_renderer.renderMode = ParticleSystemRenderMode.VerticalBillboard;
		}
		else
		{
			Debug.LogWarning("MilMo_Particle: rendering mode of type: " + particleRenderMode + " is not supported in " + ParticleObject.GetGameObject().name);
			_renderer.renderMode = ParticleSystemRenderMode.Billboard;
			_renderer.sortMode = ParticleSystemSortMode.None;
		}
		_renderer.lengthScale = template.lengthScale;
		_renderer.velocityScale = template.velocityScale;
		_renderer.maxParticleSize = template.maxParticleSize;
		if (template.uvAnimationXTile > 1 || template.uvAnimationYTile > 1)
		{
			_textureSheetAnimation.enabled = true;
			_textureSheetAnimation.numTilesX = template.uvAnimationXTile;
			_textureSheetAnimation.numTilesY = template.uvAnimationYTile;
			_textureSheetAnimation.cycleCount = (int)template.uvAnimationCycles;
		}
		if (template.rotation != Vector3.zero)
		{
			_main.startRotation3D = true;
			_main.startRotationX = MathF.PI / 180f * template.rotation.x;
			_main.startRotationY = MathF.PI / 180f * template.rotation.y;
			_main.startRotationZ = MathF.PI / 180f * template.rotation.z;
		}
		if (template.manualEmit)
		{
			_main.loop = false;
		}
		_renderer.alignment = template.renderAlignment;
		if (template.sizeStart != 1f || template.sizeEnd != 1f)
		{
			_sizeOverLifetime.enabled = true;
			_sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, template.sizeStart), new Keyframe(1f, template.sizeEnd)));
		}
		if (template.preWarm)
		{
			_main.prewarm = true;
		}
		if (MilMo_ParticleContainer.DevMode)
		{
			Debug.Log("MilMo_Particle: Template applied for " + template.name);
		}
	}

	public static MilMo_Particle Load(MilMo_SFFile file)
	{
		if (file == null)
		{
			return null;
		}
		if (MilMo_ParticleContainer.DevMode)
		{
			Debug.Log("MilMo_Particle: Adding ParticleSystem for " + file.Name);
		}
		GameObject gameObject = new GameObject("[ParticleGroup] " + file.Name);
		MilMo_ParticleTemplate milMo_ParticleTemplate = gameObject.AddComponent<MilMo_ParticleTemplate>();
		milMo_ParticleTemplate.Load(file);
		GameObject gameObject2 = new GameObject("[Effect] " + milMo_ParticleTemplate.effectName);
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.layer = 1;
		_particleSystem = gameObject2.AddComponent<ParticleSystem>();
		if (!_particleSystem)
		{
			Debug.LogWarning("MilMo_Particle: Failed to add ParticleSystem for effect " + gameObject2.name);
			return null;
		}
		_main = _particleSystem.main;
		_emission = _particleSystem.emission;
		_shape = _particleSystem.shape;
		_renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
		_velocityOverLifetime = _particleSystem.velocityOverLifetime;
		_colorOverLifetime = _particleSystem.colorOverLifetime;
		_textureSheetAnimation = _particleSystem.textureSheetAnimation;
		_sizeOverLifetime = _particleSystem.sizeOverLifetime;
		_forceOverLifetime = _particleSystem.forceOverLifetime;
		return new MilMo_Particle(new MilMo_ParticleObject(gameObject)
		{
			Template = milMo_ParticleTemplate
		});
	}
}
