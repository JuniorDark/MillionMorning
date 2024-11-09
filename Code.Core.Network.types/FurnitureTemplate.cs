using System.Collections.Generic;

namespace Code.Core.Network.types;

public class FurnitureTemplate : HomeEquipmentTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new FurnitureTemplate(reader);
		}
	}

	private readonly string _homePack;

	private readonly sbyte _isDoor;

	private readonly IList<FurnitureState> _states;

	private readonly IList<FurnitureAttachNode> _attachNodes;

	private readonly string _doorEnterSound;

	private readonly string _doorExitSound;

	private const int TYPE_ID = 92;

	public override int GetTypeId()
	{
		return 92;
	}

	public FurnitureTemplate(MessageReader reader)
		: base(reader)
	{
		_homePack = reader.ReadString();
		_isDoor = reader.ReadInt8();
		_states = new List<FurnitureState>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_states.Add(new FurnitureState(reader));
		}
		_attachNodes = new List<FurnitureAttachNode>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_attachNodes.Add(new FurnitureAttachNode(reader));
		}
		_doorEnterSound = reader.ReadString();
		_doorExitSound = reader.ReadString();
	}

	public FurnitureTemplate(string homePack, sbyte isDoor, IList<FurnitureState> states, IList<FurnitureAttachNode> attachNodes, string doorEnterSound, string doorExitSound, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_homePack = homePack;
		_isDoor = isDoor;
		_states = states;
		_attachNodes = attachNodes;
		_doorEnterSound = doorEnterSound;
		_doorExitSound = doorExitSound;
	}

	public string GetHomePack()
	{
		return _homePack;
	}

	public sbyte GetIsDoor()
	{
		return _isDoor;
	}

	public IList<FurnitureState> GetStates()
	{
		return _states;
	}

	public IList<FurnitureAttachNode> GetAttachNodes()
	{
		return _attachNodes;
	}

	public string GetDoorEnterSound()
	{
		return _doorEnterSound;
	}

	public string GetDoorExitSound()
	{
		return _doorExitSound;
	}

	public override int Size()
	{
		int num = 11 + base.Size();
		num += MessageWriter.GetSize(_homePack);
		foreach (FurnitureState state in _states)
		{
			num += state.Size();
		}
		num += (short)(_attachNodes.Count * 26);
		num += MessageWriter.GetSize(_doorEnterSound);
		return num + MessageWriter.GetSize(_doorExitSound);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_homePack);
		writer.WriteInt8(_isDoor);
		writer.WriteInt16((short)_states.Count);
		foreach (FurnitureState state in _states)
		{
			state.Write(writer);
		}
		writer.WriteInt16((short)_attachNodes.Count);
		foreach (FurnitureAttachNode attachNode in _attachNodes)
		{
			attachNode.Write(writer);
		}
		writer.WriteString(_doorEnterSound);
		writer.WriteString(_doorExitSound);
	}
}
