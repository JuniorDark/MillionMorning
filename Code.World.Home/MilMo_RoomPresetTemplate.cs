using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;

namespace Code.World.Home;

public class MilMo_RoomPresetTemplate : MilMo_Template
{
	private IList<HomeEquipment> _equipment;

	public TemplateReference RoomTemplateReference { get; private set; }

	public HomeEquipment Door { get; private set; }

	public IEnumerable<HomeEquipment> Equipment => _equipment;

	public override bool LoadFromNetwork(Template t)
	{
		RoomPresetTemplate roomPresetTemplate = t as RoomPresetTemplate;
		base.LoadFromNetwork(roomPresetTemplate);
		if (roomPresetTemplate == null)
		{
			return true;
		}
		RoomTemplateReference = roomPresetTemplate.GetRoomTemplate();
		Door = roomPresetTemplate.GetDoor();
		_equipment = roomPresetTemplate.GetEquipment();
		return true;
	}

	public static MilMo_RoomPresetTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RoomPresetTemplate(category, path, filePath, "RoomPreset");
	}

	protected MilMo_RoomPresetTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}
}
