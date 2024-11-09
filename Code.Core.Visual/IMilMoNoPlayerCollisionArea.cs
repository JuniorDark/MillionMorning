using UnityEngine;

namespace Code.Core.Visual;

public interface IMilMoNoPlayerCollisionArea
{
	bool Enabled { get; }

	Vector3 Position { get; }

	float SqrRadius { get; }
}
