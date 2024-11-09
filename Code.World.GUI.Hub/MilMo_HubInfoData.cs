using System.Collections.Generic;
using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Hub;

public class MilMo_HubInfoData
{
	public class MilMo_HubItemInfoData
	{
		public string Identifier = "";

		public string Model = "";

		public string IconTexture = "";

		public Vector3 Position = Vector2.zero;

		public Vector3 Scale = new Vector3(1f, 1f, 1f);

		public Vector3 MouseOverScale = new Vector3(1f, 1f, 1f);

		public Vector3 Rotation = Vector3.zero;

		public MilMo_LocString Description = MilMo_LocString.Empty;

		public MilMo_LocString Text = MilMo_LocString.Empty;

		public Vector2 StateIconOffset = Vector2.zero;

		public Vector2 TextOffset = Vector2.zero;

		public bool NoTitleInTown;
	}

	public class MilMo_HubModelData
	{
		public string Model = "";

		public Vector3 Position = Vector3.zero;

		public Vector3 Scale = new Vector3(1f, 1f, 1f);

		public Vector3 Rotation = Vector3.zero;
	}

	public class LightData
	{
		public float Intensity = 0.7f;

		public float Range = 200f;

		public Vector3 Position = new Vector3(-10000f, -9900f, -10000f);

		public Vector3 Rotation = new Vector3(35f, 0f, 0f);

		public Color AmbientLightColor = Color.white;

		public Color LightColor = Color.white;

		public Color FogColor = Color.white;

		public float FogDensity;

		public bool Fog;
	}

	private string m_Music = "";

	private readonly List<MilMo_HubItemInfoData> m_Items = new List<MilMo_HubItemInfoData>();

	private readonly List<MilMo_HubModelData> m_Models = new List<MilMo_HubModelData>();

	private readonly LightData m_LightData = new LightData();

	public string Music => m_Music;

	public List<MilMo_HubItemInfoData> Items => m_Items;

	public List<MilMo_HubModelData> Models => m_Models;

	public LightData Light => m_LightData;

	public MilMo_HubInfoData(MilMo_SFFile file)
	{
		if (file == null)
		{
			Debug.Log("Error. HUB Info Data file is null.");
		}
		Read(file);
	}

	private void Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("<MAP>"))
			{
				ReadHubData(file);
			}
			else if (file.IsNext("<ENTRY>"))
			{
				m_Items.Add(ReadItem(file));
			}
			else if (file.IsNext("<MODEL>"))
			{
				MilMo_HubModelData milMo_HubModelData = ReadModel(file);
				if (milMo_HubModelData != null)
				{
					m_Models.Add(milMo_HubModelData);
				}
			}
			else if (file.IsNext("<LIGHT>"))
			{
				ReadLight(file);
			}
		}
	}

	private void ReadLight(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</LIGHT>"))
		{
			if (file.IsNext("EventTag"))
			{
				string @string = file.GetString();
				if ((@string.StartsWith("!") && MilMo_Global.EventTags.Contains(@string.Substring(1))) || (!@string.StartsWith("!") && !MilMo_Global.EventTags.Contains(@string)))
				{
					break;
				}
			}
			if (file.IsNext("AmbientLightColor"))
			{
				m_LightData.AmbientLightColor = file.GetColor();
			}
			else if (file.IsNext("Color"))
			{
				m_LightData.LightColor = file.GetColor();
			}
			else if (file.IsNext("Intensity"))
			{
				m_LightData.Intensity = file.GetFloat();
			}
			else if (file.IsNext("Rotation"))
			{
				m_LightData.Rotation = file.GetVector3();
			}
			else if (file.IsNext("Fog"))
			{
				m_LightData.Fog = file.GetBool();
			}
			else if (file.IsNext("FogDensity"))
			{
				m_LightData.FogDensity = file.GetFloat();
			}
			else if (file.IsNext("FogColor"))
			{
				m_LightData.FogColor = file.GetColor();
			}
		}
	}

	private MilMo_HubItemInfoData ReadItem(MilMo_SFFile file)
	{
		MilMo_HubItemInfoData milMo_HubItemInfoData = new MilMo_HubItemInfoData();
		while (file.NextRow())
		{
			if (file.IsNext("</ENTRY>"))
			{
				return milMo_HubItemInfoData;
			}
			if (file.IsNext("Identifier"))
			{
				milMo_HubItemInfoData.Identifier = file.GetString();
			}
			else if (file.IsNext("Model"))
			{
				milMo_HubItemInfoData.Model = file.GetString();
			}
			else if (file.IsNext("IconTexture"))
			{
				milMo_HubItemInfoData.IconTexture = file.GetString();
			}
			else if (file.IsNext("Position"))
			{
				milMo_HubItemInfoData.Position = file.GetVector3();
			}
			else if (file.IsNext("Scale"))
			{
				milMo_HubItemInfoData.Scale = file.GetVector3();
			}
			else if (file.IsNext("StateIconOffset"))
			{
				milMo_HubItemInfoData.StateIconOffset = file.GetVector2();
			}
			if (file.IsNext("Rotation"))
			{
				milMo_HubItemInfoData.Rotation = file.GetVector3();
			}
			else if (file.IsNext("MouseoverScale"))
			{
				milMo_HubItemInfoData.MouseOverScale = file.GetVector3();
			}
			else if (file.IsNext("Description"))
			{
				milMo_HubItemInfoData.Description = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("Text"))
			{
				milMo_HubItemInfoData.Text = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("TextOffset"))
			{
				milMo_HubItemInfoData.TextOffset = file.GetVector2();
			}
			else if (file.IsNext("NoTitleInTown"))
			{
				milMo_HubItemInfoData.NoTitleInTown = true;
			}
		}
		return milMo_HubItemInfoData;
	}

	private MilMo_HubModelData ReadModel(MilMo_SFFile file)
	{
		MilMo_HubModelData milMo_HubModelData = new MilMo_HubModelData();
		while (file.NextRow())
		{
			if (file.IsNext("</MODEL>"))
			{
				return milMo_HubModelData;
			}
			if (file.IsNext("EventTag"))
			{
				string @string = file.GetString();
				if (@string.StartsWith("!") && MilMo_Global.EventTags.Contains(@string.Substring(1)))
				{
					return null;
				}
				if (!@string.StartsWith("!") && !MilMo_Global.EventTags.Contains(@string))
				{
					return null;
				}
			}
			if (file.IsNext("Model"))
			{
				milMo_HubModelData.Model = file.GetString();
			}
			else if (file.IsNext("Position"))
			{
				milMo_HubModelData.Position = file.GetVector3();
			}
			else if (file.IsNext("Rotation"))
			{
				milMo_HubModelData.Rotation = file.GetVector3();
			}
			else if (file.IsNext("Scale"))
			{
				milMo_HubModelData.Scale = file.GetVector3();
			}
		}
		return milMo_HubModelData;
	}

	private void ReadHubData(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAP>"))
		{
			if (file.IsNext("Music"))
			{
				m_Music = file.GetString();
			}
		}
	}
}
