using Code.Core.Network.types;
using Code.Core.Utility;

namespace Code.Core.Items.Home.HomeEquipmentTemplate;

public abstract class MilMo_HomeSurfaceTemplate : MilMo_HomeEquipmentTemplate
{
	private string _mIconPath;

	public string HomeSurface { get; private set; }

	public override string IconPath => _mIconPath;

	public override string ExternThumbnailURL => _mIconPath.Substring("Content/".Length) + ".png";

	protected MilMo_HomeSurfaceTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is Code.Core.Network.types.HomeSurfaceTemplate homeSurfaceTemplate))
		{
			return false;
		}
		HomeSurface = homeSurfaceTemplate.GetHomeSurface();
		string text = HomeSurface.Replace('.', '/');
		string text2 = MilMo_Utility.RemoveFileNameFromFullPath(text);
		string text3 = MilMo_Utility.ExtractNameFromPath(text);
		_mIconPath = "Content/Homes/" + text2 + "Icons/Icon" + text3;
		return true;
	}
}
