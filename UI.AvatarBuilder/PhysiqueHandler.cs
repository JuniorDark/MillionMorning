using Code.World.CharBuilder;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder;

public class PhysiqueHandler : MonoBehaviour
{
	[SerializeField]
	private Slider boyHeightSliders;

	[SerializeField]
	private Slider girlHeightSliders;

	[SerializeField]
	private GenderSwapButton swapToBoyButton;

	[SerializeField]
	private GenderSwapButton swapToGirlButton;

	private AvatarEditor _avatarEditor;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		_avatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogWarning("Unable to get AvatarEditor!");
		}
		else
		{
			RegisterListeners();
		}
	}

	private void RegisterListeners()
	{
		swapToBoyButton.OnClick += SetGender;
		swapToGirlButton.OnClick += SetGender;
		boyHeightSliders.onValueChanged.AddListener(SetHeight);
		girlHeightSliders.onValueChanged.AddListener(SetHeight);
	}

	public void SetGender(bool male)
	{
		AvatarGender gender = ((!male) ? AvatarGender.Female : AvatarGender.Male);
		SetGender(gender);
	}

	public void SetGender(AvatarGender gender)
	{
		if (CheckLink())
		{
			_avatarEditor.SetGender((int)gender);
		}
	}

	public void SetHeight(float newHeight)
	{
		if (CheckLink())
		{
			_avatarEditor.SetHeight(newHeight);
		}
	}

	private bool CheckLink()
	{
		if (_avatarEditor == null)
		{
			Debug.LogError(base.name + ": Unable to find AvatarEditor");
			return false;
		}
		return true;
	}
}
