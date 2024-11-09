using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WallpaperTemplate : HomeSurfaceTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WallpaperTemplate(reader);
		}
	}

	private const int TYPE_ID = 128;

	public override int GetTypeId()
	{
		return 128;
	}

	public WallpaperTemplate(MessageReader reader)
		: base(reader)
	{
	}

	public WallpaperTemplate(string homeSurface, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(homeSurface, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
	}

	public override int Size()
	{
		return base.Size();
	}
}
