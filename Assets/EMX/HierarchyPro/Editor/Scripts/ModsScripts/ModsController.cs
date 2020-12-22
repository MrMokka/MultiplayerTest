using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using EMX.HierarchyPlugin.Editor.Mods;

namespace EMX.HierarchyPlugin.Editor
{


	interface IExternalWindowMod
	{
		bool Alive();
		bool RepaintNow();

		Func<bool, float, bool> currentAction { get; set; }
	}

	class EditorSubscriber
	{
		[Obsolete]
		internal int pid = 0;


		internal Action OnSceneOpening = null;
		internal Action OnUndoAction = null;
		internal Action OnSelectionChanged = null;
		internal Action OnHierarchyChanged = null;
		internal Action OnPlayModeStateChanged = null;
		internal Action OnUpdate = null;
		internal Action<bool> OnGlobalKeyPressed = null;
		internal Action<string, Rect> OnProjectWindow = null;
		internal Action<SceneView> duringSceneGui = null;
		internal Action OnModifyKeyChanged = null;
		internal List<Func<bool>> OnEditorWantsToQuit = new List<Func<bool>>();
		internal Action OnAssetImport = null;
		internal Action OnResetDrawStack = null;

		internal void copy_to(EditorSubscriber sbs)
		{
			if (OnSceneOpening != null) sbs.OnSceneOpening += OnSceneOpening;
			if (OnUndoAction != null) sbs.OnUndoAction += OnUndoAction;
			if (OnSelectionChanged != null) sbs.OnSelectionChanged += OnSelectionChanged;
			if (OnHierarchyChanged != null) sbs.OnHierarchyChanged += OnHierarchyChanged;
			if (OnPlayModeStateChanged != null) sbs.OnPlayModeStateChanged += OnPlayModeStateChanged;
			if (OnUpdate != null) sbs.OnUpdate += OnUpdate;
			if (OnGlobalKeyPressed != null) sbs.OnGlobalKeyPressed += OnGlobalKeyPressed;
			if (OnProjectWindow != null) sbs.OnProjectWindow += OnProjectWindow;
			if (OnModifyKeyChanged != null) sbs.OnModifyKeyChanged += OnModifyKeyChanged;
			//if (OnEditorWantsToQuit != null) sbs.OnEditorWantsToQuit += OnEditorWantsToQuit;
			if (OnAssetImport != null) sbs.OnAssetImport += OnAssetImport;
			if (OnResetDrawStack != null) sbs.OnResetDrawStack += OnResetDrawStack;
		}



		internal Action BuildedOnGUI_first = null;
		internal Action BuildedOnGUI_middle = null;
		internal Action BuildedOnGUI_middle_plusLayout = null;
		internal Action BuildedOnGUI_last = null;
		internal Action BuildedOnGUI_last_butBeforeGL = null;

		internal List<IModSaver> saveModsInterator = new List<IModSaver>();

		internal List<ExternalMod_Button> ExternalMod_Buttons = new List<ExternalMod_Button>();
		internal List<ExternalMod_MenuItem> ExternalMod_MenuItems = new List<ExternalMod_MenuItem>();
	}
	class ExternalMod_Button
	{
		internal ExternalMod_Button(Type wint)
		{
			if (!wint.IsSubclassOf(typeof(ExternalModRoot))) throw new Exception(wint.Name);
			this.windowType = wint;
		}

		internal Func<string> icon = null;
		internal string text = "Graph";
		internal Action<int, string> release = null;
		internal int priority = 0;
		internal Type windowType = null;
	}
	class ExternalMod_MenuItem
	{
		internal string text = "Graph";
		internal int priority = 0;
		internal string path = "A/B";
		internal Action<int, string> release = null;
	}

	internal partial class ModsController
	{

		PluginInstance p;
		internal ModsController(PluginInstance p)
		{
			this.p = p;
			toolBarModification = new Mods.ToolBarModification(p);

			// funEditorFontsModification = new FunEditorFontsModification();
			backgroundDecorations = new Mods.BackgroundDecorations(0);

			setActiveMod = new Mods.SetActiveMod(p.pluginID);
			rightModsManager = new Mods.RightModsManager(p);
			componentsIconsMod = new Mods.ComponentsIcons_Mod(p.pluginID);

			externalMods = new List<Mods.ExternalModRoot>();

			/*externalMods.Add(ex_BookmarksforGameObjectsMod = new Mods.BookmarksforGameObjectsMod(p));
			externalMods.Add(ex_BookmarksforProjectviewMod = new Mods.BookmarksforProjectviewMod(p));
			externalMods.Add(ex_HyperGraphMod = new Mods.HyperGraph.HyperGraphMod(p));
			externalMods.Add(ex_LastScenesHistoryMod = new Mods.LastScenesHistoryMod(p));
			externalMods.Add(ex_LastSelectionHistoryMod = new Mods.LastSelectionHistoryMod(p));*/
		}

		internal Mods.ToolBarModification toolBarModification;

		//EDITOR
		// FunEditorFontsModification funEditorFontsModification;
		internal Mods.BackgroundDecorations backgroundDecorations;

		//MODS
		internal Mods.SetActiveMod setActiveMod;
		internal Mods.RightModsManager rightModsManager;
		internal Mods.ComponentsIcons_Mod componentsIconsMod;



		internal List<Mods.ExternalModRoot> externalMods = new List<Mods.ExternalModRoot>();

