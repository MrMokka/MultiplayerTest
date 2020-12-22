
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EMX.HierarchyPlugin.Editor
{
	class MainMenuItems
	{
		// removing chars like %#z or %#y will disable hotkeys
		//---------------------------------------------//
		const string SETTINGS = MENU_PATH + "Settings";
		//---------------------------------------------//
		const string SEL_BACK = MENU_PATH + "Selection Backward &%#z";
		const string SEL_FORW = MENU_PATH + "Selection Forward &%#y";
		//---------------------------------------------//
		const string FRZ_TOGGLE = MENU_PATH + "Lock(Unlock) GameObject &#l";
		const string FRZ_UNLOCKALL = MENU_PATH + "Unlock All &%#l";
		//---------------------------------------------//
		const string EXT_HYPERGRAPH = MENU_PATH + "Open HyperGrapg Mod &%#x";
		const string EXT_BOOKMARKS = MENU_PATH + "Open BookMarks Mod &%#n";
		const string EXT_PROJECTFOLDERS = MENU_PATH + "Open Project Folders Mod &%#m";
		//---------------------------------------------//



		const string MENU_PATH = "Window/" + Root.PN + "/";
		internal const int P = 10000;
		static PluginInstance adapter { get { return Root.p[0]; } }

		[MenuItem(SETTINGS, true, P + 3)]
		static bool OpenSettings_IsValid() { return true; }
		[MenuItem(SETTINGS, false, P + 3)]
		static void OpenSettings() { Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>(); }


		//---------------------------------------------//

		[MenuItem(FRZ_TOGGLE, true, P + 85)]
		public static bool ToggleLock_IsValid() { return Mods.Mod_Freeze.IsValid(); }
		[MenuItem(FRZ_TOGGLE, false, P + 85)]
		public static void ToggleLock() { Mods.Mod_Freeze.ToggleFreeze(); }
		[MenuItem(FRZ_UNLOCKALL, true, P + 89)]
		public static bool UnlockAll_IsValid() { return Mods.Mod_Freeze.IsValid(); }
		[MenuItem(FRZ_UNLOCKALL, false, P + 89)]
		public static void UnlockAll() { Mods.Mod_Freeze.UnclockAll(); }

		//---------------------------------------------//



/*
		[MenuItem(MENU_PATH + "Welcome Screen", false, P + 129)]
		public static void OpenWelcomeScreen() { WelcomeScreen.Init(null); }
		*/






	}
}

