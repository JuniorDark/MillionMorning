using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Emote;
using UnityEngine;

namespace Code.World.CharBuilder;

public class MilMo_ShopEmoteHandler
{
	private readonly MilMo_Emote _introEmote;

	private readonly string _genericIdle;

	private readonly string _eyesIdle;

	private readonly string _mouthIdle;

	private readonly string _hairIdle;

	private readonly string _upperBodyIdle;

	private readonly string _lowerBodyIdle;

	private readonly string _feetIdle;

	private readonly MilMo_Emote _firstEyesEmote;

	private readonly MilMo_Emote _firstMouthEmote;

	private readonly MilMo_Emote _firstHairEmote;

	private readonly MilMo_Emote _firstUpperBodyEmote;

	private readonly MilMo_Emote _firstLowerBodyEmote;

	private readonly MilMo_Emote _firstFeetEmote;

	private readonly List<MilMo_Emote> _eyesEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _mouthEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _hairEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _upperBodyEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _lowerBodyEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _feetEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _blinkEmotes = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _eyesEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _mouthEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _hHairEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _upperBodyEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _lowerBodyEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _feetEmotesUnplayed = new List<MilMo_Emote>();

	private readonly List<MilMo_Emote> _blinkEmotesUnplayed = new List<MilMo_Emote>();

	private MilMo_Emote _recentEmote;

	private bool _firstEyeSwap = true;

	private bool _firstMouthSwap = true;

	private bool _firstHairSwap = true;

	private bool _firstUpperBodySwap = true;

	private bool _firstLowerBodySwap = true;

	private bool _firstFeetSwap = true;

	public MilMo_ShopEmoteHandler()
	{
		_introEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapIntro01");
		_genericIdle = "Idle";
		_eyesIdle = "SwapEyesIdle";
		_mouthIdle = "SwapFaceIdle";
		_hairIdle = "SwapHairIdle";
		_upperBodyIdle = "SwapUpperBodyIdle";
		_lowerBodyIdle = "SwapLowerBodyIdle";
		_feetIdle = "SwapFeetIdle";
		_firstEyesEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapEyes01");
		_firstMouthEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFace01");
		_firstHairEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapHair01");
		_firstUpperBodyEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapUpperBody01");
		_firstLowerBodyEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapLowerBody01");
		_firstFeetEmote = MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFeet01");
		_blinkEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.Blink"));
		_blinkEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.Blink"));
		_blinkEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.DoubleBlink"));
		_blinkEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SlowBlink"));
		_blinkEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SlowBlink"));
		_eyesEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapEyes02"));
		_eyesEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapEyes03"));
		_eyesEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapEyes04"));
		_mouthEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFace02"));
		_mouthEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFace03"));
		_mouthEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFace04"));
		_hairEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapHair02"));
		_hairEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapHair03"));
		_hairEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapHair04"));
		_upperBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapUpperBody02"));
		_upperBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapUpperBody03"));
		_upperBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapUpperBody04"));
		_lowerBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapLowerBody02"));
		_lowerBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapLowerBody03"));
		_lowerBodyEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapLowerBody04"));
		_feetEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFeet02"));
		_feetEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFeet03"));
		_feetEmotes.Add(MilMo_EmoteSystem.GetEmoteByName("Emotes.SwapFeet04"));
	}

	public void PlayIntroEmote(MilMo_Avatar avatar)
	{
		if (avatar == null)
		{
			Debug.LogWarning("MilMo_ShopEmoteHandler: Tried to play intro emote but Avatar was null.");
		}
		else
		{
			avatar.PlayEmoteLocal(_introEmote);
		}
	}

