namespace Code.Core.Network.types;

public class UpgradableInventoryEntry : InventoryEntry
{
	public new class Factory : InventoryEntry.Factory
	{
		public override InventoryEntry Create(MessageReader reader)
		{
			return new UpgradableInventoryEntry(reader);
		}
	}

	private readonly string _itemTrackIcon;

	private readonly int _currentLevel;

	private readonly TemplateReference _nextLevel;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public UpgradableInventoryEntry(MessageReader reader)
		: base(reader)
	{
		_itemTrackIcon = reader.ReadString();
		_currentLevel = reader.ReadInt32();
		if (reader.ReadInt8() == 1)
		{
			_nextLevel = new TemplateReference(reader);
		}
	}

	public UpgradableInventoryEntry(string itemTrackIcon, int currentLevel, TemplateReference nextLevel, int id, short amount, sbyte equipped, sbyte favorite, Item item)
		: base(id, amount, equipped, favorite, item)
	{
		_itemTrackIcon = itemTrackIcon;
		_currentLevel = currentLevel;
		_nextLevel = nextLevel;
	}

	public string GetItemTrackIcon()
	{
		return _itemTrackIcon;
	}

	public int GetCurrentLevel()
	{
		return _currentLevel;
	}

	public TemplateReference GetNextLevel()
	{
		return _nextLevel;
	}

	public override int Size()
	{
		int num = 7 + base.Size();
		num += MessageWriter.GetSize(_itemTrackIcon);
		if (_nextLevel != null)
		{
			num += _nextLevel.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_itemTrackIcon);
		writer.WriteInt32(_currentLevel);
		if (_nextLevel == null)
		{
			writer.WriteInt8(0);
			return;
		}
		writer.WriteInt8(1);
		_nextLevel.Write(writer);
	}
}
