using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerMOTD : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 277;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMOTD(reader);
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

	private const int OPCODE = 277;

	private string MOTD;

	private FeaturedNewsItem featuredNews;

	private string newsImage;

	private string newsImageLink;

	private ServerMOTD(MessageReader reader)
	{
		MOTD = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			featuredNews = new FeaturedNewsItem(reader);
		}
		newsImage = reader.ReadString();
		newsImageLink = reader.ReadString();
	}

	public ServerMOTD(string MOTD, FeaturedNewsItem featuredNews, string newsImage, string newsImageLink)
	{
		this.MOTD = MOTD;
		this.featuredNews = featuredNews;
		this.newsImage = newsImage;
		this.newsImageLink = newsImageLink;
	}

	public string getMOTD()
	{
		return MOTD;
	}

	public FeaturedNewsItem getFeaturedNews()
	{
		return featuredNews;
	}

	public string getNewsImage()
	{
		return newsImage;
	}

	public string getNewsImageLink()
	{
		return newsImageLink;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += MessageWriter.GetSize(MOTD);
		if (featuredNews != null)
		{
			num += featuredNews.Size();
		}
		num += MessageWriter.GetSize(newsImage);
		num += MessageWriter.GetSize(newsImageLink);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(277);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(MOTD);
		if (featuredNews == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			featuredNews.Write(messageWriter);
		}
		messageWriter.WriteString(newsImage);
		messageWriter.WriteString(newsImageLink);
		return messageWriter.GetData();
	}
}
