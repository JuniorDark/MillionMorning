using System.Collections.Generic;

namespace Code.Core.Network.types;

public class FloorFurnitureTemplate : FurnitureTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new FloorFurnitureTemplate(reader);
		}
	}

	private readonly FurnitureGrid _grid;

	private readonly sbyte _isSmall;

	private readonly sbyte _isCarpet;

	private const int TYPE_ID = 130;

	public override int GetTypeId()
	{
		return 130;
	}

	public FloorFurnitureTemplate(MessageReader reader)
		: base(reader)
	{
		_grid = new FurnitureGrid(reader);
		_isSmall = reader.ReadInt8();
		_isCarpet = reader.ReadInt8();
	}

	public FloorFurnitureTemplate(FurnitureGrid grid, sbyte isSmall, sbyte isCarpet, string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(homePack, isDoor, states, attachNodes, doorEnterSound, doorExitSound, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_grid = grid;
		_isSmall = isSmall;
		_isCarpet = isCarpet;
	}

	public FurnitureGrid GetGrid()
	{
		return _grid;
	}

	public sbyte GetIsSmall()
	{
		return _isSmall;
	}

	public sbyte GetIsCarpet()
	{
		return _isCarpet;
	}

	public override int Size()
	{
		return 2 + base.Size() + _grid.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_grid.Write(writer);
		writer.WriteInt8(_isSmall);
		writer.WriteInt8(_isCarpet);
	}
}
