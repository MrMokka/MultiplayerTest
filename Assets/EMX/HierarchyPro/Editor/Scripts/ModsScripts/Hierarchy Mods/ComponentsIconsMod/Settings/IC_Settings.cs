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

        //0-simple mono ; 1-first char of name ; 2-two first chars of name
        internal int COMPONENTS_MONO_ICON_TYPE { get { return GET("COMPONENTS_MONO_ICON_TYPE", 1); } set { SET("COMPONENTS_MONO_ICON_TYPE", value); p.RESET_DRAWSTACK(0); } }

        
        internal bool COMPONENTS_DRAW_ICONS_SHADOW { get { return GET("COMPONENTS_DRAW_ICONS_SHADOW", false); } set { SET("COMPONENTS_DRAW_ICONS_SHADOW", value); p.RESET_DRAWSTACK(0); } }
        internal int COMPONENTS_MONO_SPLIT_MODE { get { return GET("COMPONENTS_MONO_SPLIT_MODE", 0); } set { SET("COMPONENTS_MONO_SPLIT_MODE", value); p.RESET_DRAWSTACK(0); } }
        internal int COMPONENTS_ICONS_SPACE { get { return GET("COMPONENTS_ICONS_SPACE", -1); } set { SET("COMPONENTS_ICONS_SPACE", value); p.RESET_DRAWSTACK(0); } }
        internal int COMPONENTS_ICONS_CAT_SPACE { get { return GET("COMPONENTS_ICONS_CAT_SPACE", 2); } set { SET("COMPONENTS_ICONS_CAT_SPACE", value); p.RESET_DRAWSTACK(0); } }
        internal int COMPONENTS_ICONS_SIZE { get { return GET("COMPONENTS_ICONS_SIZE", 10); } set { SET("COMPONENTS_ICONS_SIZE", value); p.RESET_DRAWSTACK(0); } }

        internal int COMPONENTS_NEXT_TO_NAME_PADDING { get { return GET("COMPONENTS_NEXT_TO_NAME_PADDING", 10); } set { SET("COMPONENTS_NEXT_TO_NAME_PADDING", value); p.RESET_DRAWSTACK(0); } }
        internal int COMPONENTS_MARGIN_TOP { get { return GET("COMPONENTS_MARGIN_TOP", 0); } set { SET("COMPONENTS_MARGIN_TOP", value); p.RESET_DRAWSTACK(0); } }

        internal bool COMPONENTS_DRAW_GLOBALCUSTOM_ICONS { get { return GET("COMPONENTS_DRAW_GLOBALCUSTOM_ICONS", true); } set { SET("COMPONENTS_DRAW_GLOBALCUSTOM_ICONS", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_DRAW_MONO_ICONS { get { return GET("COMPONENTS_DRAW_MONO_ICONS", true); } set { SET("COMPONENTS_DRAW_MONO_ICONS", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_DRAW_DEFAULT_ICONS { get { return GET("COMPONENTS_DRAW_DEFAULT_ICONS", true); } set { SET("COMPONENTS_DRAW_DEFAULT_ICONS", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_DRAW_ICONS_MONO_BG_INVERSE { get { return !COMPONENTS_DRAW_ICONS_MONO_BG; } set { COMPONENTS_DRAW_ICONS_MONO_BG = !value; } }
        internal bool COMPONENTS_DRAW_ICONS_MONO_BG { get { return GET("COMPONENTS_DRAW_ICONS_MONO_BG", true); } set { SET("COMPONENTS_DRAW_ICONS_MONO_BG", value); p.RESET_DRAWSTACK(0); } }

        internal bool COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR { get { return GET("COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR", true); } set { SET("COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS { get { return GET("COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS", false); } set { SET("COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS", value); p.RESET_DRAWSTACK(0); } }


        internal int COMPONENTS_ATTRIBUTES_MARGIN { get { return GET("COMPONENTS_ATTRIBUTES_MARGIN", 6); } set { SET("COMPONENTS_ATTRIBUTES_MARGIN", value); p.RESET_DRAWSTACK(0); } }

        internal bool COMPONENTS_ATTRIBUTES_BUTTONS { get { return GET("COMPONENTS_ATTRIBUTES_BUTTONS", true); } set { SET("COMPONENTS_ATTRIBUTES_BUTTONS", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_ATTRIBUTES_FIELDS { get { return GET("COMPONENTS_ATTRIBUTES_FIELDS", true); } set { SET("COMPONENTS_ATTRIBUTES_FIELDS", value); p.RESET_DRAWSTACK(0); } }
        internal bool COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED { get { return GET("COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED", false); } set { SET("COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED", value); p.RESET_DRAWSTACK(0); } }



        //   internal bool SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER { get { return GET( "SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER", false ); } set { SET( "SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER", value ); } }

    }



}
