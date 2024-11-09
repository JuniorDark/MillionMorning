using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
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

public class MilMo_LevelBoss : MilMo_MovableObject
{
	private MilMo_ObjectEffect _healingEffect;

	private Vector3 _nameTagOffset = Vector3.zero;

	private readonly List<MilMo_ParticleDamageEffect> _particleDamageEffects = new List<MilMo_ParticleDamageEffect>();

	private MilMo_GenericReaction _changeModeReaction;

	private TemplateReference _currentModeTemplate;

	private new MilMo_LevelBossTemplate Template
	{
		get
		{
			return base.Template as MilMo_LevelBossTemplate;
		}
		set
		{
			base.Template = value;
		}
	}

	private new MilMo_BossMover Mover
	{
		get
		{
			return base.Mover as MilMo_BossMover;
		}
		set
		{
			base.Mover = value;
		}
	}

	public MilMo_LevelBoss(bool useSpawnEffects)
		: base("Content/Creatures/", useSpawnEffects)
	{
		CreatureUpdateReaction = MilMo_EventSystem.Listen("object_update", StoreUpdateMessage);
		CreatureUpdateReaction.Repeating = true;
		InitialMoverPositionTime = Time.time;
		DamageFlashDuration = 0.5f;
		AudioRolloffFactor = 0.5f;
	}

	public override void Update()
	{
		if (Paused)
		{
			return;
		}
		base.Update();
		if (_healingEffect != null && !_healingEffect.Update())
		{
			_healingEffect = null;
		}
		foreach (MilMo_ParticleDamageEffect particleDamageEffect in _particleDamageEffects)
		{
			particleDamageEffect.Update();
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
		if (LifePhase != MilMo_LifePhase.Dead)
		{
			if (Mover != null)
			{
				Mover.Update();
			}
			if (base.GameObject != null)
			{
				LastKnownPosition = base.GameObject.transform.position;
			}
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		foreach (MilMo_ParticleDamageEffect particleDamageEffect in _particleDamageEffects)
		{
			particleDamageEffect.FixedUpdate();
		}
	}

	public override void UpdateHealth(float newHealth)
	{
		base.Health = newHealth;
		if (_healingEffect != null && (base.Health >= Template.MaxHealth || base.Health <= 0f))
		{
			StopHealingEffect();
		}
	}

	public override void Damage(float damage)
	{
		base.Damage(damage);
		if (base.GameObject != null && Mover != null && Template != null && Mover.CurrentMode != null && !string.IsNullOrEmpty(Mover.CurrentMode.HealingEffect) && base.Health > 0f && base.Health < Template.MaxHealth && _healingEffect == null)
		{
			_healingEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, Mover.CurrentMode.HealingEffect);
		}
		else if (base.Health <= 0f && _healingEffect != null)
		{
			StopHealingEffect();
		}
	}

	public override bool HasKnockBack()
	{
		return false;
	}

	public override void Read(Code.Core.Network.types.LevelObject levelObject, OnReadDone callback)
	{
		base.Read(levelObject, callback);
		Boss boss = levelObject as Boss;
		Mover = new MilMo_BossMover();
		if (boss != null)
		{
			base.Health = boss.GetHealth();
			_currentModeTemplate = boss.GetCurrentMode();
			_changeModeReaction = MilMo_EventSystem.Listen("boss_change_mode", HandleChangeModeMessage);
			_changeModeReaction.Repeating = true;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(boss.GetTemplate(), FinishRead);
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_currentModeTemplate, ModeTemplateArrived);
	}

	private void StoreUpdateMessage(object msgAsObj)
	{
		if (msgAsObj is ServerObjectUpdate serverObjectUpdate && serverObjectUpdate.getUpdate().GetId() == base.Id)
		{
			PendingUpdateMessage = (CreatureUpdate)serverObjectUpdate.getUpdate();
			InitialMoverPositionTime = Time.time;
		}
	}

	private void HandleChangeModeMessage(object msgAsObj)
	{
		if (msgAsObj is ServerBossChangeMode serverBossChangeMode && serverBossChangeMode.getBossId() == base.Id)
		{
			_currentModeTemplate = serverBossChangeMode.getMode();
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(_currentModeTemplate, ModeTemplateArrived);
		}
	}

	protected override void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (!timeOut)
		{
			Template = template as MilMo_LevelBossTemplate;
			if (Template == null)
			{
				Debug.LogWarning("Boss " + FullPath + " has no template.");
				return;
			}
			Mover.Template = Template;
			VisualRepName = Template.VisualRep;
			base.FinishRead(template, timeOut: false);
		}
	}