	public void PlayIdleAnimation(MilMo_Avatar avatar, string category)
	{
		if (avatar == null)
		{
			Debug.LogWarning("MilMo_ShopEmoteHandler: Tried to play idle animation but Avatar was null.");
			return;
		}
		if (string.IsNullOrEmpty(category))
		{
			avatar.PlayAnimation(_genericIdle);
			return;
		}
		switch (category)
		{
		case "EYES":
			avatar.PlayAnimation(_eyesIdle);
			break;
		case "MOUTH":
			avatar.PlayAnimation(_mouthIdle);
			break;
		case "HAIR":
			avatar.PlayAnimation(_hairIdle);
			break;
		case "SHIRT":
			avatar.PlayAnimation(_upperBodyIdle);
			break;
		case "PANTS":
			avatar.PlayAnimation(_lowerBodyIdle);
			break;
		case "SHOES":
			avatar.PlayAnimation(_feetIdle);
			break;
		}
	}

	public void PlayFirstEmote(MilMo_Avatar avatar, string category)
	{
		if (avatar == null)
		{
			Debug.LogWarning("MilMo_ShopEmoteHandler: Tried to play first time emote but Avatar was null.");
			return;
		}
		MilMo_Emote emote = null;
		switch (category)
		{
		case "EYES":
			if (_firstEyeSwap)
			{
				emote = _firstEyesEmote;
				_firstEyeSwap = false;
				break;
			}
			return;
		case "MOUTH":
			if (_firstMouthSwap)
			{
				emote = _firstMouthEmote;
				_firstMouthSwap = false;
				break;
			}
			return;
		case "HAIR":
			if (_firstHairSwap)
			{
				emote = _firstHairEmote;
				_firstHairSwap = false;
				break;
			}
			return;
		case "SHIRT":
			if (_firstUpperBodySwap)
			{
				emote = _firstUpperBodyEmote;
				_firstUpperBodySwap = false;
				break;
			}
			return;
		case "PANTS":
			if (_firstLowerBodySwap)
			{
				emote = _firstLowerBodyEmote;
				_firstLowerBodySwap = false;
				break;
			}
			return;
		case "SHOES":
			if (_firstFeetSwap)
			{
				emote = _firstFeetEmote;
				_firstFeetSwap = false;
				break;
			}
			return;
		}
		avatar.PlayEmoteLocal(emote);
	}

	public void PlayAutoEmote(MilMo_Avatar avatar, string category)
	{
		if (avatar == null)
		{
			Debug.LogWarning("MilMo_ShopEmoteHandler: Tried to play auto emote but Avatar was null.");
			return;
		}
		MilMo_Emote emote = null;
		switch (category)
		{
		case "EYES":
			emote = GetRandomEmote(_eyesEmotes, _eyesEmotesUnplayed);
			break;
		case "MOUTH":
			emote = GetRandomEmote(_mouthEmotes, _mouthEmotesUnplayed);
			break;
		case "HAIR":
			emote = GetRandomEmote(_hairEmotes, _hHairEmotesUnplayed);
			break;
		case "SHIRT":
			emote = GetRandomEmote(_upperBodyEmotes, _upperBodyEmotesUnplayed);
			break;
		case "PANTS":
			emote = GetRandomEmote(_lowerBodyEmotes, _lowerBodyEmotesUnplayed);
			break;
		case "SHOES":
			emote = GetRandomEmote(_feetEmotes, _feetEmotesUnplayed);
			break;
		}
		avatar.PlayEmoteLocal(emote);
	}

	public void PlayRandomBlink(MilMo_Avatar avatar)
	{
		MilMo_Emote randomEmote = GetRandomEmote(_blinkEmotes, _blinkEmotesUnplayed);
		avatar.PlayEmoteLocal(randomEmote);
	}

	private MilMo_Emote GetRandomEmote(List<MilMo_Emote> available, List<MilMo_Emote> unplayed)
	{
		MilMo_Emote milMo_Emote;
		do
		{
			if (unplayed.Count == 0)
			{
				unplayed.InsertRange(0, available);
			}
			int index = Random.Range(0, unplayed.Count);
			milMo_Emote = unplayed[index];
			unplayed.RemoveAt(index);
		}
		while (_recentEmote == milMo_Emote);
		_recentEmote = milMo_Emote;
		return milMo_Emote;
	}
}
