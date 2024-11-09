using Code.World.Level.LevelObject;

namespace Code.World.Attack;

public abstract class MilMo_Attack
{
	protected bool _isHit;

	public bool IsHit => _isHit;

	public abstract float TargetRadius { get; }

	public abstract float TargetSqrRadius { get; }

	protected MilMo_Attack(bool isHit)
	{
		_isHit = isHit;
	}

	public virtual void Resolve()
	{
	}

	public virtual void Resolve(MilMo_LevelProjectile projectile)
	{
	}
}
