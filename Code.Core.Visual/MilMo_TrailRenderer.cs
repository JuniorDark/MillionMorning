using System;
using System.Collections;
using UnityEngine;

namespace Code.Core.Visual;

[ExecuteInEditMode]
[AddComponentMenu("MilMo/Trail")]
public class MilMo_TrailRenderer : MonoBehaviour
{
	private class Point
	{
		public float TimeCreated;

		public Vector3 Position;

		public bool LineBreak;
	}

	public bool emit = true;

	public float emitTime;

	public Material material;

	public float lifeTime = 1f;

	public Color[] colors;

	public float[] sizes;

	public float uvLengthScale = 0.01f;

	public bool higherQualityUVs = true;

	public int movePixelsForRebuild = 6;

	public float maxRebuildTime = 0.1f;

	public float minVertexDistance = 0.1f;

	public float maxVertexDistance = 10f;

	public float maxAngle = 3f;

	public bool autoDestruct;

	private readonly ArrayList _points = new ArrayList();

	private GameObject _o;

	private MeshFilter _meshFilter;

	private Vector3 _lastPosition;

	private Vector3 _lastCameraPosition1;

	private Vector3 _lastCameraPosition2;

	private float _lastRebuildTime;

	private bool _lastFrameEmit = true;

	private void Enable()
	{
		_lastPosition = base.transform.position;
		if (!(_o != null))
		{
			_o = new GameObject("Trail");
			_o.transform.parent = null;
			_o.transform.position = Vector3.zero;
			_o.transform.rotation = Quaternion.identity;
			_o.transform.localScale = Vector3.one;
			_meshFilter = _o.AddComponent<MeshFilter>();
			_o.AddComponent<MeshRenderer>();
			_o.GetComponent<Renderer>().material = material;
		}
	}

	private void Start()
	{
		Enable();
	}

	private void OnEnable()
	{
		Enable();
	}

	private void OnDisable()
	{
		UnityEngine.Object.Destroy(_o);
	}

