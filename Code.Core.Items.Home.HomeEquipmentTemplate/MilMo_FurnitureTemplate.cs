using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipmentTemplate;

public abstract class MilMo_FurnitureTemplate : MilMo_HomeEquipmentTemplate
{
	private string _mIconPath;

	private readonly IList<MilMo_FurnitureState> _mStates = new List<MilMo_FurnitureState>();

	private readonly List<MilMo_AttachNodeTemplate> _mAttachNodes = new List<MilMo_AttachNodeTemplate>();

	public List<MilMo_AttachNodeTemplate> AttachNodes => _mAttachNodes;

	public string HomePack { get; private set; }

	public IList<MilMo_FurnitureState> States => _mStates;

	public bool IsDoor { get; private set; }

	public string DoorEnterSound { get; private set; }

	public string DoorExitSound { get; private set; }

	public virtual bool IsCarpet => false;

	public virtual bool IsCurtain => false;

	public override string IconPath => _mIconPath;

	public override string ExternThumbnailURL => _mIconPath.Substring("Content/".Length) + ".png";

	public override bool IsSkin => IsDoor;

	protected MilMo_FurnitureTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			Debug.LogWarning("Failed to load furniture template " + t.GetTemplateType());
			return false;
		}
		if (!(t is Code.Core.Network.types.FurnitureTemplate furnitureTemplate))
		{
			return true;
		}
		HomePack = furnitureTemplate.GetHomePack();
		IsDoor = furnitureTemplate.GetIsDoor() != 0;
		DoorEnterSound = furnitureTemplate.GetDoorEnterSound();
		DoorExitSound = furnitureTemplate.GetDoorExitSound();
		string text = HomePack.Replace('.', '/');
		string text2 = MilMo_Utility.RemoveFileNameFromFullPath(text);
		string text3 = MilMo_Utility.ExtractNameFromPath(text);
		_mIconPath = "Content/Homes/" + text2 + "Icons/Icon" + text3;
		foreach (FurnitureState state in furnitureTemplate.GetStates())
		{
			_mStates.Add(new MilMo_FurnitureState(state.GetActions()));
		}
		foreach (FurnitureAttachNode attachNode in furnitureTemplate.GetAttachNodes())
		{
			MilMo_AttachNodeTemplate item = new MilMo_AttachNodeTemplate(attachNode);
			_mAttachNodes.Add(item);
		}
		return true;
	}
}
