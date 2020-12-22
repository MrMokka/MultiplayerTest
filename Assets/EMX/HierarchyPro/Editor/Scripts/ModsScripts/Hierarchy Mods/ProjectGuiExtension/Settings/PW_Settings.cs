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

        
      //  internal bool DISPLAY_FILES_EXTENSION { get { return GET( "DISPLAY_FILES_EXTENSION", true ); } set { SET( "DISPLAY_FILES_EXTENSION", value ); p.modsController.REBUILD_PLUGINS(); } }
        internal bool DRAW_EXTENSION_IN_PROJECT { get { return GET("DRAW_EXTENSION_IN_PROJECT", true); } set { SET("DRAW_EXTENSION_IN_PROJECT", value); p.modsController.REBUILD_PLUGINS(); } }

        internal Color DRAW_EXTENSION_COLOR { get { return GET("DRAW_EXTENSION_COLOR", Color.gray); } set { SET("DRAW_EXTENSION_COLOR", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_FONT_SIZE { get { return GET("DRAW_EXTENSION_FONT_SIZE", 7); } set { SET("DRAW_EXTENSION_FONT_SIZE", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_OFFSET_X { get { return GET("DRAW_EXTENSION_OFFSET_X", 0); } set { SET("DRAW_EXTENSION_OFFSET_X", value); p.RepaintAllViews(); } }
        internal int DRAW_EXTENSION_OFFSET_Y { get { return GET("DRAW_EXTENSION_OFFSET_Y", 0); } set { SET("DRAW_EXTENSION_OFFSET_Y", value); p.RepaintAllViews(); } }

    }
}
