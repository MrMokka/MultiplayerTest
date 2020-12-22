using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using System.Reflection;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{

	internal partial class PluginInstance
	{



		internal void invoke_unUndo()
		{
			if (_OnUndoAction != null) _OnUndoAction();
		}

		/*   internal void EditorSceneManagerOnSceneUnloaded( Scene arg0, LoadSceneMode loadSceneMode )
           {
               if ( SubcripeSceneLoader_method != null ) SubcripeSceneLoader_method();
               //ltg.Clear(); NEW_CACHE
               CloseWindows();
           }*/



		internal void invoke_DuringSceneGUI(SceneView sv)
		{
			if (_DuringSceneGui != null) _DuringSceneGui(sv);
		}
		internal void invoke_ModifiyKeyChanged()
		{
			if (_OnModifyKeyChanged != null) _OnModifyKeyChanged();

		}

		static bool compileFlag;
		internal static int[] LastActiveScenesList_HashCode = new int[0];
		internal static string[] LastActiveScenesList_Guids = new string[0];
		/*private int lastSceneID = -1;
		private string lastScenePath = null;
		private string lastSceneGUID = null;*/
		//private string[] lastSceneGUID_ALL = new string[0];
		/*	internal void invoke_EditorSceneManagerOnSceneOpening3()
			{
				__scene();
			}
			internal void invoke_EditorSceneManagerOnSceneOpening2(Scene s, Scene s2)
			{
				__scene();
			}*/

		internal void invoke_EditorSceneManagerOnSceneOpening(Scene scene, OpenSceneMode mode)
		{
			invoke_SceneChanging();
		}
		internal void invoke_SceneChanging()
		{
			if (_OnSceneOpening != null) _OnSceneOpening();

			need_ClearHierarchyObjects1 = true;

			LastActiveScene = SceneManager.GetActiveScene();
			//Debug.Log("asd");
			/*lastScenePath = null;
			lastSceneID = -1;
			lastSceneGUID = null;*/
			//ltg.Clear(); NEW_CACHE
			windowsManager.CloseWindows();
			gl.ClearStacks();
		}
		internal bool wasSceneMoved = false;
		internal void invoke_SceneMoved()
		{
			if (!wasSceneMoved) return;
			wasSceneMoved = false;
			for (int i = 0; i < EditorSceneManager.sceneCount; i++)
			{
				var s = EditorSceneManager.GetSceneAt(i);
				if (!s.IsValid() || !s.isLoaded) continue;
				HierarchyTempSceneData.SaveOnScenePathChanged(s);
			}
			try_to_detect_scene_changing();
			if (_OnSceneOpening != null) _OnSceneOpening();
		}


		internal Scene _LastActiveScene;
		internal Scene LastActiveScene
		{
			get
			{
				if (!_LastActiveScene.IsValid()) _LastActiveScene = SceneManager.GetActiveScene();
				return _LastActiveScene;
			}
			set
			{
				if (_LastActiveScene != value)
				{
					_LastActiveScene = value;
					modsController.INVOKE_FOR_EXTERNAL<ExternalModRoot>(r => r.RepaintNow());

					/*	lastSceneID = -1;
						lastScenePath = null;
						lastSceneGUID = null;*/
				}
			}
		}



		internal void invoke_onSelectionChanged()
		{
			ha._IsSelectedCache.Clear();
			ha._IsDraggedCache.Clear();
			ha.OnSelectionChanged_SaveCache();
			if (_OnSelectionChanged != null) _OnSelectionChanged();
		}
		internal void invoke_onHierarchyChanged()
		{
			invoke_onHierarchyChanged(false);
		}
		internal void invoke_onHierarchyChanged(bool skipClear)
		{
			if (_OnHierarchyChanged != null) _OnHierarchyChanged();
			//  Cache.ClearAdditionalCache();
			if (!skipClear) Cache.ClearHierarchyObjects(false);
		}
		internal void invoke_onPlayModeStateChanged(PlayModeStateChange state)
		{
			if (_OnPlayModeStateChanged != null) _OnPlayModeStateChanged();
		}

		internal void invoke_OnProjectWindow(string guid, Rect selectionrect)
		{
			if (_OnProjectWindow != null) _OnProjectWindow(guid, selectionrect);
		}

		internal bool invoke_OnEditorWantsToQuit()
		{
			var res = true;
			foreach (var item in _OnEditorWantsToQuit)
				res &= item();
			return res;
		}

		Action<bool> _OnGlobalKeyPressed = null;
		Action _OnSceneOpening = null;
		Action _OnUndoAction = null;
		Action _OnSelectionChanged = null;
		Action _OnHierarchyChanged = null;
		Action _OnPlayModeStateChanged = null;
		Action _OnUpdate = null;
		Action<string, Rect> _OnProjectWindow = null;
		Action<SceneView> _DuringSceneGui = null;
		Action _OnModifyKeyChanged = null;
		List<Func<bool>> _OnEditorWantsToQuit = new List<Func<bool>>();
		Action _OnAssetImport = null;
		Action _OnResetDrawStack = null;


		internal class gui_actions
		{
			internal Action BuildedOnGUI_first;
			internal Action BuildedOnGUI_middle;
			internal Action BuildedOnGUI_middle_plusLayout;
			internal Action BuildedOnGUI_last;
			internal Action BuildedOnGUI_last_butBeforeGL;
		}
		internal gui_actions[] g = { new gui_actions(), new gui_actions() };



		internal void REBUILD_EDITOR_EVENTS(EditorSubscriber sbs)
		{
			modsController.toolBarModification.hotButtons.RegistrateButton(sbs);
			if (par_e.USE_RIGHT_CLICK_MENU_MOD)
				if (par_e.DRAW_RIGHTCLIK_MENU_HOTBUTTONS) RightClickOnGameObjectMenuRegistrator.RegistrateMenuItem(sbs.ExternalMod_MenuItems);


			g[0].BuildedOnGUI_first = sbs.BuildedOnGUI_first;
			g[0].BuildedOnGUI_middle = sbs.BuildedOnGUI_middle;
			g[0].BuildedOnGUI_middle_plusLayout = sbs.BuildedOnGUI_middle_plusLayout;
			g[0].BuildedOnGUI_last = sbs.BuildedOnGUI_last;
			g[0].BuildedOnGUI_last_butBeforeGL = sbs.BuildedOnGUI_last_butBeforeGL;


			_OnSceneOpening = sbs.OnSceneOpening;
			_OnUndoAction = sbs.OnUndoAction;
			_OnUndoAction += new UNDO().UNDO_AC;
			_OnSelectionChanged = sbs.OnSelectionChanged;
			_OnHierarchyChanged = sbs.OnHierarchyChanged;
			_OnPlayModeStateChanged = sbs.OnPlayModeStateChanged;
			_OnUpdate = sbs.OnUpdate;
			_OnGlobalKeyPressed = sbs.OnGlobalKeyPressed;
			_DuringSceneGui = sbs.duringSceneGui;
			_OnModifyKeyChanged = sbs.OnModifyKeyChanged;
			_OnProjectWindow = sbs.OnProjectWindow;
			_OnEditorWantsToQuit = sbs.OnEditorWantsToQuit;
			_OnAssetImport = sbs.OnAssetImport;
			_OnResetDrawStack = sbs.OnResetDrawStack;

			modsController.saveModsInterator = sbs.saveModsInterator;

		}






		internal void invoke_ON_ASSET_IMPORT()
		{

			if (_OnAssetImport != null) _OnAssetImport();
			if (pluginID == 1)
			{
				//need_ClearHierarchyObjects1 = true;
				//ClearTree( true );
			}
			TODO_Tools.ALL_ASSETS_PATHS = null;
		}






		internal void invoke_ReloadAfterAssetDeletingOrPasting() //old = Again_Reloder_UsingWhenCopyPastOrAssets
		{

			invoke_EditorSceneManagerOnSceneOpening(EditorSceneManager.GetActiveScene(), OpenSceneMode.Single);
			// if ( Hierarchy.HierarchyAdapterInstance.onDidReloadScript != null ) Hierarchy.HierarchyAdapterInstance.onDidReloadScript();
			// if ( onUndoAction != null ) onUndoAction();

		}

	}


}
