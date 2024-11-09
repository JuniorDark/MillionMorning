using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ShopCategory
{
	private readonly string _name;

	private readonly int _id;

	private readonly IList<ShopCategory> _subCategories;

	public ShopCategory(MessageReader reader)
	{
		_name = reader.ReadString();
		_id = reader.ReadInt32();
		_subCategories = new List<ShopCategory>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_subCategories.Add(new ShopCategory(reader));
		}
	}

	public ShopCategory(string name, int id, IList<ShopCategory> subCategories)
	{
		_name = name;
		_id = id;
		_subCategories = subCategories;
	}

	public string GetName()
	{
		return _name;
	}

	public int GetId()
	{
		return _id;
	}

	public IList<ShopCategory> GetSubCategories()
	{
		return _subCategories;
	}

	public int Size()
	{
		int num = 8;
		num += MessageWriter.GetSize(_name);
		foreach (ShopCategory subCategory in _subCategories)
		{
			num += subCategory.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_name);
		writer.WriteInt32(_id);
		writer.WriteInt16((short)_subCategories.Count);
		foreach (ShopCategory subCategory in _subCategories)
		{
			subCategory.Write(writer);
		}
	}
}
