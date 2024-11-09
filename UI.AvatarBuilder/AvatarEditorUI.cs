using Core.Audio.AudioData;
using Core.Utilities;
using UnityEngine;

namespace UI.AvatarBuilder;

public class AvatarEditorUI : MonoBehaviour
{
	[SerializeField]
	private MusicAudioCueSO music;

	public void Awake()
	{
		ChildrenCleaner[] componentsInChildren = GetComponentsInChildren<ChildrenCleaner>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CleanChildren();
		}
	}

	private void Start()
	{
		music.PlayAudioCue();
	}

	private void OnDestroy()
	{
		music.StopAudioCue();
	}
}
