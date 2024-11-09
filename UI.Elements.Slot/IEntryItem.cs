using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace UI.Elements.Slot;

public interface IEntryItem
{
	Texture2D GetItemIcon();

	MilMo_LocString GetDisplayName();

	MilMo_LocString GetDescription();

	Task<Texture2D> AsyncGetIcon();
}
