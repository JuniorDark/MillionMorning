using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.Camera;
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
using Code.World.EntityStates;
using Code.World.GUI;
using Code.World.Player;
using Core;
using UI;
using UI.Marker.Combat;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public abstract class MilMo_MovableObject : MilMo_LevelObject, IMilMo_AttackTarget, IMilMo_Entity
{
	protected enum MilMo_LifePhase
	{
		Spawning,
		Alive,
		Dying1,
		Dying2,
		Dead
	}

	private readonly List<MilMo_Effect> _activeParticleEffects = new List<MilMo_Effect>();

	protected float DamageFlashDuration;

	protected float AudioRolloffFactor;

	protected AudioSourceWrapper AudioSource;

	protected MilMo_MovableAnimationHandler AnimationHandler;

	protected readonly List<MilMo_ObjectEffect> DeathEffectsPhase1 = new List<MilMo_ObjectEffect>();

	protected readonly List<MilMo_ObjectEffect> DeathEffectsPhase2 = new List<MilMo_ObjectEffect>();

	protected float DeathTimer;

	protected float DamageTimer;

	private Color _mainColor;

	private Vector4 _redFlashFadeSpeed;

	protected MilMo_LifePhase LifePhase;

	protected Vector3 LastKnownPosition = Vector3.zero;

	private List<MilMo_LevelItem> _lootItems;

	protected MilMo_MovableObjectTemplate Template;

	protected MilMo_MovableObjectMover Mover;

	protected CreatureUpdate PendingUpdateMessage;

	protected MilMo_GenericReaction CreatureUpdateReaction;

	protected float InitialMoverPositionTime;

	private MilMoObjectMarker _syncDistanceLabel;

	private GameObject _debugServerPositionMarker;

	private Bounds _boundingBox;

	private string _lastAttacker = "";

	private readonly MilMo_EntityStateManager _entityStateManager;

	private readonly CreatureMarker _creatureMarker;

	private float _health;

	public float CollisionRadius => Template.CollisionRadius;

	public float ImpactRadius => Template.ImpactRadius;

	public float MarkerYOffset => _boundingBox.extents.y;

	public float ImpactRadiusSqr => Template.ImpactRadiusSqr;

	public float ImpactHeight => Template.ImpactHeight;

	public bool ShouldBeKilled { get; set; }

	public Bounds BoundingBox => _boundingBox;

	public float MaxHealth => Template.MaxHealth;

	public float Health
	{
		get
		{
			return _health;
		}
		protected set
		{
			_health = value;
			this.OnHealthChanged?.Invoke();
		}
	}

	public string Name => Template.DisplayName.String;

	public int AvatarLevel => Template.Level;

	public event Action OnHealthChanged;

	public virtual bool IsCritter()
	{
		return false;
	}

	public virtual bool IsBoss()
	{
		return false;
	}

	public void Target()
	{
		_creatureMarker.Show(shouldShow: true);
	}

	public void UnTarget()
	{
		_creatureMarker.Show(shouldShow: false);
	}

	protected MilMo_MovableObject(string path, bool useSpawnEffect)
		: base(path, useSpawnEffect)
	{
		_entityStateManager = new MilMo_EntityStateManager(this);
		_creatureMarker = WorldSpaceManager.GetWorldSpaceObject<CreatureMarker>("CreatureMarker");
	}

	public void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		Mover?.ReadUpdate(creatureUpdate);
	}

	public MilMo_CreatureAttackTemplate Attack(TemplateReference attackRef, Transform target)
	{
		if (!IsReady)
		{
			return null;
		}
		if (!(Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(attackRef) is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate))
		{
			return null;
		}
		Attack(milMo_CreatureAttackTemplate, target);
		return milMo_CreatureAttackTemplate;
	}

	public virtual void Attack(MilMo_CreatureAttackTemplate attack, Transform target)
	{
		if (attack == null || base.GameObject == null || !MilMo_World.Instance.enabled)
		{
			return;
		}
		foreach (string attackEffect in attack.AttackEffects)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, attackEffect);
			if (objectEffect != null)
			{
				ObjectEffects.Add(objectEffect);
			}
		}
	}

	public virtual void AttackDone()
	{
	}

	public abstract void PrepareAttack(MilMo_Avatar target, TemplateReference attackRef);

	public abstract void HandleImpulse(ServerMoveableImpulse impulseMsg);

	public abstract void Stunned();

	public abstract void UpdateHealth(float newHealth);

	public abstract bool HasKnockBack();

	protected abstract float GetSusceptibility(string damageType);

	protected abstract void GenericDamageEffects(float damage);

	protected abstract MilMo_ParticleDamageEffect GetDamageParticleEmitter();

	protected abstract Vector3 GetProjectileImpactPosition(MilMo_LevelProjectile projectile);

	protected abstract void InitializeDeathTimer(float duration);

	public abstract bool IsContainer();

	public abstract bool IsDangerous();

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			return false;
		}
		Renderer componentInChildren = base.GameObject.GetComponentInChildren<Renderer>();
		if (componentInChildren != null)
		{
			_boundingBox = componentInChildren.bounds;
			_boundingBox.center = base.GameObject.transform.InverseTransformPoint(_boundingBox.center);
		}
		SetupCombatMarker();
		return true;
	}

	private void SetupCombatMarker()
	{
		if (_creatureMarker != null)
		{
			_creatureMarker.Initialize(this);
		}
		else
		{
			Debug.LogWarning("Unable to get a CombatMarker for " + Name);
		}
	}

	private void SpawnDamageParticle(IMilMo_AttackTarget attacker, float damage)
	{
		if (!MilMo_World.Instance.enabled || damage <= 0f || attacker == null || attacker.GameObject == null || Template == null)
		{
			return;
		}
		MilMo_ParticleDamageEffect damageParticleEmitter = GetDamageParticleEmitter();
		Vector3 position = attacker.GameObject.transform.position;
		position.y = 0f;
		Vector3 position2 = base.GameObject.transform.position;
		position2.y = 0f;
		float num = Template.ImpactRadius;
		Vector3 vector2;
		if (!MilMo_Utility.Equals(position, position2))
		{
			Vector3 vector = position - position2;
			float magnitude = vector.magnitude;
			if (magnitude < Template.ImpactRadius)
			{
				num = magnitude / 2f;
			}
			vector2 = vector;
			vector2.y = 0f;
			vector2.Normalize();
		}
		else
		{
			vector2 = Vector3.up;
		}
		Vector3 impactPosition = position2 + num * vector2;
		impactPosition.y = base.GameObject.transform.position.y + Template.ImpactHeight;
		damageParticleEmitter.Emit(starDirection: GetVisibleDirection(), damageAmount: damage, impactPosition: impactPosition);
	}

	protected virtual void ApplyLocalDamageImpulse(float damage, MilMo_Weapon hitWeapon)
	{
	}

	protected virtual void HandleCollectDamage(MilMo_Avatar attacker, MilMo_Weapon hitWeapon, float totalDamage)
	{
	}

	private void StartDamageFlash()
	{
		if (!MilMo_World.Instance.enabled)
		{
			return;
		}
		if (base.VisualRep != null && base.VisualRep.Renderer != null && base.VisualRep.Renderer.material != null)
		{
			if (DamageTimer <= 0f)
			{
				_mainColor = base.VisualRep.Renderer.material.color;
			}
			Color red = Color.red;
			_redFlashFadeSpeed = new Vector4(_mainColor.r - red.r, _mainColor.g - red.g, _mainColor.b - red.b, _mainColor.a - red.a) / DamageFlashDuration;
			Material[] sharedMaterials = base.VisualRep.Renderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				sharedMaterials[i].color = red;
			}
		}
		DamageTimer = DamageFlashDuration;
	}

	public float GetDamage(List<MilMo_Damage> damage)
	{
		bool collectDamage;
		return GetDamage(damage, out collectDamage);
	}

	private float GetDamage(List<MilMo_Damage> damage, out bool collectDamage)
	{
		float num = 0f;
		collectDamage = false;
		foreach (MilMo_Damage item in damage)
		{
			num += item.Value * GetSusceptibility(item.DamageType);
			if (item.DamageType == "Collect")
			{
				collectDamage = num > 0f;
			}
		}
		return num;
	}

	public void InitializeMover(Vector3 target, float speed)
	{
		if (Mover != null)
		{
			if (PendingUpdateMessage != null)
			{
				float speed2 = PendingUpdateMessage.GetSpeed();
				vector3 position = PendingUpdateMessage.GetPosition();
				vector3 target2 = PendingUpdateMessage.GetTarget();
				target = new Vector3(target2.GetX(), target2.GetY(), target2.GetZ());
				speed = speed2;
				Mover.SetRealPosition = new Vector3(position.GetX(), position.GetY(), position.GetZ());
			}
			Mover.StartMoving(target, speed, Time.time - InitialMoverPositionTime);
			MilMo_EventSystem.RemoveReaction(CreatureUpdateReaction);
			CreatureUpdateReaction = null;
		}
	}

	public void ReadHealthUpdate(ServerMoveableHealthUpdate msg)
	{
		UpdateHealth(msg.getHealth());
	}

	public virtual void Damage(float damage)
	{
		Health -= damage;
		if (Health < 0f)
		{
			Health = 0f;
		}
	}

	public void ReadLootObject(LevelItem gameItem)
	{
		new MilMo_LevelItem(spawnEffect: true).Read(gameItem, FinishReadLootObject);
	}

	private void FinishReadLootObject(bool success, MilMo_LevelObject levelObject)
	{
		if (!success)
		{
			return;
		}
		if (!(levelObject is MilMo_LevelItem milMo_LevelItem) || milMo_LevelItem.GameObject == null)
		{
			Debug.LogWarning("Failed to read loot object for " + FullPath);
			return;
		}
		if (MilMo_Level.CurrentLevel == null || MilMo_Level.CurrentLevel.VerboseName != milMo_LevelItem.FullLevelName)
		{
			Debug.LogWarning("Got loaded loot item from wrong level" + FullPath);
			milMo_LevelItem.Unload();
			return;
		}
		if (milMo_LevelItem.Template.IsUnique && MilMo_Player.Instance.Inventory.HaveItem(milMo_LevelItem.Item))
		{
			milMo_LevelItem.Unload();
			return;
		}
		milMo_LevelItem.Pause();
		if (_lootItems == null)
		{
			_lootItems = new List<MilMo_LevelItem>();
		}
		_lootItems.Add(milMo_LevelItem);
		if (LifePhase == MilMo_LifePhase.Dying2 || LifePhase == MilMo_LifePhase.Dead)
		{
			DropLoot();
		}
	}

	public void DamageEffectLocal(MilMo_Avatar attacker, MilMo_MeleeWeapon hitWeapon, float damage)
	{
		DamageEffectLocal(attacker, hitWeapon);
	}

	private void DamageEffectLocal(MilMo_Avatar attacker, MilMo_MeleeWeapon hitWeapon)
	{
		if (hitWeapon == null || LifePhase == MilMo_LifePhase.Dying2 || LifePhase == MilMo_LifePhase.Dead || base.GameObject == null)
		{
			return;
		}
		bool collectDamage = false;
		float num = 0f;
		if (hitWeapon.Template != null)
		{
			num = GetDamage(hitWeapon.Template.Damage, out collectDamage);
		}
		if (num > 0f && attacker != null)
		{
			_lastAttacker = attacker.Id;
		}
		if (collectDamage)
		{
			HandleCollectDamage(attacker, hitWeapon, num);
			return;
		}
		GenericDamageEffects(num);
		ApplyLocalDamageImpulse(num, hitWeapon);
		if (!(num <= 0f))
		{
			SpawnDamageParticle(attacker, num);
			StartDamageFlash();
		}
	}

	public void DamageEffectLocal(MilMo_LevelProjectile projectile, float damage)
	{
		DamageEffectLocal(projectile);
	}

	public void DamageEffectLocal(MilMo_LevelProjectile projectile)
	{
		if (projectile == null || LifePhase == MilMo_LifePhase.Dying2 || LifePhase == MilMo_LifePhase.Dead || base.GameObject == null)
		{
			return;
		}
		bool collectDamage = false;
		float num = 0f;
		if (projectile.Template != null)
		{
			num = GetDamage(projectile.Template.Damage, out collectDamage);
		}
		if (collectDamage)
		{
			return;
		}
		GenericDamageEffects(num);
		if (!(num <= 0f))
		{
			if (Template != null)
			{
				MilMo_ParticleDamageEffect damageParticleEmitter = GetDamageParticleEmitter();
				Vector3 projectileImpactPosition = GetProjectileImpactPosition(projectile);
				Vector3 visibleDirection = GetVisibleDirection();
				damageParticleEmitter.Emit(num, projectileImpactPosition, visibleDirection);
			}
			StartDamageFlash();
		}
	}

	public void DamageEffectLocal(float damage)
	{
		if (LifePhase != MilMo_LifePhase.Dying2 && LifePhase != MilMo_LifePhase.Dead && !(base.GameObject == null))
		{
			GenericDamageEffects(damage);
			if (!(damage <= 0f))
			{
				StartDamageFlash();
			}
		}
	}

	public bool IsDead()
	{
		return LifePhase == MilMo_LifePhase.Dead;
	}

	public virtual bool IsDeadOrDying()
	{
		if (LifePhase != MilMo_LifePhase.Dying1 && LifePhase != MilMo_LifePhase.Dying2)
		{
			return LifePhase == MilMo_LifePhase.Dead;
		}
		return true;
	}

	public virtual void Kill()
	{
		LifePhase = MilMo_LifePhase.Dying1;
		if (!string.IsNullOrEmpty(_lastAttacker) && MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null && _lastAttacker.Equals(MilMo_Player.Instance.Avatar.Id))
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_Kill", Template.Name);
		}
		float num = 0f;
		if (MilMo_World.Instance.enabled)
		{
			foreach (string item in Template.DeathEffectsPhase1)
			{
				MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, item);
				if (objectEffect != null)
				{
					num = Mathf.Max(num, objectEffect.Duration);
					DeathEffectsPhase1.Add(objectEffect);
				}
			}
		}
		InitializeDeathTimer(num);
		Mover.Kill();
	}

	public void PlaySound(AudioClip sound)
	{
		if (MilMo_World.Instance.enabled && !(sound == null) && !(AudioSource == null))
		{
			AudioSource.Clip = sound;
			AudioSource.Loop = false;
			AudioSource.Play();
		}
	}

	public void Aggro(Vector3 aggroPlayerPosition)
	{
		if (Mover != null)
		{
			Mover.Aggro();
			Mover.TurnTo(aggroPlayerPosition);
		}
	}

	public void NoAggro()
	{
		Mover?.NoAggro();
	}

	public AttackTarget AsNetworkAttackTarget()
	{
		return new CreatureTarget(base.Id);
	}

	public new virtual void Unload()
	{
		base.Unload();
		_entityStateManager.Destroy();
		Mover?.Destroy();
		_creatureMarker.Remove();
	}

	private void FinalizeLocalDamageEffect()
	{
		if (base.VisualRep != null && base.VisualRep.Renderer != null && base.VisualRep.Renderer.material != null)
		{
			base.VisualRep.Renderer.material.color = _mainColor;
		}
	}

	protected void UpdateDamageEffect()
	{
		DamageTimer -= Time.deltaTime;
		if (base.VisualRep != null && base.VisualRep.Renderer != null && base.VisualRep.Renderer.material != null)
		{
			Material[] materials = base.VisualRep.Renderer.materials;
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i].color += new Color(_redFlashFadeSpeed.x * Time.deltaTime, _redFlashFadeSpeed.y * Time.deltaTime, _redFlashFadeSpeed.z * Time.deltaTime, _redFlashFadeSpeed.w * Time.deltaTime);
			}
		}
		if (DamageTimer <= 0f)
		{
			FinalizeLocalDamageEffect();
		}
	}

	public override void Update()
	{
		for (int num = _activeParticleEffects.Count - 1; num >= 0; num--)
		{
			if (!_activeParticleEffects[num].Update())
			{
				_activeParticleEffects.RemoveAt(num);
			}
		}
		base.Update();
	}

	protected void EnterDyingPhase2()
	{
		for (int num = DeathEffectsPhase1.Count - 1; num >= 0; num--)
		{
			DeathEffectsPhase1[num].Destroy();
			DeathEffectsPhase1.RemoveAt(num);
		}
		float num2 = 0f;
		if (MilMo_World.Instance.enabled)
		{
			foreach (string item in Template.DeathEffectsPhase2)
			{
				MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(base.GameObject, item);
				if (objectEffect != null)
				{
					num2 = Mathf.Max(num2, objectEffect.Duration);
					DeathEffectsPhase2.Add(objectEffect);
				}
			}
		}
		if (DeathEffectsPhase2.Count > 0)
		{
			LifePhase = MilMo_LifePhase.Dying2;
			DeathTimer = num2;
		}
		else
		{
			LifePhase = MilMo_LifePhase.Dead;
		}
		DropLoot();
	}

	private void DropLoot()
	{
		if (_lootItems == null)
		{
			return;
		}
		Vector3 position = ((base.GameObject != null) ? base.GameObject.transform.position : LastKnownPosition);
		foreach (MilMo_LevelItem lootItem in _lootItems)
		{
			List<MilMo_MoverEffect> list = lootItem.SpawnEffects.OfType<MilMo_MoverEffect>().ToList();
			if (list.Count < 0)
			{
				break;
			}
			if (lootItem.GameObject == null)
			{
				continue;
			}
			lootItem.GameObject.transform.position = position;
			Vector3 spawnPosition = lootItem.SpawnPosition;
			foreach (MilMo_MoverEffect item in list)
			{
				item.SetTargetPosition(spawnPosition);
			}
			lootItem.Unpause();
			lootItem.SetCreationTime();
			MilMo_Level.CurrentLevel?.AddItem(lootItem);
		}
	}

	private Vector3 GetVisibleDirection()
	{
		Vector3 position = MilMo_CameraController.CameraTransform.position;
		Vector3 vector = base.GameObject.transform.position - position;
		vector.y = 0f;
		int num = UnityEngine.Random.Range(0, 2);
		if (num == 0)
		{
			num = -1;
		}
		Vector3 vector2 = new Vector3((float)num * vector.z, 0f, (float)(-1 * num) * vector.x);
		vector2.Normalize();
		float num2 = (float)Math.Acos(vector2.x);
		float minInclusive = num2 - MathF.PI / 4f;
		float maxInclusive = num2 + MathF.PI / 4f;
		float f = UnityEngine.Random.Range(minInclusive, maxInclusive);
		return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
	}

	public void AddStateEffect(string particleEffect)
	{
		if (!(base.GameObject == null))
		{
			MilMo_Effect effect = MilMo_EffectContainer.GetEffect(particleEffect, base.GameObject);
			if (effect != null)
			{
				_activeParticleEffects.Add(effect);
			}
		}
	}

	public void RemoveStateEffect(string particleEffect)
	{
		if (_activeParticleEffects.Count < 1)
		{
			return;
		}
		for (int num = _activeParticleEffects.Count - 1; num >= 0; num--)
		{
			if (!(_activeParticleEffects[num].Name != particleEffect))
			{
				_activeParticleEffects[num].Stop();
				_activeParticleEffects[num].Destroy();
				_activeParticleEffects.RemoveAt(num);
			}
		}
	}

	public MilMo_EntityStateManager GetEntityStateManager()
	{
		return _entityStateManager;
	}
}
