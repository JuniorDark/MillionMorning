using System.Collections.Generic;

namespace Code.Core.Network.types;

public class SeatTemplate : FloorFurnitureTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new SeatTemplate(reader);
		}
	}

	private readonly IList<SeatSitNode> _sitNodes;

	private const int TYPE_ID = 143;

	public override int GetTypeId()
	{
		return 143;
	}

	public SeatTemplate(MessageReader reader)
		: base(reader)
	{
		_sitNodes = new List<SeatSitNode>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_sitNodes.Add(new SeatSitNode(reader));
		}
	}

	public SeatTemplate(IList<SeatSitNode> sitNodes, FurnitureGrid grid, sbyte isSmall, sbyte isCarpet, string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(grid, isSmall, isCarpet, homePack, isDoor, states, attachNodes, doorEnterSound, doorExitSound, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_sitNodes = sitNodes;
	}

	public IList<SeatSitNode> GetSitNodes()
	{
		return _sitNodes;
	}

	public override int Size()
	{
		int num = 2 + base.Size();
		foreach (SeatSitNode sitNode in _sitNodes)
		{
			num += sitNode.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_sitNodes.Count);
		foreach (SeatSitNode sitNode in _sitNodes)
		{
			sitNode.Write(writer);
		}
	}
}
