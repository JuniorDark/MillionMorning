using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core;

public class SceneManager : MonoBehaviour
{
	[SerializeField]
	private SceneHandle guiScene;

	[SerializeField]
	private SceneHandle avatarEditorScene;

	private void Awake()
	{
		ValidateScenes();
	}

	private void Start()
	{
		LoadGUI();
	}

	private void OnDestroy()
	{
		UnloadGUI();
	}

	private void ValidateScenes()
	{
		if (!guiScene.ValidateAsset())
		{
			Debug.LogError(base.name + ": GUI Scene is not valid!");
		}
		if (!avatarEditorScene.ValidateAsset())
		{
			Debug.LogError(base.name + ": Avatar Editor Scene is not valid!");
		}
	}

	public void LoadGUI()
	{
		LoadScene(guiScene);
	}

	public void UnloadGUI()
	{
		UnloadScene(guiScene);
	}

	public void LaunchAvatarEditor()
	{
		LoadScene(avatarEditorScene);
	}

	public void ExitAvatarEditor()
	{
		UnloadScene(avatarEditorScene);
	}

	private async void LoadScene(SceneHandle sceneHandle)
	{
		SceneInstance? sceneInstance = await sceneHandle.LoadAdditiveAsync();
		if (!sceneInstance.HasValue)
		{
			Debug.LogError("Failed to launch AvatarEditor, sceneInstance is null");
			return;
		}
		Scene scene = sceneInstance.Value.Scene;
		DisableAnyAdditionalEventSystem(scene);
		DisableAnyAdditionalAudioListener(scene);
	}

	private async void UnloadScene(SceneHandle sceneHandle)
	{
		await sceneHandle.UnloadAsync();
	}

	private void DisableAnyAdditionalAudioListener(Scene scene)
	{
		AudioListener[] array = Object.FindObjectsOfType<AudioListener>();
		if (array.Length > 1)
		{
			AudioListener audioListener = array.First((AudioListener c) => c.gameObject.scene == scene);
			if ((bool)audioListener)
			{
				audioListener.enabled = false;
			}
		}
	}

	private void DisableAnyAdditionalEventSystem(Scene scene)
	{
		EventSystem[] array = Object.FindObjectsOfType<EventSystem>();
		if (array.Length > 1)
		{
			EventSystem eventSystem = array.First((EventSystem c) => c.gameObject.scene == scene);
			if ((bool)eventSystem)
			{
				eventSystem.enabled = false;
			}
		}
	}
}
