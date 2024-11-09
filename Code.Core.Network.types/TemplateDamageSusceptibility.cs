using System.Collections.Generic;

namespace Code.Core.Network.types;

public class TemplateDamageSusceptibility : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new TemplateDamageSusceptibility(reader);
		}
	}

	private readonly IList<Damage> _damages;

	private const int TYPE_ID = 6;

	public override int GetTypeId()
	{
		return 6;
	}

	public TemplateDamageSusceptibility(MessageReader reader)
		: base(reader)
	{
		_damages = new List<Damage>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_damages.Add(new Damage(reader));
		}
	}

	public TemplateDamageSusceptibility(IList<Damage> damages, string type, TemplateReference reference)
		: base(type, reference)
	{
		_damages = damages;
	}

	public IList<Damage> GetDamages()
	{
		return _damages;
	}

	public override int Size()
	{
		int num = 2 + base.Size();
		foreach (Damage damage in _damages)
		{
			num += damage.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_damages.Count);
		foreach (Damage damage in _damages)
		{
			damage.Write(writer);
		}
	}
}
