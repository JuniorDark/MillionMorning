using System.Collections.Generic;

namespace Code.Core.Network.types;

public class TemplateCreatureAttack : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new TemplateCreatureAttack(reader);
		}
	}

	private readonly vector3 _offset;

	private readonly IList<string> _attackEffects;

	private readonly IList<string> _preparationEffects;

	private const int TYPE_ID = 5;

	public override int GetTypeId()
	{
		return 5;
	}

	public TemplateCreatureAttack(MessageReader reader)
		: base(reader)
	{
		_offset = new vector3(reader);
		_attackEffects = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_attackEffects.Add(reader.ReadString());
		}
		_preparationEffects = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_preparationEffects.Add(reader.ReadString());
		}
	}

	public TemplateCreatureAttack(vector3 offset, IList<string> attackEffects, IList<string> preparationEffects, string type, TemplateReference reference)
		: base(type, reference)
	{
		_offset = offset;
		_attackEffects = attackEffects;
		_preparationEffects = preparationEffects;
	}

	public vector3 GetOffset()
	{
		return _offset;
	}

	public IList<string> GetAttackEffects()
	{
		return _attackEffects;
	}

	public IList<string> GetPreparationEffects()
	{
		return _preparationEffects;
	}

	public override int Size()
	{
		int num = 16 + base.Size();
		num += (short)(2 * _attackEffects.Count);
		foreach (string attackEffect in _attackEffects)
		{
			num += MessageWriter.GetSize(attackEffect);
		}
		num += (short)(2 * _preparationEffects.Count);
		foreach (string preparationEffect in _preparationEffects)
		{
			num += MessageWriter.GetSize(preparationEffect);
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_offset.Write(writer);
		writer.WriteInt16((short)_attackEffects.Count);
		foreach (string attackEffect in _attackEffects)
		{
			writer.WriteString(attackEffect);
		}
		writer.WriteInt16((short)_preparationEffects.Count);
		foreach (string preparationEffect in _preparationEffects)
		{
			writer.WriteString(preparationEffect);
		}
	}
}
