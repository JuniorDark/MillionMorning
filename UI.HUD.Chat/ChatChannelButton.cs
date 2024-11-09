using Core.GameEvent.Types.ChatChannel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Chat;

public class ChatChannelButton : MonoBehaviour
{
	[SerializeField]
	private ChatChannelSO channel;

	[SerializeField]
	private bool startActive;

	[SerializeField]
	private ChangeChatChannelEvent eventToTrigger;

	private Button _button;

	private void Awake()
	{
		if (!channel)
		{
			Debug.LogWarning("ChatChannelTab " + base.gameObject.name + ": Missing channel");
			return;
		}
		if (!eventToTrigger)
		{
			Debug.LogWarning("ChatChannelTab " + base.gameObject.name + ": Missing event");
			return;
		}
		_button = GetComponent<Button>();
		if (!_button)
		{
			Debug.LogWarning("ChatChannelTab " + base.gameObject.name + ": Missing button");
		}
		else
		{
			_button.image.color = channel.tabColor;
		}
	}

	private void Start()
	{
		base.gameObject.SetActive(startActive);
	}

	public void ActivateChannel()
	{
		if ((bool)channel && (bool)eventToTrigger)
		{
			eventToTrigger.Raise(channel);
		}
	}

	public void OnChannelChanged(ChatChannelSO newChannel)
	{
		if ((bool)channel && (bool)_button)
		{
			_button.interactable = channel != newChannel;
		}
	}
}
