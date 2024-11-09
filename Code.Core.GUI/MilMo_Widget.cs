using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.GUI.GameDialog;
using UnityEngine;

namespace Code.Core.GUI;

public class MilMo_Widget
{
	protected enum LookAtType
	{
		None,
		AlongForwardVector,
		PosMoverTargetPosition,
		FixedScreenPosition
	}

	public enum DragType
	{
		None,
		Self,
		Parent,
		Ancestor
	}

	public delegate void CustomFunc(object obj);

	public string Identifier = "Widget " + MilMo_UserInterface.GetRandomID();

	public MilMo_UserInterface UI;

	public MilMo_Widget Parent;

	protected bool IsWindow;

	protected bool IsGameDialog;

	private MilMo_Widget _lastFocus;

	private MilMo_Widget _lastAncestor;

	private bool _fixedRes;

	public Vector2 Pos;

	private readonly Vector2 _textureOffset = Vector2.zero;

	private Vector2 _lastPos;

	public Vector2 Scale;

	public Vector2 Align;

	private Vector2 _extraDrawTextSize;

	private Vector2 _realignPos;

	private MilMo_GUI.CropMode _cropMode;

	private MilMo_Mover.UpdateFunc _moveType = MilMo_Mover.UpdateFunc.Spring;

	protected MilMo_Mover.UpdateFunc ScaleType = MilMo_Mover.UpdateFunc.Spring;

	private MilMo_Mover.UpdateFunc _rotationType = MilMo_Mover.UpdateFunc.Spring;

	public readonly MilMo_Mover PosMover = new MilMo_Mover();

	public readonly MilMo_Mover ScaleMover = new MilMo_Mover();

	public readonly MilMo_Mover AngleMover = new MilMo_Mover();

	public MilMo_Widget GetMotionFrom;

	public MilMo_Widget GetPosFrom;

	protected LookAtType LookAtMode;

	private readonly Vector2 _lookAtPosition = Vector2.zero;

	protected MilMo_Texture CurrentTexture;

	public Color CurrentColor = Color.white;

	public Color TextColor = Color.white;

	public Color TextDropShadowColor = Color.black;

	public Color TextSelectionColor;

	public Color TextOutlineColor;

	public bool UseParentAlpha = true;

	public Color TargetColor = Color.white;

	public Color DefaultColor = Color.white;

	protected Color TargetTextColor = Color.white;

	public Color DefaultTextColor = Color.white;

	public bool FadeToDefaultColor = true;

	public bool FadeToDefaultTextColor = true;

	public float FadeSpeed = 0.01f;

	public bool DisableAtZeroAlpha;

	private float _textAngle;

	protected GUIStyle LabelStyle;

	public GUISkin Font;

	public GUISkin Skin;

	public Vector2 TextOffset;

	public Vector2 TextDropShadowPos;

	public Vector2 TextOutline = Vector2.zero;

	private Vector2 _fontScale;

	public MilMo_GUI.Align TextAlign;

	private bool _wordWrap;

	private Vector2 _textSelection = Vector2.zero;

	private bool _hasTextSelection;

	private readonly RectOffset _textPadding = new RectOffset();

	private Vector2 _dragOffset = Vector2.zero;

	private Vector2 _fixedPointerZoneSize = Vector2.zero;

	private Vector2 _fixedPointerZoneOffset = Vector2.zero;

	public bool AllowPointerFocus = true;

	public DragType DragTarget;

	public CustomFunc CustomFunction;

	public object CustomArg;

	public int Info;

	public int Info2;

	public bool Enabled = true;

	private bool _dirtyLabelStyle = true;

	private bool _ignoreGlobalFade;

	public bool IsInvisible;

	public bool IgnoredByScrollViewRefresh;

	public bool IgnoreNextStepDueToBringToFront;

	private static bool _drawIdentifier;

	private readonly Vector2 _fontScaleFallback = new Vector2(4f, 4f);

	public float MxFillAmount = 1f;

	private const float M_Y_FILL_AMOUNT = 1f;

	public bool ShowFavoriteStar;

	public Texture StarFilledTexture;

	public Texture StarOutlineTexture;

	public bool IsFavorite;

	public bool IgnoreInputOffsetMode { get; set; }

	public Vector2 Res { get; private set; }

	public bool FixedRes
	{
		get
		{
			return _fixedRes;
		}
		set
		{
			_fixedRes = value;
			RefreshResolution();
		}
	}

	public List<MilMo_Widget> Children { get; private set; }

	public virtual bool IgnoreGlobalFade
	{
		protected get
		{
			return _ignoreGlobalFade;
		}
		set
		{
			_ignoreGlobalFade = value;
			for (int i = 0; i < Children.Count; i++)
			{
				MilMo_Widget milMo_Widget = Children[i];
				if (milMo_Widget != null)
				{
					milMo_Widget.IgnoreGlobalFade = value;
				}
			}
		}
	}

	public MilMo_Texture Texture { get; protected set; }

	public MilMo_LocString Text { get; protected set; }

	public void ColorTo(float r, float g, float b, float a)
	{
		TargetColor.r = r;
		TargetColor.g = g;
		TargetColor.b = b;
		TargetColor.a = a;
	}

	public void ColorTo(Color col)
	{
		TargetColor.r = col.r;
		TargetColor.g = col.g;
		TargetColor.b = col.b;
		TargetColor.a = col.a;
	}

	public void ColorNow(float r, float g, float b, float a)
	{
		CurrentColor.r = r;
		CurrentColor.g = g;
		CurrentColor.b = b;
		CurrentColor.a = a;
		TargetColor.r = r;
		TargetColor.g = g;
		TargetColor.b = b;
		TargetColor.a = a;
	}

	public void ColorNow(Color col)
	{
		CurrentColor.r = col.r;
		CurrentColor.g = col.g;
		CurrentColor.b = col.b;
		CurrentColor.a = col.a;
		TargetColor.r = col.r;
		TargetColor.g = col.g;
		TargetColor.b = col.b;
		TargetColor.a = col.a;
	}

	public void SetColor(float r, float g, float b, float a)
	{
		ColorNow(r, g, b, a);
	}

	public virtual void SetColor(Color col)
	{
		ColorNow(col.r, col.g, col.b, col.a);
	}

	public void SetDefaultColor(float r, float g, float b, float a)
	{
		DefaultColor.r = r;
		DefaultColor.g = g;
		DefaultColor.b = b;
		DefaultColor.a = a;
		CurrentColor.r = r;
		CurrentColor.g = g;
		CurrentColor.b = b;
		CurrentColor.a = a;
		TargetColor.r = r;
		TargetColor.g = g;
		TargetColor.b = b;
		TargetColor.a = a;
	}

