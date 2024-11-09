namespace Code.Core.Network.types;

public class CollectedInfo
{
	private readonly string _item;

	private readonly sbyte _completed;

	private readonly short _amountToCollect;

	public CollectedInfo(MessageReader reader)
	{
		_item = reader.ReadString();
		_completed = reader.ReadInt8();
		_amountToCollect = reader.ReadInt16();
	}

	public CollectedInfo(string item, sbyte completed, short amountToCollect)
	{
		_item = item;
		_completed = completed;
		_amountToCollect = amountToCollect;
	}

	public string GetItem()
	{
		return _item;
	}

	public sbyte GetCompleted()
	{
		return _completed;
	}

	public short GetAmountToCollect()
	{
		return _amountToCollect;
	}

	public int Size()
	{
		return 5 + MessageWriter.GetSize(_item);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_item);
		writer.WriteInt8(_completed);
		writer.WriteInt16(_amountToCollect);
	}
}
