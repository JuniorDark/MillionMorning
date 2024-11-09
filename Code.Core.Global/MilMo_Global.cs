using Code.Core.Emote;
using Code.World.ImageEffects;
using UnityEngine;

namespace Code.Core.Global;

public static class MilMo_Global
{
	public static string SteamLoginServer = "";

	public static string WebLoginURL = "";

	public static string WebLoginPasswordResetURL = "";

	public static string RemoteGameHost = "";

	public static string AuthorizationToken = null;

	public static GameObject MainGameObject = null;

	public static MilMo_ImageEffectsHandler ImageEffectsHandler;

	public static UnityEngine.Camera Camera = null;

	public static GameObject AudioListener = null;

	public static string[] EventTags = new string[0];

	public static GameObject ParticleLight;

	public static readonly MilMo_RandomEmoteContainer TheBlinks = new MilMo_RandomEmoteContainer("Blinks");

	public static readonly MilMo_RandomEmoteContainer TheIdleVariations = new MilMo_RandomEmoteContainer("IdleVariations");

	private static float _pauseTime;

	public static bool PositionExtrapolation = false;

	public static bool CubicInterpolation = false;

	public const float GRAVITY = -7.64f;

	public const int SERVER_TIMEOUT = 25000;

	public static UnityEngine.Camera MainCamera
	{
		get
		{
			if (!Application.isPlaying)
			{
				return UnityEngine.Camera.current;
			}
			return UnityEngine.Camera.main;
		}
	}

	public static void Destroy(Object obj, bool forceImmediate = false)
	{
		if (!(obj == null))
		{
			if (!Application.isPlaying || forceImmediate)
			{
				Object.DestroyImmediate(obj, allowDestroyingAssets: true);
			}
			else
			{
				Object.Destroy(obj);
			}
		}
	}
}