	public void SetDefaultColor(Color col)
	{
		DefaultColor.r = col.r;
		DefaultColor.g = col.g;
		DefaultColor.b = col.b;
		DefaultColor.a = col.a;
		CurrentColor.r = col.r;
		CurrentColor.g = col.g;
		CurrentColor.b = col.b;
		CurrentColor.a = col.a;
		TargetColor.r = col.r;
		TargetColor.g = col.g;
		TargetColor.b = col.b;
		TargetColor.a = col.a;
	}

	public void AlphaTo(float a)
	{
		TargetColor.a = a;
	}

	public void SetAlpha(float a)
	{
		CurrentColor.a = a;
		TargetColor.a = a;
	}

	public void FadeAlpha(float start, float end)
	{
		DefaultColor.a = end;
		CurrentColor.a = end;
		TargetColor.a = end;
		SetAlpha(start);
	}

	private float Fade(float aPos, float aTarget)
	{
		if (aPos > aTarget)
		{
			aPos -= FadeSpeed;
			if (aPos < aTarget)
			{
				aPos = aTarget;
			}
		}
		else if (aPos < aTarget)
		{
			aPos += FadeSpeed;
			if (aPos > aTarget)
			{
				aPos = aTarget;
			}
		}
		return aPos;
	}

	public void SetFadeSpeed(float f)
	{
		FadeSpeed = ((FadeSpeed > 0f) ? f : 0f);
	}

	public MilMo_Widget(MilMo_UserInterface ui)
	{
		if (ui == null)
		{
			throw new ArgumentNullException(null, "user interface is null when creating the widget.");
		}
		UI = ui;
		Children = new List<MilMo_Widget>();
		Res = new Vector2(1f, 1f);
		GoToNow(50f, 50f);
		ScaleNow(50f, 50f);
		Align.x = 0.5f;
		Align.y = 0.5f;
		SetAngle(0f);
		Texture = new MilMo_Texture("Content/GUI/Batch01/Textures/Core/Invisible");
		Texture.AsyncLoad();
		CurrentTexture = Texture;
		TextSelectionColor = new Color(1f, 0.5f, 0f, 1f);
		Skin = UI.Skin;
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetTextDropShadowPos(0f, 0f);
		Text = MilMo_LocString.Empty;
		Font = UI.Skin;
		SetTextOffset(0f, 0f);
		SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		SetFontScale(1f);
		SetTextAngle(0f);
		CustomFunction = Nothing;
		RefreshResolution();
	}

	public void RefreshResolution()
	{
		Res = ((!FixedRes) ? UI.Res : new Vector2(1f, 1f));
		for (int i = 0; i < Children.Count; i++)
		{
			MilMo_Widget milMo_Widget = Children[i];
			milMo_Widget.FixedRes = FixedRes;
			milMo_Widget.RefreshResolution();
		}
	}

