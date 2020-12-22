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

        internal float ADDITIONA_INPUT_WINDOWS_WIDTH { get { return GET( "ADDITIONA_INPUT_WINDOWS_WIDTH", 1.2f ); } set { SET( "ADDITIONA_INPUT_WINDOWS_WIDTH", value ); } }
        internal float ADDITIONA_SEARCH_WINDOWS_WIDTH { get { return GET( "ADDITIONA_SEARCH_WINDOWS_WIDTH", 1f ); } set { SET( "ADDITIONA_SEARCH_WINDOWS_WIDTH", value ); } }
        internal bool BIND_SEARCH_TO_LEFT { get { return GET( "BIND_SEARCH_TO_LEFT", true ); } set { SET( "BIND_SEARCH_TO_LEFT", value ); } }
        internal bool SEARCH_USE_ROOT_ONLY { get { return GET( "SEARCH_USE_ROOT_ONLY", false ); } set { SET( "SEARCH_USE_ROOT_ONLY", value ); } }
        internal bool SEARCH_SHOW_DISABLED_OBJECT { get { return GET( "SEARCH_SHOW_DISABLED_OBJECT", false ); } set { SET( "SEARCH_SHOW_DISABLED_OBJECT", value ); } }
        internal bool SEARCH_PIN_WIN_BYDEFAULT { get { return GET( "SEARCH_PIN_WIN_BYDEFAULT", false ); } set { SET( "SEARCH_PIN_WIN_BYDEFAULT", value ); } }
        internal int SEARCH_SORT_MODE_GET( string type ) { return GET( "SEARCH_SORT_MODE_" + type, 0 ); }
        internal void SEARCH_SORT_MODE_SET( string type, int value ) { SET( "SEARCH_SORT_MODE_" + type, value );  }

    }
}
