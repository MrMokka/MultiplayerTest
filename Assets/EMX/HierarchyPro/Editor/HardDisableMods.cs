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

	// Add false or true if you want to disable or enable any module without using the menu.
	class HardDisableMods
	{
		//integrated
		static internal bool? USE_TOPBAR_MOD = null;
		static internal bool? USE_BACKGROUNDDECORATIONS_MOD = null;
		static internal bool? USE_AUTOSAVE_MOD = null;
		static internal bool? USE_SNAP_MOD = null;
		static internal bool? USE_RIGHT_CLICK_MENU_MOD = null;
		//right side
		static internal bool? USE_RIGHT_ALL_MODS = null;
		static internal bool? USE_SETACTIVE_MOD = null;
		static internal bool? USE_COMPONENTS_ICONS_MOD = null;
		static internal bool? USE_PLAYMODE_SAVER_MOD = null;
		//left drop-down window
		static internal bool? USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD = null;
		static internal bool? USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD = null;
		static internal bool? USE_CUSTOM_PRESETS_MOD = null;
		//project
		static internal bool? USE_PROJECT_SETTINGS = null;
		static internal bool? USE_PROJECT_MANUAL_HIGHLIGHTER_MOD = null;
		static internal bool? USE_PROJECT_AUTO_HIGHLIGHTER_MOD = null;
		static internal bool? USE_PROJECT_GUI_EXTENSIONS = null;
		//external //HYPER_GRAPH - always enabled
		static internal bool? USE_BOOKMARKS_HIERARCHY_MOD = null;
		static internal bool? USE_BOOKMARKS_PROJECT_MOD = null;
		static internal bool? USE_LAST_SELECTION_MOD = null;
		static internal bool? USE_LAST_SCENES_MOD = null;
		static internal bool? USE_HIER_EXPANDED_MOD = null;
	}
}
