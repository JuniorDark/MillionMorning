using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WallFurnitureTemplate : FurnitureTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WallFurnitureTemplate(reader);
		}
	}

	private readonly sbyte _width;

	private readonly float _pivot;

	private readonly sbyte _isCurtain;

	private const int TYPE_ID = 131;

	public override int GetTypeId()
	{
		return 131;
	}

	public WallFurnitureTemplate(MessageReader reader)
		: base(reader)
	{
		_width = reader.ReadInt8();
		_pivot = reader.ReadFloat();
		_isCurtain = reader.ReadInt8();
	}

	public WallFurnitureTemplate(sbyte width, float pivot, sbyte isCurtain, string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(homePack, isDoor, states, attachNodes, doorEnterSound, doorExitSound, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_width = width;
		_pivot = pivot;
		_isCurtain = isCurtain;
	}

	public sbyte GetWidth()
	{
		return _width;
	}

	public float GetPivot()
	{
		return _pivot;
	}

	public sbyte GetIsCurtain()
	{
		return _isCurtain;
	}

	public override int Size()
	{
		return 6 + base.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt8(_width);
		writer.WriteFloat(_pivot);
		writer.WriteInt8(_isCurtain);
	}
}
