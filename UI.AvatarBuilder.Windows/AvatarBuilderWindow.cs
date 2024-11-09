using Core.Avatar;
using UnityEngine;

namespace UI.AvatarBuilder.Windows;

public abstract class AvatarBuilderWindow : MonoBehaviour
{
	protected AvatarEditorAnimations AnimationHandler;

	protected AvatarEditorCamera CameraHandler;

	protected const string DEFAULT = "";

	protected const string EYES = "EYES";

	protected const string MOUTH = "MOUTH";

	protected const string HAIR = "HAIR";

	protected const string SHIRT = "SHIRT";

	protected const string PANTS = "PANTS";

	protected const string SHOES = "SHOES";

	protected virtual void OnEnable()
	{
		if (AnimationHandler == null)
		{
			GetAnimationHandler();
		}
		if (CameraHandler == null)
		{
			GetCameraHandler();
		}
	}

	private void GetCameraHandler()
	{
		CameraHandler = Object.FindObjectOfType<AvatarEditorCamera>();
		if (CameraHandler == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find AvatarEditorCamera");
		}
	}

	private void GetAnimationHandler()
	{
		AnimationHandler = Object.FindObjectOfType<AvatarEditorAnimations>();
		if (AnimationHandler == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find AvatarEditorAnimations");
		}
	}
}
