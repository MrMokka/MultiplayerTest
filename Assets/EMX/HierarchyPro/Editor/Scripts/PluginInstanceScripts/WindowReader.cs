using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{

	struct TempModData
	{
		internal int sibling, width;
		internal bool enabled;
		internal Rect rect;
		internal bool fade_narrow;
		internal RightModBaseClass targetMod;
		internal int i_targetMod;
	}

	internal struct temp_draw
	{
		internal HierarchyObject o;
		internal Rect selectionRect;
	}
	internal class Window
	{

		internal EditorWindow Instance;
		internal float? oldPositionWidth;
		internal Rect position;
		internal Rect windowPosition;
		internal PluginInstance p { get { return Root.p[0]; } }
		internal int modsCount;
		internal TempModData[] tempModsData = new TempModData[50];
		internal float[] modulesPosesX = Enumerable.Repeat(-1f, 50).ToArray();

		internal int drew_mods_count = 0;
		internal temp_draw[] drew_mods_objects = new temp_draw[200];
		internal int pluginID;





		internal static Dictionary<int, Window> _update = new Dictionary<int, Window>();
		internal static void AssignInstance(int plugin, ref Window targetWindow, Type overrideType) // plugin.SceneHierarchyWindowRoot
		{

			var guiView = Root.GUIView_current.GetValue(null, null) as UnityEngine.Object;
			EditorWindow Instance;
			if (guiView.GetType().Name == "MaximizedHostView")
			{
				Instance = Root.HostView_actualView.GetValue(guiView, null) as EditorWindow;
			}
			else
			{
				var wins = (List<EditorWindow>)Root.DockArea_m_Panes.GetValue(guiView);
				var selected = (int)Root.DockArea_selected.GetValue(guiView, null);
				// var ht = Scene
				/*  Debug.Log(selected);
				  Debug.Log(wins.Count);
				  Debug.Log(wins[1] + " " + plugin.SceneHierarchyWindowRoot);*/
				var T = overrideType;
				if (selected >= 0 && selected < wins.Count && wins[selected].GetType() == T) Instance = wins[selected] as EditorWindow;
				else if (wins.Count == 1) Instance = wins[0] as EditorWindow;
				else Instance = wins.FirstOrDefault(w => w.GetType() == T) ?? throw new Exception("Cannot find hierarchy window");

			}

			// if (!_update.TryGetValue(Instance.GetInstanceID(), out targetWindow)) _update.Add(Instance.GetInstanceID(), targetWindow = new Window());

			if (!_update.ContainsKey(Instance.GetInstanceID()))
			{
				targetWindow = new Window();
				targetWindow.pluginID = plugin;
				targetWindow.Instance = Instance;
				//targetWindow.p = plugin;

				_update.Add(Instance.GetInstanceID(), targetWindow);

				if (plugin == 0) Root.p[0].lastHierarchyWindw = targetWindow;
				else Root.p[0].lastProjectWindw = targetWindow;
			}
			else
			{
				targetWindow = _update[Instance.GetInstanceID()];
			}

			// Instance = Root.View_window.GetValue( guiView, null ) as UnityEngine.Object;



			targetWindow.position = (Rect)Root.View_position.GetValue(guiView, null);
			targetWindow.windowPosition = (Rect)Root.View_windowPosition.GetValue(guiView, null);

		}

		static MethodInfo GetTotalSizeMethodInfo;
		float? lastContentSize = null;
		internal void FinalEvents(PluginInstance plugin)
		{

			if (GetTotalSizeMethodInfo == null) GetTotalSizeMethodInfo = plugin.m_TreeViewFieldInfo.FieldType.GetMethod("GetContentSize", (BindingFlags)(-1));
			var ___ContentSize = (Vector2)GetTotalSizeMethodInfo.Invoke(plugin.TreeController_current, null);
			if (!lastContentSize.HasValue) lastContentSize = ___ContentSize.y;
			if (___ContentSize.y != lastContentSize)
			{   // adapter.lastmPos = ((Rect)adapter.m_Pos.GetValue( w )).height;
				//plugin.HeightFixIfDrag();
				lastContentSize = ___ContentSize.y;
				plugin.invoke_onHierarchyChanged(true);
			}
		}



		public class ReflType
		{
			public bool isprop;
			public FieldInfo field;
			public PropertyInfo prop;

			public ReflType(FieldInfo f, PropertyInfo p)
			{
				this.field = f;
				this.prop = p;
				this.isprop = p != null;
			}

			public ReflType(Type type, string key)
			{
				this.field = type.GetField(key, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
				if (this.field == null) this.prop = type.GetProperty(key, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
				this.isprop = this.prop != null;
				if (this.prop == null && this.field == null) throw new Exception(key);
			}

			public object GetValue(object target)
			{
				return isprop ? prop.GetValue(target, null) : field.GetValue(target);
			}

			public void SetValue(object target, object args)
			{
				if (isprop) prop.SetValue(target, args, null);
				else field.SetValue(target, args);
			}
		}
		internal static ReflType k_LineHeight, k_IndentWidth, k_BaseIndent, k_IconWidth;
		static FieldInfo foldoutYOffset, m_UseHorizontalScroll, m_Ping, m_PingStyle;
		internal static void InitFields()
		{
			var GameObjectTreeViewGUI = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameObjectTreeViewGUI") ?? throw new Exception("GameObjectTreeViewGUI");
			var treeViewBaseType = GameObjectTreeViewGUI.BaseType;
			k_LineHeight = new ReflType(treeViewBaseType, "k_LineHeight");
			k_IndentWidth = new ReflType(treeViewBaseType, "k_IndentWidth");
			k_BaseIndent = new ReflType(treeViewBaseType, "k_BaseIndent");
			k_IconWidth = new ReflType(treeViewBaseType, "k_IconWidth");
			foldoutYOffset = treeViewBaseType.GetField("foldoutYOffset", ~(BindingFlags.InvokeMethod | BindingFlags.Static));
			if (foldoutYOffset == null) foldoutYOffset = treeViewBaseType.GetField("customFoldoutYOffset", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("foldoutYOffset");
			m_UseHorizontalScroll = treeViewBaseType.GetField("m_UseHorizontalScroll", (BindingFlags)(-1)) ?? throw new Exception("m_UseHorizontalScroll");
			m_Ping = treeViewBaseType.GetField("m_Ping", (BindingFlags)(-1)) ?? throw new Exception("m_Ping");
			m_PingStyle = m_Ping.FieldType.GetField("m_PingStyle", (BindingFlags)(-1)) ?? throw new Exception("m_PingStyle");

		}

		static bool HAS_SES<T>(string key)
		{
			if (typeof(T) == typeof(int)) return SessionState.GetInt(key, -999) != -999;
			else if (typeof(T) == typeof(float)) return SessionState.GetFloat(key, -999) != -999;
			throw new Exception("HAS_SES");
		}

		string PREFIX { get { return Folders.PREFS_PATH + "_" + pluginID + "_"; } }
		static MethodInfo dataInitMinimal = null;

		internal void SET_LINE_HEIGHT(PluginInstance p, object gui_currentTree)
		{

			if (p.WIN_SET.USE_LINE_HEIGHT)
			{

				bool init = false;
				if (!HAS_SES<int>(PREFIX + "k_LineHeight"))
				{
					SessionState.SetInt(PREFIX + "k_LineHeight", (int)(float)k_LineHeight.GetValue(gui_currentTree));
					SessionState.SetInt(PREFIX + "foldoutYOffset", (int)(float)foldoutYOffset.GetValue(gui_currentTree));
					init = true;
				}





				var H = p.WIN_SET.LINE_HEIGHT;
				if ((int)(float)k_LineHeight.GetValue(gui_currentTree) != H)
				{
					k_LineHeight.SetValue(gui_currentTree, H);
					if (dataInitMinimal == null) dataInitMinimal = p.data_currentTree.GetType().GetMethod("InitializeMinimal", (System.Reflection.BindingFlags)(-1));
					try
					{
						dataInitMinimal.Invoke(p.data_currentTree, null);
					}
					catch { }

				}


				//foldoutYOffset.SetValue( gui_currentTree,(float) Mathf.RoundToInt( (H - EditorGUIUtility.singleLineHeight) / 2 ) );

				foreach (var item in p.ha.INTERNAL_LABEL_STYLES)
				{
					// item.fixedHeight = H;
					item.SetHeight(H);
				}

				var ping = m_Ping.GetValue(gui_currentTree);
				var style = m_PingStyle.GetValue(ping) as GUIStyle;
				if (style != null)
				{
					if (init)
					{
						SessionState.SetFloat(PREFIX + "style.fixedHeight", style.fixedHeight);
						SessionState.SetBool(PREFIX + "style.stretchHeight", style.stretchHeight);
					}
					style.fixedHeight = H;
					style.stretchHeight = true;
					//fixedHeight.SetValue(style, H, null);
				}
			}

		}
		internal void RESET_LINE_HEIGHT(PluginInstance p, EditorWindow w)
		{
			if (HAS_SES<int>(PREFIX + "k_LineHeight"))
			{
				SessionState.EraseInt(PREFIX + "k_LineHeight");
				if (!w) return;
				var TreeController_current = p.GetTreeViewontroller(pluginID, w);
				var gui_currentTree = p._gui.GetValue(TreeController_current);
				k_LineHeight.SetValue(gui_currentTree, (float)SessionState.GetInt(PREFIX + "k_LineHeight", 16));
				foldoutYOffset.SetValue(gui_currentTree, (float)SessionState.GetInt(PREFIX + "foldoutYOffset", 0));
				var ping = m_Ping.GetValue(gui_currentTree);
				var style = m_PingStyle.GetValue(ping) as GUIStyle;
				if (style != null)
				{
					style.fixedHeight = SessionState.GetFloat(PREFIX + "style.fixedHeight", 16);
					style.stretchHeight = SessionState.GetBool(PREFIX + "style.stretchHeight", false);

					//fixedHeight.SetValue(style, H, null);
				}

				foreach (var item in p.ha.INTERNAL_LABEL_STYLES)
				{
					item.SetHeight(16);
				}
			}
		}
		void SET_CHILD_INDENT(PluginInstance p, object gui_currentTree)
		{

			if (p.WIN_SET.USE_CHILD_INDENT)
			{
				//if (gui_currentTree == null) throw new Exception("gui_currentTree = null");
				if (!HAS_SES<int>(PREFIX + "k_IndentWidth"))
				{
					SessionState.SetInt(PREFIX + "k_IndentWidth", (int)(float)k_IndentWidth.GetValue(gui_currentTree));
					SessionState.SetInt(PREFIX + "k_BaseIndent", (int)(float)k_BaseIndent.GetValue(gui_currentTree));
				}
				var addIndent = Mathf.RoundToInt(14 - p.WIN_SET.CHILD_INDENT);
				k_IndentWidth.SetValue(gui_currentTree, (float)p.WIN_SET.CHILD_INDENT);
				if (p.ha.IS_SEARCH_MODE_OR_PREFAB_OPENED()) k_BaseIndent.SetValue(gui_currentTree,/* defaultextraInsertionMarkerIndent +*/ (float)p.ha.LEFT_PADDING);
				else k_BaseIndent.SetValue(gui_currentTree,/* defaultextraInsertionMarkerIndent +*/ (float)addIndent + p.ha.LEFT_PADDING);

			}

		}
		internal void RESET_CHILD_INDENT(PluginInstance p, EditorWindow w)
		{
			if (HAS_SES<int>(PREFIX + "k_IndentWidth"))
			{
				SessionState.EraseInt(PREFIX + "k_IndentWidth");
				if (!w) return;
				var TreeController_current = p.GetTreeViewontroller(pluginID, w);
				var gui_currentTree = p._gui.GetValue(TreeController_current);
				k_IndentWidth.SetValue(gui_currentTree, (float)SessionState.GetInt(PREFIX + "k_IndentWidth", 14));
				k_BaseIndent.SetValue(gui_currentTree, (float)SessionState.GetInt(PREFIX + "k_BaseIndent", 0));
			}
		}
		void SET_DEFAULT_ICON_SIZE(PluginInstance p, object gui_currentTree)
		{
			if (p.WIN_SET.USE_OVERRIDE_DEFAULT_ICONS_SIZE)
			{
				//if (!p.window.Instance) return;
				//var TreeController_current = p.GetTreeViewontroller(p.window.Instance);
				//	var gui_currentTree = p._gui.GetValue(TreeController_current);
				if (!HAS_SES<int>(PREFIX + "k_IconWidth"))
				{
					SessionState.SetInt(PREFIX + "k_IconWidth", (int)(float)k_IconWidth.GetValue(gui_currentTree));
				}
				k_IconWidth.SetValue(gui_currentTree, (float)p.WIN_SET.OVERRIDE_DEFAULT_ICONS_SIZE);
			}

		}
		internal static int DefaultIconSize(PluginInstance p) { return (int)(float)k_IconWidth.GetValue(p.gui_currentTree); }
		internal void RESET_DEFAULT_ICON_SIZE(PluginInstance p, EditorWindow w)
		{
			if (HAS_SES<int>(PREFIX + "k_IconWidth"))
			{
				SessionState.EraseInt(PREFIX + "k_IconWidth");
				if (!w) return;
				var TreeController_current = p.GetTreeViewontroller(pluginID, w);
				var gui_currentTree = p._gui.GetValue(TreeController_current);
				k_IconWidth.SetValue(gui_currentTree, (float)SessionState.GetInt(PREFIX + "k_IconWidth", (int)16));
			}
		}


		internal class intCache
		{

			bool[] cache_b = new bool[100];
			int?[] cache = new int?[100];

			public int? this[int index]
			{
				get
				{
					if (cache_b[index]) return cache[index];
					cache_b[index] = true;
					var b = SessionState.GetInt("EMX|FontsSizeBackup|" + index, -1);
					if (b == -1) cache[index] = null;
					else cache[index] = b;
					return cache[index];
				}
				set
				{
					if (cache_b[index] && cache[index] == value) return;
					cache_b[index] = true;
					cache[index] = value;
					SessionState.SetInt("EMX|FontsSizeBackup|" + index, value.Value);
				}
			}

		}
		internal static intCache fonts = new intCache();
		//static intCache?[] fonts = new intCache?[0];
		void SET_GAMEOBJECTS_NAMES(PluginInstance p)
		{
			if (p.WIN_SET.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE)
			{
				//if (fonts.Length != p.ha.INTERNAL_LABEL_STYLES.Count) fonts = new int?[p.ha.INTERNAL_LABEL_STYLES.Count];
				//Array.Resize( ref fonts, p.ha.INTERNAL_LABEL_STYLES.Count );
				for (int i = 0; i < p.ha.INTERNAL_LABEL_STYLES.Count; i++)
				{
					if (!fonts[i].HasValue) fonts[i] = p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize;
					p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize = p.WIN_SET.OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE;
					if (i == 0) p.ha.currentDefaultFontSize = p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize;
				}
			}
			else if (p.WIN_SET_INVERSE.USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE)
			{
				//if (fonts.Length != p.ha.INTERNAL_LABEL_STYLES.Count) fonts = new int?[p.ha.INTERNAL_LABEL_STYLES.Count];
				for (int i = 0; i < p.ha.INTERNAL_LABEL_STYLES.Count; i++)
				{
					if (!fonts[i].HasValue) fonts[i] = p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize;
					p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize = fonts[i].Value;
					if (i == 0) p.ha.currentDefaultFontSize = p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize;
				}
			}
		}
		internal void RESET_GAMEOBJECTS_NAMES(PluginInstance p, EditorWindow w)
		{
			//if (fonts.Length == p.ha.INTERNAL_LABEL_STYLES.Count)
			{
				for (int i = 0; i < p.ha.INTERNAL_LABEL_STYLES.Count; i++)
				{
					if (fonts[i].HasValue) p.ha.INTERNAL_LABEL_STYLES[i].style.fontSize = fonts[i].Value;
				}
			}
		}
		internal void SetHeightAndIndents(PluginInstance p)
		{

			if (!p.window.Instance) return;
			var TreeController_current = p.GetTreeViewontroller(pluginID, p.window.Instance);
			var gui_currentTree = p._gui.GetValue(TreeController_current);


			if ((bool)m_UseHorizontalScroll.GetValue(gui_currentTree) != p.WIN_SET.USE_HORISONTAL_SCROLL)
				m_UseHorizontalScroll.SetValue(gui_currentTree, p.WIN_SET.USE_HORISONTAL_SCROLL);

			SET_LINE_HEIGHT(p, gui_currentTree);
			SET_CHILD_INDENT(p, gui_currentTree);
			SetHeightAndIndents_Again(p);
			SET_DEFAULT_ICON_SIZE(p, gui_currentTree);

		}

		internal void SetHeightAndIndents_Again(PluginInstance p)
		{
			SET_GAMEOBJECTS_NAMES(p);
		}






		internal Dictionary<int, Action> rightModIndexAndOnMouseUp = new Dictionary<int, Action>();

		internal void CheckMouseEvents()
		{

			/*
            foreach ( var item in externalMods )
            {
                if ( item.Value.Alive() && item.Value.currentAction != null )
                {
                    if ( !item.Value.currentAction( false, deltaTime ) )
                        item.Value.RepaintNow();
                }
            }*/
			if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
			{
				var repaint = false;
				if (mouseEvent != null)
				{
					mouseEvent(false, p.deltaTime);
					repaint = true;
				}
				if (mouseEventDrag != null)
				{
					mouseEventDrag(false, p.deltaTime);
					repaint = true;
				}
				if (rightModIndexAndOnMouseUp.Count != 0)
				{
					repaint = true;
				}
				if (Event.current.rawType == EventType.MouseUp /*|| Event.current.keyCode == KeyCode.Escape*/)
				{
					EVENT_HELPER_ONUP();
				}
				if (repaint)
				{
					p.RepaintWindowInUpdate(pluginID);
				}
			}



			if (mouseEvent != null || mouseEventDrag != null || rightModIndexAndOnMouseUp.Count != 0)
			{
				p.ha.DisableHover();

			}
		}


		internal Rect DragRect;

		Action<bool, float> __mouseEvent;
		internal Action<bool, float> mouseEvent
		{
			get { return __mouseEvent; }
			set
			{
				__mouseEvent = value;
				if (value != null) p.PUSH_ONMOUSEUP(pluginID, EVENT_HELPER_ONUP);
			}
		}

		Action<bool, float> __mouseEventDrag;
		internal RightModBaseClass captured_mod;
		internal bool allow;
		internal int captured_mod_arr;

		internal Action<bool, float> mouseEventDrag
		{
			get { return __mouseEventDrag; }
			set
			{
				__mouseEventDrag = value;
				if (value != null) p.PUSH_ONMOUSEUP(pluginID, EVENT_HELPER_ONUP);
			}
		}

		internal void EVENT_HELPER_ONUP()
		{
			var repaint = false;

			if (mouseEvent != null)
			{
				mouseEvent(true, p.deltaTime);
				mouseEvent = null;
				// GUIUtility.hotControl = 0;
				Tools.EventUse();
				repaint = true;
			}
			if (mouseEventDrag != null)
			{
				mouseEventDrag(true, p.deltaTime);
				mouseEventDrag = null;
				//GUIUtility.hotControl = 0;
				Tools.EventUse();
				repaint = true;
			}
			if (rightModIndexAndOnMouseUp.Count != 0)
			{
				foreach (var rightModOnUp in rightModIndexAndOnMouseUp.Values.ToArray())
					rightModOnUp();
				rightModIndexAndOnMouseUp.Clear();
				repaint = true;
			}
			if (repaint)
			{
				p.RepaintWindowInUpdate(pluginID);
			}
		}

	}
}
