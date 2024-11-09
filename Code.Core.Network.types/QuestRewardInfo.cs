namespace Code.Core.Network.types;

public class QuestRewardInfo
{
	private readonly string _rewardItemIdentifier;

	private readonly short _amount;

	private readonly sbyte _gender;

	public QuestRewardInfo(MessageReader reader)
	{
		_rewardItemIdentifier = reader.ReadString();
		_amount = reader.ReadInt16();
		_gender = reader.ReadInt8();
	}

	public QuestRewardInfo(string rewardItemIdentifier, short amount, sbyte gender)
	{
		_rewardItemIdentifier = rewardItemIdentifier;
		_amount = amount;
		_gender = gender;
	}

	public string GetRewardItemIdentifier()
	{
		return _rewardItemIdentifier;
	}

	public short GetAmount()
	{
		return _amount;
	}

	public sbyte GetGender()
	{
		return _gender;
	}

	public int Size()
	{
		return 5 + MessageWriter.GetSize(_rewardItemIdentifier);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_rewardItemIdentifier);
		writer.WriteInt16(_amount);
		writer.WriteInt8(_gender);
	}
}
