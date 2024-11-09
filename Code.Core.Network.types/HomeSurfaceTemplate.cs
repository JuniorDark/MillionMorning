using System.Collections.Generic;

namespace Code.Core.Network.types;

public class HomeSurfaceTemplate : HomeEquipmentTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new HomeSurfaceTemplate(reader);
		}
	}

	private readonly string _homeSurface;

	private const int TYPE_ID = 91;

	public override int GetTypeId()
	{
		return 91;
	}

	public HomeSurfaceTemplate(MessageReader reader)
		: base(reader)
	{
		_homeSurface = reader.ReadString();
	}

	public HomeSurfaceTemplate(string homeSurface, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_homeSurface = homeSurface;
	}

	public string GetHomeSurface()
	{
		return _homeSurface;
	}

	public override int Size()
	{
		return 2 + base.Size() + MessageWriter.GetSize(_homeSurface);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_homeSurface);
	}
}
