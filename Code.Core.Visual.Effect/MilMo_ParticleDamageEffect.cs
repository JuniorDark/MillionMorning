using System.Collections.Generic;
using Code.Core.Command;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_ParticleDamageEffect
{
	private const string DAMAGE_STAR_SCRIPT = "Star";

	private const int MEDIUM_STAR_LIMIT = 2;

	private const int LARGE_STAR_LIMIT = 3;

	private const float SMALL_STAR_SCALE = 0.2f;

	private const float MEDIUM_STAR_SCALE = 0.4f;

	private const float LARGE_STAR_SCALE = 0.6f;

	private static readonly Color SmallStarColor = Color.yellow;

	private static readonly Color MediumStarColor = new Color(1f, 0.5f, 0f, 1f);

	private static readonly Color LargeStarColor = Color.red;

	private const float STAR_SPEED = 4f;

	private static Vector3 _impactStartScale = new Vector3(0f, 0f, 0f);

	private static Vector2 _impactTargetScale = new Vector3(2f, 2f, 2f);

	private const float IMPACT_START_ALPHA = 0.3f;

	private const float IMPACT_FADE_SPEED = 0.4f;

	private const string IMPACT_VISUALREP_PATH = "Content/Particles/Batch01/Bash";

	public const float IMPACT_SCALE_PLAYER = 0.75f;

	private const float IMPACT_SCALE_CREATURE = 1f;

	private static float _impactScalePull = 0.05f;

	private static float _impactScaleDrag = 0.5f;

	private static readonly Vector3 ImpactRotation = new Vector3(0f, 0f, 0f);

	private static readonly Vector3 ImpactVelocity = new Vector3(0f, 0f, 0f);

	private readonly List<MilMo_MeshParticle> _impactMeshes = new List<MilMo_MeshParticle>();

	private GameObject _damageStarEmitter;

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	public MilMo_ParticleDamageEffect(Transform parentTransform, float damageStarYOffset)
	{
		_damageStarEmitter = MilMo_ParticleContainer.GetParticle("Star", parentTransform.position);
		_damageStarEmitter.transform.parent = parentTransform;
		_damageStarEmitter.transform.localPosition = Vector3.up * damageStarYOffset;
	}

	public void Validate(Transform parentTransform, float damageStarYOffset)
	{
		if (_damageStarEmitter == null)
		{
			_damageStarEmitter = MilMo_ParticleContainer.GetParticle("Star", parentTransform.position);
		}
		_damageStarEmitter.transform.parent = parentTransform;
		_damageStarEmitter.transform.localPosition = Vector3.up * damageStarYOffset;
	}

	public void Update()
	{
		for (int num = _impactMeshes.Count - 1; num >= 0; num--)
		{
			if (!_impactMeshes[num].Update())
			{
				_impactMeshes.RemoveAt(num);
			}
		}
	}

	public void FixedUpdate()
	{
		foreach (MilMo_MeshParticle impactMesh in _impactMeshes)
		{
			impactMesh.FixedUpdate();
		}
	}

	public void Destroy()
	{
		if (_damageStarEmitter != null)
		{
			Object.Destroy(_damageStarEmitter);
			_damageStarEmitter = null;
		}
		foreach (MilMo_MeshParticle impactMesh in _impactMeshes)
		{
			impactMesh.Destroy();
		}
	}

	public void Emit(float damageAmount, Vector3 impactPosition, Vector3 starDirection, float impactMeshScale = 1f, string impactMeshPath = "Content/Particles/Batch01/Bash")
	{
		Vector3 rotation = new Vector3(MilMo_Utility.Random(), MilMo_Utility.Random(), MilMo_Utility.Random()) * 360f;
		MilMo_MeshParticle milMo_MeshParticle = new MilMo_MeshParticle(impactMeshPath, impactPosition, rotation, _impactStartScale, 0.3f, ImpactVelocity, ImpactRotation, 0.4f);
		if (milMo_MeshParticle.Mover != null)
		{
			milMo_MeshParticle.Mover.SetUpdateFunc(2);
			milMo_MeshParticle.Mover.ScalePull = _impactScalePull;
			milMo_MeshParticle.Mover.ScaleDrag = _impactScaleDrag;
			milMo_MeshParticle.Mover.ScaleTo(_impactTargetScale * impactMeshScale);
		}
		_impactMeshes.Add(milMo_MeshParticle);
		Color value;
		float startSize;
		if (damageAmount < 2f)
		{
			value = SmallStarColor;
			startSize = 0.2f;
		}
		else if (damageAmount < 3f)
		{
			value = MediumStarColor;
			startSize = 0.4f;
		}
		else
		{
			value = LargeStarColor;
			startSize = 0.6f;
		}
		ParticleSystem[] componentsInChildren = _damageStarEmitter.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem obj in componentsInChildren)
		{
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
			{
				position = _damageStarEmitter.transform.position,
				velocity = starDirection * 4f,
				startSize = startSize,
				startColor = Color.white
			};
			ParticleSystem.MainModule main = obj.main;
			main.loop = false;
			obj.GetComponent<ParticleSystemRenderer>().material.SetColor(TintColor, value);
			obj.Emit(emitParams, 1);
		}
	}

	private static string Debug_ImpactMeshScalePull(string[] args)
	{
		if (args.Length < 2)
		{
			return "Scale pull: " + _impactScalePull;
		}
		_impactScalePull = MilMo_Utility.StringToFloat(args[1]);
		return "Scale pull set to " + _impactScalePull;
	}

	private static string Debug_ImpactMeshScaleDrag(string[] args)
	{
		if (args.Length < 2)
		{
			return "Scale drag: " + _impactScaleDrag;
		}
		_impactScaleDrag = MilMo_Utility.StringToFloat(args[1]);
		return "Scale drag set to " + _impactScaleDrag;
	}

	private static string Debug_ImpactMeshStartScale(string[] args)
	{
		Vector3 impactStartScale;
		if (args.Length < 2)
		{
			impactStartScale = _impactStartScale;
			return "Start scale: " + impactStartScale.ToString();
		}
		if (args.Length >= 4)
		{
			_impactStartScale = new Vector3(MilMo_Utility.StringToFloat(args[1]), MilMo_Utility.StringToFloat(args[2]), MilMo_Utility.StringToFloat(args[3]));
		}
		else
		{
			_impactStartScale = MilMo_Utility.StringToFloat(args[1]) * Vector3.one;
		}
		impactStartScale = _impactStartScale;
		return "Start scale set to " + impactStartScale.ToString();
	}

	private static string Debug_ImpactMeshTargetScale(string[] args)
	{
		Vector2 impactTargetScale;
		if (args.Length < 2)
		{
			impactTargetScale = _impactTargetScale;
			return "Target scale: " + impactTargetScale.ToString();
		}
		if (args.Length >= 4)
		{
			_impactTargetScale = new Vector3(MilMo_Utility.StringToFloat(args[1]), MilMo_Utility.StringToFloat(args[2]), MilMo_Utility.StringToFloat(args[3]));
		}
		else
		{
			_impactTargetScale = MilMo_Utility.StringToFloat(args[1]) * Vector3.one;
		}
		impactTargetScale = _impactTargetScale;
		return "Target scale set to " + impactTargetScale.ToString();
	}

	public static void RegisterCommands()
	{
		MilMo_Command.Instance.RegisterCommand("ImpactMeshScalePull", Debug_ImpactMeshScalePull);
		MilMo_Command.Instance.RegisterCommand("ImpactMeshScaleDrag", Debug_ImpactMeshScaleDrag);
		MilMo_Command.Instance.RegisterCommand("ImpactMeshStartScale", Debug_ImpactMeshStartScale);
		MilMo_Command.Instance.RegisterCommand("ImpactMeshTargetScale", Debug_ImpactMeshTargetScale);
	}
}
