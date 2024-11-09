using Code.World.CharBuilder;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class HeightSelectionHandler : SelectionHandling
{
	[SerializeField]
	private Slider maleHeightSlider;

	[SerializeField]
	private Slider femaleHeightSlider;

	private const int MIN = 0;

	private const int MAX = 1;

	private const int MALE = 0;

	private const int FEMALE = 1;

	private void Awake()
	{
		if (maleHeightSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find MaleSlider");
		}
		else if (femaleHeightSlider == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find FemaleSlider");
		}
	}

	public override void Init()
	{
		AvatarHandler[] avatarHandlers = AvatarEditor.GetAvatarHandlers();
		float[] heightRange = avatarHandlers[0].GetHeightRange();
		float[] heightRange2 = avatarHandlers[1].GetHeightRange();
		float height = AvatarEditor.GetHeight(AvatarGender.Male);
		float height2 = AvatarEditor.GetHeight(AvatarGender.Female);
		maleHeightSlider.minValue = heightRange[0];
		maleHeightSlider.maxValue = heightRange[1];
		maleHeightSlider.value = height;
		femaleHeightSlider.minValue = heightRange2[0];
		femaleHeightSlider.maxValue = heightRange2[1];
		femaleHeightSlider.value = height2;
	}
}
