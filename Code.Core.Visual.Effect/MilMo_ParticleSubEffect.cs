using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_ParticleSubEffect : MilMo_SubEffect
{
	private bool _ancestorActive = true;

	public MilMo_ParticleSubEffect(MilMo_ParticleAction action, Vector3 position)
	{
		Setup(action, null, position, Vector3.zero, 0f);
	}

	public MilMo_ParticleSubEffect(MilMo_ParticleAction action, GameObject parent, Vector3 dynamicOffset)
	{
		Setup(action, parent, default(Vector3), dynamicOffset, 0f);
	}

	public MilMo_ParticleSubEffect(MilMo_ParticleAction action, GameObject parent, float staticYPos)
	{
		Setup(action, parent, default(Vector3), Vector3.zero, staticYPos);
	}

	private void Setup(MilMo_ParticleAction action, GameObject parent, Vector3 position, Vector3 dynamicOffset, float staticYPos)
	{
		if (MilMo_ParticleContainer.DevMode)
		{
			Debug.Log("ParticleSubEffect: " + action.PropParticle + ", Parent: " + parent?.ToString() + ", Emitter: " + base.EmittingObject);
		}
		Transform transform = ((parent != null) ? FindParentTransform(action, parent) : null);
		if ((double)Math.Abs(staticYPos) > 0.0 && parent != null)
		{
			YLocked = true;
			YPos = staticYPos;
			if (transform != null)
			{
				position = transform.TransformPoint(action.Offset);
			}
			position.y = staticYPos;
		}
		else if (parent != null)
		{
			if (transform != null)
			{
				position = transform.TransformPoint(action.Offset + dynamicOffset);
			}
		}
		else
		{
			position += action.Offset;
		}
		base.EmittingObject = ((parent != null) ? MilMo_ParticleContainer.GetParticle(action.PropParticle, position, parent.transform.rotation) : MilMo_ParticleContainer.GetParticle(action.PropParticle, position));
		if ((double)Math.Abs(staticYPos) > 0.0 || parent == null)
		{
			if (!action.Rotation.Equals(Vector3.zero))
			{
				base.EmittingObject.transform.rotation = Quaternion.Euler(action.Rotation);
			}
			if (!action.Scale.Equals(Vector3.one))
			{
				base.EmittingObject.transform.localScale = action.Scale;
			}
		}
		List<GameObject> list = new List<GameObject>();
		if ((double)Math.Abs(staticYPos) > 0.0 && parent != null)
		{
			base.EmittingObject.transform.parent = transform;
			if (transform != null)
			{
				base.EmittingObject.transform.localPosition = transform.position - position;
			}
		}
		else if (parent != null)
		{
			while (parent != null)
			{
				if (!parent.activeSelf)
				{
					list.Add(parent);
					parent.SetActive(value: true);
				}
				Transform parent2 = parent.transform.parent;
				parent = ((parent2 != null) ? parent2.gameObject : null);
			}
			Quaternion rotation = base.EmittingObject.transform.rotation;
			base.EmittingObject.transform.SetParent(transform, worldPositionStays: false);
			base.EmittingObject.transform.localPosition = action.Offset + dynamicOffset;
			base.EmittingObject.transform.rotation = rotation;
		}
		ParticleSystem[] componentsInChildren = base.EmittingObject.GetComponentsInChildren<ParticleSystem>();
		MilMo_ParticleTemplate component = base.EmittingObject.GetComponent<MilMo_ParticleTemplate>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Emit((!component || !component.oneShot) ? 1 : ((int)component.minEmission));
			if (component.oneShot)
			{
				UnityEngine.Object.Destroy(particleSystem.gameObject, Math.Max(component.maxEnergy, Duration));
			}
			IsStopped = false;
		}
		MilMo_TrailRenderer[] componentsInChildren2 = base.EmittingObject.GetComponentsInChildren<MilMo_TrailRenderer>();
		foreach (MilMo_TrailRenderer milMo_TrailRenderer in componentsInChildren2)
		{
			if ((bool)milMo_TrailRenderer && milMo_TrailRenderer.emit)
			{
				IsStopped = false;
			}
		}
		Duration = action.Duration;
		CurrentTime = 0f;
		foreach (GameObject item in list)
		{
			item.SetActive(value: false);
		}
	}

	public override bool Update()
	{
		if (base.EmittingObject == null)
		{
			return false;
		}
		if (YLocked)
		{
			Vector3 localPosition = base.EmittingObject.transform.localPosition;
			localPosition.y = YPos;
			base.EmittingObject.transform.localPosition = localPosition;
		}
		CurrentTime += Time.deltaTime;
		ParticleSystem[] componentsInChildren = base.EmittingObject.GetComponentsInChildren<ParticleSystem>();
		MilMo_TrailRenderer[] componentsInChildren2 = base.EmittingObject.GetComponentsInChildren<MilMo_TrailRenderer>();
		MilMo_ParticleTemplate component = base.EmittingObject.GetComponent<MilMo_ParticleTemplate>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			if (component.manualEmit && !((float)particleSystem.particleCount >= particleSystem.emission.rateOverTime.constantMax))
			{
				ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
				emitParams.position = base.EmittingObject.transform.position;
				emitParams.applyShapeToPosition = true;
				ParticleSystem.EmitParams emitParams2 = emitParams;
				particleSystem.Emit(emitParams2, 1);
			}
		}
		GameObject ancestor = MilMo_Utility.GetAncestor(base.EmittingObject);
		if (ancestor.activeSelf != _ancestorActive)
		{
			array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.EmissionModule emission = array[i].emission;
				emission.enabled = ancestor.activeSelf && !IsStopped && (!(CurrentTime >= Duration) || !(Duration > 0f));
			}
			MilMo_TrailRenderer[] array2 = componentsInChildren2;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].emit = ancestor.activeSelf && !IsStopped && (!(CurrentTime >= Duration) || !(Duration > 0f));
			}
			_ancestorActive = ancestor.activeSelf;
		}
		if (componentsInChildren.Length == 0 && componentsInChildren2.Length == 0)
		{
			if (MilMo_ParticleContainer.DevMode)
			{
				Debug.Log("ParticleSubEffect: No more emitters in " + base.EmittingObject.name);
			}
			UnityEngine.Object.Destroy(base.EmittingObject);
			return false;
		}
		if (IsStopped || (CurrentTime >= Duration && Duration > 0f))
		{
			DestroyWhenDone();
			return false;
		}
		return true;
	}

	public override void DestroyWhenDone()
	{
		IsStopped = true;
		ParticleSystem[] componentsInChildren = base.EmittingObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem obj in componentsInChildren)
		{
			ParticleSystem.EmissionModule emission = obj.emission;
			emission.enabled = false;
			ParticleSystem.MainModule main = obj.main;
			main.loop = false;
		}
		MilMo_TrailRenderer[] componentsInChildren2 = base.EmittingObject.GetComponentsInChildren<MilMo_TrailRenderer>();
		foreach (MilMo_TrailRenderer obj2 in componentsInChildren2)
		{
			obj2.emit = false;
			obj2.autoDestruct = true;
		}
		UnityEngine.Object.Destroy(base.EmittingObject);
	}

	public override void Stop()
	{
		IsStopped = true;
		ParticleSystem[] componentsInChildren = base.EmittingObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem obj in componentsInChildren)
		{
			ParticleSystem.EmissionModule emission = obj.emission;
			emission.enabled = false;
			ParticleSystem.MainModule main = obj.main;
			main.loop = false;
		}
		MilMo_TrailRenderer[] componentsInChildren2 = base.EmittingObject.GetComponentsInChildren<MilMo_TrailRenderer>();
		foreach (MilMo_TrailRenderer obj2 in componentsInChildren2)
		{
			obj2.emit = false;
			obj2.autoDestruct = true;
		}
	}

	private static Transform FindParentTransform(MilMo_ParticleAction action, GameObject parent)
	{
		if (action.AttachNode == "")
		{
			return parent.transform;
		}
		SkinnedMeshRenderer componentInChildren = parent.GetComponentInChildren<SkinnedMeshRenderer>();
		if (!componentInChildren)
		{
			Debug.Log("Trying to attach particle '" + action.PropParticle + "' to a specific bone on game object '" + parent.name + "', but that object doesn't have a SkinnedMeshRenderer. The effect will be attached to the game object position instead.");
			return parent.transform;
		}
		Transform transform = componentInChildren.bones.FirstOrDefault((Transform t) => t.name == action.AttachNode);
		if (!transform)
		{
			Debug.Log("Trying to attach particle '" + action.PropParticle + "' to bone '" + action.AttachNode + "' of game object '" + parent.name + "', but that bone doesn't exist. The effect will be attached to the game object position instead.");
			return parent.transform;
		}
		return transform;
	}
}
