using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network.nexus;
using Code.World.Player;
using Core.Audio.AudioData;
using Core.GameEvent;
using Core.Utilities;
using TMPro;
using UI.HUD.ContextMenu.Options;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ContextMenu;

public class ContextMenu : HudElement
{
	[Header("Objects")]
	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	private Transform optionsContainer;

	[SerializeField]
	private RectTransform rectTransform;

	[Header("Sound")]
	[SerializeField]
	public UIAudioCueSO wrongSound;

	[SerializeField]
	public UIAudioCueSO confirmSound;

	private MilMo_GenericReaction _clickListener;

	private IList<ContextMenuOption> _options;

	private bool _isEnabled;

	public string RemotePlayerId { get; private set; }

	public string RemotePlayerName { get; private set; }

	public bool IsLocalPlayer => RemotePlayerId == MilMo_Player.Instance.Id;

	public event Action OnRebuildContextMenu;

	public void Init(IIdentity id)
	{
		if (_isEnabled)
		{
			RemotePlayerId = id.UserIdentifier.ToString();
			RemotePlayerName = id.Name;
			if (!IsLocalPlayer)
			{
				OnRebuildContextMenu += RebuildContextMenu;
				Show();
			}
		}
	}

	private void Awake()
	{
		Initialize();
		_options = new List<ContextMenuOption>();
		Clear();
		AddButtons();
		GameEvent.ShowContextMenuEvent.RegisterAction(Init);
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		GameEvent.ShowContextMenuEvent.UnregisterAction(Init);
	}

	private void Initialize()
	{
		if (!caption)
		{
			Debug.LogError(base.gameObject.name + ": Could not load caption!");
		}
		else if (!optionsContainer)
		{
			Debug.LogError(base.gameObject.name + ": Could not load optionsContainer!");
		}
		else if (!rectTransform)
		{
			Debug.LogError(base.gameObject.name + ": Could not load rectTransform!");
		}
		else if (!wrongSound)
		{
			Debug.LogError(base.gameObject.name + ": Could not load wrongSound!");
		}
		else if (!confirmSound)
		{
			Debug.LogError(base.gameObject.name + ": Could not load confirmSound!");
		}
	}

	private void Start()
	{
		SetStuff();
	}

	private void SetStuff()
	{
		foreach (ContextMenuOption option in _options)
		{
			option.SetStuff();
		}
	}

	private void Clear()
	{
		for (int num = optionsContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(optionsContainer.transform.GetChild(num).gameObject);
		}
		_options.Clear();
	}

	private void ToggleOptions()
	{
		foreach (ContextMenuOption option in _options)
		{
			option.Toggle();
		}
	}

	private void SetCaption()
	{
		string text = ((MilMo_Player.Instance.Avatar.Role == 0) ? RemotePlayerName : (RemotePlayerName + " (" + RemotePlayerId + ")"));
		caption.text = text;
	}

	private void Show()
	{
		SetCaption();
		ToggleOptions();
		base.transform.SetAsLastSibling();
		base.gameObject.SetActive(value: true);
		this.OnRebuildContextMenu?.Invoke();
	}

	private void RebuildContextMenu()
	{
		StartCoroutine(RebuildContextMenuCoroutine());
	}

	private IEnumerator RebuildContextMenuCoroutine()
	{
		yield return new WaitForFixedUpdate();
		LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
	}

	public void Close()
	{
		RemotePlayerId = null;
		RemotePlayerName = null;
		caption.text = "";
		base.gameObject.SetActive(value: false);
	}

	private void AddButtons()
	{
		AddOption(new ShowProfileContextMenuOption(this));
		AddOption(new TeleportToFriendContextMenuOption(this));
		AddOption(new GiveLeaderContextMenuOption(this));
		AddOption(new InviteToGroupContextMenuOption(this));
		AddOption(new KickFromGroupContextMenuOption(this));
		AddOption(new SendMessageContextMenuOption(this));
		AddOption(new AddFriendContextMenuOption(this));
		AddOption(new VisitHomeContextMenuOption(this));
		AddOption(new BanPlayerContextMenuOption(this));
		AddOption(new UnbanPlayerContextMenuOption(this));
		AddOption(new KickFromHomeContextMenuOption(this));
		AddOption(new RemoveFriendContextMenuOption(this));
	}

	private void AddOption(BaseContextMenuOption option)
	{
		ContextMenuOption contextMenuOption = Instantiator.Instantiate<ContextMenuOption>("ContextMenuOption", optionsContainer);
		if (contextMenuOption == null)
		{
			Debug.LogWarning(base.gameObject.name + ": ContextMenuOption is null");
			return;
		}
		contextMenuOption.Initialize(option, this);
		_options.Add(contextMenuOption);
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		_isEnabled = shouldShow;
	}
}
