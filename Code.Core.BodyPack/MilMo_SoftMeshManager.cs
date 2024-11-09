using System;
using System.Collections.Generic;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack;

public class MilMo_SoftMeshManager
{
	private readonly SkinnedMeshRenderer _renderer;

	private readonly bool _boy;

	private readonly List<MilMo_SoftMesh> _currentSoftMeshes = new List<MilMo_SoftMesh>();

	private Texture2D _atlas;

	private Texture2D _nextAtlas;

	private int _maxAtlasSize = MilMo_BodyPackSystem.MaxAtlasSize;

	private Rect[] _currentUVRects;

	private Mesh _mesh;

	private bool _dirtyAtlas = true;

	private bool _dirtyMesh = true;

	private bool _enabled = true;

	private readonly Material _coloringMaterial;

	private Dictionary<string, int> _colorIndices;

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int MainColor = Shader.PropertyToID("_Color");

	private const string SHADER = "Shaders/skinLayerShader";

	public int MaxAtlasSize
	{
		get
		{
			return _maxAtlasSize;
		}
		set
		{
			if (_maxAtlasSize != value)
			{
				_maxAtlasSize = value;
				_dirtyAtlas = true;
				_dirtyMesh = true;
			}
		}
	}

	public Dictionary<string, int> ColorIndices
	{
		get
		{
			return _colorIndices;
		}
		set
		{
			_colorIndices = value;
			_dirtyAtlas = true;
		}
	}

