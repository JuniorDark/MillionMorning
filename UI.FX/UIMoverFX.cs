using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class UIMoverFX : MonoBehaviour
{
	[SerializeField]
	private Transform destination;

	[SerializeField]
	private float time = 2f;

	private UnityAction _onComplete;

	public void MoveToTargetAndDie(Transform target = null)
	{
		_onComplete = (UnityAction)Delegate.Combine(_onComplete, new UnityAction(Die));
		MoveToTarget(target);
	}

	public void MoveToTarget(Transform target = null)
	{
		if (target != null)
		{
			destination = target;
		}
		if (destination == null)
		{
			OnComplete();
			return;
		}
		LTBezierPath bezierPath = GetBezierPath();
		base.transform.LeanScale(Vector3.one * 1.2f, time * 0.3f).setEase(LeanTweenType.easeOutBounce);
		base.transform.LeanMove(bezierPath, time).setEase(LeanTweenType.easeOutQuart).setOnComplete(OnComplete)
			.setDelay(time * 0.2f);
		base.transform.LeanScale(Vector3.zero, time * 0.5f).setEase(LeanTweenType.easeInSine).setDelay(time * 0.8f);
	}

	private Vector3 GetCenter()
	{
		Vector3 position = new Vector3(0.5f, 0.5f, 0f);
		Camera main = Camera.main;
		if (!(main != null))
		{
			return new Vector3((float)Screen.width * position.x, (float)Screen.height * position.y, 0f);
		}
		return main.ViewportToWorldPoint(position);
	}

	private LTBezierPath GetBezierPath()
	{
		Vector3 center = GetCenter();
		Vector3 position = base.transform.position;
		Vector3 vector = ((destination != null) ? destination.position : position);
		Vector3 vector2 = new Vector3((position.x + center.x) * 0.5f, center.y);
		Vector3 vector3 = new Vector3((vector.x + center.x) * 0.5f, center.y);
		return new LTBezierPath(new Vector3[4] { position, vector3, vector2, vector });
	}

	private void OnComplete()
	{
		_onComplete?.Invoke();
	}

	private void Die()
	{
		UnityEngine.Object.Destroy(base.gameObject, 0.1f);
	}

	private void OnDrawGizmosSelected()
	{
		LTBezierPath bezierPath = GetBezierPath();
		Gizmos.DrawLine(bezierPath.point(0f), bezierPath.point(0.125f));
		Gizmos.DrawLine(bezierPath.point(0.125f), bezierPath.point(0.25f));
		Gizmos.DrawLine(bezierPath.point(0.25f), bezierPath.point(0.375f));
		Gizmos.DrawLine(bezierPath.point(0.375f), bezierPath.point(0.5f));
		Gizmos.DrawLine(bezierPath.point(0.5f), bezierPath.point(0.625f));
		Gizmos.DrawLine(bezierPath.point(0.625f), bezierPath.point(0.75f));
		Gizmos.DrawLine(bezierPath.point(0.75f), bezierPath.point(0.875f));
		Gizmos.DrawLine(bezierPath.point(0.875f), bezierPath.point(1f));
	}
}
