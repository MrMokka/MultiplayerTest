#if UNITY_EDITOR

using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Examples.HierarchyPlugin
{


	public class RightClickOnGameObjectMenu_Examples : MonoBehaviour
	{


		//////// Custom ExtensionInterface_RightClickOnGameObjectMenuItem Example ////////
		/*
			To create a hotkey you can use the following special characters: % (ctrl on Windows, cmd on macOS), # (shift), & (alt).
			To create a special hotkey you can use "MySubItem/MyMenuItem %LEFT" "MySubItem/MyMenuItem %HOME" "MySubItem/MyMenuItem #ENDER".
		*/

		/////////////////////////////////////////////////////MENU ITEM TEMPLATE///////////////////////////////////////////////////////////////////////////////
		/*
			class MyMenu : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
			{

				[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
				public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu(); } // <---- !!

				public string Name { get { return "MySubItem/MyMenuItem %k"; } }
				public int PositionInMenu { get { return 0; } }

				public bool IsEnable(GameObject clickedObject) { return true; }
				public bool NeedExcludeFromMenu(GameObject clickedObject) { return false; } // or 'return clickedObject.GetComponent<MyComponent>() == null'

				public void OnClick(GameObject[] affectedObjectsArray)
				{
					throw new System.NotImplementedException();
				}
			}

		*/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



		#region ITEM 100-101 - Group/UnGroup
		class MyMenu_GroupFirst : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_GroupFirst(); }

			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 100; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (Last Center) %g"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if (onlytop.Length == 0) return;

				var center_object = onlytop[onlytop.Length - 1];

				var groupParent = center_object.transform.parent;
				var groupSiblingIndex = center_object.transform.GetSiblingIndex();

				var reference = center_object;
				var NEW_NAME = reference.name + " Group";
				// Save Previous Name Variant
				// CustomModule.SHOW_StringInput( "Group Name:", EditorPrefs.GetString( "EMX/MultyRenamer/GroupName", NEW_NAME ) , ( name ) =>
				// {   EditorPrefs.SetString( "EMX/MultyRenamer/GroupName", name );

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput("Group Name:", NEW_NAME, (name) =>
				{

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent(groupParent, false);
					//groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex(groupSiblingIndex);
					//********************************//
					//groupRoot.transform.position = center_object.transform.position;
					//groupRoot.transform.rotation = center_object.transform.rotation;
					//********************************//

					//MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if (reference.GetComponent<RectTransform>())     // canvas
					{
						var source = reference.GetComponent<RectTransform>();
						var dest = groupRoot.AddComponent<RectTransform>();
						dest.sizeDelta = source.sizeDelta;
						dest.pivot = source.pivot;
						dest.anchoredPosition3D = source.anchoredPosition3D;
						dest.localRotation = source.localRotation;
						dest.localScale = source.localScale;
						dest.anchorMin = source.anchorMin;
						dest.anchorMax = source.anchorMax;
						dest.offsetMin = source.offsetMin;
						dest.offsetMax = source.offsetMax;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					/*  if ( groupRoot.GetComponentsInParent<Canvas>( true ).Length != 0 )     // canvas
					  {   var rect = groupRoot.AddComponent<RectTransform>();
						  rect.anchorMin = Vector2.zero;
						  rect.anchorMax = Vector2.one;
						  rect.offsetMin = Vector2.zero;
						  rect.offsetMax = Vector2.zero;
						  groupRoot.AddComponent<CanvasRenderer>();
					  }*/

					Undo.RegisterCreatedObjectUndo(groupRoot, groupRoot.name);

					foreach (var gameObject in onlytop)
					{
						Undo.SetTransformParent(gameObject.transform, groupRoot.transform, groupRoot.name);
					}

					EMX.Utility.SetExpanded(groupRoot.GetInstanceID(), true);

					Selection.objects = onlytop.ToArray();
				});

				//Selection.objects = new[] { groubObject };
			}

		}
		class MyMenu_GroupWorld : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{
			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_GroupWorld(); }

			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 101; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (World Center) %#g"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if (onlytop.Length == 0) return;

				var last_object = onlytop[onlytop.Length - 1];

				var groupParent = onlytop[0].transform.parent;
				var groupSiblingIndex = onlytop[0].transform.GetSiblingIndex();

				var reference = last_object;
				var NEW_NAME = reference.name + " Group";

				// Save Previous Name Variant
				// CustomModule.SHOW_StringInput( "Group Name:", EditorPrefs.GetString( "EMX/MultyRenamer/GroupName", NEW_NAME ) , ( name ) =>
				// {   EditorPrefs.SetString( "EMX/MultyRenamer/GroupName", name );

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput("Group Name:", NEW_NAME, (name) =>
				{

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent(groupParent, false);
					//groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex(groupSiblingIndex);
					//********************************//
					//********************************//

					//  MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if (reference.GetComponent<RectTransform>())     // canvas
					{
						var source = reference.GetComponent<RectTransform>();
						var dest = groupRoot.AddComponent<RectTransform>();
						dest.sizeDelta = source.sizeDelta;
						dest.pivot = source.pivot;
						dest.anchoredPosition3D = source.anchoredPosition3D;
						dest.localRotation = source.localRotation;
						dest.localScale = source.localScale;
						dest.anchorMin = source.anchorMin;
						dest.anchorMax = source.anchorMax;
						dest.offsetMin = source.offsetMin;
						dest.offsetMax = source.offsetMax;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					Undo.RegisterCreatedObjectUndo(groupRoot, groupRoot.name);

					foreach (var gameObject in onlytop)
					{
						Undo.SetTransformParent(gameObject.transform, groupRoot.transform, groupRoot.name);
					}

					EMX.Utility.SetExpanded(groupRoot.GetInstanceID(), true);

					Selection.objects = onlytop.ToArray();
				});
				//Selection.objects = new[] { groubObject };
			}

		}
		class MyMenu_GroupAverage : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_GroupAverage(); }


			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 102; } }
			public override string Name { get { return ItemsPlacementFolder + "Group (Average Center)"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderBy(go => go.transform.GetSiblingIndex()).ToArray();

				if (onlytop.Length == 0) return;

				var last_object = onlytop[onlytop.Length - 1];

				var groupParent = onlytop[0].transform.parent;
				var groupSiblingIndex = onlytop[0].transform.GetSiblingIndex();


				var NEW_NAME = last_object.name + " Group";


				// Save Previous Name Variant
				// CustomModule.SHOW_StringInput( "Group Name:", EditorPrefs.GetString( "EMX/MultyRenamer/GroupName", NEW_NAME ) , ( name ) =>
				// {   EditorPrefs.SetString( "EMX/MultyRenamer/GroupName", name );

				// Top Object Name Variant
				EMX.Utility.SHOW_StringInput("Group Name:", NEW_NAME, (name) =>
				{

					var groupRoot = new GameObject(name);
					groupRoot.transform.SetParent(groupParent, false);
					groupRoot.transform.localScale = Vector3.one;
					groupRoot.transform.SetSiblingIndex(groupSiblingIndex);
					//********************************//
					Vector3 center = Vector3.zero;
					Vector3 rot = Vector3.zero;

					foreach (var item in onlytop)
					{
						center += item.transform.position;
						rot += item.transform.eulerAngles;
					}

					center /= onlytop.Length;
					rot /= onlytop.Length;
					groupRoot.transform.position = center;
					groupRoot.transform.eulerAngles = rot;
					//********************************//

					//MyMenu_Utils.AssignUniqueName( groupRoot ); // name
					if (groupRoot.GetComponentsInParent<Canvas>(true).Length != 0)     // canvas
					{
						var rect = groupRoot.AddComponent<RectTransform>();
						rect.anchorMin = Vector2.zero;
						rect.anchorMax = Vector2.one;
						rect.offsetMin = Vector2.zero;
						rect.offsetMax = Vector2.zero;
						groupRoot.AddComponent<CanvasRenderer>();
					}

					Undo.RegisterCreatedObjectUndo(groupRoot, groupRoot.name);

					foreach (var gameObject in onlytop)
					{
						Undo.SetTransformParent(gameObject.transform, groupRoot.transform, groupRoot.name);
					}

					EMX.Utility.SetExpanded(groupRoot.GetInstanceID(), true);

					Selection.objects = onlytop.ToArray();
				});
				//Selection.objects = new[] { groubObject };
			}

		}

		class MyMenu_UnGroup : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{


			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_UnGroup(); }

			public override bool IsEnable(GameObject clickedObject) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 103; } }
			public override string Name { get { return ItemsPlacementFolder + "UnGroup"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var ungroupedObjects = new List<GameObject>();
				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray);

				foreach (var ungroupedRoot in onlytop)
				{
					var ungroupSiblinkIndex = ungroupedRoot.transform.GetSiblingIndex();
					var ungroupParent = ungroupedRoot.transform.parent;
					var undoName = ungroupedRoot.name;

					for (int i = ungroupedRoot.transform.childCount - 1; i >= 0; i--)
					{
						var o = ungroupedRoot.transform.GetChild(i);
						Undo.SetTransformParent(o.transform, ungroupParent, "Remove " + undoName);

						if (!Application.isPlaying) Undo.RegisterFullObjectHierarchyUndo(o, "Remove " + undoName);

						o.SetSiblingIndex(ungroupSiblinkIndex);

						if (!Application.isPlaying) EditorUtility.SetDirty(o);

						ungroupedObjects.Add(o.gameObject);
					}

					if (!Application.isPlaying) Undo.DestroyObjectImmediate(ungroupedRoot); else Destroy(ungroupedRoot);
				}

				Selection.objects = ungroupedObjects.ToArray();
			}

		}

		#endregion





		#region ITEM 200-203 - Sibling

		class MyMenu_Sibling0 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Sibling0(); }

			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 200; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set Previous Sibling Index %'"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderBy(o => o.GetSiblingIndex()).ToArray();
				List<Transform> moveBack = new List<Transform>();

				foreach (var item in obs.Select(o => new { sib = o.GetSiblingIndex(), transform = o }).ToArray())
				{
					var sib = item.sib - 1;
					Undo.SetTransformParent(item.transform, item.transform.parent, "Set Previous Sibling Index");

					if (sib < 0) moveBack.Add(item.transform);

					item.transform.SetSiblingIndex(sib);
				}

				foreach (var transform in moveBack)
				{
					transform.SetAsFirstSibling();
				}
			}
		}
		class MyMenu_Sibling1 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Sibling1(); }


			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 201; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set Next Sibling Index %/"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderByDescending(o => o.GetSiblingIndex()).ToArray();
				List<Transform> moveBack = new List<Transform>();

				foreach (var item in obs.Select(o => new { sib = o.GetSiblingIndex(), transform = o }).ToArray())
				{
					var sib = item.sib + 1;
					Undo.SetTransformParent(item.transform, item.transform.parent, "Set Next Sibling Index");
					var nned = sib;
					item.transform.SetSiblingIndex(sib);

					if (nned != item.transform.GetSiblingIndex()) moveBack.Add(item.transform);
				}

				foreach (var transform in moveBack)
				{
					transform.SetAsLastSibling();
				}
			}
		}
		class MyMenu_Sibling2 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Sibling2(); }


			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 202; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set As First Sibling %["; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderByDescending(o => o.GetSiblingIndex()).ToArray();

				foreach (var item in obs)
				{
					Undo.SetTransformParent(item, item.parent, "Set As First Sibling");
					item.SetAsFirstSibling();
				}
			}
		}
		class MyMenu_Sibling3 : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Sibling3(); }

			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 203; } }
			public override string Name { get { return ItemsPlacementFolder + "Sibling/Set As Last Sibling %]"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderBy(o => o.GetSiblingIndex()).ToArray();

				foreach (var item in obs)
				{
					Undo.SetTransformParent(item, item.parent, "Set As Last Sibling");
					item.SetAsLastSibling();
				}
			}
		}



		class MyMenu_ParentClear : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_ParentClear(); }

			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 204; } }
			public override string Name { get { return ItemsPlacementFolder + "Move To Parent &%["; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderBy(o => o.GetSiblingIndex()).ToArray();

				foreach (var item in obs)
				{
					if (!item.parent)
					{
						Undo.SetTransformParent(item, item.parent, "Move To Parent");
						item.SetAsLastSibling();
					}

					else
					{
						Undo.SetTransformParent(item, item.parent.parent, "Move To Parent");
						item.SetAsLastSibling();
					}
				}
			}
		}



		class MyMenu_Parenter_SetParent : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{


			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Parenter_SetParent(); }


			internal static GameObject _CurrentParent;
			internal static GameObject CurrentParent
			{
				get
				{
					if (!_CurrentParent)
					{
						var previousID = EditorPrefs.GetInt("EMX/Set As Target for Moving", -1);
						_CurrentParent = EditorUtility.InstanceIDToObject(previousID) as GameObject;
					}

					return _CurrentParent;
				}

				set
				{
					if (_CurrentParent != value)
					{
						_CurrentParent = value;
						EditorPrefs.SetInt("EMX/Set As Target for Moving", value ? value.GetInstanceID() : -1);
					}
				}
			}
			public override bool IsEnable(GameObject clickedObject) { return MyMenu_Parenter_SetParent.CurrentParent != clickedObject; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 210; } }
			public override string Name { get { return ItemsPlacementFolder + "Set As Target for Moving %&P"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				CurrentParent = affectedObjectsArray[0];
			}
		}
		class MyMenu_Parenter_MoveToSettedParent : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{


			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Parenter_MoveToSettedParent(); }


			string text { get { return "Move To {" + (MyMenu_Parenter_SetParent.CurrentParent ? MyMenu_Parenter_SetParent.CurrentParent.name : "Not Assigned") + "}"; } }
			public override bool IsEnable(GameObject clickedObject) { return MyMenu_Parenter_SetParent.CurrentParent && MyMenu_Parenter_SetParent.CurrentParent != clickedObject; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 211; } }
			public override string Name { get { return ItemsPlacementFolder + text + " %&R"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				if (!MyMenu_Parenter_SetParent.CurrentParent) return;

				var obs = affectedObjectsArray.Select(g => g.transform).ToArray();

				if (obs.Length == 0) return;

				obs = obs.OrderBy(o => o.GetSiblingIndex()).ToArray();

				foreach (var item in obs)
				{
					if (MyMenu_Parenter_SetParent.CurrentParent.transform == item.transform) continue;

					Undo.SetTransformParent(item, MyMenu_Parenter_SetParent.CurrentParent.transform, text);
					item.SetAsLastSibling();
				}
			}
		}
		#endregion





		#region ITEM 500 - DuplicateNextToObject

		class MyMenu_DuplicateNextToObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_DuplicateNextToObject(); }


			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 500; } }
			public override string Name { get { return ItemsPlacementFolder + "Duplicate Next To Object %#d"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{

				var onlytop = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).OrderByDescending(o => o.transform.GetSiblingIndex());

				List<GameObject> clonedObjects = new List<GameObject>();

				foreach (var gameObject in onlytop)
				{
					var oldSib = gameObject.transform.GetSiblingIndex();
					Selection.objects = new[] { gameObject };
					EMX.Utility.DuplicateSelection();
					var clonedObject = Selection.activeGameObject;
					MyMenu_Utils.AssignUniqueName(clonedObject);
					clonedObject.transform.SetSiblingIndex(oldSib + 1);
					clonedObjects.Add(clonedObject);
				}

				Selection.objects = clonedObjects.ToArray();

			}
		}

		#endregion




		#region ITEM 750 - Rename
		class MyMenu_Rename : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_Rename(); }


			public override bool IsEnable(GameObject clickedObject) { return true; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }
			public override int PositionInMenu { get { return 750; } }
			public override string Name { get { return ItemsPlacementFolder + "Multy Renamer"; } }
			public override void OnClick(GameObject[] affectedObjectsArray)
			{

				EMX.Utility.SHOW_StringInput("Find:", EditorPrefs.GetString("EMX/MultyRenamer/Find", ""), (find) =>
				{
					if (string.IsNullOrEmpty(find)) return;

					EditorPrefs.SetString("EMX/MultyRenamer/Find", find);
					EMX.Utility.SHOW_StringInput("Replace:", EditorPrefs.GetString("EMX/MultyRenamer/Replace", ""), (replace) =>
					{
						EditorPrefs.SetString("EMX/MultyRenamer/Replace", replace);

						foreach (var item in affectedObjectsArray)
						{
							if (!item) continue;

							Undo.RecordObject(item, "Multy Renamer");
							item.name = item.name.Replace(find, replace);
							if (!Application.isPlaying) EditorUtility.SetDirty(item);
						}
					});
				});
			}
		}
		#endregion






		#region ITEM 1000-1001 - ExpandSelecdedObject/CollapseSelecdedObject

		class MyMenu_ExpandSelecdedObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_ExpandSelecdedObject(); }


			public override bool IsEnable(GameObject clickedObject) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 1000; } }
			public override string Name { get { return ItemsPlacementFolder + "Expand Selection"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				foreach (var result in affectedObjectsArray.Select(o => o.GetInstanceID()))
					EMX.Utility.SetExpandedWithChildren(result, true);
			}

		}


		class MyMenu_CollapseSelecdedObject : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_CollapseSelecdedObject(); }


			public override bool IsEnable(GameObject clickedObject) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 1001; } }
			public override string Name { get { return ItemsPlacementFolder + "Collapse Selection"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				foreach (var result in MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray).Select(o => o.GetInstanceID()))
					EMX.Utility.SetExpandedWithChildren(result, false);
			}

		}

		#endregion


		class MyMenu_RemoveMissingComponents : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_RemoveMissingComponents(); }


			bool HasMissingScript(GameObject go) { return go.GetComponents<Component>().Any(c => !c); }
			public override bool IsEnable(GameObject clickedObject) { return HasMissingScript(clickedObject); }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 1010; } }
			public override string Name { get { return ItemsPlacementFolder + "Remove Missing Scripts"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				foreach (var result in affectedObjectsArray)
				{
					if (!HasMissingScript(result)) continue;

					//Undo.RecordObject( result, "Remove Missing Scripts" );
					Undo.RegisterFullObjectHierarchyUndo(result, "Remove Missing Scripts");
#if UNITY_2019_1_OR_NEWER
					GameObjectUtility.RemoveMonoBehavioursWithMissingScript(result);
#else
			var components = result.GetComponents<Component>();
			var serializedObject = new SerializedObject(result);
			var prop = serializedObject.FindProperty("m_Component");
			int r = 0;
			
			for ( int j = 0 ; j < components.Length ; j++ )
			{	if ( components[j] == null )
				{	prop.DeleteArrayElementAtIndex( j - r );
					r++;
				}
			}
			
			serializedObject.ApplyModifiedProperties();
#endif
					if (!Application.isPlaying) EditorUtility.SetDirty(result);
				}

				foreach (var item in affectedObjectsArray.Select(g => g.scene).Distinct())
				{
					if (!item.IsValid()) continue;

					UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(item);
				}
			}

		}

		#region ITEM 2000-2001 - ReverseChildrenOrder/SelectOnlyTopObjects/SelectAllChildren

		class MyMenu_ReverseChildrenOrder : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_ReverseChildrenOrder(); }


			public override bool IsEnable(GameObject clickedObject) { return clickedObject.transform.childCount > 0; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 2000; } }
			public override string Name { get { return ItemsPlacementFolder + "Reverse Children Order"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				foreach (var gameObject in MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray))
				{
					var T = gameObject.transform;

					for (int i = 0; i < gameObject.transform.childCount; i++)
					{
						Undo.SetTransformParent(T.GetChild(i), T.GetChild(i).transform.parent, "Reverse Children Order");
						T.GetChild(i).SetAsFirstSibling();
					}
				}
			}

		}

		class MyMenu_SelectOnlyTopObjects : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_SelectOnlyTopObjects(); }


			public override bool IsEnable(GameObject clickedObject) { return Selection.gameObjects.Length >= 2; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 2001; } }
			public override string Name { get { return ItemsPlacementFolder + "Select Only Top Objects"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				Selection.objects = MyMenu_Utils.GetOnlyTopObjects(affectedObjectsArray);
			}

		}


		class MyMenu_SelectAllChildren : EMX.CustomizationHierarchy.ExtensionInterface_RightClickOnGameObjectMenuItem
		{

			[InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
			public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyMenu_SelectAllChildren(); }


			public override bool IsEnable(GameObject clickedObject) { return clickedObject.transform.childCount != 0; }
			public override bool NeedExcludeFromMenu(GameObject clickedObject) { return false; }

			public override int PositionInMenu { get { return 2002; } }
			public override string Name { get { return ItemsPlacementFolder + "Select All Children"; } }


			public override void OnClick(GameObject[] affectedObjectsArray)
			{
				Selection.objects = affectedObjectsArray.SelectMany(s => s.GetComponentsInChildren<Transform>(true)).Select(s => s.gameObject).ToArray();
			}

		}

		#endregion






		#region - Utils

		static class MyMenu_Utils
		{
			public static void AssignUniqueName(GameObject o)
			{

				var usedNames = new SortedDictionary<string, string>();
				var childList = o.transform.parent
								? new Transform[o.transform.parent.childCount].Select((t, i) => o.transform.parent.GetChild(i))
								: o.scene.GetRootGameObjects().Select(go => go.transform);

				foreach (var child in childList.Where(child => child != o.transform))
				{
					if (!usedNames.ContainsKey(child.name)) usedNames.Add(child.name, child.name);
				}// existing names

				if (!usedNames.ContainsKey(o.name)) return;



				var number = 1;
				var name = o.name;

				var leftBracket = name.IndexOf('(');
				var rightBracket = name.IndexOf(')');

				if (leftBracket != -1 && rightBracket != -1 && rightBracket - leftBracket > 1)
				{
					int parseResult;

					if (int.TryParse(name.Substring(leftBracket + 1, rightBracket - leftBracket - 1), out parseResult))
					{
						number = parseResult + 1;
						name = name.Remove(leftBracket);
					}
				}// previous value



				name = name.TrimEnd();

				while (usedNames.ContainsKey(name + " (" + number + ")")) ++number;

				o.name = name + " (" + number + ")"; //result

			}

			public static GameObject[] GetOnlyTopObjects(GameObject[] affectedObjectsArray)
			{
				var converted = affectedObjectsArray.Select(a => new { a, par = a.GetComponentsInParent<Transform>(true).Where(p => p != a.transform) });
				return
					converted.Where(c => c.par.Count(p => affectedObjectsArray.Contains(p.gameObject)) == 0).
					Select(g => g.a).ToArray();
			}
		}

		#endregion


	}
}//namespace

#endif
