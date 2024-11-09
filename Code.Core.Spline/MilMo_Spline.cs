using System;
using System.Collections.Generic;
using Code.Core.Command;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Spline;

public class MilMo_Spline
{
	public class MilMo_WayPoint
	{
		public Vector3 Position;

		public Vector3 Tangent;

		public readonly float Time;

		public MilMo_WayPoint(Vector3 position, Vector3 tangent, float time)
		{
			Position = position;
			Tangent = tangent;
			Time = time;
		}

		public MilMo_WayPoint(Vector3 position, float time)
		{
			Position = position;
			Tangent = Vector3.zero;
			Time = time;
		}
	}

	public class SplinePoint
	{
		public Vector3 Position;

		public Vector3 Tangent;

		public Vector3 Binormal;

		public Vector3 Normal;
	}

	private static readonly GameObject MArrow;

	private static readonly GameObject MDot;

	private static bool _mVisualizeSplines;

	private readonly List<GameObject> _visualization = new List<GameObject>();

	private bool _visualized;

	private bool _tangentsGenerated;

	public MilMo_SplineTemplate Template { get; private set; }

	public List<MilMo_WayPoint> Waypoints { get; protected set; }

	static MilMo_Spline()
	{
	}

	public static void RegisterCommands()
	{
		MilMo_Command.Instance.RegisterCommand("VisualizeSplines", Debug_VisualizeSplines);
	}

	public MilMo_Spline()
	{
		Waypoints = new List<MilMo_WayPoint>();
	}

	protected MilMo_Spline(MilMo_SplineTemplate template)
	{
		Waypoints = new List<MilMo_WayPoint>();
		if (template.Waypoints.Count > 0)
		{
			AddWaypoint(new Vector3(template.Waypoints[0].GetPosition().GetX(), template.Waypoints[0].GetPosition().GetY(), template.Waypoints[0].GetPosition().GetZ()), Quaternion.Euler(new Vector3(template.Waypoints[0].GetRotation().GetX(), template.Waypoints[0].GetRotation().GetY(), template.Waypoints[0].GetRotation().GetZ())) * Vector3.forward * 10f, template.Waypoints[0].GetTime());
			for (int i = 1; i < template.Waypoints.Count - 1; i++)
			{
				AddWaypoint(new Vector3(template.Waypoints[i].GetPosition().GetX(), template.Waypoints[i].GetPosition().GetY(), template.Waypoints[i].GetPosition().GetZ()), template.Waypoints[i].GetTime());
			}
			if (template.Waypoints.Count > 1)
			{
				AddWaypoint(new Vector3(template.Waypoints[template.Waypoints.Count - 1].GetPosition().GetX(), template.Waypoints[template.Waypoints.Count - 1].GetPosition().GetY(), template.Waypoints[template.Waypoints.Count - 1].GetPosition().GetZ()), Quaternion.Euler(new Vector3(template.Waypoints[template.Waypoints.Count - 1].GetRotation().GetX(), template.Waypoints[template.Waypoints.Count - 1].GetRotation().GetY(), template.Waypoints[template.Waypoints.Count - 1].GetRotation().GetZ())) * Vector3.forward * 10f, template.Waypoints[template.Waypoints.Count - 1].GetTime());
				GenerateInnerTangents();
			}
		}
		Template = template;
	}

	public void AddWaypoint(Vector3 position, Vector3 tangent, float time)
	{
		Waypoints.Add(new MilMo_WayPoint(position, tangent, time));
	}

	public void AddWaypoint(Vector3 position, float time)
	{
		Waypoints.Add(new MilMo_WayPoint(position, time));
	}

	private void GenerateInnerTangents()
	{
		if (Waypoints.Count < 2)
		{
			throw new InvalidOperationException("Must add more waypoints before inner tangents can be generated");
		}
		if (Waypoints[0].Tangent == Vector3.zero || Waypoints[Waypoints.Count - 1].Tangent == Vector3.zero)
		{
			throw new InvalidOperationException("Must define a tangent for first and last waypoint.");
		}
		for (int i = 1; i < Waypoints.Count - 1; i++)
		{
			Waypoints[i].Tangent = 0.5f * (Waypoints[i].Position - Waypoints[i - 1].Position + (Waypoints[i + 1].Position - Waypoints[i].Position));
		}
		_tangentsGenerated = true;
	}

