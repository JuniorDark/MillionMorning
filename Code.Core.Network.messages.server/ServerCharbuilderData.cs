using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerCharbuilderData : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 77;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCharbuilderData(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 77;

	private IList<string> skinColors;

	private IList<string> eyeColors;

	private IList<string> boyMouths;

	private IList<string> boyEyes;

	private IList<string> boyEyeBrows;

	private IList<int> hairColors;

	private IList<Code.Core.Network.types.Template> boyHairs;

	private IList<string> girlMouths;

	private IList<string> girlEyes;

	private IList<string> girlEyeBrows;

	private IList<Code.Core.Network.types.Template> girlHairs;

	private IList<Code.Core.Network.types.Template> boyShirts;

	private IList<Code.Core.Network.types.Template> boyPants;

	private IList<Code.Core.Network.types.Template> boyShoes;

	private IList<Code.Core.Network.types.Template> girlShirts;

	private IList<Code.Core.Network.types.Template> girlPants;

	private IList<Code.Core.Network.types.Template> girlShoes;

	private IList<string> moods;

	private ServerCharbuilderData(MessageReader reader)
	{
		skinColors = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			skinColors.Add(reader.ReadString());
		}
		eyeColors = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			eyeColors.Add(reader.ReadString());
		}
		boyMouths = new List<string>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			boyMouths.Add(reader.ReadString());
		}
		boyEyes = new List<string>();
		short num7 = reader.ReadInt16();
		for (short num8 = 0; num8 < num7; num8++)
		{
			boyEyes.Add(reader.ReadString());
		}
		boyEyeBrows = new List<string>();
		short num9 = reader.ReadInt16();
		for (short num10 = 0; num10 < num9; num10++)
		{
			boyEyeBrows.Add(reader.ReadString());
		}
		hairColors = new List<int>();
		short num11 = reader.ReadInt16();
		for (short num12 = 0; num12 < num11; num12++)
		{
			hairColors.Add(reader.ReadInt32());
		}
		boyHairs = new List<Code.Core.Network.types.Template>();
		short num13 = reader.ReadInt16();
		for (short num14 = 0; num14 < num13; num14++)
		{
			boyHairs.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		girlMouths = new List<string>();
		short num15 = reader.ReadInt16();
		for (short num16 = 0; num16 < num15; num16++)
		{
			girlMouths.Add(reader.ReadString());
		}
		girlEyes = new List<string>();
		short num17 = reader.ReadInt16();
		for (short num18 = 0; num18 < num17; num18++)
		{
			girlEyes.Add(reader.ReadString());
		}
		girlEyeBrows = new List<string>();
		short num19 = reader.ReadInt16();
		for (short num20 = 0; num20 < num19; num20++)
		{
			girlEyeBrows.Add(reader.ReadString());
		}
		girlHairs = new List<Code.Core.Network.types.Template>();
		short num21 = reader.ReadInt16();
		for (short num22 = 0; num22 < num21; num22++)
		{
			girlHairs.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		boyShirts = new List<Code.Core.Network.types.Template>();
		short num23 = reader.ReadInt16();
		for (short num24 = 0; num24 < num23; num24++)
		{
			boyShirts.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		boyPants = new List<Code.Core.Network.types.Template>();
		short num25 = reader.ReadInt16();
		for (short num26 = 0; num26 < num25; num26++)
		{
			boyPants.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		boyShoes = new List<Code.Core.Network.types.Template>();
		short num27 = reader.ReadInt16();
		for (short num28 = 0; num28 < num27; num28++)
		{
			boyShoes.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		girlShirts = new List<Code.Core.Network.types.Template>();
		short num29 = reader.ReadInt16();
		for (short num30 = 0; num30 < num29; num30++)
		{
			girlShirts.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		girlPants = new List<Code.Core.Network.types.Template>();
		short num31 = reader.ReadInt16();
		for (short num32 = 0; num32 < num31; num32++)
		{
			girlPants.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		girlShoes = new List<Code.Core.Network.types.Template>();
		short num33 = reader.ReadInt16();
		for (short num34 = 0; num34 < num33; num34++)
		{
			girlShoes.Add(Code.Core.Network.types.Template.Create(reader.ReadTypeCode(), reader));
		}
		moods = new List<string>();
		short num35 = reader.ReadInt16();
		for (short num36 = 0; num36 < num35; num36++)
		{
			moods.Add(reader.ReadString());
		}
	}

	public ServerCharbuilderData(IList<string> skinColors, IList<string> eyeColors, IList<string> boyMouths, IList<string> boyEyes, IList<string> boyEyeBrows, IList<int> hairColors, IList<Code.Core.Network.types.Template> boyHairs, IList<string> girlMouths, IList<string> girlEyes, IList<string> girlEyeBrows, IList<Code.Core.Network.types.Template> girlHairs, IList<Code.Core.Network.types.Template> boyShirts, IList<Code.Core.Network.types.Template> boyPants, IList<Code.Core.Network.types.Template> boyShoes, IList<Code.Core.Network.types.Template> girlShirts, IList<Code.Core.Network.types.Template> girlPants, IList<Code.Core.Network.types.Template> girlShoes, IList<string> moods)
	{
		this.skinColors = skinColors;
		this.eyeColors = eyeColors;
		this.boyMouths = boyMouths;
		this.boyEyes = boyEyes;
		this.boyEyeBrows = boyEyeBrows;
		this.hairColors = hairColors;
		this.boyHairs = boyHairs;
		this.girlMouths = girlMouths;
		this.girlEyes = girlEyes;
		this.girlEyeBrows = girlEyeBrows;
		this.girlHairs = girlHairs;
		this.boyShirts = boyShirts;
		this.boyPants = boyPants;
		this.boyShoes = boyShoes;
		this.girlShirts = girlShirts;
		this.girlPants = girlPants;
		this.girlShoes = girlShoes;
		this.moods = moods;
	}

	public IList<string> getSkinColors()
	{
		return skinColors;
	}

	public IList<string> getEyeColors()
	{
		return eyeColors;
	}

	public IList<string> getBoyMouths()
	{
		return boyMouths;
	}

	public IList<string> getBoyEyes()
	{
		return boyEyes;
	}

	public IList<string> getBoyEyeBrows()
	{
		return boyEyeBrows;
	}

	public IList<int> getHairColors()
	{
		return hairColors;
	}

	public IList<Code.Core.Network.types.Template> getBoyHairs()
	{
		return boyHairs;
	}

	public IList<string> getGirlMouths()
	{
		return girlMouths;
	}

	public IList<string> getGirlEyes()
	{
		return girlEyes;
	}

	public IList<string> getGirlEyeBrows()
	{
		return girlEyeBrows;
	}

	public IList<Code.Core.Network.types.Template> getGirlHairs()
	{
		return girlHairs;
	}

	public IList<Code.Core.Network.types.Template> getBoyShirts()
	{
		return boyShirts;
	}

	public IList<Code.Core.Network.types.Template> getBoyPants()
	{
		return boyPants;
	}

	public IList<Code.Core.Network.types.Template> getBoyShoes()
	{
		return boyShoes;
	}

	public IList<Code.Core.Network.types.Template> getGirlShirts()
	{
		return girlShirts;
	}

	public IList<Code.Core.Network.types.Template> getGirlPants()
	{
		return girlPants;
	}

	public IList<Code.Core.Network.types.Template> getGirlShoes()
	{
		return girlShoes;
	}

	public IList<string> getMoods()
	{
		return moods;
	}

	public byte[] GetData()
	{
		int num = 40;
		num += (short)(2 * skinColors.Count);
		foreach (string skinColor in skinColors)
		{
			num += MessageWriter.GetSize(skinColor);
		}
		num += (short)(2 * eyeColors.Count);
		foreach (string eyeColor in eyeColors)
		{
			num += MessageWriter.GetSize(eyeColor);
		}
		num += (short)(2 * boyMouths.Count);
		foreach (string boyMouth in boyMouths)
		{
			num += MessageWriter.GetSize(boyMouth);
		}
		num += (short)(2 * boyEyes.Count);
		foreach (string boyEye in boyEyes)
		{
			num += MessageWriter.GetSize(boyEye);
		}
		num += (short)(2 * boyEyeBrows.Count);
		foreach (string boyEyeBrow in boyEyeBrows)
		{
			num += MessageWriter.GetSize(boyEyeBrow);
		}
		num += (short)(hairColors.Count * 4);
		num += (short)(boyHairs.Count * 2);
		foreach (Code.Core.Network.types.Template boyHair in boyHairs)
		{
			num += boyHair.Size();
		}
		num += (short)(2 * girlMouths.Count);
		foreach (string girlMouth in girlMouths)
		{
			num += MessageWriter.GetSize(girlMouth);
		}
		num += (short)(2 * girlEyes.Count);
		foreach (string girlEye in girlEyes)
		{
			num += MessageWriter.GetSize(girlEye);
		}
		num += (short)(2 * girlEyeBrows.Count);
		foreach (string girlEyeBrow in girlEyeBrows)
		{
			num += MessageWriter.GetSize(girlEyeBrow);
		}
		num += (short)(girlHairs.Count * 2);
		foreach (Code.Core.Network.types.Template girlHair in girlHairs)
		{
			num += girlHair.Size();
		}
		num += (short)(boyShirts.Count * 2);
		foreach (Code.Core.Network.types.Template boyShirt in boyShirts)
		{
			num += boyShirt.Size();
		}
		num += (short)(boyPants.Count * 2);
		foreach (Code.Core.Network.types.Template boyPant in boyPants)
		{
			num += boyPant.Size();
		}
		num += (short)(boyShoes.Count * 2);
		foreach (Code.Core.Network.types.Template boyShoe in boyShoes)
		{
			num += boyShoe.Size();
		}
		num += (short)(girlShirts.Count * 2);
		foreach (Code.Core.Network.types.Template girlShirt in girlShirts)
		{
			num += girlShirt.Size();
		}
		num += (short)(girlPants.Count * 2);
		foreach (Code.Core.Network.types.Template girlPant in girlPants)
		{
			num += girlPant.Size();
		}
		num += (short)(girlShoes.Count * 2);
		foreach (Code.Core.Network.types.Template girlShoe in girlShoes)
		{
			num += girlShoe.Size();
		}
		num += (short)(2 * moods.Count);
		foreach (string mood in moods)
		{
			num += MessageWriter.GetSize(mood);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(77);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)skinColors.Count);
		foreach (string skinColor2 in skinColors)
		{
			messageWriter.WriteString(skinColor2);
		}
		messageWriter.WriteInt16((short)eyeColors.Count);
		foreach (string eyeColor2 in eyeColors)
		{
			messageWriter.WriteString(eyeColor2);
		}
		messageWriter.WriteInt16((short)boyMouths.Count);
		foreach (string boyMouth2 in boyMouths)
		{
			messageWriter.WriteString(boyMouth2);
		}
		messageWriter.WriteInt16((short)boyEyes.Count);
		foreach (string boyEye2 in boyEyes)
		{
			messageWriter.WriteString(boyEye2);
		}
		messageWriter.WriteInt16((short)boyEyeBrows.Count);
		foreach (string boyEyeBrow2 in boyEyeBrows)
		{
			messageWriter.WriteString(boyEyeBrow2);
		}
		messageWriter.WriteInt16((short)hairColors.Count);
		foreach (int hairColor in hairColors)
		{
			messageWriter.WriteInt32(hairColor);
		}
		messageWriter.WriteInt16((short)boyHairs.Count);
		foreach (Code.Core.Network.types.Template boyHair2 in boyHairs)
		{
			messageWriter.WriteTypeCode(boyHair2.GetTypeId());
			boyHair2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)girlMouths.Count);
		foreach (string girlMouth2 in girlMouths)
		{
			messageWriter.WriteString(girlMouth2);
		}
		messageWriter.WriteInt16((short)girlEyes.Count);
		foreach (string girlEye2 in girlEyes)
		{
			messageWriter.WriteString(girlEye2);
		}
		messageWriter.WriteInt16((short)girlEyeBrows.Count);
		foreach (string girlEyeBrow2 in girlEyeBrows)
		{
			messageWriter.WriteString(girlEyeBrow2);
		}
		messageWriter.WriteInt16((short)girlHairs.Count);
		foreach (Code.Core.Network.types.Template girlHair2 in girlHairs)
		{
			messageWriter.WriteTypeCode(girlHair2.GetTypeId());
			girlHair2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)boyShirts.Count);
		foreach (Code.Core.Network.types.Template boyShirt2 in boyShirts)
		{
			messageWriter.WriteTypeCode(boyShirt2.GetTypeId());
			boyShirt2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)boyPants.Count);
		foreach (Code.Core.Network.types.Template boyPant2 in boyPants)
		{
			messageWriter.WriteTypeCode(boyPant2.GetTypeId());
			boyPant2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)boyShoes.Count);
		foreach (Code.Core.Network.types.Template boyShoe2 in boyShoes)
		{
			messageWriter.WriteTypeCode(boyShoe2.GetTypeId());
			boyShoe2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)girlShirts.Count);
		foreach (Code.Core.Network.types.Template girlShirt2 in girlShirts)
		{
			messageWriter.WriteTypeCode(girlShirt2.GetTypeId());
			girlShirt2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)girlPants.Count);
		foreach (Code.Core.Network.types.Template girlPant2 in girlPants)
		{
			messageWriter.WriteTypeCode(girlPant2.GetTypeId());
			girlPant2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)girlShoes.Count);
		foreach (Code.Core.Network.types.Template girlShoe2 in girlShoes)
		{
			messageWriter.WriteTypeCode(girlShoe2.GetTypeId());
			girlShoe2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)moods.Count);
		foreach (string mood2 in moods)
		{
			messageWriter.WriteString(mood2);
		}
		return messageWriter.GetData();
	}
}
