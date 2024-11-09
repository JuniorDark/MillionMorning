using System;
using Code.Core.Items.Home;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using UnityEngine;

namespace Code.World.Home.FurnitureActions;

public class MilMo_ActionObjectEffect : MilMo_FurnitureStateAction
{
	private string ObjectEffectName { get; }

	public MilMo_ActionObjectEffect(ActionObjectEffect objectEffect)
	{
		ObjectEffectName = objectEffect.GetObjectEffectName();
	}

	public override void Activate<T>(T furniture)
	{
		if (!(furniture is MilMo_HomeFurniture furniture2))
		{
			throw new NotImplementedException("Got non home furniture for object effect action");
		}
		ActivateInternal(furniture2);
	}

	public override void Deactivate<T>(T furniture)
	{
		if (!(furniture is MilMo_HomeFurniture furniture2))
		{
			throw new NotImplementedException("Got non home furniture for object effect action");
		}
		DeactivateInternal(furniture2);
	}

	private void ActivateInternal(MilMo_HomeFurniture furniture)
	{
		MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(furniture.GameObject, ObjectEffectName);
		if (objectEffect == null)
		{
			Debug.LogWarning("Failed to create object effect " + ObjectEffectName + " for furniture " + furniture.Item.Identifier);
		}
		else
		{
			furniture.AddObjectEffect(objectEffect);
		}
	}

	private void DeactivateInternal(MilMo_HomeFurniture furniture)
	{
		furniture.ClearObjectEffects();
	}
}
