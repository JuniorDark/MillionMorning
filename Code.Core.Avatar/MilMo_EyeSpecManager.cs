using Code.Core.Emote;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_EyeSpecManager
{
	private static float _maxOffsetTurn = 0.02f;

	private static float _maxOffsetUp = 0.01f;

	private static float _minOffsetTurn = -0.02f;

	private static float _minOffsetUp = -0.01f;

	private static float _angleUVScale = 0.05f;

	private const string FOLLOW_BONE = "Root";

	private const string REFERENCE_BONE = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip";

	private MilMo_EmoteManager _emoteManager;

	private Transform _followTransform;

	private Transform _referenceTransform;

	private bool _enabled = true;

	public void Initialize(MilMo_EmoteManager emoteManager, GameObject avatar)
	{
		_emoteManager = emoteManager;
		_followTransform = avatar.transform.Find("Root");
		_referenceTransform = avatar.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip");
	}

	public void LateUpdate()
	{
		if (_enabled && _emoteManager != null && (bool)_followTransform && (bool)_referenceTransform)
		{
			Vector3 forward = _referenceTransform.forward;
			Vector3 forward2 = _followTransform.forward;
			float x = Mathf.Clamp((forward.x - forward2.x) * _angleUVScale, _minOffsetTurn, _maxOffsetTurn);
			float y = Mathf.Clamp((forward.y - forward2.y) * _angleUVScale, _minOffsetUp, _maxOffsetUp);
			_emoteManager.SetEyeSpec(new Vector2(x, y));
		}
	}
}
