using System.Collections.Generic;
using System.Linq;
using Code.Core.Config;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public static class MilMo_ParticleContainer
{
	public static readonly bool DevMode = MilMo_Config.Instance.IsTrue("Debug.ParticleSystem", defaultValue: false);

	private static readonly Dictionary<string, MilMo_Particle> Particles = new Dictionary<string, MilMo_Particle>();

	public static GameObject GetParticle(string name, Vector3 position, Quaternion rotation = default(Quaternion))
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		GameObject gameObject;
		if (Particles.TryGetValue(name, out var value))
		{
			if (value == null)
			{
				return null;
			}
			if (rotation == default(Quaternion))
			{
				rotation = value.ParticleObject.GetGameObject().transform.rotation;
			}
			gameObject = Instantiate(value, position, rotation);
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: true);
			}
			return gameObject;
		}
		string text = "Particles/Scripts/" + name;
		value = MilMo_Particle.Load(MilMo_SimpleFormat.LoadLocal(text));
		Particles.Add(name, value);
		if (value == null)
		{
			Debug.Log("Failed to load particle " + text);
			return null;
		}
		value.ParticleObject.GetGameObject().SetActive(value: false);
		if (rotation == default(Quaternion))
		{
			rotation = value.ParticleObject.GetGameObject().transform.rotation;
		}
		gameObject = Instantiate(value, position, rotation);
		if ((bool)gameObject)
		{
			gameObject.SetActive(value: true);
		}
		return gameObject;
	}

	public static void UnloadAll()
	{
		foreach (MilMo_Particle item in Particles.Values.Where((MilMo_Particle particle) => particle != null))
		{
			MilMo_Global.Destroy(item.ParticleObject.GetGameObject());
		}
		Particles.Clear();
	}

	private static GameObject Instantiate(MilMo_Particle particle, Vector3 position, Quaternion rotation)
	{
		return Object.Instantiate(particle.ParticleObject.GetGameObject(), position, rotation);
	}
}
