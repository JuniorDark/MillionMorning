using System.Collections.Generic;

namespace Code.Core.Network.types;

public class FurnitureState
{
	private readonly int _duration;

	private readonly IList<FurnitureStateAction> _actions;

	public FurnitureState(MessageReader reader)
	{
		_duration = reader.ReadInt32();
		_actions = new List<FurnitureStateAction>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_actions.Add(FurnitureStateAction.Create(reader.ReadTypeCode(), reader));
		}
	}

	public FurnitureState(int duration, IList<FurnitureStateAction> actions)
	{
		_duration = duration;
		_actions = actions;
	}

	public int GetDuration()
	{
		return _duration;
	}

	public IList<FurnitureStateAction> GetActions()
	{
		return _actions;
	}

	public int Size()
	{
		int num = 6;
		num += (short)(_actions.Count * 2);
		foreach (FurnitureStateAction action in _actions)
		{
			num += action.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_duration);
		writer.WriteInt16((short)_actions.Count);
		foreach (FurnitureStateAction action in _actions)
		{
			writer.WriteTypeCode(action.GetTypeId());
			action.Write(writer);
		}
	}
}
