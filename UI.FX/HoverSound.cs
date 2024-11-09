using Core.Audio.AudioData;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.FX;

public class HoverSound : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	[SerializeField]
	private UIAudioCueSO sound;

	private void Awake()
	{
		if (sound == null)
		{
			Debug.LogWarning(base.name + ": I have no sound and therefore no purpose");
			base.enabled = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)sound)
		{
			sound.PlayAudioCue();
		}
	}
}
