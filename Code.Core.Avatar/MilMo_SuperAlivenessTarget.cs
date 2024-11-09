using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_SuperAlivenessTarget
{
	private const float PRIO_FACTOR = 1f;

	private const float DISTANCE_FACTOR = 0.2f;

	private const float TIME_FACTOR = 1f;

	private readonly float _height;

	public Vector3 Position
	{
		get
		{
			Vector3 position = Transform.position;
			position.y += _height;
			return position;
		}
	}

	public Transform Transform { get; }

	private MilMo_SuperAlivenessManager.Prio Prio { get; set; }

	public float TotalPrio { get; private set; }

	public float StartTime { get; private set; }

	public MilMo_SuperAlivenessTarget(Transform transform, MilMo_SuperAlivenessManager.Prio prio, float sqrDistance, bool useBoundingBoxForHeight)
	{
		Transform = transform;
		Prio = prio;
		StartTime = Time.time;
		if (useBoundingBoxForHeight)
		{
			Renderer componentInChildren = Transform.gameObject.GetComponentInChildren<Renderer>();
			if ((bool)componentInChildren)
			{
				_height = componentInChildren.bounds.extents.y;
			}
		}
		TotalPrio = ((sqrDistance <= 36f) ? (1f * (float)Prio - 0.2f * sqrDistance - 0f) : 0f);
	}

	public void ChangePrioClass(MilMo_SuperAlivenessManager.Prio newPrio, bool resetTime)
	{
		Prio = newPrio;
		if (resetTime)
		{
			StartTime = Time.time;
		}
	}

	public void UpdateTotalPrio(Transform watcherTransform, out bool inRange)
	{
		inRange = false;
		if (!Transform || !MilMo_Utility.GetAncestor(Transform.gameObject).activeSelf)
		{
			return;
		}
		Vector3 from = Transform.position - watcherTransform.position;
		float sqrMagnitude = from.sqrMagnitude;
		inRange = sqrMagnitude <= 36f;
		if (!inRange)
		{
			TotalPrio = 0f;
			return;
		}
		from.y = 0f;
		if (Vector3.Angle(from, watcherTransform.forward) > 100f)
		{
			TotalPrio = 0f;
			return;
		}
		float num = Time.time - StartTime;
		TotalPrio = 1f * (float)Prio - 0.2f * sqrMagnitude - 1f * num;
	}
}
