using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Code.Core.Items.AbilityEffect;

public abstract class MilMo_AbilityEffectFunction
{
	private static readonly Dictionary<string, string> Variables;

	protected readonly string Variable;

	protected readonly string Operator;

	static MilMo_AbilityEffectFunction()
	{
		Variables = new Dictionary<string, string>();
		Variables.Add("Health", "float");
		Variables.Add("MaxHealth", "float");
		Variables.Add("RunSpeed", "float");
		Variables.Add("WalkSpeed", "float");
		Variables.Add("RunJumpSpeed", "float");
		Variables.Add("WalkJumpSpeed", "float");
		Variables.Add("SwimSpeed", "float");
		Variables.Add("RunAnimation", "string");
	}

	protected MilMo_AbilityEffectFunction(string variable, string op)
	{
		Variable = variable;
		Operator = op;
	}

	public abstract void Prepare();

	public abstract void Activate();

	public abstract void Deactivate();

	public static MilMo_AbilityEffectFunction Create(string variable, string op, string value)
	{
		if (variable.Equals("Nothing", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_AbilityEffectFunctionString("Nothing", null, null);
		}
		if (!Variables.TryGetValue(variable, out var value2))
		{
			Debug.LogWarning("Unknown ability variable '" + variable + "'.");
			return null;
		}
		if (op.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!(value2 == "float"))
			{
				if (value2 == "string")
				{
					return new MilMo_AbilityEffectFunctionString(variable, "reset", null);
				}
				return null;
			}
			return new MilMo_AbilityEffectFunctionFloat(variable, "reset", null);
		}
		if (op != "+" && op != "-" && op != "*" && op != "/" && op != "=" && !op.Equals("var", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Unknown ability operator '" + op + "'.");
			return null;
		}
		if (op.Equals("var", StringComparison.InvariantCultureIgnoreCase))
		{
			if (!Variables.TryGetValue(value, out var value3))
			{
				Debug.LogWarning("Unknown ability variable '" + variable + "'.");
				return null;
			}
			if (value3 != value2)
			{
				Debug.LogWarning("Ability is trying to assign a " + value3 + " to a " + value2 + ".");
				return null;
			}
			if (value2 == "float")
			{
				return new MilMo_AbilityEffectFunctionFloat(variable, op, value);
			}
			if (value2 == "string")
			{
				return new MilMo_AbilityEffectFunctionString(variable, op, value);
			}
		}
		if (!(value2 == "float"))
		{
			if (value2 == "string")
			{
				return new MilMo_AbilityEffectFunctionString(variable, op, value);
			}
			Debug.LogWarning("Unknown ability variable type ''.");
			return null;
		}
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
		numberFormatInfo.NumberDecimalSeparator = ".";
		float value4 = float.Parse(value, numberFormatInfo);
		return new MilMo_AbilityEffectFunctionFloat(variable, op, value4);
	}
}
