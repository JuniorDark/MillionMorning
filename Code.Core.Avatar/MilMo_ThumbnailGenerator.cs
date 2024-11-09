using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_ThumbnailGenerator
{
	public delegate void ThumbnailDone(bool success, string playerId, Texture2D texture);

	private const int THUMBNAIL_WIDTH = 64;

	private const int THUMBNAIL_HEIGHT = 64;

	private const float AVATAR_TO_CAMERA_DISTANCE = 0.7f;

	private const float CAMERA_HEIGHT_OFFSET = 1f;

	private static MilMo_ThumbnailGenerator _instance;

	private readonly UnityEngine.Camera _camera;

	private readonly Dictionary<string, List<ThumbnailDone>> _callbacks = new Dictionary<string, List<ThumbnailDone>>();

	private MilMo_GenericReaction _thumbnailAvatarInfoListener;

	private MilMo_GenericReaction _thumbnailInfoNoAvatarListener;

	public static MilMo_ThumbnailGenerator Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			_instance = new MilMo_ThumbnailGenerator();
			return _instance;
		}
	}

	private MilMo_ThumbnailGenerator()
	{
		GameObject gameObject = new GameObject("_Thumbnail", typeof(UnityEngine.Camera));
		_camera = gameObject.GetComponent<UnityEngine.Camera>();
		_camera.cullingMask = 2048;
		_camera.enabled = false;
		_thumbnailAvatarInfoListener = MilMo_EventSystem.Listen("thumbnail_avatar_info", GotThumbnailAvatarInfo);
		_thumbnailAvatarInfoListener.Repeating = true;
		_thumbnailInfoNoAvatarListener = MilMo_EventSystem.Listen("thumbnail_avatar_info_no_avatar", GotThumbnailAvatarInfoNoAvatar);
		_thumbnailInfoNoAvatarListener.Repeating = true;
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_thumbnailAvatarInfoListener);
		_thumbnailAvatarInfoListener = null;
		MilMo_EventSystem.RemoveReaction(_thumbnailInfoNoAvatarListener);
		_thumbnailInfoNoAvatarListener = null;
	}

	private void GotThumbnailAvatarInfo(object messageAsObject)
	{
		if (messageAsObject is ServerThumbnailAvatarInfo serverThumbnailAvatarInfo && _callbacks.ContainsKey(serverThumbnailAvatarInfo.getUserId()))
		{
			MilMo_Avatar milMo_Avatar = new MilMo_Avatar(thumbnailMode: true);
			milMo_Avatar.SetInitializedCallback(InternalCreateThumbnail, serverThumbnailAvatarInfo.getUserId());
			milMo_Avatar.Read(null, serverThumbnailAvatarInfo.getAvatar());
		}
	}

	private void GotThumbnailAvatarInfoNoAvatar(object messageAsObject)
	{
		if (!(messageAsObject is ServerThumbnailAvatarInfoNoAvatar serverThumbnailAvatarInfoNoAvatar) || !_callbacks.ContainsKey(serverThumbnailAvatarInfoNoAvatar.getUserId()))
		{
			return;
		}
		foreach (ThumbnailDone item in _callbacks[serverThumbnailAvatarInfoNoAvatar.getUserId()])
		{
			item(success: false, serverThumbnailAvatarInfoNoAvatar.getUserId(), null);
		}
		_callbacks.Remove(serverThumbnailAvatarInfoNoAvatar.getUserId());
	}

	public void Generate(string playerId, ThumbnailDone callback)
	{
		if (Singleton<GameNetwork>.Instance == null || !Singleton<GameNetwork>.Instance.IsConnectedToGameServer)
		{
			callback(success: false, playerId, null);
			return;
		}
		if (_callbacks.ContainsKey(playerId))
		{
			_callbacks[playerId].Add(callback);
			return;
		}
		if (!Singleton<GameNetwork>.Instance.RequestThumbnailAvatarInfo(playerId))
		{
			callback(success: false, playerId, null);
			return;
		}
		List<ThumbnailDone> value = new List<ThumbnailDone> { callback };
		_callbacks.Add(playerId, value);
	}

	private void InternalCreateThumbnail(MilMo_Avatar avatar, string playerId)
	{
		if (!_callbacks.ContainsKey(playerId))
		{
			avatar?.Destroy();
			return;
		}
		List<ThumbnailDone> list = _callbacks[playerId];
		_callbacks.Remove(playerId);
		if (avatar == null)
		{
			foreach (ThumbnailDone item in list)
			{
				item(success: false, playerId, null);
			}
			return;
		}
		Light[] array = Object.FindObjectsOfType(typeof(Light)) as Light[];
		List<Light> list2 = new List<Light>();
		if (array != null)
		{
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (array[num].gameObject.activeSelf && array[num].enabled)
				{
					array[num].gameObject.SetActive(value: false);
					array[num].enabled = false;
					list2.Add(array[num]);
				}
			}
		}
		Color ambientLight = RenderSettings.ambientLight;
		GameObject gameObject = new GameObject("ThumbnailLight");
		Light light = gameObject.AddComponent<Light>();
		light.type = LightType.Directional;
		light.shadows = LightShadows.None;
		light.renderMode = LightRenderMode.ForcePixel;
		light.color = Color.white;
		light.intensity = 1f;
		gameObject.transform.eulerAngles = new Vector3(0f, 35f, 0f);
		RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
		Vector3 position = new Vector3(0f, -1000f, 0.7f * avatar.Height);
		Vector3 position2 = new Vector3(0f, -1000f + 1f * avatar.Height, 0f);
		Quaternion rotation = Quaternion.LookRotation(Vector3.back);
		Quaternion rotation2 = Quaternion.LookRotation(Vector3.forward);
		avatar.GameObject.transform.position = position;
		avatar.GameObject.transform.rotation = rotation;
		Transform transform = _camera.transform;
		transform.position = position2;
		transform.rotation = rotation2;
		Color backgroundColor = _camera.backgroundColor;
		_camera.backgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 1f);
		RenderTexture temporary = RenderTexture.GetTemporary(64, 64, 16);
		_camera.targetTexture = temporary;
		Renderer[] componentsInChildren = avatar.GameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = 11;
		}
		_camera.Render();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(64, 64, TextureFormat.ARGB32, mipChain: false)
		{
			name = "AvatarThumbnail"
		};
		texture2D.ReadPixels(new Rect(0f, 0f, 64f, 64f), 0, 0, recalculateMipMaps: false);
		texture2D.Apply(updateMipmaps: false);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
		avatar.Destroy();
		foreach (Light item2 in list2)
		{
			item2.gameObject.SetActive(value: true);
			item2.enabled = true;
		}
		RenderSettings.ambientLight = ambientLight;
		MilMo_Global.Destroy(gameObject);
		foreach (ThumbnailDone item3 in list)
		{
			item3(success: true, playerId, texture2D);
		}
	}
}
