using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.Items.Home;

public sealed class MilMo_AttachNodeTemplate
{
	public short Id { get; private set; }

	public Vector3 Position { get; private set; }

	public Vector3 Rotation { get; private set; }

	public MilMo_AttachNodeTemplate(FurnitureAttachNode attachNode)
	{
		Id = attachNode.GetId();
		Position = new Vector3(attachNode.GetPosition().GetX(), attachNode.GetPosition().GetY(), attachNode.GetPosition().GetZ());
		Rotation = new Vector3(attachNode.GetRotation().GetX(), attachNode.GetRotation().GetY(), attachNode.GetRotation().GetZ());
	}
}
