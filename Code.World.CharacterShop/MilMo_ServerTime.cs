using System;
using Code.Core.Utility;

namespace Code.World.CharacterShop;

public class MilMo_ServerTime
{
	private long m_CurrentServerTime = -1L;

	private static MilMo_ServerTime m_Instance;

	public static MilMo_ServerTime Instance => m_Instance ?? (m_Instance = new MilMo_ServerTime());

	public DateTime GetServerTimeAsDateTime()
	{
		return MilMo_Utility.GetDateTimeFromMilliseconds(m_CurrentServerTime);
	}

	public void SetServerTimeInGMT(long timestamp)
	{
		m_CurrentServerTime = timestamp;
	}
}
