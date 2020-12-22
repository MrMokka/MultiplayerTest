using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
    class SA_Window : ScriptableObject
    {
    }

    [CustomEditor(typeof(SA_Window))]
    class GameObjectSetActiveModSettingsEditor : MainRoot
    {

        internal static string set_text = "Use GameObject SetActive Mod (Hierarchy Window)";
        internal static string set_key = "USE_SETACTIVE_MOD";

        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            Draw.RESET();

            //   GUI.Button( Draw.R2, "xxx", Draw.s( "preToolbar" ) );
            // GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
            Draw.BACK_BUTTON();
            Draw.TOG_TIT(set_text, set_key);
            Draw.Sp(10);

            using (ENABLE.USE(set_key))
            {

                Draw.TOG("Replace prefab button", "SET_ACTIVE_PREFAB_BUTTON_OFFSET");
                Draw.TOG("Small style", "SET_ACTIVE_SMALL_BOOL");
                Draw.TOG("Smooth camera movement to objects", "SET_ACTIVE_SMOOTH_FRAME");
                Draw.HELP("Use the right-click to move scene camera to GameObject ('F' or double click like)");

            }


            Draw.HRx4RED();
            GUI.Label(Draw.R, "Quick tips:");
            Draw.HELP_TEXTURE("HELP_SETACTIVE");
            Draw.HELP("Use draging to enable/disable several objects.", drawTog: true);

        }
    }
}
