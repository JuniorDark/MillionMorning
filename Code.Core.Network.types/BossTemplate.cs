using System.Collections.Generic;

namespace Code.Core.Network.types;

public class BossTemplate : MovableObjectTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new BossTemplate(reader);
		}
	}

	private readonly TemplateReference _idleMode;

	private readonly IList<TemplateReference> _aggroModes;

	private readonly sbyte _noHeathBar;

	private const int TYPE_ID = 75;

	public override int GetTypeId()
	{
		return 75;
	}

	public BossTemplate(MessageReader reader)
		: base(reader)
	{
		_idleMode = new TemplateReference(reader);
		_aggroModes = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_aggroModes.Add(new TemplateReference(reader));
		}
		_noHeathBar = reader.ReadInt8();
	}

	public BossTemplate(TemplateReference idleMode, IList<TemplateReference> aggroModes, sbyte noHeathBar, string visualRep, float maxHealth, float collisionRadius, float impactHeight, float impactRadius, sbyte immobile, IList<string> deathEffectsPhase1, IList<string> deathEffectsPhase2, int level, string displayName, float markerYOffset, string type, TemplateReference reference)
		: base(visualRep, maxHealth, collisionRadius, impactHeight, impactRadius, immobile, deathEffectsPhase1, deathEffectsPhase2, level, displayName, markerYOffset, type, reference)
	{
		_idleMode = idleMode;
		_aggroModes = aggroModes;
		_noHeathBar = noHeathBar;
	}

	public TemplateReference GetIdleMode()
	{
		return _idleMode;
	}

	public IList<TemplateReference> GetAggroModes()
	{
		return _aggroModes;
	}

	public sbyte GetNoHeathBar()
	{
		return _noHeathBar;
	}

	public override int Size()
	{
		int num = 3 + base.Size();
		num += _idleMode.Size();
		foreach (TemplateReference aggroMode in _aggroModes)
		{
			num += aggroMode.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_idleMode.Write(writer);
		writer.WriteInt16((short)_aggroModes.Count);
		foreach (TemplateReference aggroMode in _aggroModes)
		{
			aggroMode.Write(writer);
		}
		writer.WriteInt8(_noHeathBar);
	}
}
