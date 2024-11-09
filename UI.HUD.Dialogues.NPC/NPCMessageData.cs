using System.Collections.Generic;
using Localization;

namespace UI.HUD.Dialogues.NPC;

public class NPCMessageData
{
	private readonly NpcMessageTypes? _messageType;

	private readonly int _npcId;

	private readonly string _actorName;

	private readonly string _portraitKey;

	private readonly string _voicePath;

	private readonly List<LocalizedStringWithArgument> _messages;

	public NPCMessageData(NpcMessageTypes? messageType, int npcId, string actorName, string portraitKey, string voicePath, List<LocalizedStringWithArgument> messages)
	{
		_messageType = messageType;
		_npcId = npcId;
		_actorName = actorName;
		_portraitKey = portraitKey;
		_voicePath = voicePath;
		_messages = messages;
	}

	public NpcMessageTypes? GetMessageType()
	{
		return _messageType;
	}

	public int GetNpcId()
	{
		return _npcId;
	}

	public string GetActorName()
	{
		return _actorName;
	}

	public string GetPortraitKey()
	{
		return _portraitKey;
	}

	public string GetVoicePath()
	{
		return _voicePath;
	}

	public List<LocalizedStringWithArgument> GetMessages()
	{
		return _messages;
	}
}
