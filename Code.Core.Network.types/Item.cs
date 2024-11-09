using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Item
{
	public class Factory
	{
		public virtual Item Create(MessageReader reader)
		{
			return new Item(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly TemplateReference _template;

	private readonly IList<string> _modifiers;

	private const int TYPE_ID = 0;

	static Item()
	{
		ChildFactories = new Factory[3];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new HomeEquipment.Factory();
		ChildFactories[2] = new Furniture.Factory();
	}

	public static Item Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public Item(MessageReader reader)
	{
		_template = new TemplateReference(reader);
		_modifiers = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_modifiers.Add(reader.ReadString());
		}
	}

	public Item(TemplateReference template, IList<string> modifiers)
	{
		_template = template;
		_modifiers = modifiers;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public IList<string> GetModifiers()
	{
		return _modifiers;
	}

	public virtual int Size()
	{
		int num = 2;
		num += _template.Size();
		num += (short)(2 * _modifiers.Count);
		foreach (string modifier in _modifiers)
		{
			num += MessageWriter.GetSize(modifier);
		}
		return num;
	}

	public virtual void Write(MessageWriter writer)
	{
		_template.Write(writer);
		writer.WriteInt16((short)_modifiers.Count);
		foreach (string modifier in _modifiers)
		{
			writer.WriteString(modifier);
		}
	}
}
