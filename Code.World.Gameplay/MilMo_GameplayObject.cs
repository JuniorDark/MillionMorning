using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Visual.Effect;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayObject : MilMo_LevelObject
{
	public const float CLOSE_TO_GAMEPLAY_OBJECT_DISTANCE_SQR = 2.25f;

	public const float SHOW_CAPTURE_ZONE_COUNTDOWN_DISTANCE = 10f;

	private const long CAPTURE_FLAG_SHRINK_DURATION = 4000L;

	private static bool _debugMode;

	private readonly List<MilMo_GameplayTrigger> _triggers = new List<MilMo_GameplayTrigger>();

	private MilMo_GameplayObjectTemplate _template;

	private readonly List<MilMo_ObjectEffect> _activeEffects = new List<MilMo_ObjectEffect>();

	private MilMo_Effect _activeEffect;

	private bool _responsibleForGameObjectDestroy;

	private string _capturerPlayerId;

	private bool _isZoneCaptured;

	private int _zoneCapturerTeamId = -1;

	private MilMo_GenericReaction _moverStartMovingReaction;

	private MilMo_GenericReaction _moverChangeDirectionReaction;

	private List<float> _initialLastTriggerActivationTimes = new List<float>();

	private Vector3 _targetScale = new Vector3(0f, 0f, 0f);

	private long _shrinkEndTimeStamp;

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	public string Room { get; private set; }

	private MilMo_GameplayObjectMover Mover { get; set; }

	public Vector3 StartPosition => SpawnPosition;

	public bool IsMovingPlatform { get; set; }

	public bool IsCaptured { get; private set; }

	public bool PlayerIsCapturer
	{
		get
		{
			if (_capturerPlayerId != null)
			{
				return _capturerPlayerId == MilMo_Player.Instance.Id;
			}
			return false;
		}
	}

	public bool IsCapturingZone { get; private set; }

	public int CurrentZoneCapturingTeamId { get; private set; }

	public long CaptureZoneTimeStamp { get; private set; }

	public string GetTemplateIdentifier()
	{
		return _template.Identifier;
	}

	public void PlayerCapturedObject(string playerId)
	{
		IsCaptured = true;
		_capturerPlayerId = playerId;
		PlayCaptureSound();
	}

	public void SetCapturerAsParent()
	{
		if (!IsCaptured || _capturerPlayerId == null)
		{
			return;
		}
		MilMo_Avatar milMo_Avatar = null;
		if (_capturerPlayerId == MilMo_Player.Instance.Id)
		{
			milMo_Avatar = MilMo_Player.Instance.Avatar;
		}
		else if (MilMo_Level.CurrentLevel.Players.ContainsKey(_capturerPlayerId))
		{
			milMo_Avatar = MilMo_Level.CurrentLevel.Players[_capturerPlayerId].Avatar;
		}
		if (milMo_Avatar != null)
		{
			base.GameObject.AddComponent<MilMo_GameplayObjectSaver>();
			base.GameObject.transform.parent = milMo_Avatar.GameObject.transform;
			base.GameObject.transform.localPosition = new Vector3(0f, 0.8f, -0.1f);
			base.GameObject.transform.localRotation = Quaternion.Euler(0f, 90f, -30f);
			_targetScale = new Vector3(0.2f, 0.2f, 0.2f);
			DateTime value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			_shrinkEndTimeStamp = DateTime.UtcNow.AddMilliseconds(4000.0).Subtract(value).Ticks / 10000;
			if (_capturerPlayerId == MilMo_Player.Instance.Id)
			{
				milMo_Avatar.UsePvpFlagRunSpeed = true;
				MilMo_PlayerControllerBase.SetMovementMode();
			}
		}
	}

	public void UpdatePosition(vector3 position, bool resetScale)
	{
		IsCaptured = false;
		base.GameObject.transform.parent = null;
		if (_capturerPlayerId == MilMo_Player.Instance.Id)
		{
			MilMo_Player.Instance.Avatar.UsePvpFlagRunSpeed = false;
			MilMo_PlayerControllerBase.RunSpeed = 7.5f;
		}
		_capturerPlayerId = null;
		base.GameObject.transform.position = new Vector3(position.GetX(), position.GetY(), position.GetZ());
		foreach (MilMo_GameplayTrigger trigger in _triggers)
		{
			trigger.UpdatePosition(position);
		}
		if (resetScale)
		{
			_targetScale = new Vector3(0f, 0f, 0f);
			base.GameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void SetTargetScale(ServerGameplayObjectShrink msg)
	{
		_targetScale = ((msg.getTargetScale() != null) ? msg.getTargetScale().GetAsVector() : new Vector3(0f, 0f, 0f));
		_shrinkEndTimeStamp = msg.getShrinkEndTimeStamp();
	}

	public void TeamCapturedZone(int teamId)
	{
		_isZoneCaptured = true;
		_zoneCapturerTeamId = teamId;
	}

	public void SetZoneCaptured(bool playSound)
	{
		if (!_isZoneCaptured || _zoneCapturerTeamId == -1)
		{
			return;
		}
		Component[] componentsInChildren = base.GameObject.GetComponentsInChildren(typeof(Renderer), includeInactive: true);
		if (componentsInChildren == null)
		{
			return;
		}
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i] as Renderer;
			if (!(renderer == null))
			{
				Material material = renderer.material;
				if (!(material == null))
				{
					material.SetColor(TintColor, MilMo_Level.CurrentLevel.PvpHandler.GetTeamColor(_zoneCapturerTeamId));
				}
			}
		}
		if (playSound)
		{
			PlayCaptureSound();
		}
		_activeEffect = MilMo_EffectContainer.GetEffect("PvPEffect2", base.GameObject);
	}

	private async void PlayCaptureSound()
	{
		AudioSourceWrapper audioSource = base.GameObject.GetComponent<AudioSourceWrapper>();
		if (audioSource == null)
		{
			audioSource = base.GameObject.AddComponent<AudioSourceWrapper>();
		}
		audioSource.PlayOnAwake = false;
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync("Content/Sounds/Batch01/PVP/warhorn");
		if (!(audioClip == null))
		{
			audioSource.Clip = audioClip;
			audioSource.Loop = false;
			audioSource.Play();
		}
	}

	public void ResetZone()
	{
		if (!_isZoneCaptured)
		{
			return;
		}
		_isZoneCaptured = false;
		_zoneCapturerTeamId = -1;
		Component[] componentsInChildren = base.GameObject.GetComponentsInChildren(typeof(Renderer), includeInactive: true);
		if (componentsInChildren == null)
		{
			return;
		}
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i] as Renderer;
			if (!(renderer == null))
			{
				Material material = renderer.material;
				if (!(material == null))
				{
					material.SetColor(TintColor, base.VisualRep.DefaultColor);
				}
			}
		}
		TeamStoppedCapturingZone();
	}

	public void TeamStartedCapturingZone(int newCapturingTeamId, long captureTimeStamp)
	{
		IsCapturingZone = true;
		CurrentZoneCapturingTeamId = newCapturingTeamId;
		CaptureZoneTimeStamp = captureTimeStamp;
	}

	public void TeamStoppedCapturingZone()
	{
		IsCapturingZone = false;
		CurrentZoneCapturingTeamId = -1;
		CaptureZoneTimeStamp = 0L;
	}

	public MilMo_GameplayObject()
		: base("Content/Worlds/", useSpawnEffect: false)
	{
		_moverStartMovingReaction = MilMo_EventSystem.Listen("gameplay_object_start_moving", GotStartMovingMessage);
		_moverStartMovingReaction.Repeating = true;
		_moverChangeDirectionReaction = MilMo_EventSystem.Listen("gameplay_object_change_direction", GotChangeDirectionMessage);
		_moverChangeDirectionReaction.Repeating = true;
	}

	public override void Update()
	{
		base.Update();
		for (int num = _activeEffects.Count - 1; num >= 0; num--)
		{
			if (!_activeEffects[num].Update())
			{
				_activeEffects.RemoveAt(num);
			}
		}
		if (_activeEffect != null && !_activeEffect.Update())
		{
			_activeEffect = null;
		}
		if (Mover != null)
		{
			Mover.Update();
		}
		if (_debugMode)
		{
			foreach (MilMo_GameplayTrigger trigger in _triggers)
			{
				trigger.Debug_Draw();
			}
		}
		UpdateScale();
	}

	private void UpdateScale()
	{
		DateTime t = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(_shrinkEndTimeStamp).ToLocalTime();
		Vector3 localScale;
		if (DateTime.Compare(t, DateTime.Now) <= 0)
		{
			localScale = ((_targetScale.magnitude != 0f) ? _targetScale : Scale);
		}
		else
		{
			Vector3 a = new Vector3(Scale.x, Scale.y, Scale.z);
			if (_template.Shrinkage != 0f)
			{
				float shrinkage = _template.Shrinkage;
				a = new Vector3(_targetScale.x + shrinkage, _targetScale.y, _targetScale.z + shrinkage);
			}
			long num = 4000L;
			if (_template.ShrinkDuration != 0L)
			{
				num = _template.ShrinkDuration;
			}
			float t2 = (float)(num - Convert.ToInt64(Math.Floor(t.Subtract(DateTime.Now).TotalMilliseconds))) / (float)num;
			localScale = Vector3.Lerp(a, _targetScale, t2);
		}
		base.GameObject.transform.localScale = localScale;
	}

	public override void Read(LevelObject levelObject, OnReadDone callback)
	{
		base.Read(levelObject, callback);
		if (!(levelObject is GameplayObject gameplayObject))
		{
			return;
		}
		if (gameplayObject.GetMoverSpline() != null)
		{
			Mover = new MilMo_GameplayObjectMover(this, gameplayObject.GetMoverSpline(), gameplayObject.GetMoverTime());
		}
		Scale = new Vector3(gameplayObject.GetScale().GetX(), gameplayObject.GetScale().GetY(), gameplayObject.GetScale().GetZ());
		Room = gameplayObject.GetRoom();
		_initialLastTriggerActivationTimes = new List<float>();
		foreach (float item in (List<float>)gameplayObject.GetTimesSinceLastTriggerActivations())
		{
			_initialLastTriggerActivationTimes.Add(Time.time - item);
		}
		IsCaptured = gameplayObject.IsCaptured();
		_capturerPlayerId = gameplayObject.GetCapturerPlayerId();
		_isZoneCaptured = gameplayObject.IsZoneCaptured();
		_zoneCapturerTeamId = gameplayObject.GetZoneCapturerTeamId();
		if (_isZoneCaptured)
		{
			WaitForMaterial = true;
		}
		if (gameplayObject.GetTargetScale() != null)
		{
			_targetScale = gameplayObject.GetTargetScale().GetAsVector();
		}
		else
		{
			_targetScale = new Vector3(0f, 0f, 0f);
		}
		_shrinkEndTimeStamp = gameplayObject.GetShrinkEndTimeStamp();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(gameplayObject.GetTemplate(), FinishRead);
	}

	public void CheckPlayerContact(MilMo_Player player)
	{
		foreach (MilMo_GameplayTrigger trigger in _triggers)
		{
			trigger.UpdatePlayerDistance(player.Avatar.Position);
			if (Time.time - trigger.LatestActivationRequest >= 0.5f && trigger.ShouldActivate(0))
			{
				if (MilMo_Player.Instance.IsExhausted)
				{
					MilMo_Player.Instance.RequestRespawn(null);
					break;
				}
				if (trigger.Template.ActivationCheck == 1)
				{
					trigger.LatestActivationRequest = Time.time;
					Singleton<GameNetwork>.Instance.RequestActivateGameplayTrigger(base.Id, trigger.Id);
				}
				else
				{
					trigger.Activate(player);
				}
			}
		}
	}

	public void ActivateTrigger(MilMo_Player player, int triggerId)
	{
		if (triggerId < _triggers.Count)
		{
			if (_triggers[triggerId].Template.ActivationCheck == 1)
			{
				MilMo_EventSystem.Instance.PostEvent("tutorial_UseGameplayObject", _template.Name);
			}
			_triggers[triggerId].Activate(player);
		}
	}

	public void ActivateTrigger(MilMo_RemotePlayer player, int triggerId)
	{
		if (triggerId < _triggers.Count)
		{
			_triggers[triggerId].Activate(player);
		}
	}

	public void AddObjectEffect(MilMo_ObjectEffect effect)
	{
		if (effect != null)
		{
			_activeEffects.Add(effect);
		}
	}

	public override void Unload()
	{
		base.Unload();
		foreach (MilMo_ObjectEffect activeEffect in _activeEffects)
		{
			activeEffect.Destroy();
		}
		_activeEffects.Clear();
		if (Mover != null)
		{
			Mover.Destroy();
		}
		foreach (MilMo_GameplayTrigger trigger in _triggers)
		{
			trigger.Destroy();
		}
		if (_responsibleForGameObjectDestroy && base.GameObject != null)
		{
			UnityEngine.Object.Destroy(base.GameObject);
		}
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			if (!string.IsNullOrEmpty(_template.VisualRep))
			{
				return false;
			}
			base.GameObject = new GameObject(_template.Name);
			base.GameObject.transform.position = SpawnPosition;
			base.GameObject.transform.rotation = Quaternion.Euler(SpawnRotation);
			base.GameObject.transform.localScale = Scale;
			_responsibleForGameObjectDestroy = true;
		}
		foreach (MilMo_GameplayTrigger trigger in _triggers)
		{
			trigger.GameObjectDone();
		}
		if (Mover != null)
		{
			Mover.SetGameObjectVisualRep(base.VisualRep);
		}
		MilMo_EventSystem.RemoveReaction(_moverChangeDirectionReaction);
		_moverChangeDirectionReaction = null;
		MilMo_EventSystem.RemoveReaction(_moverStartMovingReaction);
		_moverStartMovingReaction = null;
		return true;
	}

	protected override void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (timeOut)
		{
			Debug.LogWarning("Template request timed out when loading template for gameplay object with visualrep " + FullPath);
			return;
		}
		_template = template as MilMo_GameplayObjectTemplate;
		if (_template == null)
		{
			Debug.LogWarning("Gameplay object " + FullPath + " has no template.");
			return;
		}
		for (int i = 0; i < _template.TriggerTemplateReferences.Count; i++)
		{
			MilMo_GameplayTrigger milMo_GameplayTrigger = new MilMo_GameplayTrigger(i, this);
			milMo_GameplayTrigger.Read(_template.TriggerTemplateReferences[i]);
			milMo_GameplayTrigger.LatestActivationByAnyPlayer = _initialLastTriggerActivationTimes[i];
			_triggers.Add(milMo_GameplayTrigger);
		}
		VisualRepName = _template.VisualRep;
		base.FinishRead(template, timeOut: false);
	}

	public void MoverStartMoving(TemplateReference splineRef)
	{
		if (Mover == null)
		{
			Mover = new MilMo_GameplayObjectMover(this, splineRef, 0f);
			Mover.SetGameObjectVisualRep(base.VisualRep);
		}
		else
		{
			Mover.GotStartMovingMessage(splineRef);
		}
	}

	public void MoverChangeDirection(int direction)
	{
		if (Mover != null)
		{
			Mover.GotChangeDirectionMessage(direction);
		}
	}

	private void GotStartMovingMessage(object msgAsObj)
	{
		if (msgAsObj is ServerGameplayObjectStartMoving serverGameplayObjectStartMoving && serverGameplayObjectStartMoving.getObjectId() == base.Id)
		{
			MoverStartMoving(serverGameplayObjectStartMoving.getSpline());
		}
	}

	private void GotChangeDirectionMessage(object msgAsObj)
	{
		if (msgAsObj is ServerGameplayObjectChangeDirection serverGameplayObjectChangeDirection && serverGameplayObjectChangeDirection.getObjectId() == base.Id)
		{
			MoverChangeDirection(serverGameplayObjectChangeDirection.getNewDirection());
		}
	}

	public static string Debug_ToggleDebugMode(string[] args)
	{
		_debugMode = !_debugMode;
		return _debugMode.ToString();
	}
}
