using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_ParticleObject
{
	private readonly GameObject _gameObject;

	public MilMo_ParticleTemplate Template { get; set; }

	public MilMo_ParticleObject(GameObject gameObject)
	{
		_gameObject = gameObject;
	}

	public void SetActive(bool state)
	{
		_gameObject.SetActive(state);
	}

	public GameObject GetGameObject()
	{
		return _gameObject;
	}
}
