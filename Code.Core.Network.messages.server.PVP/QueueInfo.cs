using Code.World.GUI.PVP;

namespace Code.Core.Network.messages.server.PVP;

public class QueueInfo
{
	private sbyte matchMode;

	private short queueSize;

	private short maxQueueSize;

	private short scoreGoal;

	private bool canJoinNow;

	public MilMo_MatchMode MatchMode => (MilMo_MatchMode)matchMode;

	public short QueueSize => queueSize;

	public short MaxQueueSize => maxQueueSize;

	public short ScoreGoal => scoreGoal;

	public bool CanJoinNow => canJoinNow;

	public QueueInfo(MessageReader reader)
	{
		matchMode = reader.ReadInt8();
		queueSize = reader.ReadInt16();
		maxQueueSize = reader.ReadInt16();
		scoreGoal = reader.ReadInt16();
		canJoinNow = reader.ReadInt8() == 1;
	}

	public QueueInfo(sbyte matchMode, short queueSize, short maxQueueSize, short scoreGoal, bool canJoinNow)
	{
		this.matchMode = matchMode;
		this.queueSize = queueSize;
		this.maxQueueSize = maxQueueSize;
		this.scoreGoal = scoreGoal;
		this.canJoinNow = canJoinNow;
	}

	public static int size()
	{
		return 8;
	}

	public void write(MessageWriter writer)
	{
		writer.WriteInt8(matchMode);
		writer.WriteInt16(queueSize);
		writer.WriteInt16(maxQueueSize);
		writer.WriteInt16(scoreGoal);
		writer.WriteInt8((sbyte)(canJoinNow ? 1 : 0));
	}
}
