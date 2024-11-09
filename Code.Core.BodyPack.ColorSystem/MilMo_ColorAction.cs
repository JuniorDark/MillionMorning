using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public abstract class MilMo_ColorAction
{
	public string Name { get; protected set; }

	public abstract bool Read(MilMo_SFFile file);

	public abstract void Apply(Material m, int layer);
}
