using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.Level.LevelInfo;

public class MilMo_WorldImageInfoData
{
	public float ScrollerMultiplier { get; private set; }

	public bool IsStatic { get; private set; }

	public MilMo_ResourceManager.Priority Priority { get; private set; }

	public MilMo_Mover.UpdateFunc PosMover { get; private set; }

	public Vector2 PosMoverVel { get; private set; }

	public bool PosMoverLooping { get; private set; }

	public Vector2 PosMoverLoopReset { get; private set; }

	public Vector2 PosMoverLoopValue { get; private set; }

	public string World { get; }

	public string Texture { get; private set; }

	public Vector2 Position { get; private set; }

	public Vector2 Scale { get; private set; }

	public MilMo_WorldImageInfoData(string world)
	{
		PosMoverLoopReset = Vector2.zero;
		PosMoverVel = Vector2.zero;
		PosMover = MilMo_Mover.UpdateFunc.Nothing;
		Priority = MilMo_ResourceManager.Priority.Medium;
		ScrollerMultiplier = 1f;
		PosMoverLoopValue = Vector2.zero;
		Scale = Vector2.zero;
		World = world;
	}

	public bool Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("</IMAGE>"))
			{
				return true;
			}
			if (file.IsNext("FileName"))
			{
				Texture = file.GetString();
			}
			else if (file.IsNext("IsStatic"))
			{
				IsStatic = true;
			}
			else if (file.IsNext("ScrollMultiplier"))
			{
				ScrollerMultiplier = file.GetFloat();
			}
			else if (file.IsNext("Position"))
			{
				Position = file.GetVector2();
			}
			else if (file.IsNext("Scale"))
			{
				Scale = file.GetVector2();
			}
			else if (file.IsNext("PosMover.Looping"))
			{
				PosMoverLooping = true;
			}
			else if (file.IsNext("PosMover.Vel"))
			{
				PosMoverVel = file.GetVector2();
			}
			else if (file.IsNext("PosMover.LoopReset"))
			{
				PosMoverLoopReset = file.GetVector2();
			}
			else if (file.IsNext("PosMover.LoopVal"))
			{
				PosMoverLoopValue = file.GetVector2();
			}
			else if (file.IsNext("PosMover.UpdateFunc"))
			{
				PosMover = file.GetString().ToUpper() switch
				{
					"LINEAR" => MilMo_Mover.UpdateFunc.Linear, 
					"NOTHING" => MilMo_Mover.UpdateFunc.Nothing, 
					"SINUS" => MilMo_Mover.UpdateFunc.Sinus, 
					"SPRING" => MilMo_Mover.UpdateFunc.Spring, 
					_ => PosMover, 
				};
			}
			else if (file.IsNext("Priority"))
			{
				string text = file.GetString().ToUpper();
				MilMo_ResourceManager.Priority priority = ((!(text == "HIGH")) ? ((!(text == "LOW")) ? Priority : MilMo_ResourceManager.Priority.Low) : MilMo_ResourceManager.Priority.High);
				Priority = priority;
			}
		}
		Debug.LogWarning($"Failed to load World image information in {file.Path} on line {file.GetLineNumber()}");
		return false;
	}
}
