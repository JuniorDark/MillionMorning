using System.Collections.Generic;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.Items.Home;

public sealed class MilMo_FurnitureState
{
	public delegate MilMo_FurnitureStateAction CreateAction(FurnitureStateAction action);

	private static CreateAction _createCallback;

	private IList<MilMo_FurnitureStateAction> Actions { get; set; }

	public static void RegisterActionCreateCallback(CreateAction callback)
	{
		_createCallback = callback;
	}

	public MilMo_FurnitureState(IEnumerable<FurnitureStateAction> actions)
	{
		Actions = new List<MilMo_FurnitureStateAction>();
		if (_createCallback == null)
		{
			return;
		}
		foreach (FurnitureStateAction action in actions)
		{
			MilMo_FurnitureStateAction milMo_FurnitureStateAction = _createCallback(action);
			if (milMo_FurnitureStateAction == null)
			{
				Debug.LogWarning("Failed to create furniture action " + action.GetTemplateType());
			}
			else
			{
				Actions.Add(milMo_FurnitureStateAction);
			}
		}
	}

	public void Activate<T>(T furniture)
	{
		foreach (MilMo_FurnitureStateAction action in Actions)
		{
			action.Activate(furniture);
		}
	}

	public void Deactivate<T>(T furniture)
	{
		foreach (MilMo_FurnitureStateAction action in Actions)
		{
			action.Deactivate(furniture);
		}
	}
}
