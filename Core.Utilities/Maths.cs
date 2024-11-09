using UnityEngine;

namespace Core.Utilities;

public static class Maths
{
	public static bool InsideDistanceSqr(float testDistanceSquared, float maxDistanceSqr)
	{
		return testDistanceSquared < maxDistanceSqr;
	}

	public static bool IsPointWithinCollider(Collider collider, Vector3 point)
	{
		return (collider.ClosestPoint(point) - point).sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon;
	}

	public static bool FloatEquals(float first, float second)
	{
		return Mathf.Abs(first - second) <= 0.001f;
	}

	public static bool FloatNotEquals(float first, float second)
	{
		return Mathf.Abs(first - second) > 0.001f;
	}
}
