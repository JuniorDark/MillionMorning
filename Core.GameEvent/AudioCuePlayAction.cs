using Core.Audio.AudioData;
using UnityEngine;

namespace Core.GameEvent;

public delegate AudioCueKey AudioCuePlayAction(AudioCueSO audioCue, AudioConfigurationSO audioConfiguration, Vector3 positionInSpace);
