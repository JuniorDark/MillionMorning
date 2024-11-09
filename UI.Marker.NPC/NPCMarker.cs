using System;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core.Interaction;
using Core.Utilities;
using TMPro;
using UI.FX;
using UI.Marker.States;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.Marker.NPC;

public class NPCMarker : Marker
{
	private const float MAX_TEXT_DISTANCE_SQR = 225f;

	public static string AddressableAddress = "NPCMarker";

	[Header("NPCMarker")]
	[SerializeField]
	private Image markerIcon;

	[SerializeField]
	private TMP_Text markerText;

	[Header("Assets")]
	public AssetReferenceSprite activeQuestMarkerAsset;

	public AssetReferenceSprite nextQuestMarkerAsset;

	public AssetReferenceSprite completeQuestMarkerAsset;

	public AssetReferenceSprite shopMarkerAsset;

	public AssetReferenceSprite travelMarkerAsset;

	private MarkerState _currentState;

	private float _distanceToNPCSqr;

	private MilMo_LocString _npcName;

	private Transform _player;

	private Vector3 _npcToPlayerNormalized;

	private bool _visible;

	private bool _inRange;

	private UIAlphaFX _fader;

	private int _textAnimation;

	public Sprite ActiveQuestMarkerSprite { get; private set; }

	public Sprite NextQuestMarkerSprite { get; private set; }

	public Sprite CompleteQuestMarkerSprite { get; private set; }

	public Sprite ShopMarkerSprite { get; private set; }

	public Sprite TravelMarkerSprite { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (!InitializeAddressables())
		{
			Debug.LogError(base.gameObject.name + ": Unable to load Addressables for NPCMarker");
			return;
		}
		if (markerIcon == null)
		{
			Debug.LogWarning(base.gameObject.name + ": markerIcon is null");
			return;
		}
		if (markerText == null)
		{
			Debug.LogWarning(base.gameObject.name + ": markerText is null");
			return;
		}
		markerText.gameObject.transform.localScale = Vector3.zero;
		_fader = GetComponent<UIAlphaFX>();
		if (_fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _fader is missing");
		}
		else
		{
			_fader.FadeOutFast();
		}
	}

	protected void Update()
	{
		RefreshVisible();
		RefreshDistance();
	}

	public void Initialize(Transform player, IHasInteraction interactableObject, MilMo_LocString npcName, float interactionRadius, byte interactionState)
	{
		if (interactableObject == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		_player = player;
		base.transform.position = interactableObject.GetPosition();
		InitializeState();
		UpdateState(interactionState);
		SetMarkerText(npcName);
		SetMarkerOffset(interactableObject.GetMarkerOffset());
		interactable.Initialize(interactableObject, interactionRadius);
		base.Initialize();
	}

	private bool InitializeAddressables()
	{
		if (!AddressablesRuntimeCheck())
		{
			return false;
		}
		if (!LoadAddressables())
		{
			return false;
		}
		return true;
	}

	private bool LoadAddressables()
	{
		ActiveQuestMarkerSprite = activeQuestMarkerAsset.LoadAssetAsync<Sprite>().WaitForCompletion();
		if (!ActiveQuestMarkerSprite)
		{
			return false;
		}
		NextQuestMarkerSprite = nextQuestMarkerAsset.LoadAssetAsync<Sprite>().WaitForCompletion();
		if (!NextQuestMarkerSprite)
		{
			return false;
		}
		CompleteQuestMarkerSprite = completeQuestMarkerAsset.LoadAssetAsync<Sprite>().WaitForCompletion();
		if (!CompleteQuestMarkerSprite)
		{
			return false;
		}
		ShopMarkerSprite = shopMarkerAsset.LoadAssetAsync<Sprite>().WaitForCompletion();
		if (!ShopMarkerSprite)
		{
			return false;
		}
		TravelMarkerSprite = travelMarkerAsset.LoadAssetAsync<Sprite>().WaitForCompletion();
		if (!TravelMarkerSprite)
		{
			return false;
		}
		return true;
	}

	private bool AddressablesRuntimeCheck()
	{
		if (!activeQuestMarkerAsset.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing addressable sprite!");
			return false;
		}
		if (!nextQuestMarkerAsset.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing addressable sprite!");
			return false;
		}
		if (!completeQuestMarkerAsset.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing addressable sprite!");
			return false;
		}
		if (!shopMarkerAsset.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing addressable sprite!");
			return false;
		}
		if (!travelMarkerAsset.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing addressable sprite!");
			return false;
		}
		return true;
	}

	private void RefreshVisible()
	{
		MilMo_Player instance = MilMo_Player.Instance;
		bool flag = instance != null && !instance.IsTalking;
		if (_visible != flag)
		{
			_visible = flag;
			if (_visible)
			{
				_fader.FadeIn();
			}
			else
			{
				_fader.FadeOut();
			}
		}
	}

	private void RefreshDistance()
	{
		Vector3 position = base.transform.position;
		float sqrMagnitude = (((_player != null) ? _player.position : position) - position).sqrMagnitude;
		if (Math.Abs(_distanceToNPCSqr - sqrMagnitude) > 0.001f)
		{
			_distanceToNPCSqr = sqrMagnitude;
			bool shouldEnable = Maths.InsideDistanceSqr(_distanceToNPCSqr, 225f);
			ShowText(shouldEnable);
		}
	}

	private void ShowText(bool shouldEnable)
	{
		if (!(markerText == null))
		{
			if (_textAnimation != 0)
			{
				LeanTween.cancel(_textAnimation);
				_textAnimation = 0;
			}
			_textAnimation = LeanTween.scale(markerText.gameObject, shouldEnable ? Vector3.one : Vector3.zero, 0.2f).setOnComplete((Action)delegate
			{
				_textAnimation = 0;
			}).id;
		}
	}

	public void ChangeMarkerIcon(Sprite sprite)
	{
		if (!(markerIcon == null))
		{
			if (sprite == null)
			{
				markerIcon.enabled = false;
				return;
			}
			markerIcon.enabled = true;
			markerIcon.sprite = sprite;
		}
	}

	public void ShowIcon(bool shouldEnable)
	{
		if (markerIcon != null && markerIcon.enabled != shouldEnable)
		{
			markerIcon.enabled = shouldEnable;
		}
	}

	public void SetMarkerText(MilMo_LocString text)
	{
		if (markerText != null)
		{
			markerText.text = text?.String;
		}
	}

	private void InitializeState()
	{
		_currentState = new InitialState(this);
	}

	public void SetState(MarkerState state)
	{
		_currentState = state;
		StartCoroutine(_currentState.Start());
	}

	public void UpdateState(byte interactionState)
	{
		switch (interactionState)
		{
		case 0:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.HasNextQuest());
			break;
		}
		case 1:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.QuestActive());
			break;
		}
		case 2:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.QuestToFinish());
			break;
		}
		case 3:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.NoMoreQuests());
			break;
		}
		case 4:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.InShop());
			break;
		}
		case 5:
		{
			Coroutine coroutine = StartCoroutine(_currentState?.Travel());
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("interactionState", interactionState, null);
		}
	}
}
