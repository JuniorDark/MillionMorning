using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Items.Home;

public sealed class MilMo_SitPointTemplate
{
	public short Id { get; private set; }

	public Vector3 Position { get; private set; }

	public Vector3 Rotation { get; private set; }

	public string Pose { get; private set; }

	public bool RelativeTransform { get; private set; }

	public MilMo_SitPointTemplate(MilMo_SFFile propsFile)
	{
		Pose = "Sit";
		while (propsFile.NextRow() && !propsFile.IsNext("</SITNODE>"))
		{
			if (propsFile.IsNext("Id"))
			{
				Id = (short)propsFile.GetInt();
			}
			else if (propsFile.IsNext("Position"))
			{
				Position = propsFile.GetVector3();
			}
			else if (propsFile.IsNext("Rotation"))
			{
				Rotation = propsFile.GetVector3();
			}
			else if (propsFile.IsNext("Pose"))
			{
				Pose = propsFile.GetString();
			}
			else if (propsFile.IsNext("<EXITNODE>"))
			{
				while (propsFile.NextRow() && !propsFile.IsNext("</EXITNODE>"))
				{
				}
			}
			else if (propsFile.IsNext("<CAMERA>"))
			{
				while (propsFile.NextRow() && !propsFile.IsNext("</CAMERA>"))
				{
				}
			}
		}
		RelativeTransform = false;
	}

	public MilMo_SitPointTemplate(SeatSitNode seatTemplateNode)
	{
		Id = seatTemplateNode.GetId();
		Pose = seatTemplateNode.GetPose();
		Position = new Vector3(seatTemplateNode.GetPosition().GetX(), seatTemplateNode.GetPosition().GetY(), seatTemplateNode.GetPosition().GetZ());
		Rotation = new Vector3(seatTemplateNode.GetRotation().GetX(), seatTemplateNode.GetRotation().GetY(), seatTemplateNode.GetRotation().GetZ());
		RelativeTransform = true;
	}
}
