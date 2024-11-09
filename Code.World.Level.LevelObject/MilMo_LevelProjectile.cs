using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual.Effect;
using Code.World.Attack;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public sealed class MilMo_LevelProjectile : MilMo_LevelObject
{
	private MilMo_Effect _trail;

	private MilMo_Effect _impact;

	private float _totalDistanceTraveled;

	private readonly Transform _target;

	private readonly Vector3 _targetOffset;

	private Vector3 _direction;

	private readonly MilMo_Attack _attack;

	private bool _impactParticleDone;

	public bool IsDead { get; private set; }

	public MilMo_ProjectileTemplate Template { get; private set; }

	public string FirerId { get; private set; }

	public MilMo_LevelProjectile(string fullLevelName)
		: base("Content/Projectiles/", useSpawnEffect: true)
	{
		FirerId = "";
		base.FullLevelName = fullLevelName;
	}

	public MilMo_LevelProjectile(string fullLevelName, Vector3 position, Vector3 direction, MilMo_ProjectileTemplate template, Transform target, Vector3 targetOffset, MilMo_Attack attack, string firerId, OnReadDone callback)
		: base(position, direction, Vector3.one, "Content/Projectiles/" + template.VisualRep)
	{
		BasePath = "Content/Projectiles/";
		VisualRepName = template.VisualRep;
		base.FullLevelName = fullLevelName;
		_direction = direction;
		ReadDoneCallback = callback;
		SpawnEffectNames.AddRange(template.SpawnEffects);
		Template = template;
		_target = target;
		_targetOffset = targetOffset;
		_attack = attack;
		FirerId = firerId;
		FinishRead(null, timeOut: false);
	}

	public override void Update()
	{
		if (_impact != null)
		{
			if (!_impactParticleDone && !_impact.Update())
			{
				_impactParticleDone = true;
			}
			UpdateSpawnEffects();
			if (_impactParticleDone && !IsSpawning)
			{
				Unload();
			}
		}
		else
		{
			if (IsDead)
			{
				return;
			}
			base.Update();
			if (Paused)
			{
				return;
			}
			_trail?.Update();
			if (!(base.GameObject == null))
			{
				Vector3 position = base.GameObject.transform.position;
				Vector3 vector = Vector3.zero;
				if (_target != null)
				{
					vector = _target.TransformPoint(_targetOffset);
				}
				if (_target != null && !MilMo_Utility.Equals(_target.position, base.GameObject.transform.position))
				{
					_direction = (vector - base.GameObject.transform.position).normalized;
					base.GameObject.transform.rotation = Quaternion.LookRotation(_direction);
				}
				position += Time.deltaTime * Template.Speed * _direction;
				if (_attack != null && _target != null && MilMo_Physics.PointLineSegmentSqrDistance(base.GameObject.transform.position, position, vector) <= _attack.TargetSqrRadius)
				{
					_attack.Resolve(this);
					ImpactKill();
				}
				if (_target == null && (double)MilMo_Physics.GetDistanceToGround(position) <= 0.01)
				{
					ImpactKill();
				}
				_totalDistanceTraveled += (position - base.GameObject.transform.position).magnitude;
				base.GameObject.transform.position = position;
				if (_target == null && _totalDistanceTraveled > Template.Range && !IsSpawning)
				{
					Unload();
				}
			}
		}
	}

	public void ImpactKill()
	{
		if (!string.IsNullOrEmpty(Template.ImpactParticle))
		{
			_impact = MilMo_EffectContainer.GetEffect(Template.ImpactParticle, base.GameObject.transform.position);
		}
		_trail?.Stop();
		if (Template.ImpactSound != null)
		{
			GameObject impactSoundPlayerObject = new GameObject("ProjectileImpactSound");
			AudioSourceWrapper audioSourceWrapper = impactSoundPlayerObject.AddComponent<AudioSourceWrapper>();
			audioSourceWrapper.Clip = Template.ImpactSound;
			audioSourceWrapper.Loop = false;
			audioSourceWrapper.Play();
			AudioSourceWrapper component = base.GameObject.GetComponent<AudioSourceWrapper>();
			if (component != null)
			{
				audioSourceWrapper.MaxDistance = component.MaxDistance;
				audioSourceWrapper.RolloffMode = component.RolloffMode;
			}
			MilMo_EventSystem.At(Template.ImpactSound.length, delegate
			{
				MilMo_Global.Destroy(impactSoundPlayerObject);
			});
		}
		if (!IsSpawning)
		{
			base.GameObject.SetActive(value: false);
			Renderer[] componentsInChildren = base.GameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (_impact == null)
		{
			Unload();
		}
	}

	public override void Unload()
	{
		_impact?.DestroyWhenDone();
		_trail?.Destroy();
		IsDead = true;
		base.Unload();
	}

	public void ReadFromNetworkMessage(ServerProjectileSpawned msg, OnReadDone callback)
	{
		Vector3 position = new Vector3(msg.getPosition().GetX(), msg.getPosition().GetY(), msg.getPosition().GetZ());
		Vector3 direction = new Vector3(msg.getDirection().GetX(), msg.getDirection().GetY(), msg.getDirection().GetZ());
		Create(position, direction, msg.getProjectileId(), msg.getProjectileTemplate(), "-1", callback);
	}

	private void Create(Vector3 position, Vector3 direction, int id, TemplateReference templateRef, string firerId, OnReadDone callback)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(templateRef, delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || !(template is MilMo_ProjectileTemplate milMo_ProjectileTemplate))
			{
				callback?.Invoke(success: false, null);
			}
			else
			{
				Template = milMo_ProjectileTemplate;
				ReadDoneCallback = callback;
				base.Id = id;
				SpawnPosition = position;
				SpawnRotation = direction;
				_direction = direction;
				SpawnEffectNames.AddRange(milMo_ProjectileTemplate.SpawnEffects);
				VisualRepName = milMo_ProjectileTemplate.VisualRep;
				FirerId = firerId;
				FinishRead(null, timeOut: false);
			}
		});
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			_attack.Resolve(this);
			return false;
		}
		_trail = MilMo_EffectContainer.GetEffect(Template.Trail, base.GameObject);
		if (base.GameObject != null && !MilMo_Utility.Equals(_direction, Vector3.zero))
		{
			base.GameObject.transform.rotation = Quaternion.LookRotation(_direction);
		}
		return true;
	}
}
