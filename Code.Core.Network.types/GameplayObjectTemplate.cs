using System.Collections.Generic;

namespace Code.Core.Network.types;

public class GameplayObjectTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new GameplayObjectTemplate(reader);
		}
	}

	private readonly string _visualRep;

	private readonly IList<TemplateReference> _triggers;

	private readonly float _shrinkage;

	private readonly long _shrinkDuration;

	private const int TYPE_ID = 10;

	public override int GetTypeId()
	{
		return 10;
	}

	public GameplayObjectTemplate(MessageReader reader)
		: base(reader)
	{
		_visualRep = reader.ReadString();
		_triggers = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_triggers.Add(new TemplateReference(reader));
		}
		_shrinkage = reader.ReadFloat();
		_shrinkDuration = reader.ReadInt64();
	}

	public GameplayObjectTemplate(string visualRep, IList<TemplateReference> triggers, string type, TemplateReference reference, float shrinkage, long shrinkDuration)
		: base(type, reference)
	{
		_visualRep = visualRep;
		_triggers = triggers;
		_shrinkage = shrinkage;
		_shrinkDuration = shrinkDuration;
	}

	public string GetVisualRep()
	{
		return _visualRep;
	}

	public IList<TemplateReference> GetTriggers()
	{
		return _triggers;
	}

	public float GetShrinkage()
	{
		return _shrinkage;
	}

	public long GetShrinkDuration()
	{
		return _shrinkDuration;
	}

	public override int Size()
	{
		int num = 16 + base.Size();
		num += MessageWriter.GetSize(_visualRep);
		foreach (TemplateReference trigger in _triggers)
		{
			num += trigger.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_visualRep);
		writer.WriteInt16((short)_triggers.Count);
		foreach (TemplateReference trigger in _triggers)
		{
			trigger.Write(writer);
		}
		writer.WriteFloat(_shrinkage);
		writer.WriteInt64(_shrinkDuration);
	}
}
