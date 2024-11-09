using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.Core.Visual.Effect;
using Code.World.Attack;
using Code.World.CreatureMover;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_LevelCreature : MilMo_MovableObject
{
	private const float DEATH_IMPULSE_IN_AIR_TIME = 0.6f;

	private MilMo_ParticleDamageEffect _particleDamageEffect;

	private bool _deathImpulseInAir;

	private bool _deathImpulseFalling;

	private float _deathImpulseStartInAirStartTime;

	private new MilMo_LevelCreatureTemplate Template
	{
		get
		{
			return base.Template as MilMo_LevelCreatureTemplate;
		}
		set
		{
			base.Template = value;
		}
	}

	private new MilMo_CreatureMover Mover
	{
		get
		{
			return base.Mover as MilMo_CreatureMover;
		}
		set
		{
			base.Mover = value;
		}
	}

	public MilMo_LevelCreature(bool useSpawnEffects)
		: base("Content/Creatures/", useSpawnEffects)
	{
		CreatureUpdateReaction = MilMo_EventSystem.Listen("object_update", StoreUpdateMessage);
		CreatureUpdateReaction.Repeating = true;
		InitialMoverPositionTime = Time.time;
		DamageFlashDuration = 0.5f;
		AudioRolloffFactor = 1f;
	}

	public override bool IsCritter()
	{
		return !IsDangerous();
	}

	public override bool IsDeadOrDying()
	{
		if (base.IsDeadOrDying())
		{
			return true;
		}
		if (Mover != null)
		{
			return Mover.HasDeathImpulse;
		}
		return false;
	}

	public override void Update()
	{
		if (Paused)
		{
			return;
		}
		if (_particleDamageEffect != null)
		{
			_particleDamageEffect.Update();
		}
		if (DamageTimer > 0f)
		{
			UpdateDamageEffect();
		}
		if (LifePhase == MilMo_LifePhase.Dying1)
		{
			UpdateDyingPhase1();
		}
		if (LifePhase == MilMo_LifePhase.Dying2)
		{
			UpdateDyingPhase2();
		}
		if (LifePhase == MilMo_LifePhase.Dead)
		{
			base.Update();
			return;
		}
		if (Mover != null)
		{
			bool flag = true;
			if (Mover.HasDeathImpulse)
			{
				if (_deathImpulseFalling)
				{
					flag = false;
					if (!Mover.UpdateDeathImpulseFall())
					{
						LifePhase = MilMo_LifePhase.Dead;
					}
				}
				else if (_deathImpulseInAir)
				{
					flag = false;
					if (Time.time - _deathImpulseStartInAirStartTime > 0.6f)
					{
						_deathImpulseFalling = true;
						_deathImpulseInAir = false;
						Mover.SetupDeathImpulseFall();
					}
				}
				else if (Mover.DistanceToTarget < 0.1f || Time.time - Mover.DeathImpulseStartTime > 2f)
				{
					_deathImpulseInAir = true;
					_deathImpulseStartInAirStartTime = Time.time;
				}
			}
			if (flag)
			{
				Mover.Update();
			}
		}
		base.Update();
		if (base.GameObject != null)
		{
			LastKnownPosition = base.GameObject.transform.position;
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		Mover?.FixedUpdate();
		_particleDamageEffect?.FixedUpdate();
	}

	public override void HandleImpulse(ServerMoveableImpulse impulseMsg)
	{
		Mover?.HandleRealImpulse(impulseMsg);
	}

	public override void Stunned()
	{
		Mover?.Stunned();
	}

	public override void UpdateHealth(float newHealth)
	{
		base.Health = newHealth;
	}

	public override bool HasKnockBack()
	{
		if (Template != null)
		{
			return Template.HasKnockBack();
		}
		return false;
	}

	public override void Read(Code.Core.Network.types.LevelObject gameCreature, OnReadDone callback)
	{
		base.Read(gameCreature, callback);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(((Creature)gameCreature).GetTemplate(), FinishRead);
	}

	private void StoreUpdateMessage(object msgAsObj)
	{
		if (msgAsObj is ServerObjectUpdate serverObjectUpdate && serverObjectUpdate.getUpdate().GetId() == base.Id)
		{
			PendingUpdateMessage = (CreatureUpdate)serverObjectUpdate.getUpdate();
			InitialMoverPositionTime = Time.time;
		}
	}

	protected override void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (!timeOut)
		{
			Template = template as MilMo_LevelCreatureTemplate;
			if (Template == null)
			{
				Debug.LogWarning("Creature " + FullPath + " has no template.");
				return;
			}
			Mover = Template.GetMoverInstance();
			base.Health = Template.MaxHealth;
			VisualRepName = Template.VisualRep;
			base.FinishRead(template, timeOut: false);
		}
	}

	protected override bool FinishLoad()
	{
		bool useSpawnEffect = UseSpawnEffect;
		UseSpawnEffect = false;
		if (!base.FinishLoad())
		{
			return false;
		}
		AudioSource = base.GameObject.AddComponent(typeof(AudioSourceWrapper)) as AudioSourceWrapper;
		MilMo_AudioUtils.SetRollOffFactor(AudioSource, AudioRolloffFactor);
		AnimationHandler = base.GameObject.AddComponent(typeof(MilMo_MovableAnimationHandler)) as MilMo_MovableAnimationHandler;
		if (AnimationHandler != null)
		{
			AnimationHandler.VisualRep = base.VisualRep;
		}
		Mover?.Initialize(base.VisualRep);
		if (useSpawnEffect)
		{
			CreateSpawnEffects();
		}
		UseSpawnEffect = useSpawnEffect;
		if (base.GameObject.GetComponent(typeof(Animation)) == null)
		{
			base.GameObject.AddComponent(typeof(Animation));
		}
		Mover?.PlayAnimation("Walk", 0.3f, WrapMode.Loop);
		return IsReady = base.GameObject != null && AudioSource != null && Mover != null;
	}

	protected override float GetSusceptibility(string damageType)
	{
		if (Template == null)
		{
			return 0f;
		}
		return Template.GetSusceptibility(damageType);
	}

	protected override void GenericDamageEffects(float damage)
	{
		if (MilMo_World.Instance.enabled && Template != null)
		{
			if (damage > 0f && Template.DamageSound != null && AudioSource != null)
			{
				AudioSource.Clip = Template.DamageSound;
				AudioSource.Loop = false;
				AudioSource.Play();
			}
			else if (damage <= 0f && Template.NoDamageSound != null && AudioSource != null)
			{
				AudioSource.Clip = Template.NoDamageSound;
				AudioSource.Loop = false;
				AudioSource.Play();
			}
		}
	}

	protected override void ApplyLocalDamageImpulse(float damage, MilMo_Weapon hitWeapon)
	{
		if (Mover != null && hitWeapon != null && hitWeapon.Template != null)
		{
			float impact = ((damage > 0f) ? hitWeapon.Template.Impact : (0.3f * hitWeapon.Template.Impact));
			Mover.AddLocalImpulse(impact);
		}
	}

	protected override void HandleCollectDamage(MilMo_Avatar attacker, MilMo_Weapon hitWeapon, float totalDamage)
	{
		if (!(base.Health <= totalDamage) || hitWeapon.GameObject == null || Mover == null)
		{
			return;
		}
		Transform[] componentsInChildren = hitWeapon.GameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (!(transform.name != "CollectNode"))
			{
				Mover.DeathCollectNode = transform;
				break;
			}
		}
	}

	protected override MilMo_ParticleDamageEffect GetDamageParticleEmitter()
	{
		return _particleDamageEffect ?? (_particleDamageEffect = new MilMo_ParticleDamageEffect(base.GameObject.transform, Template.ImpactHeight));
	}

	protected override Vector3 GetProjectileImpactPosition(MilMo_LevelProjectile projectile)
	{
		Vector3 position = projectile.Position;
		Vector3 position2 = base.GameObject.transform.position;
		Vector3 vector = ((position != position2) ? (position - position2).normalized : Vector3.up);
		return position2 + Template.ImpactRadius * vector;
	}

	protected override void InitializeDeathTimer(float duration)
	{
		DeathTimer = ((DeathEffectsPhase1.Count < 1 || Mover == null || !Mover.DeathCollectNode) ? 0f : duration);
	}

	public override void Kill()
	{
		MilMo_CreatureMover mover = Mover;
		if (mover != null && mover.HasDeathImpulse)
		{
			Debug.Log("Got kill request for creature with death impulse");
		}
		else
		{
			base.Kill();
		}
	}

	public override void PrepareAttack(MilMo_Avatar target, TemplateReference attackRef)
	{
		if (!IsReady || !MilMo_World.Instance.enabled)
		{
			return;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(attackRef, delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || !(template is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate))
			{
				Debug.LogWarning("Failed to load creature attack template " + attackRef.GetPath());
			}
			else if (!(base.GameObject == null))
			{
				foreach (string preparationEffect in milMo_CreatureAttackTemplate.PreparationEffects)
				{
					MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, preparationEffect);
					if (objectEffect != null)
					{
						ObjectEffects.Add(objectEffect);
					}
				}
				if (target != null && target.GameObject != null)
				{
					Mover.TurnTo(target.Position);
				}
			}
		});
	}

	public override void Attack(MilMo_CreatureAttackTemplate attack, Transform target)
	{
		base.Attack(attack, target);
		if (target != null && Mover != null)
		{
			Mover.TurnTo(target.position);
		}
	}

	public override bool IsContainer()
	{
		return Mover is MilMo_StandStillMover;
	}

	public override bool IsDangerous()
	{
		return Template.IsDangerous;
	}

	public override void Unload()
	{
		base.Unload();
		_particleDamageEffect?.Destroy();
	}

	private void UpdateDyingPhase1()
	{
		if (base.GameObject == null)
		{
			LifePhase = MilMo_LifePhase.Dead;
			return;
		}
		DeathTimer -= Time.deltaTime;
		for (int num = DeathEffectsPhase1.Count - 1; num >= 0; num--)
		{
			if (!DeathEffectsPhase1[num].Update())
			{
				DeathEffectsPhase1.RemoveAt(num);
			}
		}
		bool flag = true;
		if (base.GameObject.transform.parent != null && !MilMo_Utility.Equals(base.GameObject.transform.localPosition, Vector3.zero, 0.01f))
		{
			flag = false;
		}
		if (DeathTimer <= 0f && flag)
		{
			EnterDyingPhase2();
		}
	}

	private void UpdateDyingPhase2()
	{
		if (base.GameObject == null)
		{
			LifePhase = MilMo_LifePhase.Dead;
			return;
		}
		DeathTimer -= Time.deltaTime;
		for (int num = DeathEffectsPhase2.Count - 1; num >= 0; num--)
		{
			if (!DeathEffectsPhase2[num].Update())
			{
				DeathEffectsPhase2.RemoveAt(num);
			}
		}
		bool flag = true;
		if (base.GameObject.transform.parent != null && !MilMo_Utility.Equals(base.GameObject.transform.localPosition, Vector3.zero, 0.01f))
		{
			flag = false;
		}
		if (DeathTimer <= 0f && flag)
		{
			for (int num2 = DeathEffectsPhase2.Count - 1; num2 >= 0; num2--)
			{
				DeathEffectsPhase2[num2].Destroy();
				DeathEffectsPhase2.RemoveAt(num2);
			}
			LifePhase = MilMo_LifePhase.Dead;
		}
	}
}
