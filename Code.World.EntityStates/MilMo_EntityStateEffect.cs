namespace Code.World.EntityStates;

internal class MilMo_EntityStateEffect
{
	public class EffectTypes
	{
		public const string HEAL = "HEAL";

		public const string HARM = "HARM";

		public const string SPEED = "SPEED";

		public const string JUMP = "JUMP";

		public const string PROTECT = "PROTECT";

		public const string WEAKEN = "WEAKEN";

		public const string STRENGTHEN = "STRENGTHEN";

		public const string REFLECT = "REFLECT";

		public const string SWIM_SPEED = "SWIMSPEED";

		public const string TAUNT = "TAUNT";

		public const string MAX_HEALTH = "MAXHEALTH";
	}

	internal readonly string ParticleEffect;

	private readonly IMilMo_Entity _entity;

	internal int Id { get; }

	internal float Modifier { get; }

	internal string EffectType { get; }

	internal bool IsActive { get; private set; }

	internal bool IsPermanent { get; }

	internal int StackCount { get; }

	internal MilMo_EntityStateEffect(string particleEffect, string type, int id, float modifier, IMilMo_Entity entity, bool isPermanent, int stackCount)
	{
		ParticleEffect = particleEffect;
		_entity = entity;
		Id = id;
		IsPermanent = isPermanent;
		Modifier = modifier;
		EffectType = type.ToUpper();
		IsActive = false;
		StackCount = stackCount;
	}

	internal void Activate()
	{
		if (!IsActive)
		{
			IsActive = true;
			if (_entity != null && !(ParticleEffect == "") && _entity.GetEntityStateManager()?.GetActiveInStatesSum(ParticleEffect) <= StackCount)
			{
				_entity.AddStateEffect(ParticleEffect);
			}
		}
	}

	internal void Deactivate()
	{
		if (IsActive)
		{
			IsActive = false;
			if (_entity != null && !(ParticleEffect == ""))
			{
				_entity.RemoveStateEffect(ParticleEffect);
			}
		}
	}
}
