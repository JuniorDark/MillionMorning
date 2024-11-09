namespace Code.Core.Network.types;

public class KilledInfo
{
	private readonly string _creatureDisplayName;

	private readonly string _creatureVisualRep;

	private readonly short _amountToKill;

	private readonly short _amountKilled;

	public KilledInfo(MessageReader reader)
	{
		_creatureDisplayName = reader.ReadString();
		_creatureVisualRep = reader.ReadString();
		_amountToKill = reader.ReadInt16();
		_amountKilled = reader.ReadInt16();
	}

	public KilledInfo(string creatureDisplayName, string creatureVisualRep, short amountToKill, short amountKilled)
	{
		_creatureDisplayName = creatureDisplayName;
		_creatureVisualRep = creatureVisualRep;
		_amountToKill = amountToKill;
		_amountKilled = amountKilled;
	}

	public string GetCreatureDisplayName()
	{
		return _creatureDisplayName;
	}

	public string GetCreatureVisualRep()
	{
		return _creatureVisualRep;
	}

	public short GetAmountToKill()
	{
		return _amountToKill;
	}

	public short GetAmountKilled()
	{
		return _amountKilled;
	}

	public int Size()
	{
		return 8 + MessageWriter.GetSize(_creatureDisplayName) + MessageWriter.GetSize(_creatureVisualRep);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_creatureDisplayName);
		writer.WriteString(_creatureVisualRep);
		writer.WriteInt16(_amountToKill);
		writer.WriteInt16(_amountKilled);
	}
}
