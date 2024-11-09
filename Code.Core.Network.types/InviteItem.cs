namespace Code.Core.Network.types;

public class InviteItem
{
	private readonly Item _item;

	private readonly sbyte _gender;

	private readonly int _amount;

	private readonly int _invitesNeeded;

	public InviteItem(MessageReader reader)
	{
		_item = Item.Create(reader.ReadTypeCode(), reader);
		_gender = reader.ReadInt8();
		_amount = reader.ReadInt32();
		_invitesNeeded = reader.ReadInt32();
	}

	public InviteItem(Item item, sbyte gender, int amount, int invitesNeeded)
	{
		_item = item;
		_gender = gender;
		_amount = amount;
		_invitesNeeded = invitesNeeded;
	}

	public Item GetItem()
	{
		return _item;
	}

	public sbyte GetGender()
	{
		return _gender;
	}

	public int GetAmount()
	{
		return _amount;
	}

	public int GetInvitesNeeded()
	{
		return _invitesNeeded;
	}

	public int Size()
	{
		return 11 + _item.Size();
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteTypeCode(_item.GetTypeId());
		_item.Write(writer);
		writer.WriteInt8(_gender);
		writer.WriteInt32(_amount);
		writer.WriteInt32(_invitesNeeded);
	}
}
