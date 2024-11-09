using System.Collections.Generic;
using System.Linq;
using Code.Core.Collision;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Visual;
using Code.World.Level;
using UnityEngine;

namespace Code.World.GroundMaterials;

public class MilMo_GroundMaterialManager
{
	private readonly string[] _groundTypes = new string[8];

	private Texture2D _firstMaterial;

	private Texture2D _secondMaterial;

	private Vector3 _terrainPosition;

	private float _terrainWidth;

	private float _terrainHeight;

	public void Load(MilMo_LevelData data, Vector3 terrainPosition, float terrainWidth, float terrainHeight)
	{
		MilMo_SFFile groundMaterials = data.GroundMaterials;
		if (groundMaterials == null)
		{
			return;
		}
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		while (groundMaterials.NextRow())
		{
			if (groundMaterials.IsNext("GROUNDMATERIALS"))
			{
				dictionary.Add(groundMaterials.GetLineNumber(), groundMaterials.GetString());
			}
		}
		groundMaterials.Reset();
		int num = 0;
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<int, string> item in dictionary)
			{
				if (MilMo_Global.EventTags.Contains(item.Value) || item.Value == "Default")
				{
					num = item.Key + 1;
				}
			}
		}
		int num2 = 0;
		while (groundMaterials.NextRow() && num2 != 8)
		{
			if (groundMaterials.GetLineNumber() >= num)
			{
				if (groundMaterials.IsNext("GROUNDMATERIALS"))
				{
					break;
				}
				string @string = groundMaterials.GetString();
				if (!MilMo_GroundMaterials.IsMaterial(@string))
				{
					Debug.LogWarning("Got invalid ground material " + @string + " in level " + data.VerboseName);
					num2++;
				}
				else
				{
					_groundTypes[num2++] = @string;
				}
			}
		}
		_firstMaterial = data.GroundMaterialPrimary;
		_secondMaterial = data.GroundMaterialSecondary;
		_terrainPosition = terrainPosition;
		_terrainWidth = terrainWidth;
		_terrainHeight = terrainHeight;
		groundMaterials.Reset();
	}

	public string GetMaterialAtPosition(Vector3 pos, out bool terrain)
	{
		terrain = true;
		if (Physics.Raycast(pos, Vector3.down, out var hitInfo, 1000f) && (double)(Mathf.Abs(MilMo_Physics.GetTerrainHeight(pos) - pos.y) - hitInfo.distance) > 0.1)
		{
			terrain = false;
			MilMo_VisualRepComponent component = hitInfo.collider.gameObject.GetComponent<MilMo_VisualRepComponent>();
			if (component == null)
			{
				return "";
			}
			MilMo_VisualRepData data = component.GetData();
			if (data == null)
			{
				return "";
			}
			string groundMaterial = data.groundMaterial;
			if (!string.IsNullOrEmpty(groundMaterial) && MilMo_GroundMaterials.IsMaterial(groundMaterial))
			{
				return groundMaterial;
			}
			return "";
		}
		Vector2 vector = default(Vector2);
		vector.x = (pos.x - _terrainPosition.x) / _terrainWidth;
		vector.y = (pos.z - _terrainPosition.z) / _terrainHeight;
		Vector2 vector2 = vector;
		Color color = new Color(0f, 0f, 0f, 0f);
		Color color2 = new Color(0f, 0f, 0f, 0f);
		if ((bool)_firstMaterial)
		{
			color = _firstMaterial.GetPixelBilinear(vector2.x, vector2.y);
		}
		if ((bool)_secondMaterial)
		{
			color2 = _secondMaterial.GetPixelBilinear(vector2.x, vector2.y);
		}
		int num = -1;
		float num2 = 0f;
		if (color.r > num2)
		{
			num = 0;
			num2 = color.r;
		}
		if (color.g > num2)
		{
			num = 1;
			num2 = color.g;
		}
		if (color.b > num2)
		{
			num = 2;
			num2 = color.b;
		}
		if (color.a > num2)
		{
			num = 3;
			num2 = color.a;
		}
		if (color2.r > num2)
		{
			num = 4;
			num2 = color2.r;
		}
		if (color2.g > num2)
		{
			num = 5;
			num2 = color2.g;
		}
		if (color2.b > num2)
		{
			num = 6;
			num2 = color2.b;
		}
		if (color2.a > num2)
		{
			num = 7;
		}
		if (num >= 0 && num < _groundTypes.Length)
		{
			return _groundTypes[num];
		}
		return "";
	}
}
