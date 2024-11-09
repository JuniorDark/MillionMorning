using UI.FX;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.AvatarBuilder.Headers;

public class AvatarEditorHeader : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler
{
	[SerializeField]
	public AvatarEditorCategory category;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Toggle toggle;

	[Header("Rotation")]
	[SerializeField]
	private GameObject rotatingObject;

	[Header("Scaling")]
	[SerializeField]
	private UIScaleFX scaleFX;

	[SerializeField]
	private UIScaleFXPresetSO scalePreset;

	private void Awake()
	{
		toggle.isOn = false;
	}

	private void Start()
	{
		toggle.onValueChanged.AddListener(OnToggleChange);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		scaleFX.Run(scalePreset);
	}

	public void OnToggleChange(bool value)
	{
		if (value)
		{
			icon.transform.localScale = Vector3.one * 1.1f;
			rotatingObject.SetActive(value: true);
		}
		else
		{
			icon.transform.localScale = Vector3.one;
			rotatingObject.SetActive(value: false);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ToggleOn();
	}

	public void ToggleOn()
	{
		toggle.isOn = true;
	}
}
