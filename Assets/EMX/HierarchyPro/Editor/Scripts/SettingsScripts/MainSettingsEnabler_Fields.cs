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

		PluginInstance p { get { return Root.p[pluginID]; } }


		internal bool ENABLE_ALL { get { return GET("ENABLE_ALL", true); } set { SET("ENABLE_ALL", value); } }

		internal bool USE_TOPBAR_MOD { get { return HardDisableMods.USE_TOPBAR_MOD ?? GET("USE_TOPBAR_MOD", true); } set { if (SET("USE_TOPBAR_MOD", value)) SessionState.SetBool("EXM_TOOLBAR_BOOL", false); p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_FONT_MOD { get { return GET("USE_FONT_MOD", true); } set { if (SET("USE_FONT_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_AUTOSAVE_MOD { get { return HardDisableMods.USE_AUTOSAVE_MOD ?? GET("USE_AUTOSAVE_MOD", false); } set { if (SET("USE_AUTOSAVE_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_SNAP_MOD { get { return HardDisableMods.USE_SNAP_MOD ?? GET("USE_SNAP_MOD", false); } set { if (SET("USE_SNAP_MOD", value)) p.modsController.REBUILD_PLUGINS(); Mods.SnapMod.SET_ENABLE(value && ENABLE_ALL); } }
		internal bool USE_RIGHT_CLICK_MENU_MOD { get { return HardDisableMods.USE_RIGHT_CLICK_MENU_MOD ?? GET("USE_RIGHT_CLICK_MENU_MOD", true); } set { if (SET("USE_RIGHT_CLICK_MENU_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_RIGHT_ALL_MODS { get { return HardDisableMods.USE_RIGHT_ALL_MODS ?? GET("USE_RIGHT_ALL_MODS", true); } set { if (SET("USE_RIGHT_ALL_MODS", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_SETACTIVE_MOD { get { return HardDisableMods.USE_SETACTIVE_MOD ?? GET("USE_SETACTIVE_MOD", true); } set { if (SET("USE_SETACTIVE_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_COMPONENTS_ICONS_MOD { get { return HardDisableMods.USE_COMPONENTS_ICONS_MOD ?? GET("USE_COMPONENTS_ICONS_MOD", true); } set { if (SET("USE_COMPONENTS_ICONS_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_PLAYMODE_SAVER_MOD { get { return HardDisableMods.USE_PLAYMODE_SAVER_MOD ?? GET("USE_PLAYMODE_SAVER_MOD", false); } set { if (SET("USE_PLAYMODE_SAVER_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD { get { return HardDisableMods.USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD ?? GET("USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD", true); } set { if (SET("USE_HIERARCHY_MANUAL_HIGHLIGHTER_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD { get { return HardDisableMods.USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD ?? GET("USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD", true); } set { if (SET("USE_HIERARCHY_AUTO_HIGHLIGHTER_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_CUSTOM_PRESETS_MOD { get { return HardDisableMods.USE_CUSTOM_PRESETS_MOD ?? GET("USE_CUSTOM_PRESETS_MOD", true); } set { if (SET("USE_CUSTOM_PRESETS_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_PROJECT_SETTINGS { get { return HardDisableMods.USE_PROJECT_SETTINGS ?? GET("USE_PROJECT_SETTINGS", true); } set { if (SET("USE_PROJECT_SETTINGS", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_PROJECT_MANUAL_HIGHLIGHTER_MOD
		{
			get { return HardDisableMods.USE_PROJECT_MANUAL_HIGHLIGHTER_MOD ?? GET("USE_PROJECT_MANUAL_HIGHLIGHTER_MOD", false); }
			set
			{
				if (SET("USE_PROJECT_MANUAL_HIGHLIGHTER_MOD", value)) p.modsController.REBUILD_PLUGINS();
				if (!USE_PROJECT_MANUAL_HIGHLIGHTER_MOD && !USE_PROJECT_AUTO_HIGHLIGHTER_MOD) Root.RequestScriptReload();
			}
		}
		internal bool USE_PROJECT_AUTO_HIGHLIGHTER_MOD
		{
			get { return HardDisableMods.USE_PROJECT_AUTO_HIGHLIGHTER_MOD ?? GET("USE_PROJECT_AUTO_HIGHLIGHTER_MOD", false); }
			set
			{
				if (SET("USE_PROJECT_AUTO_HIGHLIGHTER_MOD", value)) p.modsController.REBUILD_PLUGINS();
				if (!USE_PROJECT_MANUAL_HIGHLIGHTER_MOD && !USE_PROJECT_AUTO_HIGHLIGHTER_MOD) Root.RequestScriptReload();
			}
		}
		internal bool USE_PROJECT_GUI_EXTENSIONS { get { return HardDisableMods.USE_PROJECT_GUI_EXTENSIONS ?? GET("USE_PROJECT_GUI_EXTENSIONS", true); } set { if (SET("USE_PROJECT_GUI_EXTENSIONS", value)) p.modsController.REBUILD_PLUGINS(); } }

		internal bool USE_BOOKMARKS_HIERARCHY_MOD { get { return HardDisableMods.USE_BOOKMARKS_HIERARCHY_MOD ?? GET("USE_BOOKMARKS_HIERARCHY_MOD", true); } set { if (SET("USE_BOOKMARKS_HIERARCHY_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_BOOKMARKS_PROJECT_MOD { get { return HardDisableMods.USE_BOOKMARKS_PROJECT_MOD ?? GET("USE_BOOKMARKS_PROJECT_MOD", true); } set { if (SET("USE_BOOKMARKS_PROJECT_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_LAST_SELECTION_MOD { get { return HardDisableMods.USE_LAST_SELECTION_MOD ?? GET("USE_LAST_SELECTION_MOD", true); } set { if (SET("USE_LAST_SELECTION_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_LAST_SCENES_MOD { get { return HardDisableMods.USE_LAST_SCENES_MOD ?? GET("USE_LAST_SCENES_MOD", true); } set { if (SET("USE_LAST_SCENES_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }
		internal bool USE_HIER_EXPANDED_MOD { get { return HardDisableMods.USE_HIER_EXPANDED_MOD ?? GET("USE_HIER_EXPANDED_MOD", true); } set { if (SET("USE_HIER_EXPANDED_MOD", value)) p.modsController.REBUILD_PLUGINS(); } }




	}

}
