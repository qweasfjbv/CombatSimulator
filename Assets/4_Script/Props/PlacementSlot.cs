using Autobattler.Controller;
using Autobattler.Utils;
using IUtil;
using System.Collections.Generic;
using UnityEngine;

namespace Autobattler.Props
{
	public enum SlotDir
	{
		Front = 0,
		Back = 2,
	}

	public class PlacementSlot : MonoBehaviour
	{
		[SerializeField, ReadOnly]
		private List<UnitController> units = new();

		private SlotDir slotDir = SlotDir.Back;
		private int slotID = -1;

		private static float[,,] relativePos = new float[4, 3, 2]
		{
			{	// Count 0
				{0f, 0f}, {0f, 0f}, {0f, 0f}
			},
			{	// Count 1
				{0f, 0f}, {0f, 0f}, {0f, 0f}
			},
			{	// Count 2
				{-.2f, 0f}, {.2f, 0f}, {0f, 0f}
			},
			{	// Count 3
				{-.2f, -.15f}, {.2f, -.15f}, {0f, .15f}
			},
		};

		public void InitSlot(SlotDir dir, int slotID)
		{
			slotDir = dir;
		}

		private void SetStartSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_START : Color.white;
		}
		private void SetEndSlot(bool on)
		{
			GetComponent<Renderer>().material.color = on ? Constants.COLOR_SLOT_END : Color.white;
		}

		public bool IsEmpty()
		{
			return units.Count == 0;
		}
		public bool IsAbleToAdd(int unitId, int rarity)
		{
			if (units.Count >= Constants.SLOT_UNIT_MAX) return false;
			if (units.Count == 0) return false;

			return units[0].IsSameUnit(unitId, rarity);
		}

		public void AddUnit(UnitController controller)
		{
			units.Add(controller);
			controller.MySlot = this;
			DropAllUnits();
		}

		public void SetUnits(List<UnitController> units)
		{
			if (units != null)
			{
				this.units = units;
				for(int i=0; i<units.Count; i++)
				{
					units[i].MySlot = this;
				}
			}
			else
			{
				this.units = null;
			}
		}

		public void OnStartStage()
		{
			GetComponent<MeshRenderer>().material.color = Color.clear;
			for(int i=0;i<units.Count;i++)
			{
				units[i].OnStartStage();
			}
		}

		public void OnEndStage()
		{
			GetComponent<MeshRenderer>().material.color = Color.white;
			for (int i = 0; i < units.Count; i++)
			{
				units[i].OnEndStage();
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
			PickAllUnits();
			isSelected = true;
			SetStartSlot(true);
		}
		public void OnRelease()
		{
			DropAllUnits();
			isSelected = false;
			SetStartSlot(false);
		}

		public void ChangeSlot(PlacementSlot slot)
		{
			OnRelease();
			if (slot == this) return;
			slot.OnRelease();

			List<UnitController> tmpUnit = slot.units;
			slot.SetUnits(this.units);
			SetUnits(tmpUnit);

			DropAllUnits();
			slot.DropAllUnits();
		}

		private void DropAllUnits()
		{
			for (int i = 0; i < units.Count; i++)
			{
				units[i].SetPlayerTeam(slotDir == SlotDir.Back ? 1 : 0, slotID);
				units[i].DropTo(transform.position + 
					Constants.SLOT_WIDTH * 
					(Quaternion.Euler(0, 90*(int)slotDir, 0) * new Vector3(relativePos[units.Count,i,0], 0f, relativePos[units.Count,i,1]))); 
			}
		}

		private void PickAllUnits()
		{
			for (int i = 0; i < units.Count; i++) { units[i].PickUp(transform.position.y); }
		}
	}
}