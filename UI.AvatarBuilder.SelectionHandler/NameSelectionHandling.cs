using TMPro;
using UnityEngine;

namespace UI.AvatarBuilder.SelectionHandler;

public class NameSelectionHandling : SelectionHandling
{
	[SerializeField]
	private TMP_InputField nameInput;

	private void Awake()
	{
		if (nameInput == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find nameInput");
		}
	}

	public override void Init()
	{
		base.Init();
		nameInput.text = AvatarEditor.GetCurrentSelection().AvatarName;
	}
}
