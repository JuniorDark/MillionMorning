using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_BlendMesh
{
	public Vector3[] Vertices;

	public Vector3[] Normals;

	public Color[] Colors;

	public int[] Triangles;

	public Vector2[] UV;

	public BoneWeight[] BoneWeights;

	public Matrix4x4[] BindPoses;

	public static void CopyToArrays(MilMo_BlendMesh from, ref Vector3[] verts, ref Color[] colors, ref Vector2[] uvs, ref BoneWeight[] boneWeights, ref Vector3[] normals, ref int[] triangles, int startPosVerts, int startPosTriangles, Rect uvOffset, bool copyColors)
	{
		int num = from.Vertices.Length;
		copyColors = copyColors && from.Colors.Length == num;
		for (int i = 0; i < num; i++)
		{
			int num2 = i + startPosVerts;
			verts[num2] = from.Vertices[i];
			if (copyColors)
			{
				colors[num2] = from.Colors[i];
			}
			else
			{
				colors[num2] = new Color(0f, 0f, 0f, 0f);
			}
			normals[num2] = from.Normals[i];
			boneWeights[num2] = from.BoneWeights[i];
			uvs[num2] = from.UV[i];
			uvs[num2].x *= uvOffset.width;
			uvs[num2].y *= uvOffset.height;
			uvs[num2].x += uvOffset.x;
			uvs[num2].y += uvOffset.y;
		}
		int num3 = from.Triangles.Length;
		for (int j = 0; j < num3; j++)
		{
			triangles[j + startPosTriangles] = from.Triangles[j] + startPosVerts;
		}
	}
}
