using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{

	[InitializeOnLoad]
	public class Root
	{

		public const string PN = "Hierarchy Free";
		public const string PN_NS = "HierarchyPlugin";
		public const string CUST_NS = "CustomizationHierarchy";
		public const string PN_FOLDER = Folders.PN_FOLDER;
		static PluginInstance[] __p = null;
		internal static PluginInstance[] p
		{
			get
			{
				if (!created || __p == null) CREATE();
				return __p;
			}
		}
		static bool created = false;


		static Icons _icons = null;
		static internal Icons icons { get { return _icons ?? (_icons = AssetDatabase.LoadAssetAtPath<Icons>(Folders.PluginInternalFolder + "/Editor/Icons/IconsArray.asset") ?? throw new Exception("Cannot load icons at path: " + Folders.PluginInternalFolder + "/Editor/Icons/IconsArray.asset")); } }
		static bool wasInit = false;
		static Root()
		{
			CREATE();
		}
		static void CREATE()
		{
			if (created) return;
			created = true;
			Folders.CheckFolders();
			Settings.MainSettingsEnabler_Window.CheckSettings();
			__p = new PluginInstance[2];
			__p[0] = PluginInstance.CreateInstance("Hierarchy");
			__p[0].par_e = new EditorSettingsAdapter(0);
			__p[0].gl = new GlDrawer(__p[0]);
			if (!__p[0].par_e.ENABLE_ALL)
			{
				return;
			}
			Init();
			//p[0].par_e
		}
		static void Init()
		{
			if (wasInit) return;
			wasInit = true;
			InitializeRoot();

			var hierarchyPlugin = PluginInstance.CreateInstance("Hierarchy");
			p[0] = hierarchyPlugin;
			// p[ 1 ] = hierarchyPlugin;
			for (int i = 0; i < p.Length; i++) if (p[i] != null) p[i].Init(i);
			ENABLE(true);

			if (EditorPrefs.GetInt(Folders.PREFS_PATH + "|showWelcomeStart", 0) != 1)
			{
				EditorPrefs.SetInt(Folders.PREFS_PATH + "|showWelcomeStart", 1);
				WelcomeScreen.Init(null);
			}
		}


		internal static void SET_EXTERNAl_MOD(Mods.ExternalModRoot mod)
		{
			if (!p[0].par_e.ENABLE_ALL) return;
			p[0].modsController.externalMods.RemoveAll(m => !m || m.currentWindow == mod.currentWindow);
			p[0].modsController.externalMods.Add(mod);
			p[0].modsController.REBUILD_PLUGINS(true);
		}
		internal static void REMOVE_EXTERNAl_MOD(Mods.ExternalModRoot mod)
		{
			if (!p[0].par_e.ENABLE_ALL) return;
			p[0].modsController.externalMods.RemoveAll(m => !m || m.currentWindow == mod.currentWindow);
			p[0].modsController.externalMods.Remove(mod);
			p[0].modsController.REBUILD_PLUGINS(true);
		}

		static bool enabled = false;
		internal static void DISABLE()
		{
			if (!enabled) return;
			enabled = false;
			foreach (var item in Root.p[0].modsController.externalMods.ToList())
			{
				if (!item) continue;
				item.Close();
			}
			var sbs = new EditorSubscriber();
			Root.p[0].REBUILD_EDITOR_EVENTS(sbs);

			foreach (var item in p)
			{
				if (item == null) continue;
				EditorApplication.hierarchyWindowItemOnGUI -= item.hierarchy_gui;
				EditorApplication.update -= item.Update;
				EditorSceneManager.sceneOpened -= item.invoke_EditorSceneManagerOnSceneOpening;
				Selection.selectionChanged -= item.invoke_onSelectionChanged;
				EditorApplication.hierarchyChanged -= item.invoke_onHierarchyChanged;
				EditorApplication.playModeStateChanged -= item.invoke_onPlayModeStateChanged;
				UnityEditor.Undo.undoRedoPerformed -= item.invoke_unUndo;
				EditorApplication.projectWindowItemOnGUI -= item.invoke_OnProjectWindow;
				EditorApplication.wantsToQuit -= item.invoke_OnEditorWantsToQuit;
#if !UNITY_2017_1_OR_NEWER
                SceneView.onSceneGUIDelegate -= item.invoke_DuringSceneGUI;
#else
				SceneView.duringSceneGui -= item.invoke_DuringSceneGUI;
#endif
				EditorApplication.modifierKeysChanged -= item.invoke_ModifiyKeyChanged;
				var info = typeof(EditorApplication).GetField("globalEventHandler", (BindingFlags)(-1));
				var value = (EditorApplication.CallbackFunction)info.GetValue(null);
				value -= item.EditorGlobalKeyPress;
				info.SetValue(null, value);
			}
			if (Mods.SnapMod.SET_ENABLE(Root.p[0].par_e.USE_SNAP_MOD && Root.p[0].par_e.ENABLE_ALL) || Root.p[0].par_e.USE_PROJECT_AUTO_HIGHLIGHTER_MOD || Root.p[0].par_e.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD)
				RequestScriptReload();
		}


		internal static void ENABLE(bool skipReload = false)
		{
			if (enabled) return;
			Init();
			enabled = true;
			foreach (var item in p)
			{
				if (item == null) continue;
				EditorApplication.hierarchyWindowItemOnGUI += item.hierarchy_gui;
				EditorApplication.update += item.Update;
				EditorSceneManager.sceneOpened += item.invoke_EditorSceneManagerOnSceneOpening;
				//  EditorSceneManager.activeSceneChangedInEditMode += item.invoke_EditorSceneManagerOnSceneOpening2;
				//  EditorSceneManager.sce += item.invoke_EditorSceneManagerOnSceneOpening3;
				Selection.selectionChanged += item.invoke_onSelectionChanged;
				EditorApplication.hierarchyChanged += item.invoke_onHierarchyChanged;
				EditorApplication.playModeStateChanged += item.invoke_onPlayModeStateChanged;
				UnityEditor.Undo.undoRedoPerformed += item.invoke_unUndo;
				EditorApplication.projectWindowItemOnGUI += item.invoke_OnProjectWindow;
				EditorApplication.wantsToQuit += item.invoke_OnEditorWantsToQuit;
#if !UNITY_2017_1_OR_NEWER
                SceneView.onSceneGUIDelegate += item.invoke_DuringSceneGUI;
#else
				SceneView.duringSceneGui += item.invoke_DuringSceneGUI;
#endif
				EditorApplication.modifierKeysChanged += item.invoke_ModifiyKeyChanged;
				var info = typeof(EditorApplication).GetField("globalEventHandler", (BindingFlags)(-1));
				var value = (EditorApplication.CallbackFunction)info.GetValue(null);
				value += item.EditorGlobalKeyPress;
				info.SetValue(null, value);
			}
			Root.p[0].modsController.REBUILD_PLUGINS();
			if (Mods.SnapMod.SET_ENABLE(Root.p[0].par_e.USE_SNAP_MOD && Root.p[0].par_e.ENABLE_ALL))
				if (!skipReload) RequestScriptReload();
		}


		internal static void RequestScriptReload()
		{
#if UNITY_2019_3_OR_NEWER
			EditorUtility.RequestScriptReload();
#else
			InternalEditorUtility.RequestScriptReload();
#endif
		}

		internal static PropertyInfo GUIView_current, View_window, View_position, View_windowPosition, DockArea_selected, HostView_actualView;
		internal static Type DockArea;
		internal static FieldInfo DockArea_m_Panes;
		internal static Type UnityEventArgsType;
		internal static MethodInfo SceneFrameMethod;

		static MethodInfo _SetMouseTooltip;

		static void InitializeRoot()
		{
			var GUIView = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.GUIView") ?? throw new Exception("GUIView");
			GUIView_current = GUIView.GetProperty("current", ~(BindingFlags.Instance | BindingFlags.InvokeMethod)) ?? throw new Exception("current");
			View_window = GUIView.BaseType.GetProperty("window", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("window");
			View_position = GUIView.BaseType.GetProperty("position", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("position");
			View_windowPosition = GUIView.BaseType.GetProperty("windowPosition", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("windowPosition");
			DockArea = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.DockArea") ?? throw new Exception("DockArea");
			DockArea_m_Panes = DockArea.GetField("m_Panes", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("m_Panes");
			DockArea_selected = DockArea.GetProperty("selected", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("selected");
			UnityEventArgsType = Assembly.GetAssembly(typeof(UnityEngine.Events.UnityEvent)).GetType("UnityEngine.Events.ArgumentCache", true) ?? throw new Exception("ArgumentCache");
			SceneFrameMethod = (typeof(SceneView).GetMethod("Frame", ~(BindingFlags.Static | BindingFlags.GetField))) ?? throw new Exception("Frame");
			_SetMouseTooltip = (typeof(GUIStyle).GetMethod("SetMouseTooltip", ~(BindingFlags.Instance | BindingFlags.GetField))) ?? throw new Exception("SetMouseTooltip");
			var HostView = Assembly.GetAssembly(typeof(EditorGUIUtility)).GetType("UnityEditor.HostView") ?? throw new Exception("HostView");
			HostView_actualView = HostView.GetProperty("actualView", ~(BindingFlags.Static | BindingFlags.InvokeMethod)) ?? throw new Exception("actualView");
			Window.InitFields();


		}

		static object[] _setTooltipArgs = new object[2];
		internal static void SetMouseTooltip(string content, Rect rect)
		{
			_c.tooltip = content;
			SetMouseTooltip(_c, rect);
		}
		static GUIContent _c = new GUIContent();
		internal static void SetMouseTooltip(GUIContent content, Rect rect)
		{

			if (content.tooltip == null || content.tooltip == "") return;

			if (!rect.Contains(Root.p[0].EVENT.mousePosition) || !Root.p[0].GUIClip_visibleRect.Contains(Root.p[0].EVENT.mousePosition)) return;

			_setTooltipArgs[0] = content.tooltip;
			_setTooltipArgs[1] = rect;
			_SetMouseTooltip.Invoke(null, _setTooltipArgs);
		}

	}



	internal class StyleInitHelper
	{
		public static implicit operator bool(StyleInitHelper h)
		{
			return h.value == true && h.proSkin == EditorGUIUtility.isProSkin;
		}
		public static implicit operator StyleInitHelper(bool h)
		{
			return new StyleInitHelper() { proSkin = PluginInstance.WAS_GUI_FLAG ? EditorGUIUtility.isProSkin : (bool?)null, value = h };
		}
		internal bool? value = null;
		internal bool? proSkin = null;
	}




	internal partial class PluginInstance
	{


		int KEY = 0;
		internal SHADER_HELPER _DEFAULT_SHADER_SHADER;
		internal SHADER_HELPER DEFAULT_SHADER_SHADER
		{
			get
			{
				if (_DEFAULT_SHADER_SHADER == null)
				{
					_DEFAULT_SHADER_SHADER = new SHADER_HELPER(KEY + "_DEFAULT_SHADER_SHADER", this)
					{
						SET_SHADER_GUID = (guid) => { },
						GET_SHADER_GUID = () => { return "70c76382e3a8a0e4f9f719883a135eff"; },
						GET_SHADER_LOCAL_PATH = () => { return "/Editor/Materials/Highlighter - Default GUI Shader.shader"; }
					};
				}

				return _DEFAULT_SHADER_SHADER;
			}
		}
		
		internal string HIGHLIGHTER_SHADER_GUID_MAIN { get { return par_e.GET(KEY + "_SHADER_GUID_MAIN", "830e0b361750b98468ce6493b692d717"); } set { par_e.SET(KEY + "_SHADER_GUID_MAIN", value); } }
		internal string HIGHLIGHTER_SHADER_GUID_ADD { get { return par_e.GET(KEY + "_SHADER_GUID_ADD", "12ace602f83e8b941a0cec6ee38c1a79"); } set { par_e.SET(KEY + "_SHADER_GUID_ADD", value); } }

		internal class SHADER_HELPER
		{
			string keyMat /*, keyShader*/;
			PluginInstance adapter;

			internal SHADER_HELPER(string key, PluginInstance adapter)
			{
				this.adapter = adapter;
				this.keyMat = key + "-Material";
				// this.keyShader = key + "-Shader";
			}

			internal Func<string> GET_SHADER_GUID;
			internal Func<string> GET_SHADER_LOCAL_PATH;
			internal Action<string> SET_SHADER_GUID;

			Shader oldSHader;
			Material _HIghlighterExternalMaterial;



			int matID
			{
				get { return adapter.par_e.GET(keyMat, -1); }
				set { adapter.par_e.SET(keyMat, value); }
			}


			internal Material ExternalMaterialReference
			{
				get
				{
					if (oldSHader != ExternalShaderReference)
					{
						oldSHader = ExternalShaderReference;
						if (oldSHader == null) _HIghlighterExternalMaterial = null;
						else _HIghlighterExternalMaterial = new Material(_HIghlighterExternalShader);
						matID = _HIghlighterExternalMaterial == null ? -1 : _HIghlighterExternalMaterial.GetInstanceID();
					}
					if (!_HIghlighterExternalMaterial && matID != -1)
					{
						_HIghlighterExternalMaterial = EditorUtility.InstanceIDToObject(matID) as Material;
						if (!_HIghlighterExternalMaterial && ExternalShaderReference)
						{
							oldSHader = null;
							return ExternalMaterialReference;
						}
					}
					return _HIghlighterExternalMaterial;
				}
			}


			bool HIghlighterExternalShader_init;
			Shader _HIghlighterExternalShader;

			internal Shader ExternalShaderReference
			{
				get
				{
					if (!HIghlighterExternalShader_init || _HIghlighterExternalShader == null)
					{
						HIghlighterExternalShader_init = true;

						if (!string.IsNullOrEmpty(GET_SHADER_GUID()))
						{
							var path = AssetDatabase.GUIDToAssetPath(GET_SHADER_GUID());
							if (string.IsNullOrEmpty(path) && GET_SHADER_LOCAL_PATH != null) path = Folders.PluginInternalFolder + GET_SHADER_LOCAL_PATH();
							if (!string.IsNullOrEmpty(path))
							{
								_HIghlighterExternalShader = AssetDatabase.LoadAssetAtPath<Shader>(path) as Shader;
							}
						}
					}

					return _HIghlighterExternalShader;
				}

				set
				{
					if (value != ExternalShaderReference)
					{
						_HIghlighterExternalShader = value;

						if (!value)
						{
							SET_SHADER_GUID("");
						}

						else
						{
							var path = AssetDatabase.GetAssetPath(value);

							if (!string.IsNullOrEmpty(path))
							{
								var guid = AssetDatabase.AssetPathToGUID(path);

								if (!string.IsNullOrEmpty(guid))
								{
									SET_SHADER_GUID(guid);
								}

								else SET_SHADER_GUID("");
							}

							else SET_SHADER_GUID("");
						}

						if (GET_SHADER_GUID() == "") _HIghlighterExternalShader = null;
					}
				}
			}
		}



		internal int pluginID;
		internal HierarchyObject o;
		internal static Window windowEmpty = new Window();
		internal Window window;
		internal Window lastHierarchyWindw;
		internal Window lastProjectWindw;
		internal Window firstWindow(int pid) { { return Window._update.Values.FirstOrDefault(w => w.pluginID == pid); } }
		//internal Window firstHierarchyWindow { get { return Window._update.Values.FirstOrDefault(w => w.pluginID == 0) ?? window; } }
		//internal Window firstProjectWindow { get { return Window._update.Values.FirstOrDefault(w => w.pluginID == 1) ?? window; } }
		internal bool UseRootWindow_0;
		internal bool UseRootWindow_1;
		internal bool UseRootWindow { get { return pluginID == 0 ? UseRootWindow_0 : UseRootWindow_1; } }
		internal string pluginname;

		Events.HierarchyActions ha_0;
		Events.HierarchyActions ha_1;
		internal Events.HierarchyActions ha { get { return pluginID == 0 ? ha_0 : ha_1; } }
		internal Events.HierarchyActions ha_G(int i) { { return i == 0 ? ha_0 : ha_1; } }
		internal EditorSettingsAdapter par_e;
		DuplicateHelper duplicate;
		WindowsManager windowsManager;
		//	internal HierarchyModification hierarchyModification;
		internal ModsController modsController;
		internal GlDrawer gl;
		internal EditorSettingsAdapter.WindowSettings WIN_SET { get { return pluginID == 0 ? par_e.HIER_WIN_SET : par_e.PROJ_WIN_SET; } }
		internal EditorSettingsAdapter.WindowSettings WIN_SET_INVERSE { get { return pluginID == 0 ? par_e.PROJ_WIN_SET : par_e.HIER_WIN_SET; } }
		internal EditorSettingsAdapter.WindowSettings WIN_SET_G(int pluginID) { return pluginID == 0 ? par_e.HIER_WIN_SET : par_e.PROJ_WIN_SET; }



		internal MethodInfo gui_getFirstAndLastRowVisible, data_FindItem_Slow,
						gui_GetRowRect, ExpansionAnimator_CullRow, data_m_dataSetExpanded, data_m_dataIsExpanded, data_m_dataSetExpandWithChildrens, hierwin_DuplicateGO;
		FieldInfo _AssetTreeState, _FolderTreeState, _TreeViewController_0, _FoldView, _AssetsView, _ViewMode, /*gui_m_LineHeight, gui_k_IndentWidth, gui_k_IconWidth,  gui_customFoldoutYOffset,*/   tree_m_ContentRect, m_UseExpansionAnimation
				// ,tree_m_KeyboardControlIDField
				;
		internal FieldInfo state_scrollPos, tree_m_ExpansionAnimator, m_SearchFieldText, tree_m_TotalRect, m_SearchFilter;
		internal MethodInfo data_GetRows, PrefabModeButton, data_GetItemRowFast, IsSearching,
#pragma warning disable
		GetInstanceIDFromGUID, GetMainAssetOrInProgressProxyInstanceID;
#pragma warning restore
		internal int hoverID;
		bool _hashoveredItem;
		internal bool hashoveredItem
		{
			get { return _hashoveredItem && !WIN_SET.HIDE_HOVER_BG; }
			set { _hashoveredItem = value; }
		}
		internal PropertyInfo _data, _gui, _state, hoveredItem, showPrefabModeButton, tree_animatingExpansion, ExpansionAnimator_endRow, data_rowCount;
		PropertyInfo data_m_RootItem, guiclip_visibleRect;
		object[] args = new object[2];
		int firstRowVisible, lastRowVisible;

		// ???
		// ???
		// ???
		internal Rect _currentClipRect;
		// ???
		// ???
		// ???


		internal IconData GetNewIcon(NewIconTexture t, string key) { return Root.icons.GetNewIcon(t, ref key); }
		internal IconData GetOldIcon(string s) { return Root.icons.GetOldIcon(ref s); }
		internal Texture2D GetExternalModOld(string s) { return Root.icons.GetOldExternalMod(ref s); }
		//  delegate void GetFirstAndLastRowVisible( out int firstRowVisible, out int lastRowVisible );
		//  GetFirstAndLastRowVisible gui_getFirstAndLastRowVisible;
		//(GetFirstAndLastRowVisible)Delegate.CreateDelegate( typeof( GetFirstAndLastRowVisible ), this,


		//internal bool NEW_PERFOMANCE { get { return pluginID == 0; } }
		internal bool NEW_PERFOMANCE = true;
		// return UNITY_CURRENT_VERSION >= UNITY_2019_VERSION;



		internal static PluginInstance CreateInstance(string name)
		{
			var res = new PluginInstance();
			res.pluginname = name;
			return res;
		}
		internal void Init(int pId)
		{
			pluginID = pId;
			Init();
			gl = new GlDrawer(this);
			par_e = new EditorSettingsAdapter(pId);
			ha_0 = new Events.HierarchyActions(0);
			ha_1 = new Events.HierarchyActions(1);
			duplicate = new DuplicateHelper(pId);
			windowsManager = new WindowsManager(pId);
			_mouse_uo_helper[0] = new Events.MouseRawUp();
			_mouse_uo_helper[1] = new Events.MouseRawUp();
			//hierarchyModification = new HierarchyModification(this);
			modsController = new ModsController(this);
			window = new Window();


			modsController.REBUILD_PLUGINS(true);
		}




		//FIELDS-
		object[] argsa = new object[1];
		internal EventType? lastEvent, fixDrawLastEvent;
		internal Event EVENT;
		internal int firstFrame;
		int index = 0;
		int num = 0;
		int numVisibleRows = 0;
		int rowCount;
		internal Vector2 scrollPos;
		bool animatingExpansion;
		object m_ExpansionAnimator;
		internal Rect m_TotalRect, m_ContentRect, GUIClip_visibleRect;
		int endRow;
		float rowWidth;
		int CalcRow(ref int index, ref int num)
		{
			int row = -1;
			for (; index < numVisibleRows; ++index)
			{
				row = firstRowVisible + index;
				if (this.animatingExpansion)
				{
					// int endRow = this.m_ExpansionAnimator.endRow;
					//  if ( this.m_ExpansionAnimator.CullRow( row, this.gui ) )
					args[0] = row;
					args[1] = gui_currentTree;
					var res = (bool)ExpansionAnimator_CullRow.Invoke(m_ExpansionAnimator, args);
					if (res)
					{
						++num;
						row = endRow + num;
					}
					else
						row += num;
					// if ( row >= this.data.rowCount )
					if (row >= rowCount)
						continue;
				}
				else
				{
					args[0] = row;
					args[1] = 0f;
					var res = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);
					if ((double)(res.y - scrollPos.y) > m_TotalRect.height)
						continue;
				}
				break;
			}
			return row;
		}
		// -

		internal float DEFAULT_ICON_SIZE
		{
			get
			{
				return Window.DefaultIconSize(this);
				//	return EditorGUIUtility.GetIconSize().x;
				//return DEFAULT_ICON_SIZE;
			}
		}

		//EVENTS-
		internal void PUSH_ONMOUSEUP(int plug, Action ac, EditorWindow win = null) { _mouse_uo_helper[plug].PUSH_ONMOUSEUP(ac, win); }
		Events.MouseRawUp[] _mouse_uo_helper = new Events.MouseRawUp[2];
		internal void PUSH_GUI_ONESHOT(int plug, Action ac)
		{
			bool allow = false;
			foreach (var w in WindowsData(plug)) if (w.Value.w.Instance) allow = true;
			if (!allow)
			{
				ac();
				return;
			}
			_oneShotGui[plug].Add(ac);
			RepaintWindowInUpdate(plug);
		}
		List<Action>[] _oneShotGui = { new List<Action>(), new List<Action>() };
		internal void PUSH_UPDATE_ONESHOT(int plugin, Action ac) { _oneShotUpdate[plugin].Add(ac); }
		List<Action>[] _oneShotUpdate = { new List<Action>(), new List<Action>() };
		// -

		// OTHER
		bool? oldProSkin;
		// --

		//MAIN GUI-
		internal object TreeController_current;
		internal object gui_currentTree, data_currentTree, state_currentTree;
		internal IList<int> current_DragSelection_List = new List<int>();
		internal IList<int> current_selectedIDs = new List<int>();

		bool GuiReady = true;
		internal Rect selectionRect;
		internal Rect fullLineRect, _first_FullLineRect, _last_FullLineRect;
		internal float rightOffset = 0;

		internal class EditorWindowData
		{
			internal int pid;
			internal EditorWindowData(int pid)
			{
				this.pid = pid;
			}
			internal Window w;
			internal object lastTree;
			internal bool nowSearch;
		}
		static Dictionary<int, EditorWindowData> allWindowsData = new Dictionary<int, EditorWindowData>();
		internal static Dictionary<int, EditorWindowData> HierarchyWindowsData { get { return allWindowsData.Where(d => d.Value.pid == 0).ToDictionary(k => k.Key, v => v.Value); } }
		internal static Dictionary<int, EditorWindowData> ProjectWindowsData { get { return allWindowsData.Where(d => d.Value.pid == 1).ToDictionary(k => k.Key, v => v.Value); } }
		internal static Dictionary<int, EditorWindowData> WindowsData(int pid) { return pid == 0 ? HierarchyWindowsData : ProjectWindowsData; }
		void CHECK_ONESHOT_GUI()
		{
			if (_oneShotGui[pluginID].Count > 0 && Event.current.type == EventType.Repaint)
			{
				foreach (var item in _oneShotGui[pluginID]) item();
				_oneShotGui[pluginID].Clear();
			}
		}
		static bool firstFixDraw = false;
		static bool firstFixDrawStart = false;
		internal bool HoverDisabled = false;
		bool shouldBuildedBeginGUI = false;

		internal int MOUSE_BUTTON_0 { get { return par_e.USE_SWAP_FOR_BUTTONS_ACTION ? 1 : 0; } }
		internal int MOUSE_BUTTON_1 { get { return par_e.USE_SWAP_FOR_BUTTONS_ACTION ? 0 : 1; } }

		internal static bool WAS_GUI_FLAG = false;
		bool[] firstFrameBool = new bool[2];
		// bool sended = false;
		internal void hierarchy_gui(int instanceID, Rect selectionRect)
		{
			pluginID = 0;
			gui(instanceID, ref selectionRect);
		}
		internal void gui(int instanceID, ref Rect selectionRect)
		{


			if (firstFrameBool[pluginID])
			{
				//Debug.Log("ASD" + Event.current.type);
				firstFrameBool[pluginID] = false;
				if (Event.current.type == EventType.Repaint)
				{
					Window.AssignInstance(pluginID, ref window, pluginID == 0 ? SceneHierarchyWindowRoot : ProjectBrowserWindowType); // WINDOW INIT
					window.Instance.SendEvent(new Event() { type = EventType.Layout });
					window.Instance.SendEvent(new Event() { type = EventType.Repaint });
					EditorGUIUtility.ExitGUI();
					return;
				}

			}
			// if ( Event.current.type == EventType.Layout ) return;
			if (!WAS_GUI_FLAG) WAS_GUI_FLAG = true;
			this.selectionRect = selectionRect;



			EVENT = Event.current;
			//var _go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;



			if (GuiReady/* || !_go*/  || lastEvent != EVENT.type)
			{

				_mouse_uo_helper[pluginID].Invoke();
				modsController.rightModsManager.headerEventsBlockRect = null;
				HoverDisabled = false;
				CHECK_ONESHOT_GUI();

				if (!oldProSkin.HasValue) oldProSkin = EditorGUIUtility.isProSkin;
				if (oldProSkin != EditorGUIUtility.isProSkin) Cache.ClearHierarchyObjects(pluginID == 1);

				if (firstFrame != 5) firstFrame++;
				GuiReady = true;
				lastEvent = EVENT.type;

				Window.AssignInstance(pluginID, ref window, pluginID == 0 ? SceneHierarchyWindowRoot : ProjectBrowserWindowType); // WINDOW INIT
				TreeController_current = GetTreeViewontroller(pluginID);
				gui_currentTree = _gui.GetValue(TreeController_current);
				data_currentTree = _data.GetValue(TreeController_current);
				state_currentTree = _state.GetValue(TreeController_current);

				ha.TryToInitializeDefaultStyles();
				animatingExpansion = (bool)tree_animatingExpansion.GetValue(TreeController_current, null);
				if (animatingExpansion)
				{
					m_ExpansionAnimator = tree_m_ExpansionAnimator.GetValue(TreeController_current);
					endRow = (int)ExpansionAnimator_endRow.GetValue(m_ExpansionAnimator, null);
					/*if ( !sended && lastEvent.Value == EventType.Layout)
					{
							EditorGUIUtility.ExitGUI();
							window.Instance.SendEvent( new Event() { type = lastEvent.Value } );
					}
					sended = true;*/
				}
				m_TotalRect = (Rect)tree_m_TotalRect.GetValue(TreeController_current);
				m_ContentRect = (Rect)tree_m_ContentRect.GetValue(TreeController_current);
				GUIClip_visibleRect = (Rect)guiclip_visibleRect.GetValue(null, null);
			
				if (!allWindowsData.ContainsKey(window.Instance.GetInstanceID()))
				{
					allWindowsData.Add(window.Instance.GetInstanceID(), new EditorWindowData(pluginID) { w = window, lastTree = TreeController_current, nowSearch = false });
					window.SetHeightAndIndents(this); // WINDOW SET
				}
				ha.BAKE_SEARCH();
				if (!ReferenceEquals(allWindowsData[window.Instance.GetInstanceID()].lastTree, TreeController_current) || allWindowsData[window.Instance.GetInstanceID()].nowSearch != ha.IS_SEARCH_MOD_OPENED())
				{
					allWindowsData[window.Instance.GetInstanceID()].lastTree = TreeController_current;
					allWindowsData[window.Instance.GetInstanceID()].nowSearch = ha.IS_SEARCH_MOD_OPENED();
					window.SetHeightAndIndents(this); // WINDOW SET
				}
				var o1 = WIN_SET.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
				var o2 = WIN_SET_INVERSE.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
				if (o1 || o2) window.SetHeightAndIndents_Again(this); // WINDOW SET



				window.FinalEvents(this);

				args[0] = args[1] = 0;
				gui_getFirstAndLastRowVisible.Invoke(gui_currentTree, args);
				firstRowVisible = (int)args[0];
				lastRowVisible = (int)args[1];
				numVisibleRows = lastRowVisible - firstRowVisible + 1;

				scrollPos = (Vector2)state_scrollPos.GetValue(state_currentTree);
				rowCount = (int)data_rowCount.GetValue(data_currentTree);
				rowWidth = Mathf.Max(GUIClip_visibleRect.width, m_ContentRect.width);
				// currentRow = firstRowVisible;
				index = 0; num = 0;

				args[0] = lastRowVisible;
				args[1] = 0f;
				_last_FullLineRect = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);

				ha.UpdateBGHover();

				window.CheckMouseEvents();
				o = null;
			}
			else
			{
				index++;
				//currentRow++;
			}

			if (!firstFixDraw)
			{
				firstFixDraw = true;
				firstFixDrawStart = true;
				fixDrawLastEvent = EVENT.type;
			}
			if (firstFixDrawStart)
			{
				if (fixDrawLastEvent != EVENT.type)
				{
					firstFixDrawStart = false;
				}
				else
				{
					//return;
				}
			}

			int row = CalcRow(ref index, ref num);
			var fakeIndex = index + 1;
			var fakeNum = num;
			CalcRow(ref fakeIndex, ref fakeNum);
			bool thisIsLast = fakeIndex == numVisibleRows || row == lastRowVisible;
			argsa[0] = row;
			// Debug.Log( row + " " + " " + numVisibleRows + " " + thisIsLast );
			var currentTreeItemFast = data_GetItemRowFast.Invoke(data_currentTree, argsa);
			args[0] = row;
			args[1] = rowWidth;
			fullLineRect = (Rect)gui_GetRowRect.Invoke(gui_currentTree, args);
			// lineRect.width = selectionRect.x + selectionRect.width;
			///fullLineRect.width = rowWidth;
			/// 
			//  selectionRect.y = fullLineRect.y;
			fullLineRect.y = selectionRect.y;

			o = pluginID == 0 ? Cache.GetHierarchyObjectByInstanceID(instanceID, null) : Cache.GetHierarchyObjectByGUID(instanceID);
			if (o != null)
			{
				o._visibleTreeItem = currentTreeItemFast as UnityEditor.IMGUI.Controls.TreeViewItem;
				if (pluginID == 1) o.WriteSibling();
			}



			if (GuiReady)
			{

				//modsController.toolBarModification. hotButtons.DrawButtonsOnTopBar();
				//modsController.toolBarModification.layoutsMod.DrawLayers();
				// _first_SelectionRect = selectionRect;
				//Debug.Log( EVENT.type + " " + fullLineRect + " " +selectionRect );
				_first_FullLineRect = fullLineRect;
				if (!firstFixDraw)
				{
					firstFixDraw = true;
					//EditorGUIUtility.ExitGUI();
					//window.Instance.SendEvent( new Event() { type =  EventType.Layout } );
					//window.Instance.SendEvent( new Event() { type =  EventType.Repaint } );
					////return;
					/*var s = GUI.skin.verticalScrollbarUpButton.fixedWidth;
					fullLineRect.width -=s;
					rowWidth -=s;
					_last_FullLineRect.width -=s;
					_last_FullLineRect.width -=s; */
					//RepaintWindow( true );
				}
				GuiReady = false;
				rightOffset = 0;
				if (ha.hasShowingPrefabHeader) ha.prebapButtonStyle.margin.right = 0;
				///if ( par_e.RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER && par_e.USE_RIGHT_ALL_MODS ) 
				rightOffset += ha.PREFAB_BUTTON_SIZE;
				if (pluginID == 0) ButtonsActionsDetect();
				if (pluginID == 0) ha.OnSelectionChanged_SaveCache();

				InternalOnGUI_first();

				shouldBuildedBeginGUI = true;
				if (g[pluginID].BuildedOnGUI_first != null) g[pluginID].BuildedOnGUI_first();
				window.drew_mods_count = 0;
			}


			//  Debug.Log(row + " " + lineRect);
			//selectionRect.height = lineRect.height = 16;
			// Debug.Log(lineRect.y);

			if (!o.Validate() || selectionRect.height <= 0 /*|| animatingExpansion && par_e.DISABLE_DRAWING_ANIMATING_ITEMS*/) // THIS IS SCENE
			{
				//EditorUtility.InstanceIDToObject( instanceID ) as SceneAsset;
				//return;
				goto end;
			}

			if (shouldBuildedBeginGUI)
			{

				shouldBuildedBeginGUI = false;
			}


			o.lastFullLineRect = fullLineRect;
			o.lastSelectionRect = selectionRect;


			if (window.drew_mods_count > window.drew_mods_objects.Length) Array.Resize(ref window.drew_mods_objects, window.drew_mods_objects.Length + 100);
			window.drew_mods_objects[window.drew_mods_count].o = o;
			window.drew_mods_objects[window.drew_mods_count].selectionRect = selectionRect;
			window.drew_mods_count++;


			// Debug.Log( row );
			// if ( row == 1 || lineRect.y != 0 ) 
			// GUI.BeginClip( selectionRect );
			// selectionRect.x -= lineRect.x;
			// selectionRect.y = 0;
			//  lineRect.x = lineRect.y = 0;
			//OnGUI();
			if (g[pluginID].BuildedOnGUI_middle != null && EVENT.type != EventType.Layout) g[pluginID].BuildedOnGUI_middle();
			if (g[pluginID].BuildedOnGUI_middle_plusLayout != null) g[pluginID].BuildedOnGUI_middle_plusLayout();
			// GUI.EndClip();


			end:;
			EditorActions_EveryFrame_AfterOnGUI();
			if (thisIsLast)
			{
				GuiReady = true;
				CHECK_ONESHOT_GUI();
				if (g[pluginID].BuildedOnGUI_last_butBeforeGL != null && EVENT.type != EventType.Layout) g[pluginID].BuildedOnGUI_last_butBeforeGL();
				gl.ReleaseStack();
				if (g[pluginID].BuildedOnGUI_last != null && EVENT.type != EventType.Layout) g[pluginID].BuildedOnGUI_last();
				// sended = false;
			}

			if (HoverDisabled) ha.internal_DisableHover();

			// Debug.Log( GetTreeItem( instanceID ) );
			// current._visibleTreeItem =
		}




		void InternalOnGUI_first()
		{

			//Init
			Colors.UpdateColorsBefore_OnGUI(this);

			//RESET_DRAW_STACKS-
			if (window.position.width != _drawStack_oldWPos[pluginID].width || window.position.height != _drawStack_oldWPos[pluginID].height)
			{
				_drawStack_oldWPos[pluginID] = window.position;
				_drawStack_reset_stacks(pluginID);
			}
			if (!_drawStack_lastCacheClean.HasValue) _drawStack_lastCacheClean = EditorApplication.timeSinceStartup;
			if (Math.Abs(_drawStack_lastCacheClean.Value - EditorApplication.timeSinceStartup) > DRAWSTACK_RESET_TIME)
			{
				_drawStack_RESET_TIMER_DRAWSTACKS();
				// Debug.Log( Math.Abs( lastCacheClean.Value - EditorApplication.timeSinceStartup ) );
				_drawStack_lastCacheClean = EditorApplication.timeSinceStartup;
			}
			if (EVENT.type == EventType.Layout && (_drawStack_resetStacks[pluginID] || _drawStack_resetTimerStack[pluginID]))
			{
				_drawStack_reset_stacks(pluginID);
				_drawStack_resetStacks[pluginID] = false;
				_drawStack_resetTimerStack[pluginID] = false;
			}
			//-

			//ANIMATION EXPAND
			if (par_e.USE_EXPANSION_ANIMATION != (bool)m_UseExpansionAnimation.GetValue(TreeController_current))
				m_UseExpansionAnimation.SetValue(TreeController_current, par_e.USE_EXPANSION_ANIMATION);


			//DUPLICATE
			duplicate.Duplicate_FirstFrame_OnGUI();


			//HOVER LINES
			if (hashoveredItem && !HoverDisabled)
			{

				if (windowsManager.InputWindowsCount() > 0) ha.internal_DisableHover();
				/*if ( GetNavigatorRect( 10000 ).y - HEIGHT < EVENT.mousePosition.y )
		DISABLE_HOVER();*/
				var h = hoveredItem.GetValue(TreeController_current, null) as UnityEditor.IMGUI.Controls.TreeViewItem;
				//hoveredItem.SetValue(tree, null, null);
				if (h != null) hoverID = h.id;
				else hoverID = -1;
			}
			else
			{
				hoverID = -1;
			}
		}




		// DRAW_STACK
		const double DRAWSTACK_RESET_TIME = 4;
		double? _drawStack_lastCacheClean;
		Rect[] _drawStack_oldWPos = new Rect[2];
		bool[] _drawStack_resetStacks = new bool[2];
		bool[] _drawStack_resetTimerStack = new bool[2];
		internal void RESET_DRAWSTACK(int plug)
		{
			if (_OnResetDrawStack != null) _OnResetDrawStack();
			_drawStack_resetStacks[plug] = true;

			//#EMX_TODO check for a little optimization
			RepaintWindowInUpdate(plug);

#if EMX_HIERARCHY_DEBUG_STACKS
		Debug.Log( "RESET_DRAW_STACKS" );
#endif
		}
		void _drawStack_RESET_TIMER_DRAWSTACKS()
		{ //  __reset_stacks();
			_drawStack_resetTimerStack[0] = true;
			_drawStack_resetTimerStack[1] = true;
		}
		void _drawStack_reset_stacks(int plug)
		{

			modsController.RESET_DRAW_STACKS(plug);
			// foreach ( var m in internalMods ) m.Value.ResetDrawStack();
			// foreach ( var m in externalMods ) m.Value.ResetDrawStack();
			/*for ( int i = 0 ; i < modules.Length ; i++ )
			{
					foreach ( var stack in modules[ i ].DRAW_STACK )
					{
							stack.Value.ResetStack();
					}
			}*/
		}
		// -





		/// <summary>
		/// #############################################################################################################################################################
		/// </summary>

		static object[] ob_arr2 = new object[1];
		static UnityEditor.IMGUI.Controls.TreeViewItem tiv;
		Dictionary<object, UnityEditor.IMGUI.Controls.TreeViewItem> __ti = new Dictionary<object, UnityEditor.IMGUI.Controls.TreeViewItem>();
		UnityEditor.IMGUI.Controls.TreeViewItem emptyreeitem = new UnityEditor.IMGUI.Controls.TreeViewItem();

		internal UnityEditor.IMGUI.Controls.TreeViewItem GetTreeItem(int id)
		{

			// var tree = m_TreeViewontroller();
			var tree = TreeController_current;
			if (tree == null) return null;

			if (!__ti.TryGetValue(tree, out tiv) || tiv == null || tiv.id != id)
			{

				var data = _data.GetValue(tree);
				ob_arr2[0] = id;
				// ob_arr2[ 1 ] = data_m_RootItem.GetValue( data, null );
				var res = data_FindItem_Slow.Invoke(data, ob_arr2) as UnityEditor.IMGUI.Controls.TreeViewItem;

				if (__ti.ContainsKey(tree))
					__ti[tree] = res;
				else
					__ti.Add(tree, res);
			}
			return tiv ?? emptyreeitem;
		}




		internal object GetTreeViewontroller(int pid, EditorWindow w = null)
		{
			if (pid == 0)
			{
				if (UseRootWindow) return m_TreeViewFieldInfo.GetValue(w ?? window.Instance);
				var sub = _SceneHierarchy.GetValue(w ?? window.Instance);
				return m_TreeViewFieldInfo.GetValue(sub);
			}
			else
			{
				return m_TreeViewFieldInfoForProject(w ?? window.Instance).GetValue(w ?? window.Instance);
			}

		}

		internal FieldInfo m_TreeViewFieldInfo
		{
			get
			{
				return _TreeViewController_0;
				//return pluginID == 0 ? _TreeViewController_0 : _TreeViewController_1;
				// if ( !window( true ) ) return _FoldView;

			}
		}
		FieldInfo m_TreeViewFieldInfoForProject(EditorWindow w)
		{
			//return _AssetsView;


			if (ProjectViewMode(w) == 1) return _FoldView;
			return _AssetsView;
		}
		internal int ProjectViewMode(EditorWindow w)
		{
			return (int)_ViewMode.GetValue(w);
		}

		internal Type SceneHierarchyWindow(int pid)
		{
			//get
			{
				if (pid == 0)
				{
					if (_SceneHierarchyWindow == null)
					{
						var ass = Assembly.GetAssembly(typeof(EditorWindow));
						_SceneHierarchyWindow = ass.GetType("UnityEditor.SceneHierarchy");
						if (UseRootWindow_0 = _SceneHierarchyWindow == null) { if ((_SceneHierarchyWindow = ass.GetType("UnityEditor.SceneHierarchyWindow")) == null) throw new Exception("UnityEditor.SceneHierarchyWindow"); }
						else if ((_SceneHierarchy = SceneHierarchyWindowRoot.GetField("m_SceneHierarchy", ~(BindingFlags.Static | BindingFlags.InvokeMethod))) == null) throw new Exception("m_SceneHierarchy");
					}
					return _SceneHierarchyWindow;
				}
				else
				{
					if (_ProjectWindow == null)
					{
						var ass = Assembly.GetAssembly(typeof(EditorWindow));
						if ((_ProjectWindow = ass.GetType("UnityEditor.ProjectBrowser")) == null) throw new Exception("UnityEditor.ProjectBrowser");
						UseRootWindow_1 = true;
					}
					return _ProjectWindow;
				}

			}
		}
		Type _SceneHierarchyWindow;
		Type _ProjectWindow;
		FieldInfo _SceneHierarchy;
		internal Type SceneHierarchyWindowRoot
		{
			get
			{
				if (_SceneHierarchyWindowRoot == null)
				{
					_SceneHierarchyWindowRoot = Assembly.GetAssembly(typeof(EditorWindow)).GetType(
							pluginID == 0 ? "UnityEditor.SceneHierarchyWindow" : "UnityEditor.ProjectBrowser"
					);
				}

				return _SceneHierarchyWindowRoot;
			}
		}
		Type _SceneHierarchyWindowRoot;
		Type _ProjectBrowserWindowType;
		internal Type ProjectBrowserWindowType
		{
			get
			{
				if (_ProjectBrowserWindowType == null)
				{
					_ProjectBrowserWindowType = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.ProjectBrowser"
					);
				}

				return _ProjectBrowserWindowType;
			}
		}



		//	internal bool tempAdapterDisableCache = false;
		internal bool DisabledSavedData()
		{
			return false;//|| !wasAdapterInitalize;
		}

		internal void Modules_SetDirty()
		{

		}

		internal void Modules_RefreshBookmarks()
		{

		}


	}



}
