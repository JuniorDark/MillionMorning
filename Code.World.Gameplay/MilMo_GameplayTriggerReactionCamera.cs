using Code.Core.EventSystem;
using Code.World.Player;

namespace Code.World.Gameplay;

public sealed class MilMo_GameplayTriggerReactionCamera : MilMo_GameplayTriggerReaction
{
	private readonly string _cameraEvent;

	public MilMo_GameplayTriggerReactionCamera(string cameraEvent)
	{
		_cameraEvent = cameraEvent;
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		MilMo_EventSystem.At(0.1f, delegate
		{
			MilMo_World.Instance.Camera.CameraEvent(_cameraEvent);
		});
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
	}
}