	private void Update()
	{
		if (emit && (double)Math.Abs(emitTime) > 0.0)
		{
			emitTime -= Time.deltaTime;
			if ((double)Math.Abs(emitTime) < 0.0)
			{
				emitTime = -1f;
			}
			if (emitTime < 0f)
			{
				emit = false;
			}
		}
		if (!emit && _points.Count == 0 && autoDestruct)
		{
			UnityEngine.Object.Destroy(_o);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		UnityEngine.Camera main = UnityEngine.Camera.main;
		if (!main)
		{
			return;
		}
		bool flag = false;
		float magnitude = (_lastPosition - base.transform.position).magnitude;
		if (emit)
		{
			if (magnitude > minVertexDistance)
			{
				bool flag2 = false;
				if (_points.Count < 3)
				{
					flag2 = true;
				}
				else
				{
					Vector3 from = ((Point)_points[_points.Count - 2]).Position - ((Point)_points[_points.Count - 3]).Position;
					Vector3 to = ((Point)_points[_points.Count - 1]).Position - ((Point)_points[_points.Count - 2]).Position;
					if (Vector3.Angle(from, to) > maxAngle || magnitude > maxVertexDistance)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					Vector3 position = base.transform.position;
					Point value = new Point
					{
						Position = position,
						TimeCreated = Time.time
					};
					_points.Add(value);
					_lastPosition = position;
				}
				else
				{
					((Point)_points[_points.Count - 1]).Position = base.transform.position;
					((Point)_points[_points.Count - 1]).TimeCreated = Time.time;
				}
			}
			else if (_points.Count > 0)
			{
				((Point)_points[_points.Count - 1]).Position = base.transform.position;
				((Point)_points[_points.Count - 1]).TimeCreated = Time.time;
			}
		}
		if (!emit && _lastFrameEmit && _points.Count > 0)
		{
			((Point)_points[_points.Count - 1]).LineBreak = true;
		}
		_lastFrameEmit = emit;
		if (_points.Count > 1)
		{
			if (main != null)
			{
				Vector3 vector = main.WorldToScreenPoint(((Point)_points[0]).Position);
				_lastCameraPosition1.z = 0f;
				Vector3 vector2 = main.WorldToScreenPoint(((Point)_points[_points.Count - 1]).Position);
				_lastCameraPosition2.z = 0f;
				if ((_lastCameraPosition1 - vector).magnitude + (_lastCameraPosition2 - vector2).magnitude > (float)movePixelsForRebuild || Time.time - _lastRebuildTime > maxRebuildTime)
				{
					flag = true;
					_lastCameraPosition1 = vector;
					_lastCameraPosition2 = vector2;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		_lastRebuildTime = Time.time;
		ArrayList arrayList = new ArrayList();
		int num = 0;
		foreach (Point point3 in _points)
		{
			if (Time.time - point3.TimeCreated > lifeTime)
			{
				arrayList.Add(point3);
			}
			num++;
		}
		foreach (Point item in arrayList)
		{
			_points.Remove(item);
		}
		arrayList.Clear();
		if (_points.Count <= 1)
		{
			return;
		}
		Vector3[] array = new Vector3[_points.Count * 2];
		Vector2[] array2 = new Vector2[_points.Count * 2];
		int[] array3 = new int[(_points.Count - 1) * 6];
		Color[] array4 = new Color[_points.Count * 2];
		num = 0;
		float num2 = 0f;
		foreach (Point point4 in _points)
		{
			float num3 = (Time.time - point4.TimeCreated) / lifeTime;
			Color color = Color.Lerp(Color.white, Color.clear, num3);
			if (colors != null && colors.Length != 0)
			{
				float num4 = num3 * (float)(colors.Length - 1);
				float num5 = Mathf.Floor(num4);
				float num6 = Mathf.Clamp(Mathf.Ceil(num4), 1f, colors.Length - 1);
				float t = Mathf.InverseLerp(num5, num6, num4);
				if (num5 >= (float)colors.Length)
				{
					num5 = colors.Length - 1;
				}
				if (num5 < 0f)
				{
					num5 = 0f;
				}
				if (num6 >= (float)colors.Length)
				{
					num6 = colors.Length - 1;
				}
				if (num6 < 0f)
				{
					num6 = 0f;
				}
				color = Color.Lerp(colors[(int)num5], colors[(int)num6], t);
			}
			float num7 = 1f;
			if (sizes != null && sizes.Length != 0)
			{
				float num8 = num3 * (float)(sizes.Length - 1);
				float num9 = Mathf.Floor(num8);
				float num10 = Mathf.Clamp(Mathf.Ceil(num8), 1f, sizes.Length - 1);
				float t2 = Mathf.InverseLerp(num9, num10, num8);
				if (num9 >= (float)sizes.Length)
				{
					num9 = sizes.Length - 1;
				}
				if (num9 < 0f)
				{
					num9 = 0f;
				}
				if (num10 >= (float)sizes.Length)
				{
					num10 = sizes.Length - 1;
				}
				if (num10 < 0f)
				{
					num10 = 0f;
				}
				num7 = Mathf.Lerp(sizes[(int)num9], sizes[(int)num10], t2);
			}
			Vector3 lhs = ((num != 0) ? (((Point)_points[num - 1]).Position - point4.Position) : (point4.Position - ((Point)_points[num + 1]).Position));
			Vector3 rhs = main.transform.position - point4.Position;
			Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
			array[num * 2] = point4.Position + normalized * (num7 * 0.5f);
			array[num * 2 + 1] = point4.Position + -normalized * (num7 * 0.5f);
			array4[num * 2] = (array4[num * 2 + 1] = color);
			array2[num * 2] = new Vector2(num2 * uvLengthScale, 0f);
			array2[num * 2 + 1] = new Vector2(num2 * uvLengthScale, 1f);
			if (num > 0 && !((Point)_points[num - 1]).LineBreak)
			{
				num2 = ((!higherQualityUVs) ? (num2 + (point4.Position - ((Point)_points[num - 1]).Position).sqrMagnitude) : (num2 + (point4.Position - ((Point)_points[num - 1]).Position).magnitude));
				array3[(num - 1) * 6] = num * 2 - 2;
				array3[(num - 1) * 6 + 1] = num * 2 - 1;
				array3[(num - 1) * 6 + 2] = num * 2;
				array3[(num - 1) * 6 + 3] = num * 2 + 1;
				array3[(num - 1) * 6 + 4] = num * 2;
				array3[(num - 1) * 6 + 5] = num * 2 - 1;
			}
			num++;
		}
		if ((bool)_meshFilter && (bool)_meshFilter.mesh)
		{
			Mesh mesh = _meshFilter.mesh;
			mesh.Clear();
			mesh.vertices = array;
			mesh.colors = array4;
			mesh.uv = array2;
			mesh.triangles = array3;
		}
	}
}
