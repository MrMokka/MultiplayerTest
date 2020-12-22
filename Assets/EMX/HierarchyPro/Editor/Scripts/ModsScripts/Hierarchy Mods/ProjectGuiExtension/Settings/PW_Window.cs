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
    class PW_Window : ScriptableObject
    {
    }

    [CustomEditor(typeof(PW_Window))]
    class ProjectWindowSettingsEditor : MainRoot
    {


        internal static string set_text = "Use files extensions drawing (Project Window)";
        internal static string set_key = "USE_PROJECT_GUI_EXTENSIONS";


        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }
        public override void OnInspectorGUI()
        {
            Draw.RESET();
            Draw.BACK_BUTTON();

            Draw.TOG_TIT(set_text, set_key);
            Draw.Sp(10);
            using (ENABLE.USE(set_key))
            {
                Draw.TOG("Display files extension", "DRAW_EXTENSION_IN_PROJECT");

                using (ENABLE.USE("DRAW_EXTENSION_IN_PROJECT", 0))
                {
                    Draw.COLOR("Extension font color", "DRAW_EXTENSION_COLOR");
                    Draw.FIELD("Extension font size", "DRAW_EXTENSION_FONT_SIZE", 4, 20);
                    Draw.FIELD("Extension offset X", "DRAW_EXTENSION_OFFSET_X", -200, 200);
                    Draw.FIELD("Extension offset Y", "DRAW_EXTENSION_OFFSET_Y", -200, 200);
                }



            }
        }
    }
}

