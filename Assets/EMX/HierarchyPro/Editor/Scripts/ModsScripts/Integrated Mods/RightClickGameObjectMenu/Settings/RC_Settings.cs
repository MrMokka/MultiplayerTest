using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{

	partial class EditorSettingsAdapter
	{


		//  internal int AS_SAVE_INTERVAL_IN_MIN { get { return Mathf.Clamp(GET("AS_SAVE_INTERVAL_IN_MIN", 5), 1, 60); } set { SET("AS_SAVE_INTERVAL_IN_MIN", value); p.RESET_DRAWSTACK(); } }
		internal bool RCGO_MENU_PLACE_TO_HIERFOLDER { get { return GET("RCGO_MENU_PLACE_TO_HIERFOLDER", false); } set { SET("RCGO_MENU_PLACE_TO_HIERFOLDER", value); } }
		internal bool RCGO_MENU_USE_HOTKEYS { get { return GET("RCGO_MENU_USE_HOTKEYS", true); } set { SET("RCGO_MENU_USE_HOTKEYS", value); } }
		string RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING { get { return GET("RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING", ""); } set { SET("RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING", value); } }


		internal string[] AllWindows = { "SceneView", "Inspector", "GameView", "SceneHierarchy"/*, "ProjectBrowser"*/};
		Dictionary<string, bool> __CUSTOMMENU_HOTKEYS_WINDOWS;
		internal Dictionary<string, bool> CUSTOMMENU_HOTKEYS_WINDOWS
		{
			get
			{
				if (__CUSTOMMENU_HOTKEYS_WINDOWS == null)
				{
					__CUSTOMMENU_HOTKEYS_WINDOWS = new Dictionary<string, bool>();
					if (RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING == "") RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = AllWindows.Aggregate((a, b) => a + " " + b);
					if (!string.IsNullOrEmpty(RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING))
						foreach (var item in RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING.Split(' ')) if (!__CUSTOMMENU_HOTKEYS_WINDOWS.ContainsKey(item)) __CUSTOMMENU_HOTKEYS_WINDOWS.Add(item, true);
				}
				return __CUSTOMMENU_HOTKEYS_WINDOWS;
			}
			set
			{
				if (value.Keys.Count == 0) RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = "";
				else RCGO_CUSTOMMENU_HOTKEYS_WINDOWS_STRING = value.Keys.Aggregate((a, b) => a + " " + b);
				__CUSTOMMENU_HOTKEYS_WINDOWS = value;
			}
		}


	}
}
