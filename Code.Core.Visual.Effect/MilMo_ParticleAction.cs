using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_ParticleAction : MilMo_EffectAction
{
	public string PropParticle { get; private set; }

	public string AttachNode { get; private set; }

	public Vector3 Offset { get; private set; }

	public Vector3 Rotation { get; private set; }

	public Vector3 Scale { get; private set; }

	private MilMo_ParticleAction()
	{
		Scale = Vector3.one;
		Rotation = Vector3.zero;
		Offset = Vector3.zero;
		AttachNode = "";
	}

	public override MilMo_SubEffect CreateSubEffect(GameObject parent, float staticYPos)
	{
		return new MilMo_ParticleSubEffect(this, parent, staticYPos);
	}

	public override MilMo_SubEffect CreateSubEffect(GameObject parent, Vector3 dynamicOffset)
	{
		return new MilMo_ParticleSubEffect(this, parent, dynamicOffset);
	}

	public override MilMo_SubEffect CreateSubEffect(Vector3 position)
	{
		return new MilMo_ParticleSubEffect(this, position);
	}

	public new static MilMo_ParticleAction Load(MilMo_SFFile file)
	{
		MilMo_ParticleAction milMo_ParticleAction = new MilMo_ParticleAction
		{
			PropParticle = file.GetString()
		};
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Bone"))
			{
				milMo_ParticleAction.AttachNode = file.GetString();
			}
			else if (file.IsNext("Offset"))
			{
				milMo_ParticleAction.Offset = file.GetVector3();
			}
			else if (file.IsNext("Rotation"))
			{
				milMo_ParticleAction.Rotation = file.GetVector3();
			}
			else if (file.IsNext("Scale"))
			{
				milMo_ParticleAction.Scale = file.GetVector3();
			}
			else
			{
				milMo_ParticleAction.ReadToken(file);
			}
		}
		milMo_ParticleAction.CreateSubEffect(default(Vector3)).Stop();
		return milMo_ParticleAction;
	}
}
