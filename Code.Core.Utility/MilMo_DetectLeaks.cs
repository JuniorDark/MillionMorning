using UnityEngine;

namespace Code.Core.Utility;

public class MilMo_DetectLeaks : MonoBehaviour
{
	public void OnGUI()
	{
		UnityEngine.GUI.color = new Color(0.1f, 0.1f, 0.1f);
		GUILayout.Label("All " + Resources.FindObjectsOfTypeAll<Object>().Length);
		GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll<Texture>().Length);
		GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll<AudioClip>().Length);
		GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll<Mesh>().Length);
		GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll<Material>().Length);
		GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll<GameObject>().Length);
		GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll<Component>().Length);
	}
}
