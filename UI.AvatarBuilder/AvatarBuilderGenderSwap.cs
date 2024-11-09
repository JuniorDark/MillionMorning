using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder;

public class AvatarBuilderGenderSwap : MonoBehaviour
{
	private AvatarEditor _avatarEditor;

	public GameObject male;

	public GameObject female;

	private void Start()
	{
		_avatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogError(base.name + ": Unable to get AvatarEditor!");
			return;
		}
		AddListeners();
		if (_avatarEditor.IsInitialized())
		{
			UpdateGenderSelection();
		}
	}

	private void AddListeners()
	{
		_avatarEditor.OnInitialized += UpdateGenderSelection;
		_avatarEditor.OnGenderChanged += UpdateGenderSelection;
	}

	private void UpdateGenderSelection()
	{
		if (_avatarEditor.CurrentAvatarHandler != null)
		{
			if (_avatarEditor.CurrentAvatarHandler.GetGender() == AvatarGender.Female)
			{
				male.SetActive(value: false);
				female.SetActive(value: true);
			}
			else
			{
				female.SetActive(value: false);
				male.SetActive(value: true);
			}
		}
	}
}
