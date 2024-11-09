using System;
using Code.World.CharBuilder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.AvatarBuilder;

public class GenderSwapButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private AvatarGender gender;

	public event Action<AvatarGender> OnClick;

	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnClick?.Invoke(gender);
	}
}
