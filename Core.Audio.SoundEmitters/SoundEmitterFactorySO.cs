using Core.Factory;
using UnityEngine;

namespace Core.Audio.SoundEmitters;

[CreateAssetMenu(fileName = "NewSoundEmitterFactory", menuName = "Factory/SoundEmitter Factory")]
public class SoundEmitterFactorySO : FactorySO<SoundEmitter>
{
	public SoundEmitter prefab;

	public override SoundEmitter Create()
	{
		return Object.Instantiate(prefab);
	}
}
