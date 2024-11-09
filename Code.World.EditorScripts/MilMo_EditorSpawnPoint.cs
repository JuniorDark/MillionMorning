using UnityEngine;

namespace Code.World.EditorScripts;

[ExecuteInEditMode]
public class MilMo_EditorSpawnPoint : MonoBehaviour
{
	public enum EnableType
	{
		True,
		False,
		OnContact
	}

	public class SpawnObject
	{
		public string category;

		public string path;

		public float probability;

		public string[] spawnEffects;
	}

	public string spawnPointName;

	public bool haveRoamingRadius;

	public float roamingRadius;

	public string walkable;

	public float interval;

	public int objectCount;

	public float height;

	public new EnableType enabled;

	public bool digPoint;

	public bool checkPlayerContact;

	public float playerContactInterval;

	public string[] spawnEffects;

	public string[] removalEffects;

	public bool spawnInWater;

	public bool spawnInCenter;

	public bool snapToGround;

	public bool overrideDefaultLifeSpan;

	public float lifeSpan;

	public string activationTime = "";

	public string deactivationTime = "";

	public string eventTag = "";
}
