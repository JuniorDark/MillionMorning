namespace Code.Core.Network.types;

public class DamageToPlayer
{
	private readonly string _playerID;

	private readonly float _healthDamage;

	private readonly float _armorDamage;

	private readonly float _healthLeft;

	public DamageToPlayer(MessageReader reader)
	{
		_playerID = reader.ReadString();
		_healthDamage = reader.ReadFloat();
		_armorDamage = reader.ReadFloat();
		_healthLeft = reader.ReadFloat();
	}

	public DamageToPlayer(string playerID, float healthDamage, float armorDamage, float healthLeft)
	{
		_playerID = playerID;
		_healthDamage = healthDamage;
		_armorDamage = armorDamage;
		_healthLeft = healthLeft;
	}

	public string GetPlayerID()
	{
		return _playerID;
	}

	public float GetHealthDamage()
	{
		return _healthDamage;
	}

	public float GetArmorDamage()
	{
		return _armorDamage;
	}

	public float GetHealthLeft()
	{
		return _healthLeft;
	}

	public int Size()
	{
		return 14 + MessageWriter.GetSize(_playerID);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_playerID);
		writer.WriteFloat(_healthDamage);
		writer.WriteFloat(_armorDamage);
		writer.WriteFloat(_healthLeft);
	}
}
