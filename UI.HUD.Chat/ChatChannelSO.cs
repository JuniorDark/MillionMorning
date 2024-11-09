using UnityEngine;

namespace UI.HUD.Chat;

[CreateAssetMenu(fileName = "ChatChannel", menuName = "Chat/Add Chat Channel")]
public class ChatChannelSO : ScriptableObject
{
	public enum MessageChannel
	{
		Standard,
		Group
	}

	public MessageChannel messageChannel;

	public Color inputColor = Color.gray;

	public Color tabColor = Color.gray;
}
