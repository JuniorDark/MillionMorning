namespace Code.Core.Items;

public interface IHaveCooldown
{
	float GetTimeRemaining();

	bool TestCooldownExpired();

	float GetCooldownProgress();

	float GetCooldown();
}