	public SplinePoint GetPointAtTime(float time)
	{
		if (Waypoints.Count < 2)
		{
			throw new InvalidOperationException("Spline has too few waypoints");
		}
		if (!_tangentsGenerated)
		{
			GenerateInnerTangents();
		}
		if (_mVisualizeSplines && !_visualized)
		{
			Visualize(0.1f);
		}
		time = Mathf.Min(Mathf.Max(Waypoints[0].Time, time), Waypoints[Waypoints.Count - 1].Time);
		int i;
		for (i = 0; i < Waypoints.Count - 1 && !(Waypoints[i + 1].Time > time); i++)
		{
		}
		i = Mathf.Min(i, Waypoints.Count - 2);
		float num = (time - Waypoints[i].Time) / (Waypoints[i + 1].Time - Waypoints[i].Time);
		float num2 = num * num;
		float num3 = num2 * num;
		SplinePoint splinePoint = new SplinePoint();
		splinePoint.Position = (2f * num3 - 3f * num2 + 1f) * Waypoints[i].Position + (num3 - 2f * num2 + num) * Waypoints[i].Tangent + (num3 - num2) * Waypoints[i + 1].Tangent + (-2f * num3 + 3f * num2) * Waypoints[i + 1].Position;
		splinePoint.Tangent = (6f * num2 - 6f * num) * Waypoints[i].Position + (3f * num2 - 4f * num + 1f) * Waypoints[i].Tangent + (3f * num2 - 2f * num) * Waypoints[i + 1].Tangent + (-6f * num2 + 6f * num) * Waypoints[i + 1].Position;
		Vector3 vector = ((!(splinePoint.Position != Waypoints[i].Position)) ? Waypoints[i + 1].Position : (Waypoints[i].Position - splinePoint.Position));
		if (vector != Vector3.zero && vector != splinePoint.Tangent)
		{
			splinePoint.Normal = Vector3.Cross(splinePoint.Tangent, vector);
			splinePoint.Binormal = Vector3.Cross(splinePoint.Tangent, splinePoint.Normal);
			if (splinePoint.Binormal.y > 0f)
			{
				splinePoint.Binormal = -splinePoint.Binormal;
			}
		}
		return splinePoint;
	}

	private void Visualize(float interval)
	{
		if (Waypoints.Count == 0)
		{
			return;
		}
		ClearVisualization();
		_visualized = true;
		if (MArrow != null)
		{
			foreach (MilMo_WayPoint waypoint in Waypoints)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(MArrow);
				gameObject.SetActive(value: true);
				SplinePoint pointAtTime = GetPointAtTime(waypoint.Time);
				gameObject.transform.position = pointAtTime.Position;
				gameObject.transform.rotation = Quaternion.LookRotation(pointAtTime.Tangent, pointAtTime.Normal);
				_visualization.Add(gameObject);
			}
		}
		if (MDot != null)
		{
			for (float num = Waypoints[0].Time + interval; num <= Waypoints[Waypoints.Count - 1].Time; num += interval)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(MDot);
				gameObject2.SetActive(value: true);
				SplinePoint pointAtTime2 = GetPointAtTime(num);
				gameObject2.transform.position = pointAtTime2.Position;
				gameObject2.transform.rotation = Quaternion.LookRotation(pointAtTime2.Tangent, pointAtTime2.Normal);
				_visualization.Add(gameObject2);
			}
		}
	}

	public void ClearVisualization()
	{
		foreach (GameObject item in _visualization)
		{
			UnityEngine.Object.Destroy(item);
		}
		_visualization.Clear();
		_visualized = false;
	}

	public void Clear()
	{
		Waypoints.Clear();
		_tangentsGenerated = false;
		ClearVisualization();
	}

	public bool IsAtEnd(float time)
	{
		if (Waypoints.Count != 0)
		{
			return time >= Waypoints[Waypoints.Count - 1].Time;
		}
		return true;
	}

	public Vector3 GetEndpoint()
	{
		if (Waypoints.Count != 0)
		{
			return Waypoints[Waypoints.Count - 1].Position;
		}
		return Vector3.zero;
	}

	protected MilMo_Spline GetInvertedSpline()
	{
		if (!_tangentsGenerated)
		{
			GenerateInnerTangents();
		}
		MilMo_Spline milMo_Spline = new MilMo_Spline();
		float time = Waypoints[Waypoints.Count - 1].Time;
		for (int num = Waypoints.Count - 1; num >= 0; num--)
		{
			milMo_Spline.Waypoints.Add(new MilMo_WayPoint(Waypoints[num].Position, -Waypoints[num].Tangent, time - Waypoints[num].Time));
		}
		milMo_Spline.Template = Template;
		milMo_Spline._tangentsGenerated = true;
		return milMo_Spline;
	}

	private static string Debug_VisualizeSplines(string[] args)
	{
		if (args.Length < 2)
		{
			_mVisualizeSplines = true;
			return "Spline visualization enabled";
		}
		_mVisualizeSplines = MilMo_Utility.StringToInt(args[1]) != 0;
		if (!_mVisualizeSplines)
		{
			return "Spline visualization disabled";
		}
		return "Spline visualization enabled";
	}
}
