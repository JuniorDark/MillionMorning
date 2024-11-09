using System;
using System.Collections.Generic;
using Code.Core.Camera;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Player.Moods;
using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class RagdollManager
{
	private readonly MilMo_Avatar _avatar;

	private static int _ragdollCount;

	private static readonly Dictionary<string, TransformInfo> TransformInfos = new Dictionary<string, TransformInfo>();

	private readonly Dictionary<string, Transform> _iterationTransforms = new Dictionary<string, Transform>();

	private Rigidbody _head;

	private Rigidbody _torso;

	public bool IsActive { get; private set; }

	private bool IsLimited
	{
		get
		{
			if (!_avatar.IsTheLocalPlayer && !_avatar.Renderer.isVisible)
			{
				return _ragdollCount > 2;
			}
			return false;
		}
	}

	public RagdollManager(MilMo_Avatar avatar)
	{
		_avatar = avatar;
	}

	public static void Initialize()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("RagdollInfo");
		while (milMo_SFFile.NextRow())
		{
			if (!milMo_SFFile.IsNext("<TRANSFORM>"))
			{
				continue;
			}
			milMo_SFFile.NextRow();
			ColliderInfo collider = null;
			RigidBodyInfo rigidBody = null;
			CharacterJointInfo characterJoint = null;
			string @string = milMo_SFFile.GetString();
			while (!milMo_SFFile.IsNext("</TRANSFORM>"))
			{
				milMo_SFFile.NextRow();
				if (milMo_SFFile.IsNext("<COLLIDER>"))
				{
					milMo_SFFile.NextRow();
					if (milMo_SFFile.IsNext("Box"))
					{
						milMo_SFFile.NextRow();
						Vector3 vector = milMo_SFFile.GetVector3();
						milMo_SFFile.NextRow();
						Vector3 vector2 = milMo_SFFile.GetVector3();
						milMo_SFFile.NextRow();
						collider = new BoxColliderInfo(vector, vector2);
					}
					else if (milMo_SFFile.IsNext("Capsule"))
					{
						milMo_SFFile.NextRow();
						float @float = milMo_SFFile.GetFloat();
						milMo_SFFile.NextRow();
						float float2 = milMo_SFFile.GetFloat();
						milMo_SFFile.NextRow();
						int @int = milMo_SFFile.GetInt();
						milMo_SFFile.NextRow();
						Vector3 vector3 = milMo_SFFile.GetVector3();
						milMo_SFFile.NextRow();
						collider = new CapsuleColliderInfo(@float, float2, @int, vector3);
					}
					if (!milMo_SFFile.IsNext("</COLLIDER>"))
					{
						throw new ArgumentException("Bad collider in rigid body");
					}
				}
				else if (milMo_SFFile.IsNext("<RIGIDBODY>"))
				{
					milMo_SFFile.NextRow();
					float float3 = milMo_SFFile.GetFloat();
					milMo_SFFile.NextRow();
					float float4 = milMo_SFFile.GetFloat();
					milMo_SFFile.NextRow();
					float float5 = milMo_SFFile.GetFloat();
					milMo_SFFile.NextRow();
					bool @bool = milMo_SFFile.GetBool();
					milMo_SFFile.NextRow();
					rigidBody = new RigidBodyInfo(float3, float4, float5, @bool);
					if (!milMo_SFFile.IsNext("</RIGIDBODY>"))
					{
						throw new ArgumentException("Bad rigidBody in rigid body");
					}
				}
				else if (milMo_SFFile.IsNext("<CHARACTER_JOINT>"))
				{
					milMo_SFFile.NextRow();
					string string2 = milMo_SFFile.GetString();
					milMo_SFFile.NextRow();
					Vector3 vector4 = milMo_SFFile.GetVector3();
					milMo_SFFile.NextRow();
					Vector3 vector5 = milMo_SFFile.GetVector3();
					milMo_SFFile.NextRow();
					Vector3 vector6 = milMo_SFFile.GetVector3();
					milMo_SFFile.NextRow();
					Vector4 vector7 = milMo_SFFile.GetVector4();
					milMo_SFFile.NextRow();
					Vector4 vector8 = milMo_SFFile.GetVector4();
					milMo_SFFile.NextRow();
					Vector4 swing1Limit = milMo_SFFile.GetVector3();
					milMo_SFFile.NextRow();
					Vector4 swing2Limit = milMo_SFFile.GetVector3();
					milMo_SFFile.NextRow();
					characterJoint = new CharacterJointInfo(string2, vector4, vector5, vector6, vector7, vector8, swing1Limit, swing2Limit);
					if (!milMo_SFFile.IsNext("</CHARACTER_JOINT>"))
					{
						throw new ArgumentException("Bad character joint in rigid body");
					}
				}
			}
			TransformInfo value = new TransformInfo(collider, rigidBody, characterJoint);
			TransformInfos.Add(@string, value);
		}
	}

	public void Update()
	{
		if (IsActive)
		{
			if (!_avatar.GameObject || !_avatar.Renderer)
			{
				Deactivate();
			}
			else if (IsLimited)
			{
				Deactivate();
			}
		}
	}

	public void Activate(Vector3 force, ForcePosition forcePosition)
	{
		if (!IsActive && !IsLimited)
		{
			if (_avatar.IsTheLocalPlayer)
			{
				MilMo_CameraController.Player = _avatar.Renderer.gameObject.transform;
			}
			_iterationTransforms.Clear();
			_avatar.DisableAnimations();
			_avatar.DisableBlobShadows();
			_avatar.SuperAlivenessManager.ForceDisable();
			_avatar.SuperAlivenessManager.DisableHeadLookController();
			_avatar.EmoteManager.SetFaceByName("RagdollFace");
			_avatar.EmoteManager.ForceOneUpdate();
			ActivateTransform(_avatar.GameObject.transform);
			ApplyForce(force, forcePosition);
			_ragdollCount++;
			IsActive = true;
		}
	}

	private void ApplyForce(Vector3 force, ForcePosition forcePosition)
	{
		switch (forcePosition)
		{
		case ForcePosition.Head:
			if (_head != null)
			{
				_head.AddForceAtPosition(force, _avatar.Head.position);
			}
			break;
		case ForcePosition.Torso:
			if (_torso != null)
			{
				Vector3 position = _avatar.Head.position;
				Vector3 position2 = new Vector3(position.x, position.y - 0.5f, position.z);
				_torso.AddForceAtPosition(force, position2);
			}
			break;
		default:
			Debug.LogWarning("RagdollManager: Got invalid forcePosition");
			break;
		}
	}

	private void ActivateTransform(Transform transform)
	{
		if (!_iterationTransforms.ContainsKey(transform.name))
		{
			_iterationTransforms.Add(transform.name, transform);
		}
		if (TransformInfos.ContainsKey(transform.name))
		{
			TransformInfos[transform.name].Setup(transform.gameObject, _iterationTransforms);
		}
		string name = transform.name;
		if (!(name == "SpineThoracicLo"))
		{
			if (name == "NeckLo")
			{
				_head = transform.gameObject.GetComponent<Rigidbody>();
			}
		}
		else
		{
			_torso = transform.gameObject.GetComponent<Rigidbody>();
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			ActivateTransform(transform.GetChild(i));
		}
	}

	public void Deactivate()
	{
		if (IsActive)
		{
			if (_avatar.IsTheLocalPlayer)
			{
				MilMo_CameraController.Player = _avatar.GameObject.transform;
			}
			DeactivateTransform(_avatar.GameObject.transform);
			_avatar.EnableAnimations();
			_avatar.EnableBlobShadows();
			_avatar.SuperAlivenessManager.ForceEnable();
			_avatar.SuperAlivenessManager.EnableHeadLookController();
			MilMo_MoodHandler moodHandler = _avatar.MoodHandler;
			if (moodHandler != null)
			{
				Mood moodByKey = moodHandler.GetMoodByKey(_avatar.Mood);
				moodHandler.SetMood(moodByKey, send: false, persist: false);
			}
			_ragdollCount--;
			IsActive = false;
		}
	}

	private static void DeactivateTransform(Transform transform)
	{
		BoxCollider component = transform.gameObject.GetComponent<BoxCollider>();
		if ((bool)component)
		{
			UnityEngine.Object.Destroy(component);
		}
		CapsuleCollider component2 = transform.gameObject.GetComponent<CapsuleCollider>();
		if ((bool)component2)
		{
			UnityEngine.Object.Destroy(component2);
		}
		CharacterJoint component3 = transform.gameObject.GetComponent<CharacterJoint>();
		if ((bool)component3)
		{
			UnityEngine.Object.Destroy(component3);
		}
		Rigidbody component4 = transform.gameObject.GetComponent<Rigidbody>();
		if ((bool)component4)
		{
			UnityEngine.Object.Destroy(component4);
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			DeactivateTransform(transform.GetChild(i));
		}
	}
}
