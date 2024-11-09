using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_AnimationEffectTemplate : MilMo_ObjectEffectTemplate
{
	public WrapMode WrapMode { get; private set; }

	public float CrossFade { get; private set; }

	public float Speed { get; private set; }

	public string AnimationName { get; private set; }

	public string FallbackAnimation { get; private set; }

	public bool AnimateOnlyIfVisible { get; private set; }

	public bool PlayAutomatically { get; private set; }

	public MilMo_AnimationEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		PlayAutomatically = true;
		AnimateOnlyIfVisible = true;
		Speed = 1f;
		CrossFade = 0.3f;
		WrapMode = WrapMode.Loop;
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Name"))
			{
				AnimationName = file.GetString();
			}
			else if (file.IsNext("Crossfade"))
			{
				CrossFade = file.GetFloat();
			}
			else if (file.IsNext("Speed"))
			{
				Speed = file.GetFloat();
			}
			else if (file.IsNext("WrapMode"))
			{
				if (file.IsNext("Loop"))
				{
					WrapMode = WrapMode.Loop;
				}
				else if (file.IsNext("ClampForever"))
				{
					WrapMode = WrapMode.ClampForever;
				}
				else if (file.IsNext("Clamp"))
				{
					WrapMode = WrapMode.Once;
				}
				else if (file.IsNext("Once"))
				{
					WrapMode = WrapMode.Once;
				}
				else if (file.IsNext("PingPong"))
				{
					WrapMode = WrapMode.PingPong;
				}
			}
			else if (file.IsNext("FallbackAnimation"))
			{
				FallbackAnimation = file.GetString();
			}
			else if (file.IsNext("AnimateOnlyIfVisible"))
			{
				AnimateOnlyIfVisible = file.GetBool();
			}
			else if (file.IsNext("PlayAutomatically"))
			{
				PlayAutomatically = file.GetBool();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_AnimationEffect(gameObject, this);
	}
}
