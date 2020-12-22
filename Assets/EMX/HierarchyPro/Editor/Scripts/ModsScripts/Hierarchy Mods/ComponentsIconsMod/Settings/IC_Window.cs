using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace EMX.HierarchyPlugin.Editor.Settings
{
    class IC_Window : ScriptableObject
    {
    }


    [CustomEditor(typeof(IC_Window))]
    class IconsforComponentsModSettingsEditor : MainRoot
    {
        internal static string set_text = "Use Icons for Components Mod (Hierarchy Window)";
        internal static string set_key = "USE_COMPONENTS_ICONS_MOD";

        public override void OnInspectorGUI()
        {
            Draw.RESET();

            Draw.BACK_BUTTON();
            Draw.TOG_TIT(set_text, set_key);
            Draw.Sp(10);

            using (ENABLE.USE(set_key))
            {

                using (GRO.UP(0))
                {
                    Draw.FIELD("Icons space", "COMPONENTS_ICONS_SPACE", -20, 20);
                    Draw.FIELD("Additional space between categories", "COMPONENTS_ICONS_CAT_SPACE", -20, 20);
                    Draw.FIELD("Icons size", "COMPONENTS_ICONS_SIZE", 1, 30);
                    Draw.FIELD("Icons margin left", "COMPONENTS_NEXT_TO_NAME_PADDING", -100, 100);
                    Draw.FIELD("Icons margin top", "COMPONENTS_MARGIN_TOP", -100, 100);
                    Draw.TOG("Draw shadow for icons", "COMPONENTS_DRAW_ICONS_SHADOW");
                }
                Draw.Sp(10);


                using (GRO.UP(0))
                {
                    Draw.TOG("Draw unity icons", "COMPONENTS_DRAW_DEFAULT_ICONS");
                    using (ENABLE.USE(Draw.GetSetter("COMPONENTS_DRAW_DEFAULT_ICONS"), 0))
                    {
                        DrawHidenIcons();
                    }
                }

                using (GRO.UP(0))
                {
                    Draw.TOG("Draw monobehaviour icons", "COMPONENTS_DRAW_MONO_ICONS");
                    using (ENABLE.USE(Draw.GetSetter("COMPONENTS_DRAW_MONO_ICONS")))
                    {
                        Draw.TOG("Use transparent style", "COMPONENTS_DRAW_ICONS_MONO_BG_INVERSE");
                        GUI.Label(Draw.R, Draw.CONT("Grouping monobehaviour scripts:"));
                        Draw.TOOLBAR(new[] { "Common\n[1] Icon", "Enable/Disable\n[2] Icons", "Each\nOwn Icon" }, "COMPONENTS_MONO_SPLIT_MODE", 40);
                        if (p.par_e.COMPONENTS_MONO_SPLIT_MODE == 2)
                        {
                            GUI.Label(Draw.R, Draw.CONT("Monobehaviour icons style:"));
                            Draw.TOOLBAR(new[] { "Blank Icon", "Icon With\n1 Char", "Icon With\n2 Chars" }, "COMPONENTS_MONO_ICON_TYPE", 40);
                        }
                    }
                }

                using (GRO.UP(0))
                {
                    Draw.TOG("Draw custom global icons", "COMPONENTS_DRAW_GLOBALCUSTOM_ICONS");

                    using (ENABLE.USE(Draw.GetSetter("COMPONENTS_DRAW_GLOBALCUSTOM_ICONS"), 0))
                    {
                        Draw.HELP("You also may use the inspector to assign a custom icon for a script");
                        Draw.TOG("Draw custom icons assigned in inspector", "COMPONENTS_DRAW_CUSTOM_ICONS_FROM_ISPECTOR");

                        Draw.TOG("Draw custom icons assigned below", "COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS");
                        using (ENABLE.USE(Draw.GetSetter("COMPONENTS_DRAW_CUSTOM_ICONS_FROM_SETTINGS"), 0))
                        {
                            DrawUserIcons();
                        }
                    }
                }

                Draw.Sp(10);
                using (GRO.UP(0))
                {

                    GUI.Label(Draw.R, Draw.CONT("[DRAW_IN_HIER] attribute"));
                    Draw.HELP("Use [DRAW_IN_HIER] attribute to display vars or method in hierarchy, you cam find some examples in the example scene");
                    Draw.FIELD("Attributes margin left", "COMPONENTS_ATTRIBUTES_MARGIN", -20, 100);
                    Draw.TOG("Draw methods", "COMPONENTS_ATTRIBUTES_BUTTONS");
                    Draw.TOG("Draw fields/properties/enums", "COMPONENTS_ATTRIBUTES_FIELDS");
                    using (ENABLE.USE(Draw.GetSetter("COMPONENTS_ATTRIBUTES_FIELDS"), 0))
                    {
                        Draw.TOG("Red null values", "COMPONENTS_ATTRIBUTES_DISPLAYING_NULLSISRED");
                    }
                }



                Draw.HRx4RED();
                GUI.Label(Draw.R, "Quick tips:");
                Draw.HELP_TEXTURE("HELP_CUSTOM_ICONS_DRAG");
                Draw.HELP("Use the left mouse button to open a special menu for quick access to functions.", drawTog: true);
                Draw.HELP("Use the ctrl+drag to drag component.", drawTog: true);
                Draw.HELP_TEXTURE("HELP_SEARCH");
                Draw.HELP("Use the right mouse button to open a special search window.", drawTog: true);

                
                Draw.HRx2();
                Draw.HELP_TEXTURE("HELP_CUSTOM_ICONS_ATT");
                Draw.HELP("You can add [DRAW_IN_HIER] attribute to display 'fields','properies','methods' and 'enums'.", drawTog: true);
                Draw.HELP("You can invoke method or change int, float, string or enum value.", drawTog: true);
                Draw.HELP("You can add attribute to any: public, private or internal fields.", drawTog: true);
                //Draw.HELP("You can add your own items using 'ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
                if (Draw.BUT("Select Example Scene")) { Selection.objects = new[] { Root.icons.example_folders[2] }; }

            }
        }




        Vector2 DrawHidenIconsscrollPos;
        void DrawHidenIcons()
        {
            var list = HierarchyCommonData.Instance().GetComponentIconHidedList();
            var RECT = Draw.RH(50);
            //  RECT.y -= 10;
            //  RECT.x += 20;
            //  RECT.width = Math.Min( W + 10 , RECT.width ) - 20;
            //  RECT.width = RECT.width - 20;
            DrawHidenIconsscrollPos = GUI.BeginScrollView(RECT, DrawHidenIconsscrollPos, new Rect(0, 0, list.Count * 32, 32), true, false);
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            //Assembly asm = typeof(Image).Assembly;
            p.INTERNAL_BOX(new Rect(0, 0, RECT.width, 32), "");

            if (list.Count == 0) p.INTERNAL_BOX(new Rect(0, 0, RECT.width, 32), "Use left-click in the hierarchy on the icon to hide");

            for (int i = 0; i < list.Count; i++)
            {
                RECT = new Rect(i * 32, 0, 32, 32);
                //print(list[i] + " " + asm.GetType(list[i]));
                Type target = null;

                foreach (var assembly in asms)
                {
                    target = assembly.GetType(list[i]);

                    if (target != null) break;
                }

                //  if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lastRect, Utilites.ObjectContent(null, asm.GetType(list[i])).image);
                var find = EditorGUIUtility.ObjectContent(null, target);

                if (Event.current.type.Equals(EventType.Repaint) && find.image != null) GUI.DrawTexture(RECT, find.image);

                if (!GUI.enabled) PluginInstance.FadeRect(RECT, 0.7f);

                // if (!GUI.enabled) if (Event.current.type.Equals(EventType.Repaint)) GUI.DrawTexture(lastRect,Hierarchy.sec);
                RECT.x += RECT.width / 2;
                RECT.height = RECT.width = RECT.width / 2;

                if (GUI.Button(RECT, "X"))
                {
                    //   Hierarchy_GUI.Undo(this, "Restore Icon");
                    //   Hierarchy_GUI.Instance(this).HiddenComponents.RemoveAt(i);
                    //   Hierarchy_GUI.SetDirtyObject(this);
                    //   DRAW_STACK.ValueChanged();
                    HierarchyCommonData.Instance().ComponentIconHidedListRemoveAt(i);
                    p.RESET_DRAWSTACK(0);
                }
            }

            GUI.EndScrollView();
        }





        DrawCustomIconsClassOld __CI;
        internal DrawCustomIconsClassOld CI
        {
            get
            {
                var res = __CI ?? (__CI = new DrawCustomIconsClassOld());
                res.A = p;
                return res;
            }
        }

        void DrawUserIcons()
        {
            //   var boxRect = EditorGUILayout.GetControlRect(GUILayout.Height(0));
            //  boxRect.height = CI.CusomIconsHeight + 12;

            //  var R = EditorGUILayout.GetControlRect(GUILayout.Height(CI.CusomIconsHeight));
            Draw.Sp(10);
            var R = Draw.RH(CI.CusomIconsHeight);
            var boxRect = R;
            boxRect.height = CI.CusomIconsHeight + 8;
            p.INTERNAL_BOX(boxRect, "");
            //R.x += 7;
            // R.y += 6;
            GUI.BeginScrollView(R, Vector2.zero, new Rect(0, 0, R.width, DrawCustomIconsClassOld.IC_H * (CI.customIcons.Count + 1)), false, false /*, GUILayout.Width(W), GUILayout.ExpandWidth(true)*/);

            var lr = R;

            CI.DrawCustomIcons(EditorWindow.focusedWindow, lr);
            EditorGUILayout.GetControlRect(GUILayout.Height(10));

            GUI.EndScrollView();

            CI.Updater(EditorWindow.focusedWindow);
        }


    }
}