		/*	internal Mods.BookmarksforGameObjectsMod ex_BookmarksforGameObjectsMod;
			internal Mods.BookmarksforProjectviewMod ex_BookmarksforProjectviewMod;
			internal Mods.HyperGraph.HyperGraphMod ex_HyperGraphMod;
			internal Mods.LastScenesHistoryMod ex_LastScenesHistoryMod;
			internal Mods.LastSelectionHistoryMod ex_LastSelectionHistoryMod;*/

		//MODS

		internal void INVOKE_FOR_EXTERNAL<T>(Action<T> ac) where T : Mods.ExternalModRoot
		{
			foreach (var item in externalMods.ToList())
			{
				if (item is T)
				{
					var r = item as T;
					if (!r) continue;
					ac(r);
				}
			}
		}





		internal void REBUILD_PLUGINS(bool skipRepaint = false)
		{



			if (p.par_e.USE_WHOLE_FUN_UNITY_FONT_SIZE)
			{
				if (Event.current == null) p.PUSH_GUI_ONESHOT(0,() =>
			   {
				   FunEditorFontsModification.Modificate(p.par_e.WHOLE_FUN_UNITY_FONT_SIZE);
			   });
				else FunEditorFontsModification.Modificate(p.par_e.WHOLE_FUN_UNITY_FONT_SIZE);
			}

			var sbs = new EditorSubscriber();


			if (p.par_e.USE_COMPONENTS_ICONS_MOD) componentsIconsMod.Subscribe(sbs); //setactive mod //INVOKE BEFORE RIGHT MODS

			if (p.par_e.USE_RIGHT_ALL_MODS) rightModsManager.Subscribe(sbs); //other right mods + custom
			if (p.par_e.USE_SETACTIVE_MOD) setActiveMod.Subscribe(sbs); //setactive mod
			if (p.par_e.USE_RIGHT_ALL_MODS) rightModsManager.SubscribePreCalc(sbs); //other right mods + custom

			if (p.par_e.HIER_WIN_SET.USE_BACKGROUNDDECORATIONS_MOD) backgroundDecorations.Subscribe(sbs);// backround colors mod //INVOKE FIRSTGUI ONLY AFTER RIGHT MODS CALCULATION
																										 //COLOR BACKGROUND HERE


			if (p.par_e.USE_PROJECT_GUI_EXTENSIONS) (new Mods.ProjectGuiExtension(p)).Subscribe(sbs); //PROJECT


			if (p.par_e.USE_AUTOSAVE_MOD) Mods.AutoSaveMod.Subscribe(sbs);
			if (p.par_e.USE_SNAP_MOD) Mods.SnapMod.Subscribe(sbs);






			//EXTERNAL MODS

			// hierarchy header hot buttons
			//Mods.HyperGraph.HyperGraphMod.SubscribeButtonsAndMenu(sbs);

			foreach (var exMod in externalMods.ToList()) exMod.SubscribeEditorInstance(sbs);

			if (p.par_e.USE_TOPBAR_MOD) toolBarModification.Install(sbs, false);
			else toolBarModification.Remove(sbs);
			//toolBarModification.hotButtons.Subscribe(sbs);
			if (p.par_e.USE_RIGHT_CLICK_MENU_MOD)
			{
				if (!menuAdded)
				{
					menuAdded = true;
					try
					{
						EventInfo e = (typeof(EditorSceneManager).Assembly.GetType("UnityEditor.SceneManagement.SceneHierarchyHooks") ?? throw new Exception("Cannot find UnityEditor.SceneManagement.SceneHierarchyHooks")).GetEvent("addItemsToGameObjectContextMenu", ~(BindingFlags.Instance | BindingFlags.InvokeMethod)) ?? throw new Exception("Cannot find addItemsToGameObjectContextMenu");
						MethodInfo mi = typeof(RightClickOnGameObjectMenuRegistrator).GetMethod("ContextMenu", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new Exception("Cannot find ContextMenu");
						var d = Delegate.CreateDelegate(e.EventHandlerType, null, mi);
						object[] args = { d };
						MethodInfo addHandler = e.GetAddMethod();
						addHandler.Invoke(null, args);
					}
					catch (Exception ex)
					{
						Debug.LogWarning("Cannot create " + Root.PN + " menu items\n" + ex.Message + "\n\n" + ex.StackTrace);
					}
				}
				RightClickOnGameObjectMenuRegistrator.SubscribeEvents(sbs);
			}



			p.REBUILD_EDITOR_EVENTS(sbs);

			if (!skipRepaint) p.RepaintWindowInUpdate_PlusResetStack(0);
			if (!skipRepaint) p.RepaintWindowInUpdate_PlusResetStack(1);
		}

		bool menuAdded = false;
		internal void RESET_DRAW_STACKS(int pluginID)
		{

			if (pluginID == 0)
			{
				setActiveMod.ResetStack();
				componentsIconsMod.ResetStack();
				rightModsManager.RESET_DRAW_STACKS();
			}
			else
			{
			}
			

			// foreach ( var m in internalMods ) m.Value.ResetDrawStack();
			//  foreach ( var m in externalMods ) m.Value.ResetDrawStack();
			/*for ( int i = 0 ; i < modules.Length ; i++ )
            {
                foreach ( var stack in modules[ i ].DRAW_STACK )
                {
                    stack.Value.ResetStack();
                }
            }*/
		}




		internal List<IModSaver> saveModsInterator = new List<IModSaver>();








	}

	interface IModSaver
	{
		bool SaveToString(HierarchyObject o, ref string result);
		bool LoadFromString(string s, HierarchyObject o);
		List<SaverType> GetSaverTypes { get; }

	}

}
