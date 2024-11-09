using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Code.Core.Spline;

public class MilMo_SplineTemplate : MilMo_Template
{
	public List<Waypoint> Waypoints { get; private set; }

	protected MilMo_SplineTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		Code.Core.Network.types.Spline spline = (Code.Core.Network.types.Spline)t;
		Waypoints = (List<Waypoint>)spline.GetWaypoints();
		return true;
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("Waypoint"))
		{
			vector3 position = new vector3(0f, 0f, 0f);
			vector3 rotation = new vector3(0f, 0f, 0f);
			float num = 0f;
			while (file.HasMoreTokens())
			{
				if (file.IsNext("Position"))
				{
					Vector3 vector = file.GetVector3();
					position = new vector3(vector.x, vector.y, vector.z);
				}
				else if (file.IsNext("Rotation"))
				{
					Vector3 vector2 = file.GetVector3();
					rotation = new vector3(vector2.x, vector2.y, vector2.z);
				}
				else if (file.IsNext("Time"))
				{
					num = file.GetFloat();
				}
				else
				{
					file.NextToken();
				}
			}
			bool flag = true;
			for (int i = 0; i < Waypoints.Count; i++)
			{
				if (num < Waypoints[i].GetTime())
				{
					Waypoints.Insert(i, new Waypoint(position, rotation, num));
					flag = false;
					break;
				}
				if (num == Waypoints[i].GetTime())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Waypoints.Add(new Waypoint(position, rotation, num));
			}
			return true;
		}
		return base.ReadLine(file);
	}

	public static MilMo_SplineTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_SplineTemplate(category, path, filePath, "Spline");
	}
}
