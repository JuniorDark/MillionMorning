using System;
using System.Collections.Generic;
using Code.Core.Items;

namespace Code.World.CharBuilder;

public class AvatarSelection
{
	public enum Colors
	{
		Undefined,
		Skin,
		Eyes,
		Hair,
		Pants,
		Shirt,
		Shoes,
		Laces
	}

	public enum Shapes
	{
		Undefined,
		Hair,
		Eyes,
		EyeBrows,
		Mouth,
		Shirt,
		Pants,
		Shoes
	}

	public int HairColor = -1;

	public string ShirtColor;

	public string PantsColor;

	public string ShoesColor;

	public string LacesColor;

	public MilMo_Wearable ShirtItem;

	public MilMo_Wearable PantsItem;

	public MilMo_Wearable ShoesItem;

	public MilMo_Wearable HairStyleItem;

	public string AvatarName { get; private set; }

	public byte Gender { get; private set; }

	public float Height { get; private set; }

	public string Mood { get; private set; } = "Normal";


	public string Eyes { get; private set; }

	public string EyeBrows { get; private set; }

	public string Mouth { get; private set; }

	public IItem Shirt { get; private set; }

	public IItem Pants { get; private set; }

	public IItem Shoes { get; private set; }

	public IItem HairStyle { get; private set; }

	public string SkinColor { get; private set; }

	public string EyeColor { get; private set; }

	public void SetAvatarName(string name)
	{
		AvatarName = name;
	}

	public void SetGender(AvatarGender gender)
	{
		Gender = (byte)gender;
	}

	public void SetHeight(float height)
	{
		Height = height;
	}

	public void SetMood(string mood)
	{
		Mood = mood;
	}

	public void SetEyes(string eyes)
	{
		Eyes = eyes;
	}

	public void SetEyeBrows(string brows)
	{
		EyeBrows = brows;
	}

	public void SetMouth(string mouth)
	{
		Mouth = mouth;
	}

	public void SetShirt(IItem shirt)
	{
		Shirt = shirt;
	}

	public void SetPants(IItem pants)
	{
		Pants = pants;
	}

	public void SetShoes(IItem shoes)
	{
		Shoes = shoes;
	}

	public void SetHairStyle(IItem item)
	{
		HairStyle = item;
	}

	public void SetSkinColor(string color)
	{
		SkinColor = color;
	}

	public void SetEyeColor(string color)
	{
		EyeColor = color;
	}

	public AvatarSelection(byte gender, float height, string eyes, string eyebrows, string mouth, string skinColor, string eyeColor, int hairColor, IItem shirt, IItem pants, IItem shoes, IItem hairstyle)
	{
		Gender = gender;
		Height = height;
		Eyes = eyes;
		EyeBrows = eyebrows;
		Mouth = mouth;
		SkinColor = skinColor;
		EyeColor = eyeColor;
		HairColor = hairColor;
		Shirt = shirt;
		Pants = pants;
		Shoes = shoes;
		HairStyle = hairstyle;
	}

	public string GetColor(Colors color)
	{
		return color switch
		{
			Colors.Undefined => throw new ArgumentOutOfRangeException("color", color, null), 
			Colors.Skin => SkinColor, 
			Colors.Eyes => EyeColor, 
			Colors.Hair => HairColor.ToString(), 
			Colors.Pants => PantsColor, 
			Colors.Shirt => ShirtColor, 
			Colors.Shoes => ShoesColor, 
			Colors.Laces => LacesColor, 
			_ => throw new ArgumentOutOfRangeException("color", color, null), 
		};
	}

	public string GetShape(Shapes shape)
	{
		return shape switch
		{
			Shapes.Undefined => throw new ArgumentOutOfRangeException("shape", shape, null), 
			Shapes.Hair => HairStyle.GetBodyPack(), 
			Shapes.Eyes => Eyes, 
			Shapes.EyeBrows => EyeBrows, 
			Shapes.Mouth => Mouth, 
			Shapes.Shirt => Shirt.GetBodyPack(), 
			Shapes.Pants => Pants.GetBodyPack(), 
			Shapes.Shoes => Shoes.GetBodyPack(), 
			_ => throw new ArgumentOutOfRangeException("shape", shape, null), 
		};
	}

	public List<string> GetShapeSelectedColorIdentifiers(Shapes shape)
	{
		return shape switch
		{
			Shapes.Undefined => throw new ArgumentOutOfRangeException("shape", shape, null), 
			Shapes.Hair => new List<string> { HairColor.ToString() }, 
			Shapes.Shirt => new List<string> { ShirtColor }, 
			Shapes.Pants => new List<string> { PantsColor }, 
			Shapes.Shoes => new List<string> { ShoesColor, LacesColor }, 
			_ => new List<string>(), 
		};
	}
}
