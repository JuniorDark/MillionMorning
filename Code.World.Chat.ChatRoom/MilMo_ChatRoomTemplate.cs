using System.Collections.Generic;
using Code.Core.Items.Home;
using Code.Core.ResourceSystem;
using Code.Core.Utility;

namespace Code.World.Chat.ChatRoom;

public class MilMo_ChatRoomTemplate
{
	private readonly List<MilMo_Transform> _mMainCameraPositions = new List<MilMo_Transform>();

	private readonly List<MilMo_SitPointTemplate> _mSitPoints = new List<MilMo_SitPointTemplate>();

	private readonly long _mId;

	public List<MilMo_Transform> MainCameraPositions => _mMainCameraPositions;

	public List<MilMo_SitPointTemplate> SitPoints => _mSitPoints;

	public long Id => _mId;

	public MilMo_ChatRoomTemplate(MilMo_SFFile propsFile)
	{
		while (propsFile.NextRow() && !propsFile.IsNext("</CHATROOM>"))
		{
			if (propsFile.IsNext("Id"))
			{
				_mId = propsFile.GetInt();
			}
			else if (propsFile.IsNext("<CAMERA>"))
			{
				MilMo_Transform milMo_Transform = new MilMo_Transform();
				while (propsFile.NextRow() && !propsFile.IsNext("</CAMERA>"))
				{
					if (propsFile.IsNext("Position"))
					{
						milMo_Transform.Position = propsFile.GetVector3();
					}
					else if (propsFile.IsNext("Rotation"))
					{
						milMo_Transform.EulerRotation = propsFile.GetVector3();
					}
				}
				_mMainCameraPositions.Add(milMo_Transform);
			}
			else if (propsFile.IsNext("<EXITNODE>"))
			{
				while (propsFile.NextRow() && !propsFile.IsNext("</EXITNODE>"))
				{
				}
			}
			else if (propsFile.IsNext("<SITNODE>"))
			{
				MilMo_SitPointTemplate item = new MilMo_SitPointTemplate(propsFile);
				_mSitPoints.Add(item);
			}
		}
	}
}
