using Code.Core.Network.nexus;
using Core.GameEvent;
using TMPro;
using UI.Sprites;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Window.FriendList;

public abstract class UIFriend : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private Image friendPortrait;

	[SerializeField]
	private TMP_Text friendName;

	private IIdentity _identity;

	public virtual void Init(IIdentity identity)
	{
		_identity = identity;
		SetPortrait(_identity.UserIdentifier.ToString());
		SetName(_identity.Name);
		SetupListeners();
	}

	protected virtual void Awake()
	{
		if (friendPortrait == null)
		{
			Debug.LogWarning(base.name + ": friendPortrait missing.");
		}
		else if (friendName == null)
		{
			Debug.LogWarning(base.name + ": friendName missing.");
		}
	}

	private void OnDestroy()
	{
		RemoveListeners();
	}

	protected abstract void SetupListeners();

	protected abstract void RemoveListeners();

	private async void SetPortrait(string id)
	{
		Sprite sprite = await new PortraitSpriteLoader(id).GetSpriteAsync();
		if (!(sprite == null) && !(friendPortrait == null))
		{
			friendPortrait.sprite = sprite;
		}
	}

	private void SetName(string playerName)
	{
		friendName.text = playerName;
	}

	public bool IsChosenIdentity(IIdentity id)
	{
		if (_identity != null)
		{
			return id.UserIdentifier == _identity.UserIdentifier;
		}
		return false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			GameEvent.ShowContextMenuEvent?.RaiseEvent(_identity);
			Debug.LogWarning("RightClicked!");
		}
	}
}
