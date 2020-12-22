using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System.Globalization;

namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{

		int pluginID;
		//  Saver s ;
		internal EditorSettingsAdapter(int pId)
		{
			pluginID = pId;
		}

	
// FS

///  #########################################################################################################################################################################################


WindowSettings _HIER_WIN_SET;
		internal WindowSettings HIER_WIN_SET { get { return _HIER_WIN_SET ?? (_HIER_WIN_SET = new WindowSettings("WIN_HIERARCHY", 0)); } }
		WindowSettings _PROJ_WIN_SET;
		internal WindowSettings PROJ_WIN_SET { get { return _PROJ_WIN_SET ?? (_PROJ_WIN_SET = new WindowSettings("WIN_PROJECT", 1)); } }



		internal bool SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER { get { return false; } }





		internal partial class WindowSettings
		{
			internal string KEY = "";
			int pluginID;

			internal WindowSettings(string key, int pluginID)
			{
				KEY = key;
				this.pluginID = pluginID;
			}

			PluginInstance p { get { return Root.p[0]; } }
			EditorSettingsAdapter s { get { return Root.p[0].par_e; } }




			internal bool USE_LINE_HEIGHT
			{
				get { return false; }
				set
				{
					if (s.SET(KEY + "_USE_LINE_HEIGHT", value))
					{
						if (!USE_LINE_HEIGHT) foreach (var item in PluginInstance.WindowsData(pluginID)) if (item.Value.w.Instance) item.Value.w.RESET_LINE_HEIGHT(p, item.Value.w.Instance);
						p.RepaintWindowInUpdate_PlusResetStack(pluginID);
					}
				}
			}
			internal int LINE_HEIGHT { get { return Mathf.Clamp(s.GET(KEY + "_LINE_HEIGHT", 16), 10, 80); } set { if (s.SET(KEY + "_LINE_HEIGHT", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID); } }

			internal bool USE_CHILD_INDENT
			{
				get { return false; }
				set
				{
					if (s.SET(KEY + "_USE_CHILD_INDENT", value))
					{
						if (!USE_CHILD_INDENT) foreach (var item in PluginInstance.WindowsData(pluginID)) if (item.Value.w.Instance) item.Value.w.RESET_CHILD_INDENT(p, item.Value.w.Instance);
						p.RepaintWindowInUpdate_PlusResetStack(pluginID);
					}
				}
			}
			internal int CHILD_INDENT { get { return s.GET(KEY + "_CHILD_INDENT", 14); } set { if (s.SET(KEY + "_CHILD_INDENT", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID); } }

			internal bool USE_OVERRIDE_DEFAULT_ICONS_SIZE
			{
				get { return false; }
				set
				{
					if (s.SET(KEY + "_USE_OVERRIDE_DEFAULT_ICONS_SIZE", value))
					{
						if (!USE_OVERRIDE_DEFAULT_ICONS_SIZE) foreach (var item in PluginInstance.WindowsData(pluginID)) if (item.Value.w.Instance) item.Value.w.RESET_DEFAULT_ICON_SIZE(p, item.Value.w.Instance);
						p.RepaintWindowInUpdate_PlusResetStack(pluginID);
					}
				}
			}
			internal int OVERRIDE_DEFAULT_ICONS_SIZE { get { return s.GET(KEY + "_OVERRIDE_DEFAULT_ICONS_SIZE", (int)16); } set { if (s.SET(KEY + "_OVERRIDE_DEFAULT_ICONS_SIZE", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID); } }

			internal bool USE_HORISONTAL_SCROLL { get { return s.GET(KEY + "_USE_HORIZONTAL_SCROLL", false); } set { if (s.SET(KEY + "_USE_HORIZONTAL_SCROLL", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID); } }
			// For Right Modules
			internal int PLUGIN_LABELS_FONT_SIZE
			{
				get { return s.GET(KEY + "_PLUGIN_LABELS_FONT_SIZE", 10); }
				set
				{
					if (s.SET(KEY + "_PLUGIN_LABELS_FONT_SIZE", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID);
				}
			}
			internal bool USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE
			{
				get { return false; }
				set
				{
					if (s.SET(KEY + "_USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", value))
					{
						if (!USE_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE) foreach (var item in PluginInstance.WindowsData(pluginID)) if (item.Value.w.Instance) item.Value.w.RESET_GAMEOBJECTS_NAMES(p, item.Value.w.Instance);
						NewClearHelper.OnFontSizeChanged();
						p.RepaintWindowInUpdate_PlusResetStack(pluginID);
					}
				}
			}
			internal int OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE
			{
				get { return s.GET(KEY + "_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", EditorStyles.label.fontSize == 0 ? 12 : EditorStyles.label.fontSize); }
				set
				{
					if (s.SET(KEY + "_OVERRIDE_FOR_GAMEOBJECTS_NAMES_LABELS_FONT_SIZE", value)) p.RepaintWindowInUpdate_PlusResetStack(pluginID);
					NewClearHelper.OnFontSizeChanged();
				}
			}





			internal bool USE_BACKGROUNDDECORATIONS_MOD
			{
				get
				{
					if (pluginID == 1 && !p.par_e.USE_PROJECT_SETTINGS) return false;
					return HardDisableMods.USE_BACKGROUNDDECORATIONS_MOD ?? s.GET(KEY + "_USE_BACKGROUNDDECORATIONS_MOD", true);
				}
				set
				{
					if (pluginID == 1 && !p.par_e.USE_PROJECT_SETTINGS) return;
					if (s.SET(KEY + "_USE_BACKGROUNDDECORATIONS_MOD", value)) p.modsController.REBUILD_PLUGINS();
				}
			}


			internal int DRAW_HIERARHCHY_CHESS_LINES { get { return s.GET(KEY + "_DRAW_HIERARHCHY_CHESS_LINES", 0); } set { if (s.SET(KEY + "_DRAW_HIERARHCHY_CHESS_LINES", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal Color COLOR_HIERARHCHY_CHESS_LINES { get { return s.GET(KEY + "_COLOR_HIERARHCHY_CHESS_LINES", EditorGUIUtility.isProSkin ? new Color(.1f, .1f, .1f, 0.1F) : new Color(.1f, .1f, .1f, 0.1F / 3)); } set { if (s.SET(KEY + "_COLOR_HIERARHCHY_CHESS_LINES", value)) p.RepaintWindowInUpdate(pluginID); } }


			internal int DRAW_HIERARHCHY_CHESS_FILLS { get { return s.GET(KEY + "_DRAW_HIERARHCHY_CHESS_FILLS", pluginID != 0 ? 2 : (EditorGUIUtility.isProSkin ? 2 : 1)); } set { if (s.SET(KEY + "_DRAW_HIERARHCHY_CHESS_FILLS", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal Color COLOR_HIERARHCHY_CHESS_FILLS { get { return s.GET(KEY + "_COLOR_HIERARHCHY_CHESS_FILLS", EditorGUIUtility.isProSkin ? new Color(20 / 255f, 20 / 255f, 20 / 255f, 0.1F) : new Color(1f, 1f, 1f, 0.1F / 3)); } set { if (s.SET(KEY + "_COLOR_HIERARHCHY_CHESS_FILLS", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal int DRAW_HIERARHCHY_CHILD_LINES { get { return s.GET(KEY + "_DRAW_HIERARHCHY_CHILD_LINES", pluginID == 0 ? 1 : 0); } set { if (s.SET(KEY + "_DRAW_HIERARHCHY_CHILD_LINES", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal Color COLOR_HIERARHCHY_CHILD_LINES { get { return s.GET(KEY + "_COLOR_HIERARHCHY_CHILD_LINES", EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 0.8f, 0.35f) : new Color(.5f, .5f, .5f, 100 / 255F)); } set { if (s.SET(KEY + "_COLOR_HIERARHCHY_CHILD_LINES", value)) p.RepaintWindowInUpdate(pluginID); } }

			internal int DRAW_CHILDS_COUNT { get { return s.GET(KEY + "_DRAW_CHILDS_COUNT", 1); } set { if (s.SET(KEY + "_DRAW_CHILDS_COUNT", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal bool DRAW_CHILDS_INVERCE_COLOR { get { return s.GET(KEY + "_DRAW_CHILDS_INVERCE_COLOR", !EditorGUIUtility.isProSkin); } set { if (s.SET(KEY + "_DRAW_CHILDS_INVERCE_COLOR", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal Color DRAW_CHILDS_COLOR { get { return s.GET(KEY + "_DRAW_CHILDS_COLOR", EditorGUIUtility.isProSkin ? new Color(0.45f, 0.45f, 0.45f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f)); } set { if (s.SET(KEY + "_DRAW_CHILDS_COLOR", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal bool HIDE_CHILDCOUNT_IFROOT { get { return s.GET(KEY + "_HIDE_CHILDCOUNT_IFROOT", true); } set { if (s.SET(KEY + "_HIDE_CHILDCOUNT_IFROOT", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal bool HIDE_CHILDCOUNT_IFEXPANDED { get { return s.GET(KEY + "_HIDE_CHILDCOUNT_IFEXPANDED", true); } set { if (s.SET(KEY + "_HIDE_CHILDCOUNT_IFEXPANDED", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal int CHILDCOUNT_ALIGMENT { get { return s.GET(KEY + "_CHILDCOUNT_ALIGMENT", 1); } set { if (s.SET(KEY + "_CHILDCOUNT_ALIGMENT", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal int CHILDCOUNT_OFFSET_X { get { return s.GET(KEY + "_CHILDCOUNT_OFFSET_X", 0); } set { if (s.SET(KEY + "_CHILDCOUNT_OFFSET_X", value)) p.RepaintWindowInUpdate(pluginID); } }
			internal int CHILDCOUNT_OFFSET_Y { get { return s.GET(KEY + "_CHILDCOUNT_OFFSET_Y", 0); } set { if (s.SET(KEY + "_CHILDCOUNT_OFFSET_Y", value)) p.RepaintWindowInUpdate(pluginID); } }

			internal bool HIDE_HOVER_BG
			{
				get
				{
					return pluginID == 0 ? s.GET(KEY + "_HIDE_HOVER_BG", false) : false;
				}
				set { if (pluginID == 0) s.SET(KEY + "_HIDE_HOVER_BG", value); }
			}

		}



		///  #########################################################################################################################################################################################
		internal bool USE_DINAMIC_BATCHING { get { return GET("USE_DINAMIC_BATCHING", true); } set { SET("USE_DINAMIC_BATCHING", value); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_SWAP_FOR_BUTTONS_ACTION { get { return GET("USE_SWAP_FOR_BUTTONS_ACTION", false); } set { SET("USE_SWAP_FOR_BUTTONS_ACTION", value); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_HOVER_FOR_BUTTONS { get { return GET("USE_HOVER_FOR_BUTTONS", false); } set { SET("USE_HOVER_FOR_BUTTONS", value); p.__STYLE_DEFBUTTON = null; p.__button = null; } }
		internal bool USE_WHOLE_FUN_UNITY_FONT_SIZE
		{
			get { return false; }
			set
			{
				if (SET("USE_WHOLE_FUN_UNITY_FONT_SIZE", value)) { p.modsController.REBUILD_PLUGINS(); }
				NewClearHelper.OnFontSizeChanged();
			}
		}
		internal int WHOLE_FUN_UNITY_FONT_SIZE
		{
			get { return GET("WHOLE_FUN_UNITY_FONT_SIZE", 12); }
			set
			{
				if (SET("WHOLE_FUN_UNITY_FONT_SIZE", value)) { p.modsController.REBUILD_PLUGINS(); }
				NewClearHelper.OnFontSizeChanged();
			}
		}
		internal bool RIGHTARROW_EXPANDS_HOVERED { get { return GET("RIGHTARROW_EXPANDS_HOVERED", true); } set { SET("RIGHTARROW_EXPANDS_HOVERED", value); } }
		internal bool USE_EXPANSION_ANIMATION { get { return GET("USE_EXPANSION_ANIMATION", false); } set { SET("USE_EXPANSION_ANIMATION", value); } }
		internal bool ESCAPE_CLOSES_PREFABMODE { get { return GET("ESCAPE_CLOSES_PREFABMODE", true); } set { SET("ESCAPE_CLOSES_PREFABMODE", value); } }
		internal bool CLOSE_PREFAB_KEY_FORALL_WINDOWS { get { return GET("CLOSE_PREFAB_KEY_FORALL_WINDOWS", false); } set { SET("CLOSE_PREFAB_KEY_FORALL_WINDOWS", value); } }
		internal bool CLOSE_PREFAB_KEY_FORHIER_ANDSCENE { get { return !CLOSE_PREFAB_KEY_FORALL_WINDOWS; } set { CLOSE_PREFAB_KEY_FORALL_WINDOWS = !value; } }
		//  internal bool DISABLE_DRAWING_ANIMATING_ITEMS { get { return GET( "DISABLE_DRAWING_ANIMATING_ITEMS", true ); } set { SET( "DISABLE_DRAWING_ANIMATING_ITEMS", value ); } }
		internal bool DOUBLECLICK_TO_EXPAND { get { return GET("DOUBLECLICK_TO_EXPAND", false); } set { SET("DOUBLECLICK_TO_EXPAND", value); } }
		internal bool SELECTION_MOVETOGETHER_UPDOWNARROWS { get { return GET("SELECTION_MOVETOGETHER_UPDOWNARROWS", true); } set { SET("SELECTION_MOVETOGETHER_UPDOWNARROWS", value); } }
		///  #########################################################################################################################################################################################
		internal bool ONDOWN_ACTION_INSTEAD_ONUP { get { return GET("ONDOWN_ACTION_INSTEAD_ONUP", false); } set { if (SET("ONDOWN_ACTION_INSTEAD_ONUP", value)) p.RepaintWindowInUpdate(0); } }
		internal bool ENABLE_CUSTOMWINDOWS_OPENANIMATION { get { return GET("ENABLE_CUSTOMWINDOWS_OPENANIMATION", true); } set { SET("ENABLE_CUSTOMWINDOWS_OPENANIMATION", value); } }
		internal bool ENABLE_OBJECTS_PING { get { return GET("ENABLE_OBJECTS_PING", true); } set { SET("ENABLE_OBJECTS_PING", value); } }
		internal bool TRACKING_COMPILE_TIME { get { return GET("TRACKING_COMPILE_TIME", false); } set { SET("TRACKING_COMPILE_TIME", value); } }
		///  #########################################################################################################################################################################################








		///  #########################################################################################################################################################################################




		// internal bool PLUGIN_FONT_AFFECT_HIERARCHYWINDOW { get { return GET( "PLUGIN_FONT_AFFECT_HIERARCHYWINDOW", false ); } set { SET( "PLUGIN_FONT_AFFECT_HIERARCHYWINDOW", value ); } }
		///  #########################################################################################################################################################################################












		// class Saver
		// {
		//  int pluginID;
		//  internal Saver( int pId )
		// {
		//      pluginID = pId;
		//  }
		char[] trim = new[] { '\n', '\r' };
		Dictionary<string, object> cache = new Dictionary<string, object>();
		string d { get { return Folders.PluginExternalFolder + "/Editor/_SAVED_DATA/.EditorSettings/"; } }
		internal string GET(string k, string def)
		{
			if (cache.ContainsKey(k)) return (string)cache[k];
			var f = d + pluginID + "-" + k;
			string res = def;
			if (File.Exists(f))
				res = File.ReadAllText(f).Trim(trim);
			else
				if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, res.ToString());
			cache.Add(k, res);
			return res;
		}
		internal void SET(string k, string val)
		{
			if (cache.ContainsKey(k) && (string)cache[k] == val) return;
			if (!cache.ContainsKey(k)) cache.Add(k, val);
			else cache[k] = val;
			var f = d + pluginID + "-" + k;
			if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, val);
		}
		internal int GET(string k, int def)
		{
			if (cache.ContainsKey(k)) return (int)cache[k];
			var f = d + pluginID + "-" + k;
			int res = def;
			if (File.Exists(f))
				res = int.Parse(File.ReadAllText(f).Trim(trim));
			else
				if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, res.ToString());
			cache.Add(k, res);
			return res;
		}
		internal bool SET(string k, int val)
		{
			if (cache.ContainsKey(k) && (int)cache[k] == val) return false;
			if (!cache.ContainsKey(k)) cache.Add(k, val);
			else cache[k] = val;
			var f = d + pluginID + "-" + k;
			if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, val.ToString());
			return true;
		}
		internal bool GET(string k, bool def)
		{
			if (cache.ContainsKey(k)) return (bool)cache[k];
			var f = d + pluginID + "-" + k;
			bool res = def;
			if (File.Exists(f))
				res = bool.Parse(File.ReadAllText(f).Trim(trim));
			else
				if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, res.ToString());
			cache.Add(k, res);
			return res;
		}
		internal bool SET(string k, bool val)
		{
			if (cache.ContainsKey(k) && (bool)cache[k] == val) return false;
			if (!cache.ContainsKey(k)) cache.Add(k, val);
			else cache[k] = val;
			var f = d + pluginID + "-" + k;
			if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, val.ToString());
			return true;
		}
		internal Color32 GET(string k, Color32 def)
		{
			if (cache.ContainsKey(k)) return (Color32)cache[k];
			var f = d + pluginID + "-" + k;
			Color32 res = def;
			if (File.Exists(f))
			{
				var b = File.ReadAllText(f).Trim(trim).Split(' ').Select(c => (byte)int.Parse(c)).ToArray();
				res = new Color32(b[0], b[1], b[2], b[3]);
			}
			else
				if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, res.r + " " + res.g + " " + res.b + " " + res.a);
			cache.Add(k, res);
			return res;
		}
		bool CE(Color32 a, Color32 b) { return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a; }
		internal bool SET(string k, Color32 val)
		{
			if (cache.ContainsKey(k) && CE((Color32)cache[k], val)) return false;
			if (!cache.ContainsKey(k)) cache.Add(k, val);
			else cache[k] = val;
			var f = d + pluginID + "-" + k;
			if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, val.r + " " + val.g + " " + val.b + " " + val.a);
			return true;
		}
		internal void SET(string k, float val)
		{
			if (cache.ContainsKey(k) && (float)cache[k] == val) return;
			if (!cache.ContainsKey(k)) cache.Add(k, val);
			else cache[k] = val;
			var f = d + pluginID + "-" + k;
			if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, val.ToString());
		}
		internal float GET(string k, float def)
		{
			if (cache.ContainsKey(k)) return (float)cache[k];
			var f = d + pluginID + "-" + k;
			float res = def;

			if (File.Exists(f))
				res = float.Parse(File.ReadAllText(f).Trim(trim).Replace(',', '.'), CultureInfo.InvariantCulture);
			else
				if (!Directory.Exists(d)) Directory.CreateDirectory(d);
			File.WriteAllText(f, res.ToString());
			cache.Add(k, res);
			return res;
		}
		// }
	}
}
