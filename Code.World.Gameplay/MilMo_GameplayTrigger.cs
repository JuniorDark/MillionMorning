using System.Collections.Generic;
using Code.Core.Collision;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Level;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Interaction;
using Localization;
using UI;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Marker;
using UI.Marker.Exit;
using UI.Marker.Object;
using UI.Sprites;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayTrigger : IHasInteraction
{
	public const float MIN_TIME_BETWEEN_ACTIVATION_REQUESTS = 0.5f;

	private static bool _isShowingMembersOnlyInfo;

	private Vector3 _worldPosition = Vector3.zero;

	private MilMo_Volume _volume;

	private readonly MilMo_GameplayObject _gameplayObject;

	private readonly List<MilMo_GameplayTriggerReaction> _reactions = new List<MilMo_GameplayTriggerReaction>();

	private float _latestActivationByLocalPlayer = -100000f;

	private float _latestActivationByAnyPlayer = -100000f;

	private bool _playerIsInside;

	private bool _mayShowLockedThinkBubble = true;

	private Marker _marker;

	private float _markerYOffset = 0.5f;

	public MilMo_GameplayTriggerTemplate Template { get; private set; }

	public int Id { get; }

	public float LatestActivationRequest { get; set; }

	public float LatestActivationByAnyPlayer
	{
		set
		{
			_latestActivationByAnyPlayer = value;
		}
	}

	private bool IsContactTrigger()
	{
		return Template.ActivationEvent == 0;
	}

	private bool IsSilent()
	{
		return IsContactTrigger();
	}

	public MilMo_GameplayTrigger(int id, MilMo_GameplayObject gameplayObject)
	{
		Id = id;
		_gameplayObject = gameplayObject;
	}

	public void Read(TemplateReference template)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(template, FinishRead);
	}

	public void GameObjectDone()
	{
		SetWorldPosition();
	}

	public bool ShouldActivate(int evt)
	{
		if (!_playerIsInside)
		{
			return false;
		}
		if (Template == null)
		{
			return false;
		}
		if (Template.MembersOnly && !MilMo_Player.Instance.IsMember)
		{
			return false;
		}
		if (Template.ActivationCheck == 2 || Template.ActivationEvent != evt)
		{
			return false;
		}
		if (Time.time - _latestActivationByLocalPlayer <= Template.CooldownPerPlayer)
		{
			return false;
		}
		if (Time.time - _latestActivationByAnyPlayer <= Template.Cooldown)
		{
			return false;
		}
		foreach (MilMo_GameplayTriggerReaction reaction in _reactions)
		{
			if (!reaction.MayActivate())
			{
				return false;
			}
		}
		if (Template.Key != null && !MilMo_Player.Instance.Inventory.HaveItem(Template.Key.Identifier))
		{
			if (_mayShowLockedThinkBubble && Template.NoKeyThink.Length > 0)
			{
				GameEvent.ThinkEvent?.RaiseEvent(Template.NoKeyThink.String);
				_mayShowLockedThinkBubble = false;
			}
			return false;
		}
		if (!string.IsNullOrEmpty(Template.CaptureKey) && !MilMo_Level.CurrentLevel.PlayerHasCapturedObject(Template.CaptureKey))
		{
			if (_mayShowLockedThinkBubble && Template.NoCaptureKeyThink.Length > 0)
			{
				GameEvent.ThinkEvent?.RaiseEvent(Template.NoCaptureKeyThink.String);
				_mayShowLockedThinkBubble = false;
			}
			return false;
		}
		if (MilMo_Player.Instance.AvatarLevel < Template.RequiredAvatarLevel && _mayShowLockedThinkBubble)
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("World_13555");
			locString.SetFormatArgs(Template.RequiredAvatarLevel);
			GameEvent.ThinkEvent?.RaiseEvent(locString.String);
			_mayShowLockedThinkBubble = false;
		}
		return !_gameplayObject.IsCaptured;
	}

	public void UpdatePlayerDistance(Vector3 playerPosition)
	{
		UpdatePlayerInside(playerPosition);
	}

	public void Destroy()
	{
		if (_marker != null)
		{
			_marker.Remove();
		}
		MilMo_GameplayTriggerTemplate template = Template;
		if (template != null && template.HasExitArrow && MilMo_Level.CurrentLevel != null)
		{
			MilMo_Level.CurrentLevel.OnUsedExitsUpdated -= GotUsedExitsInfo;
		}
	}

	private void UpdatePlayerInside(Vector3 playerPosition)
	{
		if (_volume == null)
		{
			_playerIsInside = false;
			return;
		}
		_playerIsInside = (_volume.IsDynamicVolume() ? _volume.IsInsideDynamic(playerPosition, _gameplayObject) : _volume.IsInside(playerPosition));
		if (!_playerIsInside && !_mayShowLockedThinkBubble)
		{
			_mayShowLockedThinkBubble = true;
		}
	}

	public void UpdatePosition(vector3 position)
	{
		_volume.SetPosition(position);
	}

	public void Activate(MilMo_Player player)
	{
		if (Template.HasExitArrow)
		{
			MarkExitAsUsed();
		}
		_latestActivationByLocalPlayer = Time.time;
		_latestActivationByAnyPlayer = Time.time;
		foreach (MilMo_GameplayTriggerReaction reaction in _reactions)
		{
			reaction.Activate(_gameplayObject, player);
		}
	}

	public void Activate(MilMo_RemotePlayer player)
	{
		_latestActivationByAnyPlayer = Time.time;
		foreach (MilMo_GameplayTriggerReaction reaction in _reactions)
		{
			reaction.Activate(_gameplayObject, player);
		}
	}

	private void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (timeOut)
		{
			return;
		}
		Template = template as MilMo_GameplayTriggerTemplate;
		if (Template == null)
		{
			Debug.LogWarning("Failed to load gameplay trigger template.");
			return;
		}
		foreach (GameplayTriggerReaction reaction2 in Template.Reactions)
		{
			MilMo_GameplayTriggerReaction reaction = MilMo_GameplayTriggerReaction.GetReaction(reaction2);
			if (reaction != null)
			{
				_reactions.Add(reaction);
			}
			if (reaction is MilMo_GameplayTriggerReactionMover)
			{
				_gameplayObject.IsMovingPlatform = true;
			}
		}
		if (Template.HasExitArrow)
		{
			MilMo_Level.CurrentLevel.OnUsedExitsUpdated += GotUsedExitsInfo;
		}
		SetWorldPosition();
	}

	private void SetWorldPosition()
	{
		if (_gameplayObject == null || _gameplayObject.GameObject == null)
		{
			return;
		}
		if (Template == null)
		{
			_worldPosition = _gameplayObject.StartPosition;
			return;
		}
		Vector3 vector = _gameplayObject.StartPosition - _gameplayObject.GameObject.transform.position;
		_worldPosition = _gameplayObject.GameObject.transform.TransformPoint(Template.Offset) + vector;
		foreach (MilMo_GameplayTriggerReaction reaction in _reactions)
		{
			reaction.SetPositionAndRoom(_worldPosition, _gameplayObject.Room);
		}
		GameObject gameObject = new GameObject("CollisionVolume");
		gameObject.transform.position = _worldPosition;
		gameObject.transform.parent = _gameplayObject.GameObject.transform;
		gameObject.transform.localEulerAngles = Template.RotationOffset;
		_volume = Template.VolumeTemplate.Instantiate(gameObject.transform);
		if (!(_volume is MilMo_Sphere milMo_Sphere))
		{
			return;
		}
		float radius = milMo_Sphere.GetRadius();
		if (Template.HasExitArrow)
		{
			Vector3 worldPosition = _worldPosition;
			_markerYOffset = Template.ExitArrowYOffset;
			CreateExitMarker(radius, worldPosition);
			bool shouldDisable = IsExitUsed();
			DisableExitMarker(shouldDisable);
			if ((worldPosition - MilMo_Level.CurrentLevel.EntryPoint).magnitude < 30f)
			{
				MarkExitAsUsed();
			}
		}
		else
		{
			CreateObjectMarker(radius);
		}
	}

	private void CreateObjectMarker(float radius)
	{
		MilMo_LocString objectName = new MilMo_LocString("", removeTags: false);
		ObjectMarker worldSpaceObject = WorldSpaceManager.GetWorldSpaceObject<ObjectMarker>(ObjectMarker.AddressableAddress);
		worldSpaceObject.Initialize(this, objectName, radius, IsSilent(), IsContactTrigger());
		_marker = worldSpaceObject;
	}

	private void GotUsedExitsInfo()
	{
		bool shouldDisable = IsExitUsed();
		DisableExitMarker(shouldDisable);
	}

	private bool IsExitUsed()
	{
		return MilMo_Level.CurrentLevel.UsedExits.Contains(Template.ExitArrowIdentifier);
	}

	private void MarkExitAsUsed()
	{
		DisableExitMarker(shouldDisable: true);
		Singleton<GameNetwork>.Instance.SendLevelExitUsed(MilMo_Level.CurrentLevel.VerboseName, Template.ExitArrowIdentifier);
	}

	private void CreateExitMarker(float radius, Vector3 exitArrowPosition)
	{
		ExitMarker worldSpaceObject = WorldSpaceManager.GetWorldSpaceObject<ExitMarker>(ExitMarker.AddressableAddress);
		worldSpaceObject.Initialize(this, exitArrowPosition, radius, IsSilent(), IsContactTrigger());
		_marker = worldSpaceObject;
	}

	private void DisableExitMarker(bool shouldDisable)
	{
		if (_marker != null)
		{
			_marker.Disable(IsContactTrigger() && shouldDisable);
		}
	}

	public int GetPrio()
	{
		return 1;
	}

	public void UseReaction()
	{
		if (ShouldActivate(1))
		{
			if (Template.ActivationCheck == 1)
			{
				LatestActivationRequest = Time.time;
				Singleton<GameNetwork>.Instance.RequestActivateGameplayTrigger(_gameplayObject.Id, Id);
			}
			else
			{
				Activate(MilMo_Player.Instance);
			}
		}
		else
		{
			MilMo_GameplayTriggerTemplate template = Template;
			if (template != null && template.MembersOnly && !MilMo_Player.Instance.IsMember && !_isShowingMembersOnlyInfo)
			{
				DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_467"), new LocalizedStringWithArgument("World_468"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
			}
		}
	}

	public Vector3 GetPosition()
	{
		return _worldPosition;
	}

	public Vector3 GetMarkerOffset()
	{
		return new Vector3(0f, _markerYOffset, 0f);
	}

	public string GetInteractionVerb()
	{
		return ((Template.MembersOnly && !MilMo_Player.Instance.IsMember) ? MilMo_Localization.GetLocString("Interact_Read") : Template.ActivationVerb).String;
	}

	public Interactable.InteractionType GetInteractionType()
	{
		return Interactable.InteractionType.Silver;
	}

	public void Debug_Draw()
	{
		if (_volume != null)
		{
			_volume.Debug_Draw();
		}
	}
}
