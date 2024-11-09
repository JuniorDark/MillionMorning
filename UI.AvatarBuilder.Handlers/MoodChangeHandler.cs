using Code.World.CharBuilder;
using UI.Elements.PrevNext;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class MoodChangeHandler : PrevNextBaseHandler
{
	protected AvatarEditor AvatarEditor;

	private void Start()
	{
		AvatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
	}

	public override void Handle()
	{
		AvatarEditor.SetMood(Value);
	}
}
