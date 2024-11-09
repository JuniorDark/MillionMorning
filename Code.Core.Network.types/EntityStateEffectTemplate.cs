namespace Code.Core.Network.types;

public class EntityStateEffectTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new EntityStateEffectTemplate(reader);
		}
	}

	private readonly string _effectName;

	private readonly string _effectType;

	private readonly int _stackCount;

	private readonly string _particleEffect;

	private const int TYPE_ID = 14;

	public override int GetTypeId()
	{
		return 14;
	}

	public EntityStateEffectTemplate(MessageReader reader)
		: base(reader)
	{
		_effectName = reader.ReadString();
		_effectType = reader.ReadString();
		_stackCount = reader.ReadInt32();
		_particleEffect = reader.ReadString();
	}

	public EntityStateEffectTemplate(string effectName, string effectType, int stackCount, string particleEffect, string type, TemplateReference reference)
		: base(type, reference)
	{
		_effectName = effectName;
		_effectType = effectType;
		_stackCount = stackCount;
		_particleEffect = particleEffect;
	}

	public string GetEffectName()
	{
		return _effectName;
	}

	public string GetEffectType()
	{
		return _effectType;
	}

	public int GetStackCount()
	{
		return _stackCount;
	}

	public string GetParticleEffect()
	{
		return _particleEffect;
	}

	public override int Size()
	{
		return 10 + base.Size() + MessageWriter.GetSize(_effectName) + MessageWriter.GetSize(_effectType) + MessageWriter.GetSize(_particleEffect);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_effectName);
		writer.WriteString(_effectType);
		writer.WriteInt32(_stackCount);
		writer.WriteString(_particleEffect);
	}
}
