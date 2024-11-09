using System.Collections.Generic;

namespace Code.Core.Network.types;

public class RangedCreatureAttackTemplate : TemplateCreatureAttack
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new RangedCreatureAttackTemplate(reader);
		}
	}

	private readonly TemplateReference _projectile;

	private const int TYPE_ID = 77;

	public override int GetTypeId()
	{
		return 77;
	}

	public RangedCreatureAttackTemplate(MessageReader reader)
		: base(reader)
	{
		_projectile = new TemplateReference(reader);
	}

	public RangedCreatureAttackTemplate(TemplateReference projectile, vector3 offset, IList<string> attackEffects, IList<string> preparationEffects, string type, TemplateReference reference)
		: base(offset, attackEffects, preparationEffects, type, reference)
	{
		_projectile = projectile;
	}

	public TemplateReference GetProjectile()
	{
		return _projectile;
	}

	public override int Size()
	{
		return base.Size() + _projectile.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_projectile.Write(writer);
	}
}
