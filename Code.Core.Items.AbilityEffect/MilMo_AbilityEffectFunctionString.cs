using System;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Items.AbilityEffect;

public class MilMo_AbilityEffectFunctionString : MilMo_AbilityEffectFunction
{
	private string _value;

	private string _resetValue;

	public MilMo_AbilityEffectFunctionString(string variable, string op, string value)
		: base(variable, op)
	{
		_value = value;
	}

	public override void Prepare()
	{
		if (Operator != null && Operator.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
		{
			if (Variable == "RunAnimation")
			{
				_resetValue = MilMo_PlayerControllerBase.RunAnimation;
			}
			else
			{
				Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
			}
		}
	}

	public override void Activate()
	{
		if (Variable == "Nothing")
		{
			return;
		}
		string runAnimation = "";
		switch (Operator)
		{
		case "=":
			runAnimation = _value;
			break;
		case "reset":
			runAnimation = _resetValue;
			break;
		case "var":
			if (_value == "RunAnimation")
			{
				_value = MilMo_PlayerControllerBase.RunAnimation;
				break;
			}
			Debug.LogWarning("Unknown variable '" + _value + "' in ability effect.");
			return;
		default:
			Debug.LogWarning("Unknown or inapplicable operator '" + Operator + "' in ability effect.");
			return;
		}
		if (Variable == "RunAnimation")
		{
			_resetValue = MilMo_PlayerControllerBase.RunAnimation;
			MilMo_PlayerControllerBase.RunAnimation = runAnimation;
		}
		else
		{
			Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
		}
	}

	public override void Deactivate()
	{
		if (!(Variable == "Nothing"))
		{
			if (Variable == "RunAnimation")
			{
				MilMo_PlayerControllerBase.RunAnimation = _resetValue;
			}
			else
			{
				Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
			}
		}
	}
}
