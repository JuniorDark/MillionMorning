using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip;

public class MedalTooltip : Tooltip
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text title;

	[SerializeField]
	private TMP_Text description;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Slider progressBar;

	[SerializeField]
	private TMP_Text progressText;

	public override void Show()
	{
		if (!string.IsNullOrEmpty(title.text) && !string.IsNullOrEmpty(description.text))
		{
			base.Show();
		}
	}

	public override void SetData(TooltipData data)
	{
		title.text = data.GetTitle();
		MedalTooltipData medalTooltipData = (MedalTooltipData)data;
		description.text = medalTooltipData.GetDescription();
		float progress = medalTooltipData.GetProgress();
		string text = medalTooltipData.GetProgressText();
		bool flag = (double)Mathf.Abs(progress - 1f) > 0.01;
		if (flag)
		{
			UpdateProgress(0f);
			progressText.text = text;
			float time = (1f - progress) * 0.3f + 0.3f;
			LeanTween.value(base.gameObject, 0f, progress, time).setOnUpdate(UpdateProgress).setEaseOutSine();
		}
		progressText.gameObject.SetActive(flag);
		progressBar.gameObject.SetActive(flag);
		Core.Utilities.UI.SetIcon(icon, medalTooltipData.GetIcon());
	}

	private void UpdateProgress(float val)
	{
		progressBar.value = val;
	}
}
