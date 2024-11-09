using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Avatar;

public class SkinLayerSections
{
	private readonly Dictionary<SkinLayerSection, Rect> _skinLayerUvMappingBoy = new Dictionary<SkinLayerSection, Rect>();

	private readonly Dictionary<SkinLayerSection, Rect> _skinLayerUvMappingGirl = new Dictionary<SkinLayerSection, Rect>();

	public SkinLayerSections()
	{
		AddBoySkinLayerSections();
		AddGirlSkinLayerSections();
	}

	public Rect GetSkinLayerSection(bool forBoy, string sectionName)
	{
		if (!Enum.TryParse<SkinLayerSection>(sectionName, ignoreCase: true, out var result))
		{
			Debug.LogWarning("Could not find SkinLayerSection type: " + sectionName);
			return Rect.zero;
		}
		Rect? rect = (forBoy ? GetSkinLayerSectionForBoy(result) : GetSkinLayerSectionForGirl(result));
		if (!rect.HasValue)
		{
			Debug.LogWarning("Could not find SkinLayerSection: " + sectionName + " " + (forBoy ? "for boy" : "for girl"));
			return Rect.zero;
		}
		return rect.Value;
	}

	private Rect? GetSkinLayerSectionForBoy(SkinLayerSection section)
	{
		if (!_skinLayerUvMappingBoy.TryGetValue(section, out var value))
		{
			return null;
		}
		return value;
	}

	private Rect? GetSkinLayerSectionForGirl(SkinLayerSection section)
	{
		if (!_skinLayerUvMappingGirl.TryGetValue(section, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddBoySkinLayerSections()
	{
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Head, 92, 873, 351, 1024);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Legs, 334, 870, 512, 1024);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Arms, 1, 725, 138, 848);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Hand, 0, 847, 110, 1024);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Pelvis, 128, 769, 512, 880);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Torso, 0, 512, 512, 769);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Feet, 366, 699, 496, 812);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Ear, 227, 733, 287, 772);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Neck, 132, 752, 288, 798);
		AddSection(_skinLayerUvMappingBoy, SkinLayerSection.Face, 512, 0, 1024, 512);
	}

	private void AddGirlSkinLayerSections()
	{
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Head, 90, 872, 355, 1024);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Legs, 345, 837, 512, 1024);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Arms, 0, 718, 144, 849);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Hand, 0, 848, 111, 1024);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Pelvis, 125, 762, 445, 872);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Torso, 0, 512, 512, 768);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Feet, 394, 705, 511, 829);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Ear, 0, 0, 0, 0);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Neck, 0, 512, 67, 632);
		AddSection(_skinLayerUvMappingGirl, SkinLayerSection.Face, 512, 0, 1024, 512);
	}

	private void AddSection(Dictionary<SkinLayerSection, Rect> uvMapping, SkinLayerSection section, int startU, int startV, int endU, int endV)
	{
		uvMapping.Add(section, NormalizeRect(new Rect(startU, 1024 - endV, endU - startU, endV - startV)));
	}

	private static Rect NormalizeRect(Rect r)
	{
		return new Rect(r.xMin / 1024f, r.yMin / 1024f, r.width / 1024f, r.height / 1024f);
	}
}
