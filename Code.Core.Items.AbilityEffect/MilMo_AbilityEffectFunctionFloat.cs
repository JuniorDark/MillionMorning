using System;
using Code.Core.Avatar;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Items.AbilityEffect;

public class MilMo_AbilityEffectFunctionFloat : MilMo_AbilityEffectFunction
{
	private float _value;

	private readonly string _varValue;

	private float _resetValue;

	public MilMo_AbilityEffectFunctionFloat(string variable, string op, float value)
		: base(variable, op)
	{
		_value = value;
	}

	public MilMo_AbilityEffectFunctionFloat(string variable, string op, string varValue)
		: base(variable, op)
	{
		_varValue = varValue;
	}

	public override void Prepare()
	{
		if (Operator != null && Operator.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
		{
			switch (Variable)
			{
			case "Health":
				_resetValue = MilMo_Player.Instance.Avatar.Health;
				break;
			case "MaxHealth":
				_resetValue = MilMo_Player.Instance.Avatar.MaxHealth;
				break;
			case "WalkSpeed":
				_resetValue = MilMo_PlayerControllerBase.WalkSpeed;
				break;
			case "RunSpeed":
				_resetValue = MilMo_PlayerControllerBase.RunSpeed;
				break;
			case "WalkJumpSpeed":
				_resetValue = MilMo_PlayerControllerBase.WalkJumpSpeed;
				break;
			case "RunJumpSpeed":
				_resetValue = MilMo_PlayerControllerBase.RunJumpSpeed;
				break;
			case "SwimSpeed":
				_resetValue = MilMo_PlayerControllerBase.SwimSpeed;
				break;
			default:
				Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
				break;
			}
		}
	}

	public override void Activate()
	{
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		if (avatar == null || Variable == "Nothing")
		{
			return;
		}
		float num = 0f;
		switch (Operator)
		{
		case "=":
			num = _value;
			break;
		case "reset":
			num = _resetValue;
			break;
		case "var":
			switch (_varValue)
			{
			case "Health":
				_value = avatar.Health;
				break;
			case "MaxHealth":
				_value = avatar.MaxHealth;
				break;
			case "WalkSpeed":
				_value = MilMo_PlayerControllerBase.WalkSpeed;
				break;
			case "RunSpeed":
				_value = MilMo_PlayerControllerBase.RunSpeed;
				break;
			case "WalkJumpSpeed":
				_value = MilMo_PlayerControllerBase.WalkJumpSpeed;
				break;
			case "RunJumpSpeed":
				_value = MilMo_PlayerControllerBase.RunJumpSpeed;
				break;
			case "SwimSpeed":
				_value = MilMo_PlayerControllerBase.SwimSpeed;
				break;
			default:
				Debug.LogWarning("Unknown variable '" + _varValue + "' in ability effect.");
				return;
			}
			break;
		default:
		{
			float num2;
			switch (Variable)
			{
			case "Health":
				num2 = avatar.Health;
				break;
			case "MaxHealth":
				num2 = avatar.MaxHealth;
				break;
			case "WalkSpeed":
				num2 = MilMo_PlayerControllerBase.WalkSpeed;
				break;
			case "RunSpeed":
				num2 = MilMo_PlayerControllerBase.RunSpeed;
				break;
			case "WalkJumpSpeed":
				num2 = MilMo_PlayerControllerBase.WalkJumpSpeed;
				break;
			case "RunJumpSpeed":
				num2 = MilMo_PlayerControllerBase.RunJumpSpeed;
				break;
			case "SwimSpeed":
				num2 = MilMo_PlayerControllerBase.SwimSpeed;
				break;
			default:
				Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
				return;
			}
			switch (Operator)
			{
			case "+":
				num = num2 + _value;
				break;
			case "-":
				num = num2 - _value;
				break;
			case "*":
				num = num2 * _value;
				break;
			case "/":
				num = num2 / _value;
				break;
			default:
				Debug.LogWarning("Unknown operator '" + Operator + "' in ability effect.");
				return;
			}
			break;
		}
		}
		switch (Variable)
		{
		case "Health":
			_resetValue = avatar.Health;
			avatar.UpdateHealth(num);
			break;
		case "MaxHealth":
			_resetValue = avatar.MaxHealth;
			avatar.UpdateMaxHealth(num);
			break;
		case "WalkSpeed":
			_resetValue = MilMo_PlayerControllerBase.WalkSpeed;
			MilMo_PlayerControllerBase.WalkSpeed = num;
			break;
		case "RunSpeed":
			_resetValue = MilMo_PlayerControllerBase.RunSpeed;
			MilMo_PlayerControllerBase.RunSpeed = num;
			break;
		case "WalkJumpSpeed":
			_resetValue = MilMo_PlayerControllerBase.WalkJumpSpeed;
			MilMo_PlayerControllerBase.WalkJumpSpeed = num;
			break;
		case "RunJumpSpeed":
			_resetValue = MilMo_PlayerControllerBase.RunJumpSpeed;
			MilMo_PlayerControllerBase.RunJumpSpeed = num;
			break;
		case "SwimSpeed":
			_resetValue = MilMo_PlayerControllerBase.SwimSpeed;
			MilMo_PlayerControllerBase.SwimSpeed = num;
			break;
		default:
			Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
			break;
		}
	}

	public override void Deactivate()
	{
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		if (avatar != null && !(Variable == "Nothing"))
		{
			switch (Variable)
			{
			case "Health":
				avatar.UpdateHealth(_resetValue);
				break;
			case "MaxHealth":
				avatar.UpdateMaxHealth(_resetValue);
				break;
			case "WalkSpeed":
				MilMo_PlayerControllerBase.WalkSpeed = _resetValue;
				break;
			case "RunSpeed":
				MilMo_PlayerControllerBase.RunSpeed = _resetValue;
				break;
			case "WalkJumpSpeed":
				MilMo_PlayerControllerBase.WalkJumpSpeed = _resetValue;
				break;
			case "RunJumpSpeed":
				MilMo_PlayerControllerBase.RunJumpSpeed = _resetValue;
				break;
			case "SwimSpeed":
				MilMo_PlayerControllerBase.SwimSpeed = _resetValue;
				break;
			default:
				Debug.LogWarning("Unknown variable '" + Variable + "' in ability effect.");
				break;
			}
		}
	}
}
