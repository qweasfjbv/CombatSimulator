using Cysharp.Threading.Tasks.Triggers;
using Defense.Controller;
using Defense.Manager;
using Defense.Utils;
using IUtil;
using UnityEngine;

namespace Defense.Props
{
	public class PlacementSlot : MonoBehaviour
	{
		[SerializeField, ReadOnly]
		private bool isOccupied = false;
		[SerializeField, ReadOnly]
		private UnitController unit;

		public bool IsOccupied { get {  return isOccupied; } }

		private void SetStartSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_START : Color.white;
		}
		private void SetEndSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_END : Color.white;
		}

		public void SetUnit(UnitController unit)
		{
			if (unit != null)
			{
				isOccupied = true;
				this.unit = unit;
				unit.DropTo(transform.position);
			}
			else
			{
				isOccupied = false;
				this.unit = null;
			}
		}

		private bool isSelected = false;
		public void OnHover()
		{
			if (isSelected) return;
			SetEndSlot(true);
		}
		public void OnUnhover()
		{
			if (isSelected) return;
			SetEndSlot(false);
		}
		public void OnSelect()
		{
			if (unit != null) unit.PickUp();
			isSelected = true;
			SetStartSlot(true);
		}
		public void OnRelease()
		{
			if (unit != null) unit.DropTo(transform.position);
			isSelected = false;
			SetStartSlot(false);
		}

		public void ChangeSlot(PlacementSlot slot)
		{
			OnRelease();
			if (slot == this) return;
			slot.OnRelease();

			UnitController tmpUnit = slot.unit;
			slot.SetUnit(this.unit);
			SetUnit(tmpUnit);

			unit?.DropTo(transform.position);
			slot.unit?.DropTo(slot.transform.position);
		}
	}
}