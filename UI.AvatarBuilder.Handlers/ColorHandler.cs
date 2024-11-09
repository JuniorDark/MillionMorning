using Code.World.CharBuilder;
using Core.Colors;
using Core.Handler;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public abstract class ColorHandler : BaseHandler
{
	protected ScriptableColor Color;

	protected AvatarEditor AvatarEditor;

	private void Start()
	{
		AvatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
	}

	public void Setup(ScriptableColor color)
	{
		Color = color;
	}
}
