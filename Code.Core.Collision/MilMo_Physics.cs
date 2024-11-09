using UnityEngine;

namespace Code.Core.Collision;

public static class MilMo_Physics
{
	public static float GetDistanceToGround(Vector3 pos)
	{
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(pos, Vector3.down, out hitInfo, 10000f, 95158273);
		float b = (flag ? hitInfo.distance : 0f);
		bool flag2 = false;
		float num = float.MinValue;
		if ((bool)Terrain.activeTerrain)
		{
			num = pos.y - Terrain.activeTerrain.SampleHeight(pos);
			flag2 = num >= 0f;
		}
		if (flag2 && flag)
		{
			return Mathf.Min(num, b);
		}
		if (flag2)
		{
			return num;
		}
		if (flag)
		{
			return hitInfo.distance;
		}
		return 0f;
	}

	public static float GetWorldHeight(Vector3 pos, int mask = -1)
	{
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(pos, Vector3.down, out hitInfo, 10000f, mask);
		float result = (flag ? hitInfo.point.y : 0f);
		float num = (flag ? hitInfo.distance : 0f);
		bool flag2 = false;
		float num2 = 0f;
		float num3 = float.MinValue;
		if ((bool)Terrain.activeTerrain)
		{
			num2 = Terrain.activeTerrain.SampleHeight(pos);
			num3 = pos.y - num2;
			flag2 = num3 >= 0f;
		}
		if (flag2 && flag)
		{
			if (!(num3 < num))
			{
				return result;
			}
			return num2;
		}
		if (flag2)
		{
			return num2;
		}
		if (flag)
		{
			return result;
		}
		return pos.y;
	}

	public static float GetWorldHeight(Vector3 pos, out Vector3 normal, int mask)
	{
		Vector3 vector = new Vector3(pos.x, pos.y, pos.z);
		vector.y += 2f;
		RaycastHit hitInfo;
		bool flag = Physics.Raycast(vector, Vector3.down, out hitInfo, 10000f, mask);
		float result = (flag ? hitInfo.point.y : 0f);
		float num = (flag ? hitInfo.distance : 0f);
		Vector3 vector2 = (flag ? hitInfo.normal : Vector3.up);
		bool flag2 = false;
		float num2 = 0f;
		float num3 = float.MinValue;
		if ((bool)Terrain.activeTerrain)
		{
			num2 = Terrain.activeTerrain.SampleHeight(vector);
			num3 = vector.y - num2;
			flag2 = num3 >= 0f;
		}
		if (flag2 && flag)
		{
			normal = ((num3 < num) ? GetTerrainNormal(vector) : vector2);
			if (!(num3 < num))
			{
				return result;
			}
			return num2;
		}
		if (flag2)
		{
			normal = GetTerrainNormal(vector);
			return num2;
		}
		if (flag)
		{
			normal = vector2;
			return result;
		}
		normal = Vector3.up;
		return pos.y;
	}

	public static float GetTerrainHeight(Vector3 pos)
	{
		if ((bool)Terrain.activeTerrain)
		{
			return Terrain.activeTerrain.SampleHeight(pos);
		}
		return pos.y;
	}

	public static Vector3 GetTerrainNormal(Vector3 pos)
	{
		if (!Terrain.activeTerrain)
		{
			return Vector3.up;
		}
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		Vector2 vector = new Vector2((pos.x + terrainData.size.x / 2f) / terrainData.size.x, (pos.z + terrainData.size.z / 2f) / terrainData.size.z);
		return Terrain.activeTerrain.terrainData.GetInterpolatedNormal(vector.x, vector.y);
	}

	public static float PointLineSegmentSqrDistance(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = point - lineStart;
		Vector3 rhs = lineEnd - lineStart;
		float num = Vector3.Dot(vector, rhs);
		if (num <= 0f)
		{
			return Vector3.Dot(vector, vector);
		}
		float sqrMagnitude = rhs.sqrMagnitude;
		if (num >= sqrMagnitude)
		{
			return (point - lineEnd).sqrMagnitude;
		}
		return vector.sqrMagnitude - num * num / sqrMagnitude;
	}

	public static Vector3 ClosestPointToPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 lhs = point - lineStart;
		Vector3 vector = lineEnd - lineStart;
		float num = Vector3.Dot(lhs, vector);
		if (num <= 0f)
		{
			return lineStart;
		}
		float sqrMagnitude = vector.sqrMagnitude;
		if (num >= sqrMagnitude)
		{
			return lineEnd;
		}
		Vector3 vector2 = num / sqrMagnitude * vector;
		return lineStart + vector2;
	}

	public static Vector3 ClosestPointOnRay(Vector3 rayStart, Vector3 rayDirection, Vector3 point)
	{
		Vector3 vector = point - rayStart;
		return rayStart + Vector3.Project(vector, rayDirection);
	}
}
