namespace Code.Core.Network.types;

public class DiscountData
{
	private readonly sbyte _discountPercent;

	private readonly long _discountStart;

	private readonly long _discountEnd;

	public DiscountData(MessageReader reader)
	{
		_discountPercent = reader.ReadInt8();
		_discountStart = reader.ReadInt64();
		_discountEnd = reader.ReadInt64();
	}

	public DiscountData(sbyte discountPercent, long discountStart, long discountEnd)
	{
		_discountPercent = discountPercent;
		_discountStart = discountStart;
		_discountEnd = discountEnd;
	}

	public sbyte GetDiscountPercent()
	{
		return _discountPercent;
	}

	public long GetDiscountStart()
	{
		return _discountStart;
	}

	public long GetDiscountEnd()
	{
		return _discountEnd;
	}

	public int Size()
	{
		return 17;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt8(_discountPercent);
		writer.WriteInt64(_discountStart);
		writer.WriteInt64(_discountEnd);
	}
}
