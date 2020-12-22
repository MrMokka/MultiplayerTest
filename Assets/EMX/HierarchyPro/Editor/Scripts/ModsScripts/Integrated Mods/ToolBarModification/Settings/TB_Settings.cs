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



		//CUSTOM
		internal bool DRAW_TOPBAR_CUSTOM_LEFT { get { return GET("DRAW_TOPBAR_CUSTOM_LEFT", true); } set { SET("DRAW_TOPBAR_CUSTOM_LEFT", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
		internal bool DRAW_TOPBAR_CUSTOM_RIGHT { get { return GET("DRAW_TOPBAR_CUSTOM_RIGHT", true); } set { SET("DRAW_TOPBAR_CUSTOM_RIGHT", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }

		//LAYOUTS
		internal bool DRAW_TOPBAR_LAYOUTS_BAR { get { return GET("DRAW_TOPBAR_LAYOUTS_BAR", true); } set { SET("DRAW_TOPBAR_LAYOUTS_BAR", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
		internal bool TOPBAR_LAYOUTS_DRAWCROSS { get { return GET("TOPBAR_LAYOUTS_DRAWCROSS", true); } set { SET("TOPBAR_LAYOUTS_DRAWCROSS", value); p.RepaintAllViews(); } }
		internal bool TOPBAR_LAYOUTS_AUTOSAVE { get { return GET("TOPBAR_LAYOUTS_AUTOSAVE", false); } set { SET("TOPBAR_LAYOUTS_AUTOSAVE", value); p.RepaintAllViews(); } }
		internal bool TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM { get { return GET("TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM", true); } set { SET("TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM", value); p.RepaintAllViews(); } }

		//HOT BUTTONS
		internal bool DRAW_TOPBAR_HOTBUTTONS { get { return GET("DRAW_TOPBAR_HOTBUTTONS", true); } set { SET("DRAW_TOPBAR_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
		internal int TOPBAR_HOTBUTTON_SIZE { get { return GET("TOP_EXTBUTTON_SIZE", 20); } set { SET("TOP_EXTBUTTON_SIZE", value); p.RepaintAllViews(); } }
		internal bool DRAW_HEADER_HOTBUTTONS { get { return GET("DRAW_HEADER_HOTBUTTONS", false); } set { SET("DRAW_HEADER_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
		internal int HEADER_HOTBUTTON_SEZE { get { return GET("HEADER_EXTBUTTON_SEZE", 12); } set { SET("HEADER_EXTBUTTON_SEZE", value); p.RepaintAllViews(); } }
		internal bool DRAW_RIGHTCLIK_MENU_HOTBUTTONS { get { return GET("DRAW_RIGHTCLIK_MENU_HOTBUTTONS", true); } set { SET("DRAW_RIGHTCLIK_MENU_HOTBUTTONS", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }



		internal bool TOPBAR_SWAP_LEFT_RIGHT { get { return GET("TOPBAR_SWAP_LEFT_RIGHT", false); } set { SET("TOPBAR_SWAP_LEFT_RIGHT", value); p.modsController.REBUILD_PLUGINS(); p.RepaintAllViews(); } }
		internal int TOPBAR_LEFT_MIN_BORDER_OFFSET { get { return GET("LEFT_MIN_BORDER_OFFSET", 0); } set { SET("LEFT_MIN_BORDER_OFFSET", value); p.RepaintAllViews(); } }
		internal int TOPBAR_LEFT_MAX_BORDER_OFFSET { get { return GET("LEFT_MAX_BORDER_OFFSET", 0); } set { SET("LEFT_MAX_BORDER_OFFSET", value); p.RepaintAllViews(); } }
		internal int TOPBAR_RIGHT_MIN_BORDER_OFFSET { get { return GET("RIGHT_MIN_BORDER_OFFSET", 0); } set { SET("RIGHT_MIN_BORDER_OFFSET", value); p.RepaintAllViews(); } }
		internal int TOPBAR_RIGHT_MAX_BORDER_OFFSET
		{
			get
			{
				return GET("RIGHT_MAX_BORDER_OFFSET",
#if UNITY_2020_1_OR_NEWER
			-130
#else
			0
#endif
			);
			}
			set { SET("RIGHT_MAX_BORDER_OFFSET", value); p.RepaintAllViews(); }
		}

	}
}
