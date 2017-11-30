﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using OzoneDecals;
using Selection;

namespace EditMap
{
	public partial class DecalsInfo : MonoBehaviour
	{

		private void OnEnable()
		{
			SelectionManager.Current.DisableLayer = 14;
			SelectionManager.Current.SetRemoveAction(DestroyDetails);
			SelectionManager.Current.SetSelectionChangeAction(SelectDetails);
			SelectionManager.Current.SetCustomSnapAction(OzoneDecal.SnapToGround);

			GoToSelection();
		}

		public void GoToSelection()
		{
			/*
			if (!MarkersInfo.MarkerPageChange)
			{
				Selection.SelectionManager.Current.CleanSelection();
			}
			*/

			SelectionManager.Current.SetAffectedGameObjects(DecalsControler.GetAllDecalsGo(), SelectionManager.SelectionControlTypes.Decal);
			//SelectionManager.Current.SetCustomSettings(true, true, true);


			PlacementManager.Clear();
			//if (ChangeControlerType.Current)
			//	ChangeControlerType.Current.UpdateButtons();

			//MarkerSelectionOptions.UpdateOptions();
		}



		public void SelectDetails()
		{
			if (SelectionManager.Current.AffectedGameObjects.Length == 0 || SelectionManager.Current.Selection.Ids.Count == 0)
				DecalSettingsUi.Load(null);
			else
				DecalSettingsUi.Load(SelectionManager.Current.AffectedGameObjects[SelectionManager.Current.Selection.Ids[0]].GetComponent<OzoneDecal>().Shared);

			DecalsList.UpdateSelection();
		}

		public void CustomSnapAction(Transform tr)
		{
			OzoneDecal.SnapToGround(tr);

		}

		public void DestroyDetails(List<GameObject> MarkerObjects, bool RegisterUndo = true)
		{

		}

		public void Place(Vector3[] Positions, Quaternion[] Rotations)
		{
		}



	}
}