	private void ModeTemplateArrived(MilMo_Template template, bool timeOut)
	{
		if (timeOut || template == null || template.Path != _currentModeTemplate.GetPath() || template.Category != _currentModeTemplate.GetCategory())
		{
			return;
		}
		MilMo_BossModeTemplate mode = template as MilMo_BossModeTemplate;
		if (mode == null)
		{
			Debug.LogWarning("Failed to load mode template for boss " + FullPath);
			return;
		}
		Mover.CurrentMode = mode;
		if (_healingEffect != null)
		{
			StopHealingEffect();
		}
		MilMo_EventSystem.At(1f, delegate
		{
			if (!(template.Path != _currentModeTemplate.GetPath()) && !(template.Category != _currentModeTemplate.GetCategory()) && Template != null && !string.IsNullOrEmpty(mode.HealingEffect) && base.Health > 0f && base.Health < Template.MaxHealth && base.GameObject != null && _healingEffect == null)
			{
				_healingEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, mode.HealingEffect);
			}
		});
	}

	private void StopHealingEffect()
	{
		_healingEffect.Destroy();
		_healingEffect = null;
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			return false;
		}
		AudioSource = base.GameObject.AddComponent<AudioSourceWrapper>();
		MilMo_AudioUtils.SetRollOffFactor(AudioSource, AudioRolloffFactor);
		AnimationHandler = base.GameObject.AddComponent<MilMo_MovableAnimationHandler>();
		if (AnimationHandler != null)
		{
			AnimationHandler.VisualRep = base.VisualRep;
		}
		if (Mover != null)
		{
			Mover.Initialize(base.VisualRep);
		}
		bool result = (IsReady = base.GameObject != null && AudioSource != null && Mover != null);
		if (Mover != null && Mover.CurrentMode != null && !string.IsNullOrEmpty(Mover.CurrentMode.HealingEffect) && base.Health < Template.MaxHealth && _healingEffect == null)
		{
			_healingEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, Mover.CurrentMode.HealingEffect);
		}
		_ = MilMo_World.Instance != null;
		_nameTagOffset = default(Vector3);
		if (base.MarkerYOffset != 0f)
		{
			_nameTagOffset.y += base.MarkerYOffset;
		}
		else
		{
			_nameTagOffset = base.BoundingBox.center;
			_nameTagOffset.y += base.BoundingBox.extents.y + 0.5f;
		}
		return result;
	}

	protected override float GetSusceptibility(string damageType)
	{
		if (Mover == null || Mover.CurrentMode == null)
		{
			return 0f;
		}
		return Mover.CurrentMode.GetSusceptibility(damageType);
	}

	protected override void GenericDamageEffects(float damage)
	{
		if (!MilMo_World.Instance.enabled)
		{
			return;
		}
		if (damage > 0f)
		{
			if (Mover == null || Mover.CurrentMode == null)
			{
				return;
			}
			{
				foreach (MilMo_ObjectEffect item in from effectName in Mover.CurrentMode.DamageEffects
					select MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, effectName) into effect
					where effect != null
					select effect)
				{
					ObjectEffects.Add(item);
				}
				return;
			}
		}
		if (Mover == null || Mover.CurrentMode == null)
		{
			return;
		}
		foreach (MilMo_ObjectEffect item2 in from effectName in Mover.CurrentMode.NoDamageEffects
			select MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, effectName) into effect
			where effect != null
			select effect)
		{
			ObjectEffects.Add(item2);
		}
	}

	protected override MilMo_ParticleDamageEffect GetDamageParticleEmitter()
	{
		MilMo_ParticleDamageEffect milMo_ParticleDamageEffect = new MilMo_ParticleDamageEffect(base.GameObject.transform, Template.ImpactHeight);
		_particleDamageEffects.Add(milMo_ParticleDamageEffect);
		return milMo_ParticleDamageEffect;
	}

	protected override Vector3 GetProjectileImpactPosition(MilMo_LevelProjectile projectile)
	{
		Vector3 position = projectile.Position;
		Vector3 position2 = base.GameObject.transform.position;
		Vector3 vector;
		if (!MilMo_Utility.Equals(position, position2))
		{
			vector = position - position2;
			vector.y = 0f;
			vector.Normalize();
		}
		else
		{
			vector = base.GameObject.transform.forward;
		}
		Vector3 result = position2 + Template.ImpactRadius * vector;
		result.y = projectile.Position.y;
		return result;
	}

	protected override void InitializeDeathTimer(float duration)
	{
		DeathTimer = ((DeathEffectsPhase1.Count > 0) ? duration : 0f);
	}

	public override void Attack(MilMo_CreatureAttackTemplate attack, Transform target)
	{
		if (attack != null && !(base.GameObject == null))
		{
			base.Attack(attack, target);
			if (Mover != null)
			{
				Mover.TurnToTarget = null;
			}
		}
	}

	public override void AttackDone()
	{
		base.AttackDone();
		if (Mover != null)
		{
			MilMo_BossModeTemplate currentMode = Mover.CurrentMode;
			if (currentMode != null && !string.IsNullOrEmpty(currentMode.LoopingAnimation))
			{
				AnimationHandler.PlayAnimation(currentMode.LoopingAnimation, 0.3f, WrapMode.Loop);
			}
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
				if (Mover != null && target != null && target.GameObject != null)
				{
					Mover.TurnToTarget = target.GameObject.transform;
				}
			}
		});
	}

	public override void Unload()
	{
		base.Unload();
		foreach (MilMo_ParticleDamageEffect particleDamageEffect in _particleDamageEffects)
		{
			particleDamageEffect.Destroy();
		}
	}

	public override bool IsContainer()
	{
		return false;
	}

	public override bool IsDangerous()
	{
		return true;
	}

	public override bool IsBoss()
	{
		return true;
	}

	public override void HandleImpulse(ServerMoveableImpulse impulseMsg)
	{
	}

	public override void Stunned()
	{
	}

	private void UpdateDyingPhase1()
	{
		DeathTimer -= Time.deltaTime;
		for (int num = DeathEffectsPhase1.Count - 1; num >= 0; num--)
		{
			if (!DeathEffectsPhase1[num].Update())
			{
				DeathEffectsPhase1.RemoveAt(num);
			}
		}
		if (DeathTimer <= 0f)
		{
			EnterDyingPhase2();
		}
	}

	private void UpdateDyingPhase2()
	{
		DeathTimer -= Time.deltaTime;
		for (int num = DeathEffectsPhase2.Count - 1; num >= 0; num--)
		{
			if (!DeathEffectsPhase2[num].Update())
			{
				DeathEffectsPhase2.RemoveAt(num);
			}
		}
		if (DeathTimer <= 0f)
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
