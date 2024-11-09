using Code.World.GUI;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public interface IMilMoMarkableObject
{
	bool PlayerIsInside { get; }

	float SqrDistanceToPlayer { get; }

	int UsePrio { get; }

	bool PositionIsInside(Vector3 position);

	MilMoObjectMarker CreateMarker();

	void UseReaction();

	void MarkerRemoved();
}
