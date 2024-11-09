using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_Lod
{
	public static float GlobalLodFactor = 1f;

	protected GameObject GameObject;

	public bool Culled;

	private readonly Vector3 _offsetToCenter = Vector3.zero;

	private bool _materialsFinished;

	private Vector3 _spin;

	private bool _billboard;

	private SkinQuality _skinQuality = SkinQuality.Bone1;

	private bool _skinNormals;

	private bool HaveSpin => _spin != Vector3.zero;

	public GameObject GameObjectRef
	{
		get
		{
			return GameObject;
		}
		set
		{
			GameObject = value;
			SetSkinQuality(_skinQuality, _skinNormals);
		}
	}

	public bool IsValid
	{
		get
		{
			if (!(GameObjectRef != null))
			{
				return Culled;
			}
			return true;
		}
	}

	public float DistanceSquared { get; private set; }

	public int Index { get; private set; }

	public MilMo_VisualRep ParentVisualRep { get; private set; }

	public List<MilMo_Material> Materials { get; private set; }

	public Renderer Renderer { get; protected set; }

	private SkinnedMeshRenderer SkinnedRenderer { get; set; }

	public MilMo_Lod(int index, MilMo_VisualRep parentVisualRep)
	{
		Materials = new List<MilMo_Material>();
		Index = index;
		ParentVisualRep = parentVisualRep;
		if ((bool)ParentVisualRep.GameObject)
		{
			Renderer component = ParentVisualRep.GameObject.GetComponent<Renderer>();
			if ((bool)component)
			{
				_offsetToCenter = ParentVisualRep.GameObject.transform.InverseTransformPoint(component.bounds.center);
			}
		}
	}

	public MilMo_Lod(int index, MilMo_VisualRep parentVisualRep, GameObject gameObject, float distance)
		: this(index, parentVisualRep)
	{
		GameObjectRef = gameObject;
		DistanceSquared = distance * distance;
	}

	public void SetDistance(float distanceInMeters)
	{
		DistanceSquared = distanceInMeters * distanceInMeters;
	}

	public void SetSkinQuality(SkinQuality skinQuality, bool skinNormals)
	{
		_skinQuality = skinQuality;
		if ((bool)GameObject && (bool)SkinnedRenderer)
		{
			SkinnedRenderer.quality = _skinQuality;
		}
	}

	public bool ShouldUse()
	{
		if (!GameObject)
		{
			return false;
		}
		float num = (((ParentVisualRep != null && (bool)ParentVisualRep.GameObject) ? ParentVisualRep.GameObject.transform : GameObject.transform).TransformPoint(_offsetToCenter) - MilMo_Global.Camera.transform.position).sqrMagnitude * GlobalLodFactor;
		return DistanceSquared < num;
	}

	public virtual void InitializeMaterials(MilMo_ResourceManager.Priority priority, bool pauseMode, bool async)
	{
		if (!GameObject || Culled)
		{
			return;
		}
		SkinnedRenderer = GameObject.GetComponent<SkinnedMeshRenderer>();
		Renderer = GameObject.GetComponent<Renderer>();
		if ((bool)Renderer)
		{
			if (Materials.Count > 0)
			{
				Material[] array = new Material[Materials.Count];
				int num = 0;
				foreach (MilMo_Material material in Materials)
				{
					material.Create(ParentVisualRep.Path, ParentVisualRep.Name, ParentVisualRep.AssetTag, priority, pauseMode, async);
					if (async)
					{
						material.SetColor(ParentVisualRep.DefaultColor);
					}
					array[num] = material.Material;
					num++;
				}
				Renderer.materials = array;
			}
			else
			{
				Renderer.material = ParentVisualRep.Renderer.sharedMaterial;
			}
		}
		else
		{
			Debug.LogWarning("Trying to load LOD without a renderer. " + ParentVisualRep.FullPath);
		}
	}

	private void FinishMaterials()
	{
		foreach (MilMo_Material material in Materials)
		{
			material.ResetColor();
		}
	}

	public void Update()
	{
		if (!_materialsFinished && Materials.All((MilMo_Material mat) => mat.IsDone()))
		{
			FinishMaterials();
			_materialsFinished = true;
		}
		if ((bool)GameObject && GameObject.activeInHierarchy)
		{
			if (HaveSpin)
			{
				GameObject.transform.Rotate(_spin.x * Time.deltaTime, _spin.y * Time.deltaTime, _spin.z * Time.deltaTime);
			}
			if (_billboard && (bool)MilMo_Global.MainCamera)
			{
				GameObject.transform.LookAt(MilMo_Global.MainCamera.transform.position);
			}
		}
	}

	public bool Load(MilMo_SFFile file)
	{
		if (file == null)
		{
			return false;
		}
		int num = 0;
		while (file.NextRow() && !file.IsNext("</LOD>") && !file.IsNext("</GIB>") && !file.IsNext("</MESHHELD>"))
		{
			if (file.IsNext("Distance"))
			{
				float @float = file.GetFloat();
				DistanceSquared = @float * @float;
			}
			else if (file.IsNext("null"))
			{
				GameObject = null;
				Culled = true;
			}
			else if (file.IsNext("SkinQuality"))
			{
				int @int = file.GetInt();
				if (@int >= 4)
				{
					_skinQuality = SkinQuality.Bone4;
				}
				else if (@int >= 2)
				{
					_skinQuality = SkinQuality.Bone2;
				}
				else
				{
					_skinQuality = SkinQuality.Bone1;
				}
			}
			else if (file.IsNext("SkinNormals"))
			{
				_skinNormals = true;
			}
			else if (file.IsNext("<MAT>") && MilMo_Material.LoadMaterial(Materials, file, num))
			{
				num++;
			}
		}
		return true;
	}

	public void FinishLoading()
	{
		_spin = ParentVisualRep.Spin;
		_billboard = ParentVisualRep.Billboard;
		if (ParentVisualRep.OnlyLodsUseBillboard)
		{
			_billboard = ParentVisualRep.OnlyLodsUseBillboard;
		}
		SetSkinQuality(_skinQuality, _skinNormals);
	}

	public virtual void Write(MilMo_SFFile file, MilMo_Lod template)
	{
		if (template == null)
		{
			return;
		}
		file.AddAndWrite("<LOD>");
		file.AddAndWrite("Index");
		file.Write(Index);
		if (template.DistanceSquared != DistanceSquared)
		{
			file.AddAndWrite("Distance");
			file.Write((DistanceSquared == 0f) ? 0f : Mathf.Sqrt(DistanceSquared));
		}
		if (template.Culled)
		{
			file.AddAndWrite("null");
		}
		if ((bool)Renderer)
		{
			int num = 0;
			Material[] sharedMaterials = Renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (num < template.Materials.Count)
				{
					MilMo_Material.WriteMaterial(material, file, num, template.Materials[num]);
				}
				else
				{
					MilMo_Material.WriteMaterial(material, file);
				}
				num++;
			}
		}
		file.AddAndWrite("</LOD>");
	}

	public static string Debug_GlobalLod(string[] args)
	{
		if (args.Length < 2)
		{
			return GlobalLodFactor.ToString(CultureInfo.InvariantCulture);
		}
		GlobalLodFactor = MilMo_Utility.StringToFloat(args[1]);
		return "Global LOD factor changed to " + GlobalLodFactor;
	}
}
