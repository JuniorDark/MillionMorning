namespace Code.Core.Utility;

public class MilMo_Damage
{
	public string DamageType { get; private set; }

	public float Value { get; private set; }

	public MilMo_Damage(string damageType, float value)
	{
		DamageType = damageType;
		Value = value;
	}
}
