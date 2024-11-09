namespace Code.Core.Network.types;

public class PlayerInChatRoom
{
	private readonly string _playerId;

	private readonly long _chatroomId;

	private readonly short _sitPointId;

	private readonly sbyte _isOwner;

	public PlayerInChatRoom(MessageReader reader)
	{
		_playerId = reader.ReadString();
		_chatroomId = reader.ReadInt64();
		_sitPointId = reader.ReadInt16();
		_isOwner = reader.ReadInt8();
	}

	public PlayerInChatRoom(string playerId, long chatroomId, short sitPointId, sbyte isOwner)
	{
		_playerId = playerId;
		_chatroomId = chatroomId;
		_sitPointId = sitPointId;
		_isOwner = isOwner;
	}

	public string GetPlayerId()
	{
		return _playerId;
	}

	public long GetChatroomId()
	{
		return _chatroomId;
	}

	public short GetSitPointId()
	{
		return _sitPointId;
	}

	public sbyte GetIsOwner()
	{
		return _isOwner;
	}

	public int Size()
	{
		return 13 + MessageWriter.GetSize(_playerId);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_playerId);
		writer.WriteInt64(_chatroomId);
		writer.WriteInt16(_sitPointId);
		writer.WriteInt8(_isOwner);
	}
}