	public virtual void Step()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (DisableAtZeroAlpha && CurrentColor.a < 0.0001f)
		{
			SetEnabled(e: false);
		}
		DoDrag();
		_lastPos = Pos;
		if (GetMotionFrom != null)
		{
			Pos = GetMotionFrom.Pos;
			Scale = GetMotionFrom.Scale;
			AngleMover.Val = GetMotionFrom.AngleMover.Val;
		}
		else if (GetPosFrom == null)
		{
			RestoreAlign();
			PosMover.Update();
			ScaleMover.Update();
			AngleMover.Update();
			Pos = PosMover.Val;
			Scale = ScaleMover.Val;
			_realignPos = Pos;
			ApplyAlign();
		}
		if (CurrentColor.r != TargetColor.r)
		{
			CurrentColor.r = Fade(CurrentColor.r, TargetColor.r);
		}
		if (CurrentColor.g != TargetColor.g)
		{
			CurrentColor.g = Fade(CurrentColor.g, TargetColor.g);
		}
		if (CurrentColor.b != TargetColor.b)
		{
			CurrentColor.b = Fade(CurrentColor.b, TargetColor.b);
		}
		if (CurrentColor.a != TargetColor.a)
		{
			CurrentColor.a = Fade(CurrentColor.a, TargetColor.a);
		}
		if (FadeToDefaultColor)
		{
			ColorTo(DefaultColor);
		}
		if (TextColor.r != TargetTextColor.r)
		{
			TextColor.r = Fade(TextColor.r, TargetTextColor.r);
		}
		if (TextColor.g != TargetTextColor.g)
		{
			TextColor.g = Fade(TextColor.g, TargetTextColor.g);
		}
		if (TextColor.b != TargetTextColor.b)
		{
			TextColor.b = Fade(TextColor.b, TargetTextColor.b);
		}
		if (TextColor.a != TargetTextColor.a)
		{
			TextColor.a = Fade(TextColor.a, TargetTextColor.a);
		}
		if (FadeToDefaultTextColor)
		{
			TextColorTo(DefaultTextColor);
		}
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = Children[num];
			if (milMo_Widget != null)
			{
				if (milMo_Widget.IgnoreNextStepDueToBringToFront)
				{
					milMo_Widget.IgnoreNextStepDueToBringToFront = false;
				}
				else
				{
					milMo_Widget.Step();
				}
			}
		}
	}

	public virtual void Draw()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (LookAtMode != 0)
		{
			switch (LookAtMode)
			{
			case LookAtType.AlongForwardVector:
				LookAlongForwardVector();
				break;
			case LookAtType.PosMoverTargetPosition:
				LookAtPosMoverTargetPosition();
				break;
			case LookAtType.FixedScreenPosition:
				LookAtFixedScreenPosition();
				break;
			}
		}
		GUISkin skin = UnityEngine.GUI.skin;
		UnityEngine.GUI.skin = Font;
		Rect screenPosition = GetScreenPosition();
		Rect alignedScreenPosition = GetAlignedScreenPosition();
		screenPosition.x += _textureOffset.x;
		screenPosition.y += _textureOffset.y;
		alignedScreenPosition.x += _textureOffset.x;
		alignedScreenPosition.y += _textureOffset.y;
		DoGetPosFrom();
		Vector2 pivotPoint = default(Vector2);
		pivotPoint.x = alignedScreenPosition.x;
		pivotPoint.y = alignedScreenPosition.y;
		if (AngleMover.Val != Vector2.zero)
		{
			GUIUtility.RotateAroundPivot(AngleMover.Val.x, pivotPoint);
		}
		Color currentColor = CurrentColor;
		if (UseParentAlpha && Parent != null)
		{
			currentColor.a *= Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		if (CurrentTexture != null && CurrentTexture.Texture != null)
		{
			Rect position = new Rect(screenPosition.x + 20f, screenPosition.y + 40f, screenPosition.width / 3f, screenPosition.height / 3f);
			Texture texture = (IsFavorite ? StarFilledTexture : StarOutlineTexture);
			if (!IsInvisible)
			{
				switch (_cropMode)
				{
				case MilMo_GUI.CropMode.Stretch:
					UnityEngine.GUI.DrawTexture(screenPosition, CurrentTexture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
					if (ShowFavoriteStar && texture != null)
					{
						UnityEngine.GUI.DrawTexture(position, texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
					}
					break;
				case MilMo_GUI.CropMode.Crop:
					if (!IsInvisible)
					{
						UnityEngine.GUI.DrawTexture(screenPosition, CurrentTexture.Texture, ScaleMode.ScaleAndCrop, alphaBlend: true, 0f);
					}
					if (ShowFavoriteStar && texture != null)
					{
						UnityEngine.GUI.DrawTexture(position, texture, ScaleMode.ScaleAndCrop, alphaBlend: true, 0f);
					}
					break;
				case MilMo_GUI.CropMode.Cropadelic:
					if (!IsInvisible)
					{
						UnityEngine.GUI.BeginGroup(new Rect(screenPosition.x, screenPosition.y, Scale.x * MxFillAmount, Scale.y * 1f));
					}
					UnityEngine.GUI.DrawTexture(new Rect(0f, 0f, Scale.x, Scale.y), CurrentTexture.Texture);
					if (ShowFavoriteStar && texture != null)
					{
						UnityEngine.GUI.DrawTexture(new Rect(0f, 0f, Scale.x, Scale.y), texture);
					}
					UnityEngine.GUI.EndGroup();
					break;
				}
			}
		}
		DrawText();
		if (AngleMover.Val != Vector2.zero)
		{
			RestoreMatrix();
		}
		CheckPointerFocus();
		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].Draw();
		}
		UnityEngine.GUI.skin = skin;
	}

	private void DoDrag()
	{
		if (DragTarget == DragType.None)
		{
			return;
		}
		Vector2 vector = new Vector2(-99999f, -99999f);
		if (Hover() && MilMo_Pointer.LeftButton)
		{
			MilMo_Widget milMo_Widget = null;
			switch (DragTarget)
			{
			case DragType.Self:
				milMo_Widget = this;
				break;
			case DragType.Parent:
				milMo_Widget = Parent;
				break;
			case DragType.Ancestor:
				milMo_Widget = GetAncestor();
				break;
			}
			if (milMo_Widget == null)
			{
				milMo_Widget = this;
			}
			if (_dragOffset == vector)
			{
				_dragOffset = new Vector2(milMo_Widget.Pos.x / Res.x, milMo_Widget.Pos.y / Res.y);
				_dragOffset -= MilMo_Pointer.Position - UI.GlobalPosOffset;
				_dragOffset += new Vector2(milMo_Widget.Scale.x / Res.x * milMo_Widget.Align.x, milMo_Widget.Scale.y / Res.y * milMo_Widget.Align.y);
			}
			milMo_Widget.SetPosition(_dragOffset.x + (MilMo_Pointer.Position.x - UI.GlobalPosOffset.x), _dragOffset.y + (MilMo_Pointer.Position.y - UI.GlobalPosOffset.y));
		}
		else
		{
			_dragOffset = vector;
		}
	}

	private void RestoreAlign()
	{
		Pos.x = _realignPos.x;
		Pos.y = _realignPos.y;
	}

	protected static void RestoreMatrix()
	{
		UnityEngine.GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public void BringToFront(MilMo_Widget w)
	{
		if (w != null && Children.Contains(w))
		{
			Children.Remove(w);
			Children.Add(w);
		}
	}

	protected void SendToBack(MilMo_Widget w)
	{
		if (w != null && Children.Contains(w))
		{
			Children.Remove(w);
			Children.Insert(0, w);
		}
	}

	protected MilMo_Widget GetAncestor()
	{
		MilMo_Widget milMo_Widget = this;
		while (milMo_Widget.Parent != null)
		{
			milMo_Widget = milMo_Widget.Parent;
		}
		return milMo_Widget;
	}

	public void AddChild(MilMo_Widget w)
	{
		w.UI.RemoveChild(w);
		if (w.Parent != null)
		{
			w.Parent.RemoveChild(w);
		}
		w.Parent = this;
		Children.Add(w);
	}

	public void RemoveChild(MilMo_Widget w)
	{
		w.Parent = null;
		Children.Remove(w);
	}

	public void RemoveAllChildren()
	{
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = Children[num];
			milMo_Widget.Parent = null;
			Children.Remove(milMo_Widget);
		}
	}

	public virtual void RemoveAllChildrenByType(Type childType)
	{
		for (int num = Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = Children[num];
			if (!(milMo_Widget.GetType() != childType))
			{
				milMo_Widget.Parent = null;
				Children.Remove(milMo_Widget);
			}
		}
	}

	protected MilMo_Window GetWindowAncestor()
	{
		if (IsWindow)
		{
			return this as MilMo_Window;
		}
		MilMo_Window result = null;
		if (Parent != null)
		{
			result = Parent.GetWindowAncestor();
		}
		return result;
	}

	private MilMo_PropertyPage GetPropertyPageAncestor()
	{
		if (GetType() == typeof(MilMo_PropertyPage))
		{
			return (MilMo_PropertyPage)this;
		}
		MilMo_PropertyPage result = null;
		if (Parent != null)
		{
			result = Parent.GetPropertyPageAncestor();
		}
		return result;
	}

	protected MilMo_ScrollView GetScrollViewAncestor()
	{
		if (GetType() == typeof(MilMo_ScrollView))
		{
			return (MilMo_ScrollView)this;
		}
		MilMo_ScrollView result = null;
		if (Parent != null)
		{
			result = Parent.GetScrollViewAncestor();
		}
		return result;
	}

	protected virtual Rect GetChildOffset()
	{
		return new Rect(Pos.x, Pos.y, 0f, 0f);
	}

	private Rect GetAncestorOffset()
	{
		Rect result = new Rect(0f, 0f, 0f, 0f);
		if (Parent == null)
		{
			return result;
		}
		bool flag = false;
		MilMo_Widget parent = Parent;
		while (!flag)
		{
			Rect childOffset = parent.GetChildOffset();
			if (childOffset == new Rect(0f, 0f, 0f, 0f))
			{
				flag = true;
			}
			else
			{
				result.x += childOffset.x;
				result.y += childOffset.y;
			}
			if (!flag && parent.Parent != null)
			{
				parent = parent.Parent;
			}
			else
			{
				flag = true;
			}
		}
		return result;
	}

	protected void UpdateLabelStyle()
	{
		if (_dirtyLabelStyle)
		{
			if (LabelStyle == null)
			{
				LabelStyle = new GUIStyle(UnityEngine.GUI.skin.label);
			}
			if (Font != null)
			{
				LabelStyle.font = Font.font;
			}
			LabelStyle.wordWrap = _wordWrap;
			LabelStyle.padding = _textPadding;
			switch (TextAlign)
			{
			case MilMo_GUI.Align.TopLeft:
				LabelStyle.alignment = TextAnchor.UpperLeft;
				break;
			case MilMo_GUI.Align.TopCenter:
				LabelStyle.alignment = TextAnchor.UpperCenter;
				break;
			case MilMo_GUI.Align.TopRight:
				LabelStyle.alignment = TextAnchor.UpperRight;
				break;
			case MilMo_GUI.Align.CenterLeft:
				LabelStyle.alignment = TextAnchor.MiddleLeft;
				break;
			case MilMo_GUI.Align.CenterCenter:
				LabelStyle.alignment = TextAnchor.MiddleCenter;
				break;
			case MilMo_GUI.Align.CenterRight:
				LabelStyle.alignment = TextAnchor.MiddleRight;
				break;
			case MilMo_GUI.Align.BottomLeft:
				LabelStyle.alignment = TextAnchor.LowerLeft;
				break;
			case MilMo_GUI.Align.BottomCenter:
				LabelStyle.alignment = TextAnchor.LowerCenter;
				break;
			case MilMo_GUI.Align.BottomRight:
				LabelStyle.alignment = TextAnchor.LowerRight;
				break;
			}
			_dirtyLabelStyle = false;
		}
	}

	public virtual void SetEnabled(bool e)
	{
		Enabled = e;
	}

	public bool IsEnabled()
	{
		if (!Enabled)
		{
			return false;
		}
		bool result = true;
		if (Parent != null)
		{
			result = Parent.IsEnabled();
		}
		return result;
	}

	public void SetAlignment(MilMo_GUI.Align align)
	{
		Align = MilMo_GUI.Align2Float(align);
	}

	public void SetCropMode(MilMo_GUI.CropMode cropMode)
	{
		_cropMode = cropMode;
	}

	public void SetTexture(MilMo_Texture texture)
	{
		CurrentTexture = texture;
		Texture = texture;
	}

	public void SetTexture(Texture2D texture)
	{
		SetTexture((texture != null) ? new MilMo_Texture(texture) : new MilMo_Texture("Invisible"));
	}

	public void SetTexture(string filename, bool prefixStandardGuiPath)
	{
		Texture = ((prefixStandardGuiPath && !filename.Contains("Content/")) ? new MilMo_Texture("Content/GUI/" + filename) : new MilMo_Texture(filename));
		Texture.AsyncLoad();
		CurrentTexture = Texture;
	}

	private async void SetTextureAndFadeIn(string filename)
	{
		SetTexture(await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/" + filename));
		FadeAlpha(0f, 1f);
	}

	public async void SetTextureAndFadeIn(string filename, bool simulateLag)
	{
		if (!simulateLag)
		{
			SetTextureAndFadeIn(filename);
			return;
		}
		await Task.Delay(UnityEngine.Random.Range(200, 500));
		SetTextureAndFadeIn(filename);
	}

	public virtual void SetTexture(string filename)
	{
		SetTexture(filename, prefixStandardGuiPath: true);
	}

	public void SetTextureBlack()
	{
		SetTexture("Batch01/Textures/Core/Black");
	}

	public void SetTextureWhite()
	{
		SetTexture("Batch01/Textures/Core/White");
	}

	public void SetTextureInvisible()
	{
		SetTexture("Batch01/Textures/Core/Invisible");
	}

	public void SetTextureBlackTransparent()
	{
		SetTexture("Batch01/Textures/Core/BlackTransparent");
	}

	public void SetSkin(int skin)
	{
		if (skin <= UI.NumSkins)
		{
			Skin = UI.Skins[skin];
		}
	}

	protected static void Nothing(object o)
	{
	}

	protected static void Nothing()
	{
	}

	public static string Debug_DrawIdentifierMode(string[] args)
	{
		_drawIdentifier = !_drawIdentifier;
		return _drawIdentifier.ToString();
	}

	public virtual bool Hover()
	{
		if (MilMo_UserInterface.PointerFocus != this)
		{
			return UI.AncestorOfFocused == this;
		}
		return true;
	}

	protected void CheckPointerFocus()
	{
		if (MilMo_Pointer.LeftButton || !AllowPointerFocus)
		{
			return;
		}
		Rect pointerRect = GetPointerRect();
		pointerRect = AddWindowOffset(pointerRect);
		MilMo_ScrollView scrollViewAncestor = GetScrollViewAncestor();
		if (scrollViewAncestor != null && !scrollViewAncestor.ContainsMouse())
		{
			pointerRect = new Rect(0f, 0f, 0f, 0f);
		}
		if (!pointerRect.Contains(MilMo_Pointer.Position))
		{
			return;
		}
		if (HasEnclosingParent())
		{
			if (GetParentRect().Contains(MilMo_Pointer.Position))
			{
				MilMo_UserInterface.PointerFocus = this;
			}
		}
		else
		{
			MilMo_UserInterface.PointerFocus = this;
		}
	}

	private Rect GetPointerRect()
	{
		Rect result = new Rect(Pos.x, Pos.y, Scale.x, Scale.y);
		result.x += UI.GlobalPosOffset.x;
		result.y += UI.GlobalPosOffset.y;
		if (_fixedPointerZoneSize != Vector2.zero)
		{
			result.width = _fixedPointerZoneSize.x;
			result.height = _fixedPointerZoneSize.y;
			result.x += _fixedPointerZoneOffset.x - result.width * Align.x;
			result.y += _fixedPointerZoneOffset.y - result.height * Align.y;
			result.x += Scale.x * Align.x;
			result.y += Scale.y * Align.y;
		}
		Rect ancestorPointerOffset = GetAncestorPointerOffset();
		result.x += ancestorPointerOffset.x;
		result.y += ancestorPointerOffset.y;
		result.width += ancestorPointerOffset.width;
		result.height += ancestorPointerOffset.height;
		return result;
	}

	private Rect AddWindowOffset(Rect rect)
	{
		if (GetWindowAncestor() == null)
		{
			return rect;
		}
		if (Parent == null)
		{
			return rect;
		}
		if (Parent is MilMo_PropertyPage)
		{
			rect.x += UI.GlobalInputOffset.x;
			rect.y += UI.GlobalInputOffset.y;
		}
		rect.x += UI.GlobalInputOffset.x;
		rect.y += UI.GlobalInputOffset.y;
		if (GetType() == typeof(MilMo_PropertyPage))
		{
			rect.x += UI.GlobalInputOffset.x;
			rect.y += UI.GlobalInputOffset.y;
		}
		if (GetPropertyPageAncestor() != null)
		{
			rect.x -= UI.GlobalInputOffset.x;
			rect.y -= UI.GlobalInputOffset.y;
		}
		return rect;
	}

	private Rect GetParentRect()
	{
		ReportFocusToParent();
		Rect screenPosition = UI.AncestorOfFocused.GetScreenPosition();
		if (!IgnoreInputOffsetMode)
		{
			screenPosition.x += UI.GlobalInputOffset.x;
			screenPosition.y += UI.GlobalInputOffset.y;
		}
		if (UI.AncestorOfFocused.IsWindow)
		{
			MilMo_SimpleWindow milMo_SimpleWindow = (MilMo_SimpleWindow)UI.AncestorOfFocused;
			screenPosition.x += milMo_SimpleWindow.Scale.x * 0.5f;
		}
		if (UI.AncestorOfFocused.GetType() == typeof(MilMo_SimpleBox) || UI.AncestorOfFocused.IsGameDialog)
		{
			screenPosition.x -= UI.GlobalInputOffset.x;
			screenPosition.y -= UI.GlobalInputOffset.y;
		}
		return screenPosition;
	}

	protected Rect GetAncestorPointerOffset()
	{
		Rect result = new Rect(0f, 0f, 0f, 0f);
		if (Parent == null)
		{
			return result;
		}
		bool flag = false;
		MilMo_Widget parent = Parent;
		while (!flag)
		{
			result.x += parent.Pos.x;
			result.y += parent.Pos.y;
			if (parent.GetType() == typeof(MilMo_ScrollView))
			{
				MilMo_ScrollView milMo_ScrollView = (MilMo_ScrollView)parent;
				result.x -= milMo_ScrollView.GetScrollPosition().x;
				result.y -= milMo_ScrollView.GetScrollPosition().y;
				result.x += UI.GlobalInputOffset.x;
				result.y += UI.GlobalInputOffset.y;
			}
			if (parent.IsWindow)
			{
				MilMo_SimpleWindow milMo_SimpleWindow = (MilMo_SimpleWindow)parent;
				result.x += milMo_SimpleWindow.Scale.x * 0.5f;
			}
			if (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			else
			{
				flag = true;
			}
		}
		return result;
	}

	protected virtual bool MouseInsideScroller(MilMo_ScrollView scroller)
	{
		Rect screenPosition = scroller.GetScreenPosition();
		Rect ancestorPointerOffset = scroller.GetAncestorPointerOffset();
		if (GetWindowAncestor() != null)
		{
			screenPosition.x += ancestorPointerOffset.x;
			screenPosition.y += ancestorPointerOffset.y;
			screenPosition.width += ancestorPointerOffset.width;
			screenPosition.height += ancestorPointerOffset.height;
		}
		screenPosition.x += UI.GlobalInputOffset.x;
		screenPosition.y += UI.GlobalInputOffset.y;
		return screenPosition.Contains(MilMo_Pointer.Position);
	}

	private void ReportFocusToParent()
	{
		if (Parent != null)
		{
			UI.AncestorOfFocused = Parent;
			Parent.ReportFocusToParent();
		}
	}

	public void SetFixedPointerZoneSize(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		_fixedPointerZoneSize.x = x;
		_fixedPointerZoneSize.y = y;
	}

	public void SetFixedPointerZoneOffset(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		_fixedPointerZoneOffset.x = x;
		_fixedPointerZoneOffset.y = y;
	}

	private bool HasEnclosingParent()
	{
		if (Parent != null && Parent.GetType() != typeof(MilMo_Widget) && Parent.GetType() != typeof(MilMo_GameDialog) && Parent.GetType() != typeof(MilMo_SimpleBox))
		{
			return Parent.GetType() != typeof(MilMo_ToggleBar);
		}
		return false;
	}

	public Rect GetScreenPosition()
	{
		Rect result = new Rect(Pos.x, Pos.y, Scale.x, Scale.y);
		Rect ancestorOffset = GetAncestorOffset();
		result.x += ancestorOffset.x;
		result.y += ancestorOffset.y;
		result.width += ancestorOffset.width;
		result.height += ancestorOffset.height;
		result.x += UI.GlobalPosOffset.x;
		result.y += UI.GlobalPosOffset.y;
		return result;
	}

	protected Rect GetAlignedScreenPosition()
	{
		RestoreAlign();
		Rect result = new Rect(Pos.x, Pos.y, Scale.x, Scale.y);
		ApplyAlign();
		Rect ancestorOffset = GetAncestorOffset();
		result.x += ancestorOffset.x;
		result.y += ancestorOffset.y;
		result.width += ancestorOffset.width;
		result.height += ancestorOffset.height;
		result.x += UI.GlobalPosOffset.x;
		result.y += UI.GlobalPosOffset.y;
		return result;
	}

	private void ApplyAlign()
	{
		_realignPos.x = Pos.x;
		_realignPos.y = Pos.y;
		Pos.x -= Scale.x * Align.x;
		Pos.y -= Scale.y * Align.y;
	}

	protected void DoGetPosFrom()
	{
		if (GetPosFrom != null)
		{
			Vector2 pos = GetPosFrom.Pos;
			Vector2 scale = GetPosFrom.Scale;
			Vector2 align = GetPosFrom.Align;
			pos.x += scale.x * align.x;
			pos.y += scale.y * align.y;
			GoToNow(pos.x / Res.x, pos.y / Res.y);
			RestoreAlign();
			ScaleMover.Update();
			AngleMover.Update();
			Scale = ScaleMover.Val;
			ApplyAlign();
		}
	}

	public void SetMoveType(MilMo_Mover.UpdateFunc updateType)
	{
		_moveType = updateType;
		PosMover.SetUpdateFunc(updateType);
	}

	public void GoTo(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		PosMover.Target.x = x;
		PosMover.Target.y = y;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void GoTo(Vector2 pos)
	{
		GoTo(pos.x, pos.y);
	}

	public void GoToX(float x)
	{
		x *= Res.x;
		PosMover.Target.x = x;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void GoToY(float y)
	{
		y *= Res.y;
		PosMover.Target.y = y;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void GoToNow(float x, float y)
	{
		RestoreAlign();
		x *= Res.x;
		y *= Res.y;
		PosMover.Target.x = x;
		PosMover.Target.y = y;
		Pos.x = x;
		Pos.y = y;
		PosMover.Val.x = x;
		PosMover.Val.y = y;
		PosMover.Vel.x = 0f;
		PosMover.Vel.y = 0f;
		ApplyAlign();
		PosMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
	}

	public void GoToNow(Vector2 pos)
	{
		GoToNow(pos.x, pos.y);
	}

	public void SetXPos(float x)
	{
		RestoreAlign();
		x *= Res.x;
		PosMover.Target.x = x;
		Pos.x = x;
		PosMover.Val.x = x;
		PosMover.Vel.x = 0f;
		ApplyAlign();
	}

	public void SetYPos(float y)
	{
		RestoreAlign();
		y *= Res.y;
		PosMover.Target.y = y;
		Pos.y = y;
		PosMover.Val.y = y;
		PosMover.Vel.y = 0f;
		ApplyAlign();
	}

	public virtual void SetPosition(float x, float y)
	{
		GoToNow(x, y);
		UI.UpdateAutoLayout(this);
	}

	public virtual void SetPosition(Vector2 pos)
	{
		GoToNow(pos.x, pos.y);
		UI.UpdateAutoLayout(this);
	}

	public void Impulse(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		PosMover.Vel.x += x;
		PosMover.Vel.y += y;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void Impulse(Vector2 impulse)
	{
		impulse.x *= Res.x;
		impulse.y *= Res.y;
		PosMover.Vel.x += impulse.x;
		PosMover.Vel.y += impulse.y;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void ImpulseRandom(float minX, float maxX, float minY, float maxY)
	{
		Vector2 vector = default(Vector2);
		vector.x = UnityEngine.Random.Range(minX, maxX);
		vector.y = UnityEngine.Random.Range(minY, maxY);
		vector.x *= Res.x;
		vector.y *= Res.y;
		PosMover.Vel.x += vector.x;
		PosMover.Vel.y += vector.y;
		PosMover.SetUpdateFunc(_moveType);
	}

	public void SetPosPull(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		PosMover.Pull.x = x;
		PosMover.Pull.y = y;
	}

	public void SetPosDrag(float x, float y)
	{
		PosMover.Drag.x = x;
		PosMover.Drag.y = y;
	}

	private void LookAlongForwardVector()
	{
		float num = Mathf.Atan2(_lastPos.x - Pos.x, _lastPos.y - Pos.y) * 180f / 3.14f;
		num = 360f - num;
		Angle(num);
	}

	private void LookAtPosMoverTargetPosition()
	{
		float num = Mathf.Atan2(Pos.x - PosMover.Target.x, Pos.y - PosMover.Target.y) * 180f / 3.14f;
		num = 360f - num;
		Angle(num);
	}

	private void LookAtFixedScreenPosition()
	{
		float num = Mathf.Atan2(Pos.x - _lookAtPosition.x, Pos.y - _lookAtPosition.y) * 180f / 3.14f;
		num = 360f - num;
		Angle(num);
	}

	public void SetRotationType(MilMo_Mover.UpdateFunc updateType)
	{
		_rotationType = updateType;
		if (updateType == MilMo_Mover.UpdateFunc.Linear)
		{
			AngleMover.LoopVal.x = 360f;
		}
		AngleMover.SetUpdateFunc(_rotationType);
	}

	public void Angle(float a)
	{
		AngleMover.Target.x = a;
		AngleMover.SetUpdateFunc(_rotationType);
	}

	private void AngleNow(float a)
	{
		AngleMover.Target.x = a;
		AngleMover.Val.x = a;
		AngleMover.Vel.x = 0f;
		AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
	}

	public void SetAngle(float a)
	{
		AngleNow(a);
	}

	public void SetAngleVel(float a)
	{
		AngleMover.Vel.x = a;
		AngleMover.SetUpdateFunc(_rotationType);
	}

	public void AngleImpulse(float min, float max)
	{
		AngleMover.Vel.x += UnityEngine.Random.Range(min, max);
		AngleMover.SetUpdateFunc(_rotationType);
	}

	public void SetAnglePull(float p)
	{
		AngleMover.Pull.x = p;
	}

	public void SetAngleDrag(float d)
	{
		AngleMover.Drag.x = d;
	}

	public void SetScaleType(MilMo_Mover.UpdateFunc updateType)
	{
		ScaleType = updateType;
		ScaleMover.SetUpdateFunc(updateType);
	}

	public void ScaleTo(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.Target.x = x;
		ScaleMover.Target.y = y;
		ScaleMover.SetUpdateFunc(ScaleType);
	}

	public void ScaleTo(Vector2 scale)
	{
		ScaleTo(scale.x, scale.y);
	}

	public void ScaleToAbsolute(float x, float y)
	{
		ScaleMover.Target.x = x;
		ScaleMover.Target.y = y;
		ScaleMover.SetUpdateFunc(ScaleType);
	}

	public void ScaleToAbsolute(Vector2 scale)
	{
		ScaleToAbsolute(scale.x, scale.y);
	}

	public void ScaleNow(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.Target.x = x;
		ScaleMover.Target.y = y;
		Scale.x = x;
		Scale.y = y;
		ScaleMover.Val.x = x;
		ScaleMover.Val.y = y;
		ScaleMover.Vel.x = 0f;
		ScaleMover.Vel.y = 0f;
		ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
	}

	public void ScaleNow(Vector2 scale)
	{
		ScaleNow(scale.x, scale.y);
	}

	public void ScaleNowAbsolute(float x, float y)
	{
		ScaleMover.Target.x = x;
		ScaleMover.Target.y = y;
		Scale.x = x;
		Scale.y = y;
		ScaleMover.Val.x = x;
		ScaleMover.Val.y = y;
		ScaleMover.Vel.x = 0f;
		ScaleMover.Vel.y = 0f;
		ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
	}

	public void ScaleNowAbsolute(Vector2 scale)
	{
		ScaleNowAbsolute(scale.x, scale.y);
	}

	public void SetScaleAbsolute(float x, float y)
	{
		ScaleNowAbsolute(x, y);
		UI.UpdateAutoLayout(this);
	}

	public void SetScaleAbsolute(Vector2 scale)
	{
		ScaleNowAbsolute(scale);
		UI.UpdateAutoLayout(this);
	}

	public virtual void SetScale(float x, float y)
	{
		ScaleNow(x, y);
		UI.UpdateAutoLayout(this);
	}

	public virtual void SetScale(Vector2 s)
	{
		ScaleNow(s);
		UI.UpdateAutoLayout(this);
	}

	public void SetScaleToTexture()
	{
		if (Texture != null && !Texture.Texture)
		{
			SetScale(Texture.Texture.width, Texture.Texture.height);
		}
	}

	public void SetScaleToTextureAbsolute()
	{
		if (Texture != null && (bool)Texture.Texture)
		{
			SetScaleAbsolute(new Vector2(Texture.Texture.width, Texture.Texture.height));
		}
	}

	public void ScaleToTexture()
	{
		if (Texture != null && !(Texture.Texture == null))
		{
			ScaleTo(Texture.Texture.width, Texture.Texture.height);
		}
	}

	public void SetXScale(float x)
	{
		RestoreAlign();
		x *= Res.x;
		ScaleMover.Target.x = x;
		Scale.x = x;
		ScaleMover.Val.x = x;
		ScaleMover.Vel.x = 0f;
		ApplyAlign();
	}

	public virtual void SetYScale(float y)
	{
		RestoreAlign();
		y *= Res.y;
		ScaleMover.Target.y = y;
		Scale.y = y;
		ScaleMover.Val.y = y;
		ScaleMover.Vel.y = 0f;
		ApplyAlign();
	}

	public void SetScalePull(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.Pull.x = x;
		ScaleMover.Pull.y = y;
	}

	public void SetScaleDrag(float x, float y)
	{
		ScaleMover.Drag.x = x;
		ScaleMover.Drag.y = y;
	}

	public void SetMinScaleVel(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.MinVel.x = x;
		ScaleMover.MinVel.y = y;
	}

	public void SetMinScale(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.MinVal.x = x;
		ScaleMover.MinVal.y = y;
	}

	public void SetMinScale(Vector2 val)
	{
		SetMinScale(val.x, val.y);
	}

	public void ScaleImpulse(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		ScaleMover.Vel.x += x;
		ScaleMover.Vel.y += y;
		ScaleMover.SetUpdateFunc(ScaleType);
	}

	protected virtual void DrawText()
	{
		if (_drawIdentifier)
		{
			SetFont(MilMo_GUI.Font.ArialRounded);
			Text = MilMo_Localization.GetNotLocalizedLocString(Identifier);
			if (Parent != null)
			{
				Text = MilMo_Localization.GetNotLocalizedLocString(Parent.Identifier);
			}
		}
		if (Text == null || Text.Length == 0)
		{
			return;
		}
		Rect screenPosition = GetScreenPosition();
		screenPosition.x -= _extraDrawTextSize.x / 2f;
		screenPosition.y -= _extraDrawTextSize.y / 2f;
		screenPosition.width += _extraDrawTextSize.x;
		screenPosition.height += _extraDrawTextSize.y;
		if (_textAngle != 0f)
		{
			RestoreAlign();
			GUIUtility.RotateAroundPivot(_textAngle, new Vector2(screenPosition.x + Pos.x + TextOffset.x, screenPosition.y + Pos.y + TextOffset.y));
			ApplyAlign();
		}
		string @string = Text.String;
		UpdateLabelStyle();
		Rect rect = screenPosition;
		screenPosition.width *= 1f / _fontScale.x;
		screenPosition.height *= 1f / _fontScale.y;
		rect.width = (screenPosition.width - rect.width) / 2f;
		rect.height = (screenPosition.height - rect.height) / 2f;
		screenPosition.x -= rect.width;
		screenPosition.y -= rect.height;
		Vector2 pivotPoint = new Vector2(screenPosition.x + screenPosition.width * 0.5f, screenPosition.y + screenPosition.height * 0.5f);
		if (_fontScale.x > 0f || _fontScale.y > 0f)
		{
			GUIUtility.ScaleAroundPivot(_fontScale, pivotPoint);
		}
		else
		{
			GUIUtility.ScaleAroundPivot(_fontScaleFallback, pivotPoint);
		}
		if (TextDropShadowPos.x != 0f && TextDropShadowPos.y != 0f)
		{
			Color textDropShadowColor = TextDropShadowColor;
			textDropShadowColor.a *= CurrentColor.a * TextColor.a;
			if (Parent != null && UseParentAlpha)
			{
				textDropShadowColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = textDropShadowColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Rect position = screenPosition;
			position.x += TextOffset.x + TextDropShadowPos.x;
			position.y += TextOffset.y + TextDropShadowPos.y;
			LabelStyle.hover.textColor = textDropShadowColor;
			UnityEngine.GUI.Label(position, @string, LabelStyle);
		}
		if (TextOutline.x > 0f || TextOutline.y > 0f)
		{
			Color textOutlineColor = TextOutlineColor;
			textOutlineColor.a *= CurrentColor.a * TextColor.a;
			if (Parent != null && UseParentAlpha)
			{
				textOutlineColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = textOutlineColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Rect rect2 = screenPosition;
			rect2.x += TextOffset.x;
			rect2.y += TextOffset.y;
			Rect position2 = rect2;
			position2.y -= TextOutline.y;
			Rect position3 = rect2;
			position3.x += TextOutline.x;
			Rect position4 = rect2;
			position4.x -= TextOutline.x;
			Rect position5 = rect2;
			position5.y += TextOutline.y;
			LabelStyle.hover.textColor = textOutlineColor;
			UnityEngine.GUI.Label(position2, @string, LabelStyle);
			UnityEngine.GUI.Label(position3, @string, LabelStyle);
			UnityEngine.GUI.Label(position4, @string, LabelStyle);
			UnityEngine.GUI.Label(position5, @string, LabelStyle);
		}
		screenPosition.x += TextOffset.x;
		screenPosition.y += TextOffset.y;
		if (_hasTextSelection)
		{
			Color textSelectionColor = TextSelectionColor;
			textSelectionColor.a *= CurrentColor.a;
			UnityEngine.GUI.color = textSelectionColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			GUIContent content = new GUIContent(@string);
			LabelStyle.DrawWithTextSelection(screenPosition, content, 0, (int)_textSelection.x, (int)_textSelection.y);
		}
		Color textColor = TextColor;
		textColor.a *= CurrentColor.a;
		if (Parent != null && UseParentAlpha)
		{
			textColor.a *= Parent.CurrentColor.a;
		}
		Color color2 = (LabelStyle.hover.textColor = textColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade));
		UnityEngine.GUI.color = color2;
		UnityEngine.GUI.Label(screenPosition, @string, LabelStyle);
		RestoreMatrix();
	}

	public virtual void SetText(MilMo_LocString text)
	{
		Text = text;
	}

	public string GetText()
	{
		return Text.String;
	}

	public void SetTextNoLocalization(string text)
	{
		SetText(MilMo_Localization.GetNotLocalizedLocString(text));
	}

	public void SetFont(MilMo_GUI.Font font)
	{
		switch (font)
		{
		case MilMo_GUI.Font.ArialRounded:
			Font = UI.Font0;
			break;
		case MilMo_GUI.Font.ArialRoundedMedium:
			Font = UI.Font11;
			break;
		case MilMo_GUI.Font.EborgSmall:
			Font = UI.Font4;
			break;
		case MilMo_GUI.Font.EborgMedium:
			Font = UI.Font5;
			break;
		case MilMo_GUI.Font.EborgLarge:
			Font = UI.Font6;
			break;
		case MilMo_GUI.Font.EborgXL:
			Font = UI.Font7;
			break;
		case MilMo_GUI.Font.GothamSmall:
			Font = UI.Font8;
			break;
		case MilMo_GUI.Font.GothamMedium:
			Font = UI.Font9;
			break;
		case MilMo_GUI.Font.GothamLarge:
			Font = UI.Font10;
			break;
		default:
			Font = UI.Font0;
			break;
		}
		_dirtyLabelStyle = true;
	}

	public void SetFontScale(float e)
	{
		_fontScale.x = e * Res.x;
		_fontScale.y = e * Res.y;
	}

	public void SetFontScale(float x, float y)
	{
		_fontScale.x = x * Res.x;
		_fontScale.y = y * Res.y;
	}

	public void SetFontPreset(MilMo_GUI.FontPreset fontPreset)
	{
		switch (fontPreset)
		{
		case MilMo_GUI.FontPreset.Normal:
			SetTextDropShadowPos(0f, 0f);
			TextOutline = new Vector2(0f, 0f);
			break;
		case MilMo_GUI.FontPreset.DropShadow:
			SetTextDropShadowPos(2f, 2f);
			break;
		case MilMo_GUI.FontPreset.Outline:
			TextOutline = new Vector2(1f, 1f);
			TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
			break;
		case MilMo_GUI.FontPreset.OutlineDropShadow:
			SetTextDropShadowPos(2f, 2f);
			TextOutline = new Vector2(1f, 1f);
			TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
			break;
		default:
			SetTextDropShadowPos(0f, 0f);
			TextOutline = new Vector2(0f, 0f);
			break;
		}
	}

	public void SetTextAlignment(MilMo_GUI.Align align)
	{
		TextAlign = align;
		_dirtyLabelStyle = true;
	}

	public void SetWordWrap(bool w)
	{
		_wordWrap = w;
		_dirtyLabelStyle = true;
	}

	public void SetTextDropShadowPos(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		TextDropShadowPos.x = x;
		TextDropShadowPos.y = y;
	}

	public void SetTextOutline(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		TextOutline.x = x;
		TextOutline.y = y;
	}

	public void SetTextOffset(float x, float y)
	{
		x *= Res.x;
		y *= Res.y;
		TextOffset.x = x;
		TextOffset.y = y;
	}

	public void SetTextPadding(float left, float right, float top, float bottom)
	{
		left *= Res.x;
		right *= Res.x;
		top *= Res.y;
		bottom *= Res.y;
		_textPadding.left = (int)left;
		_textPadding.right = (int)right;
		_textPadding.top = (int)top;
		_textPadding.bottom = (int)bottom;
	}

	public void TextColorTo(float r, float g, float b, float a)
	{
		TargetTextColor.r = r;
		TargetTextColor.g = g;
		TargetTextColor.b = b;
		TargetTextColor.a = a;
	}

	public void TextColorTo(Color col)
	{
		TextColorTo(col.r, col.g, col.b, col.a);
	}

	public void TextColorNow(float r, float g, float b, float a)
	{
		TextColor.r = r;
		TextColor.g = g;
		TextColor.b = b;
		TextColor.a = a;
		TargetTextColor.r = r;
		TargetTextColor.g = g;
		TargetTextColor.b = b;
		TargetTextColor.a = a;
	}

	protected void TextColorNow(Color col)
	{
		TextColorNow(col.r, col.g, col.b, col.a);
	}

	public virtual void SetTextColor(float r, float g, float b, float a)
	{
		TextColorNow(r, g, b, a);
	}

	public virtual void SetTextColor(Color col)
	{
		TextColorNow(col);
	}

	public void SetTextAngle(float a)
	{
		_textAngle = a;
	}

	public void SetExtraDrawTextSize(float x, float y)
	{
		x *= Res.y;
		y *= Res.y;
		_extraDrawTextSize = new Vector2(x, y);
	}

	public void SetDefaultTextColor(float r, float g, float b, float a)
	{
		DefaultTextColor.r = r;
		DefaultTextColor.g = g;
		DefaultTextColor.b = b;
		DefaultTextColor.a = a;
		TextColor.r = r;
		TextColor.g = g;
		TextColor.b = b;
		TextColor.a = a;
		TargetTextColor.r = r;
		TargetTextColor.g = g;
		TargetTextColor.b = b;
		TargetTextColor.a = a;
	}

	public void SetDefaultTextColor(Color col)
	{
		SetDefaultTextColor(col.r, col.g, col.b, col.a);
	}

	public void SetDefaultTextColor255(float r, float g, float b, float a)
	{
		SetDefaultTextColor(r / 255f, g / 255f, b / 255f, a / 255f);
	}

	public void SetSelection(int start, int end)
	{
		_textSelection.x = start;
		_textSelection.y = end;
		if (start == 0 && end == 0)
		{
			_hasTextSelection = false;
		}
		else
		{
			_hasTextSelection = true;
		}
	}
}