	public MilMo_SoftMeshManager(SkinnedMeshRenderer renderer, bool boy)
	{
		_renderer = renderer;
		_boy = boy;
		SetupDefaultMaterialValues();
		Shader shader = MilMo_ResourceManager.LoadShaderLocal("Shaders/skinLayerShader");
		_coloringMaterial = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	public void Destroy()
	{
		MilMo_Global.Destroy(_nextAtlas);
		MilMo_Global.Destroy(_atlas);
		MilMo_Global.Destroy(_mesh);
	}

	public void UpdateSoftMeshes(IEnumerable<MilMo_BodyPack> bodyPacks)
	{
		_currentSoftMeshes.Clear();
		foreach (MilMo_BodyPack bodyPack in bodyPacks)
		{
			foreach (MilMo_SoftMesh softMesh in bodyPack.SoftMeshes)
			{
				if (softMesh.HasContent)
				{
					_currentSoftMeshes.Add(softMesh);
				}
				else
				{
					Debug.Log("Update soft meshes with no content " + softMesh.BodyPackPath);
				}
			}
		}
		_dirtyAtlas = true;
		_dirtyMesh = true;
	}

	private void RebuildAtlas()
	{
		_nextAtlas = new Texture2D(8, 8, TextureFormat.DXT1, mipChain: false);
		List<Texture2D> list = new List<Texture2D>();
		foreach (MilMo_SoftMesh currentSoftMesh in _currentSoftMeshes)
		{
			RenderTexture active = RenderTexture.active;
			ColorGroup colorGroup = currentSoftMesh.SoftMeshLayers[0].ColorGroup;
			Texture2D texture = currentSoftMesh.SoftMeshLayers[0].Texture;
			RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 1);
			temporary.isPowerOfTwo = MilMo_ColorShaderUtil.IsPowerOfTwo(temporary.width) && MilMo_ColorShaderUtil.IsPowerOfTwo(temporary.height);
			RenderTexture.active = temporary;
			GL.Clear(clearDepth: true, clearColor: true, Color.clear);
			GL.PushMatrix();
			GL.LoadOrtho();
			_coloringMaterial.mainTexture = texture;
			_coloringMaterial.SetPass(0);
			MilMo_ColorShaderUtil.DrawQuad(new Rect(0f, 0f, 1f, 1f));
			if (_colorIndices != null && colorGroup != null && _colorIndices.ContainsKey(currentSoftMesh.BodyPackPath + ":" + colorGroup.GroupName))
			{
				try
				{
					MilMo_BodyPackSystem.GetColorFromIndex(_colorIndices[currentSoftMesh.BodyPackPath + ":" + colorGroup.GroupName]).Apply(new Rect(0f, 0f, 1f, 1f), texture);
				}
				catch (ArgumentOutOfRangeException)
				{
					Debug.LogWarning("Got invalid color index for SoftMesh in BodyPack " + currentSoftMesh.BodyPackPath + ".");
				}
			}
			IList<MilMo_TextureColorGroupPair> softMeshLayers = currentSoftMesh.SoftMeshLayers;
			for (int i = 1; i < softMeshLayers.Count; i++)
			{
				ColorGroup colorGroup2 = softMeshLayers[i].ColorGroup;
				Texture2D texture2 = softMeshLayers[i].Texture;
				Vector2 uVOffset = softMeshLayers[i].UVOffset;
				Rect rect = default(Rect);
				rect.x = uVOffset.x / (float)texture.width;
				rect.y = ((float)texture.height - uVOffset.y - (float)texture2.height) / (float)texture.height;
				rect.width = (float)texture2.width / (float)texture.width;
				rect.height = (float)texture2.height / (float)texture.height;
				Rect r = rect;
				if (_colorIndices != null && colorGroup2 != null && _colorIndices.ContainsKey(currentSoftMesh.BodyPackPath + ":" + colorGroup2.GroupName))
				{
					try
					{
						MilMo_BodyPackSystem.GetColorFromIndex(_colorIndices[currentSoftMesh.BodyPackPath + ":" + colorGroup2.GroupName]).Apply(r, texture, texture2);
					}
					catch (ArgumentOutOfRangeException)
					{
						Debug.LogWarning("Got invalid color index for SoftMesh in BodyPack " + currentSoftMesh.BodyPackPath + ".");
					}
				}
				else
				{
					_coloringMaterial.mainTexture = texture2;
					_coloringMaterial.SetPass(0);
					MilMo_ColorShaderUtil.DrawQuad(r);
				}
			}
			Texture2D texture2D = new Texture2D(texture.width, texture.height);
			texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
			GL.PopMatrix();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.active = active;
			list.Add(texture2D);
		}
		if (list.Count > 0)
		{
			_currentUVRects = _nextAtlas.PackTextures(list.ToArray(), 0, _maxAtlasSize);
		}
		foreach (Texture2D item in list)
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	private void RebuildMesh()
	{
		MilMo_BlendMesh milMo_BlendMesh = (_boy ? MilMo_BodyPackSystem.MaleBlendMesh : MilMo_BodyPackSystem.FemaleBlendMesh);
		int num = milMo_BlendMesh.Vertices.Length;
		int num2 = milMo_BlendMesh.Normals.Length;
		int num3 = milMo_BlendMesh.Triangles.Length;
		int num4 = 0;
		int num5 = milMo_BlendMesh.UV.Length;
		int num6 = milMo_BlendMesh.BoneWeights.Length;
		if (_enabled)
		{
			foreach (MilMo_SoftMesh currentSoftMesh in _currentSoftMeshes)
			{
				num += currentSoftMesh.BlendMesh.Vertices.Length;
				num2 += currentSoftMesh.BlendMesh.Normals.Length;
				num4 += currentSoftMesh.BlendMesh.Triangles.Length;
				num5 += currentSoftMesh.BlendMesh.UV.Length;
				num6 += currentSoftMesh.BlendMesh.BoneWeights.Length;
			}
		}
		Vector3[] verts = new Vector3[num];
		Vector3[] normals = new Vector3[num2];
		Color[] colors = new Color[num];
		int[] triangles = new int[num3];
		int[] triangles2 = new int[num4];
		Vector2[] uvs = new Vector2[num5];
		BoneWeight[] boneWeights = new BoneWeight[num6];
		MilMo_BlendMesh.CopyToArrays(milMo_BlendMesh, ref verts, ref colors, ref uvs, ref boneWeights, ref normals, ref triangles, 0, 0, new Rect(0f, 0f, 1f, 1f), copyColors: true);
		int num7 = milMo_BlendMesh.Vertices.Length;
		int num8 = 0;
		if (_enabled)
		{
			int num9 = 0;
			foreach (MilMo_SoftMesh currentSoftMesh2 in _currentSoftMeshes)
			{
				MilMo_BlendMesh.CopyToArrays(uvOffset: (_currentUVRects == null || num9 >= _currentUVRects.Length) ? new Rect(0f, 0f, 0f, 0f) : _currentUVRects[num9], from: currentSoftMesh2.BlendMesh, verts: ref verts, colors: ref colors, uvs: ref uvs, boneWeights: ref boneWeights, normals: ref normals, triangles: ref triangles2, startPosVerts: num7, startPosTriangles: num8, copyColors: false);
				num7 += currentSoftMesh2.BlendMesh.Vertices.Length;
				num8 += currentSoftMesh2.BlendMesh.Triangles.Length;
				num9++;
			}
		}
		MilMo_Global.Destroy(_mesh);
		_mesh = new Mesh
		{
			vertices = verts,
			colors = colors,
			uv = uvs,
			boneWeights = boneWeights,
			normals = normals
		};
		if (_enabled)
		{
			_mesh.subMeshCount = 2;
			_mesh.SetTriangles(triangles, 0);
			_mesh.SetTriangles(triangles2, 1);
		}
		else
		{
			_mesh.subMeshCount = 1;
			_mesh.SetTriangles(triangles, 0);
		}
		_mesh.bindposes = milMo_BlendMesh.BindPoses;
		_mesh.name = "CombinedPlayerMesh";
	}

	public void ApplyChanges()
	{
		if (_dirtyAtlas || _dirtyMesh)
		{
			SetupDefaultMaterialValues();
		}
		if (_dirtyAtlas)
		{
			RebuildAtlas();
			MilMo_Global.Destroy(_atlas);
			_atlas = _nextAtlas;
			if (_renderer.materials.Length > 1)
			{
				_renderer.materials[1].mainTexture = _atlas;
			}
		}
		if (_dirtyMesh)
		{
			RebuildMesh();
			_renderer.sharedMesh = _mesh;
		}
		_dirtyAtlas = false;
		_dirtyMesh = false;
	}

	public void Enable()
	{
		if (!_enabled)
		{
			_dirtyAtlas = true;
			_dirtyMesh = true;
			_enabled = true;
		}
	}

	public void Disable()
	{
		if (_enabled)
		{
			_dirtyMesh = true;
			MilMo_Global.Destroy(_atlas);
			MilMo_Global.Destroy(_nextAtlas);
			_enabled = false;
		}
	}

	private void SetupDefaultMaterialValues()
	{
		if (_enabled)
		{
			if (_renderer.materials.Length < 2)
			{
				Material material = _renderer.materials[0];
				_renderer.materials = new Material[2];
				_renderer.materials[0].shader = material.shader;
				_renderer.materials[0].CopyPropertiesFromMaterial(material);
				MilMo_Global.Destroy(material);
			}
			_renderer.materials[1] = new Material(MilMo_BodyPackSystem.GetAvatarShader(highQuality: false));
			_renderer.materials[1].SetTexture(Ramp, MilMo_BodyPackSystem.ShaderRamp);
			_renderer.materials[1].SetVector(VelvetChannel, MilMo_BodyPackSystem.CharacterVelvetChannels);
			_renderer.materials[1].SetColor(VelvetColor, MilMo_BodyPackSystem.CharacterVelvetColor);
			_renderer.materials[1].SetColor(MainColor, MilMo_BodyPackSystem.CharacterMainColor);
			_renderer.materials[1].shader = MilMo_BodyPackSystem.GetAvatarShader(highQuality: false);
		}
		else if (_renderer.materials.Length > 1)
		{
			MilMo_Global.Destroy(_renderer.materials[1]);
			Material material2 = _renderer.materials[0];
			_renderer.materials = new Material[1];
			_renderer.materials[0].shader = material2.shader;
			_renderer.materials[0].CopyPropertiesFromMaterial(material2);
			MilMo_Global.Destroy(material2);
		}
	}
}
