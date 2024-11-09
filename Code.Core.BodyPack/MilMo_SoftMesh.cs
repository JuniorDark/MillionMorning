using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_SoftMesh
{
	public delegate void SoftMeshDone(bool success);

	private readonly string _fullMeshPath;

	public MilMo_BlendMesh BlendMesh { get; private set; }

	public IList<MilMo_TextureColorGroupPair> SoftMeshLayers { get; }

	public string BodyPackPath { get; }

	public bool HasContent
	{
		get
		{
			if (SoftMeshLayers.Any((MilMo_TextureColorGroupPair softmeshLayer) => softmeshLayer.Texture == null))
			{
				return false;
			}
			return BlendMesh != null;
		}
	}

	public bool DoneLoading { get; private set; }

	public MilMo_SoftMesh(string meshPath, IList<MilMo_TextureColorGroupPair> softMeshLayers, string bodyPackPath)
	{
		BodyPackPath = bodyPackPath;
		SoftMeshLayers = softMeshLayers;
		_fullMeshPath = "Content/Bodypacks/" + meshPath;
	}

	public async void AsyncLoadContent(SoftMeshDone callback)
	{
		if (HasContent)
		{
			DoneLoading = true;
			callback(success: true);
			return;
		}
		await LoadMesh();
		foreach (MilMo_TextureColorGroupPair softMeshLayer in SoftMeshLayers)
		{
			string path = "Content/Bodypacks/" + softMeshLayer.TextureName;
			softMeshLayer.Texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(path);
		}
		DoneLoading = true;
		callback(HasContent);
	}

	private async Task LoadMesh()
	{
		GameObject gameObject = await MilMo_ResourceManager.Instance.LoadMeshAsync(_fullMeshPath);
		if (gameObject == null)
		{
			Debug.LogWarning("SoftMesh: Could not load game object for " + BodyPackPath + " at " + _fullMeshPath);
			return;
		}
		SkinnedMeshRenderer componentInChildren = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("SoftMesh: Could not find any SkinnedMeshRenderer for " + BodyPackPath + " in file " + _fullMeshPath);
		}
		else
		{
			LoadMeshData(componentInChildren.sharedMesh);
		}
	}

	private void LoadMeshData(Mesh mesh)
	{
		if (!(mesh == null))
		{
			BlendMesh = new MilMo_BlendMesh
			{
				Vertices = mesh.vertices,
				Normals = mesh.normals,
				Colors = mesh.colors,
				Triangles = mesh.triangles,
				UV = mesh.uv,
				BoneWeights = mesh.boneWeights
			};
		}
	}
}
