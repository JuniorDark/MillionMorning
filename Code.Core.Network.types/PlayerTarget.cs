namespace Code.Core.Network.types;

public class PlayerTarget : AttackTarget
{
	public new class Factory : AttackTarget.Factory
	{
		public override AttackTarget Create(MessageReader reader)
		{
			return new PlayerTarget(reader);
		}
	}

	private readonly string _playerId;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public PlayerTarget(MessageReader reader)
		: base(reader)
	{
		_playerId = reader.ReadString();
	}

	public PlayerTarget(string playerId)
	{
		_playerId = playerId;
	}

	public string GetPlayerId()
	{
		return _playerId;
	}

	public override int Size()
	{
		return 2 + MessageWriter.GetSize(_playerId);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_playerId);
	}
}
