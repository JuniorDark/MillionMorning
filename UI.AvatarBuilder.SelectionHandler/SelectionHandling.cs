using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.SelectionHandler;

public abstract class SelectionHandling : MonoBehaviour, ISelection
{
	protected AvatarEditor AvatarEditor;

	protected virtual void Start()
	{
		AvatarEditor = Object.FindObjectOfType<AvatarEditor>();
		AvatarEditorCheck();
	}

	private bool AvatarEditorCheck()
	{
		if (AvatarEditor != null)
		{
			return true;
		}
		Debug.LogError(base.gameObject.name + ": Unable to find AvatarEditor");
		return false;
	}

	protected AvatarHandler GetHandler(AvatarGender gender)
	{
		if (!AvatarEditorCheck())
		{
			return null;
		}
		return AvatarEditor.GetAvatarHandlers()[(int)gender];
	}

	public virtual void Init()
	{
	}

	public virtual void GenderSwitch()
	{
	}

	public virtual void ColorChanged(AvatarSelection.Shapes shape)
	{
	}
}
