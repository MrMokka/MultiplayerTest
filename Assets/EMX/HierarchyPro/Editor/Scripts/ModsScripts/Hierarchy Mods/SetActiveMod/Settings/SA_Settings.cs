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

        internal bool SET_ACTIVE_SMOOTH_FRAME { get { return GET("SET_ACTIVE_SMOOTH_FRAME", true); } set { SET("SET_ACTIVE_SMOOTH_FRAME", value); p.RESET_DRAWSTACK(0); } }
        //internal bool SET_ACTIVE_ALSO_PADDING_AS_OTHER { get { return GET( "SET_ACTIVE_ALSO_PADDING_AS_OTHER", false ); } set { SET( "SET_ACTIVE_ALSO_PADDING_AS_OTHER", value ); } }
        internal int SET_ACTIVE_POSITION { get { return GET("SET_ACTIVE_POSITION", 0); } set { SET("SET_ACTIVE_POSITION", value); p.RESET_DRAWSTACK(0); } }
        internal bool SET_ACTIVE_SMALL_BOOL { get { return SET_ACTIVE_POSITION == 2; } set { SET("SET_ACTIVE_POSITION", SET_ACTIVE_POSITION = value ? 2 : 0); p.RESET_DRAWSTACK(0); } }
        internal int SET_ACTIVE_STYLE { get { return GET("SET_ACTIVE_STYLE", 0); } set { SET("SET_ACTIVE_STYLE", value); p.RESET_DRAWSTACK(0); } }

        internal bool SET_ACTIVE_PREFAB_BUTTON_OFFSET { get { return GET("SET_ACTIVE_PREFAB_BUTTON_OFFSET", true); } set { SET("SET_ACTIVE_PREFAB_BUTTON_OFFSET", value); p.RESET_DRAWSTACK(0); } }


    }
}
