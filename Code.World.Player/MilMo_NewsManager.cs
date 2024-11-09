using System.Collections.Generic;
using Code.Core.Network;
using Code.Core.Network.types;
using Core;

namespace Code.World.Player;

public class MilMo_NewsManager
{
	private static MilMo_NewsManager _instance;

	private int _newsHashCode;

	private IList<int> _readNewsHashCodes = new List<int>();

	public static MilMo_NewsManager Instance => _instance ?? (_instance = new MilMo_NewsManager());

	public FeaturedNewsItem FeaturedNewsItem { get; private set; }

	public string NewsImage { get; private set; }

	public string NewsImageLink { get; private set; }

	public string Headline { get; private set; }

	public string Date { get; private set; }

	public string TextBody { get; private set; }

	public bool HasNews { get; private set; }

	public bool NewsAreNew
	{
		get
		{
			if (HasNews)
			{
				return !_readNewsHashCodes.Contains(_newsHashCode);
			}
			return false;
		}
	}

	private MilMo_NewsManager()
	{
		TextBody = "";
		Date = "";
		Headline = "";
		NewsImageLink = "";
		NewsImage = "";
	}

	public void ReadNews(string news, string newsImage, string newsImageLink, FeaturedNewsItem featuredNewsItem)
	{
		FeaturedNewsItem = featuredNewsItem;
		_newsHashCode = news.GetHashCode();
		NewsImage = newsImage;
		NewsImageLink = newsImageLink;
		string[] array = news.Split('|');
		if (array.Length >= 3)
		{
			Headline = array[0];
			Date = array[1];
			TextBody = array[2];
			HasNews = !string.IsNullOrEmpty(TextBody) || !string.IsNullOrEmpty(Headline) || !string.IsNullOrEmpty(Date);
			if (TextBody.Length > 0)
			{
				TextBody += "\n\n";
			}
		}
	}

	public void SetReadNewsHashCodes(IList<int> readNewsHashCodes)
	{
		_readNewsHashCodes = readNewsHashCodes;
	}

	public void SetNewsRead()
	{
		if (HasNews)
		{
			_readNewsHashCodes.Add(_newsHashCode);
			Singleton<GameNetwork>.Instance.SendSaveReadNews(_newsHashCode);
		}
	}
}
