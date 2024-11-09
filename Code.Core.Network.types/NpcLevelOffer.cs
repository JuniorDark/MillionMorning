namespace Code.Core.Network.types;

public class NpcLevelOffer
{
	private readonly string _fullLevelName;

	private readonly int _priceInGems;

	private readonly sbyte _priceInHelicopterTickets;

	private readonly string _travelSound;

	private readonly string _arriveSound;

	public NpcLevelOffer(MessageReader reader)
	{
		_fullLevelName = reader.ReadString();
		_priceInGems = reader.ReadInt32();
		_priceInHelicopterTickets = reader.ReadInt8();
		_travelSound = reader.ReadString();
		_arriveSound = reader.ReadString();
	}

	public NpcLevelOffer(string fullLevelName, int priceInGems, sbyte priceInHelicopterTickets, string travelSound, string arriveSound)
	{
		_fullLevelName = fullLevelName;
		_priceInGems = priceInGems;
		_priceInHelicopterTickets = priceInHelicopterTickets;
		_travelSound = travelSound;
		_arriveSound = arriveSound;
	}

	public string GetFullLevelName()
	{
		return _fullLevelName;
	}

	public int GetPriceInGems()
	{
		return _priceInGems;
	}

	public sbyte GetPriceInHelicopterTickets()
	{
		return _priceInHelicopterTickets;
	}

	public string GetTravelSound()
	{
		return _travelSound;
	}

	public string GetArriveSound()
	{
		return _arriveSound;
	}

	public int Size()
	{
		return 11 + MessageWriter.GetSize(_fullLevelName) + MessageWriter.GetSize(_travelSound) + MessageWriter.GetSize(_arriveSound);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_fullLevelName);
		writer.WriteInt32(_priceInGems);
		writer.WriteInt8(_priceInHelicopterTickets);
		writer.WriteString(_travelSound);
		writer.WriteString(_arriveSound);
	}
}
