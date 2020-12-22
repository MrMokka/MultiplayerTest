#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

namespace Examples.HierarchyPlugin
{




    /////////////////////////////////////////////////////MENU ITEM TEMPLATE///////////////////////////////////////////////////////////////////////////////
    /*
        class MyModule : EMX.CustomizationHierarchy.ExtensionInterface_CustomRightMod.Slot_1
        {

             [InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
             public static void SpikeNailRegistrationToImproveInitPerfomance() { new MyModule(); } // <---- !!!

            public override string NameOfModule { get { return "MyModule"; } }

            // In this method, you can display information and buttons
            public override void Draw(Rect drawRect, GameObject o)
            {
                // You can invoke different built-in methods for changing variables
                //        if (GUI.Button(drawRect,"string",EMX.Utility.Styles.button)) SHOW_StringInput(...
                //        if (GUI.Button(drawRect,"int",EMX.Utility.Styles.button)) SHOW_IntInput(...
                //        if (GUI.Button(drawRect,"dropdown",EMX.Utility.Styles.button)) SHOW_DropDownMenu(...
            }

            // ToString(...) method is used for the search box
            public override string ToString(GameObject o)
            {
                return null;
            }
        }
    */
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    namespace Hierarchy_Examples
    {
        #region MODULE 1 - RotationDirection
        class CustomModule_Example_RotationDirection : EMX.CustomizationHierarchy.ExtensionInterface_CustomRightMod.Slot_1
        {


            [InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
            public static void SpikeNailRegistrationToImproveInitPerfomance() { new CustomModule_Example_RotationDirection(); }



            public override string NameOfModule { get { return "Rotation"; } }


            Dictionary<Vector3, string> types = new Dictionary<Vector3, string>()
    {
        {Vector3.back, "back" },
        {Vector3.forward, "forward" },
        {Vector3.left, "left" },
        {Vector3.right, "right" },
        {Vector3.up, "up" },
        {Vector3.down, "down" }
    };

            Vector3 GetRoundedRotation(GameObject o)     //var roundRotation = o.transform.rotation.ToAngleAxis(;
            {
                var roundRotation = o.transform.localRotation * Vector3.forward;
                for (int i = 0; i < 3; i++) roundRotation[i] = (float)Math.Round(roundRotation[i], 4);
                return roundRotation;
            }

            public override string ToString(GameObject o)
            {
                var roundRotation = GetRoundedRotation(o);
                return types.ContainsKey(roundRotation) ? types[roundRotation] : "—";
            }


            public override void Draw(Rect drawRect, GameObject o, bool mouseHover)
            {
                if (GUI.Button(drawRect, ToString(o), EMX.Utility.Styles.button))
                {
                    var itemsVectors = types.Keys.ToList();
                    var itemsStrings = types.Values.ToArray();
                    var selectedIndex = itemsVectors.IndexOf(GetRoundedRotation(o));
                    SHOW_DropDownMenu(selectedIndex, itemsStrings,
                                       newIndex =>
                                       {
                                           
                                           if (!o) return;
                                           foreach (var affectsO in EMX.Utility.GetAffectsGameObjects(o))
                                           {
                                               Undo.RecordObject(affectsO.transform, "Change Rotation");
                                               affectsO.transform.localRotation = Quaternion.LookRotation(itemsVectors[newIndex]);
                                               if (!Application.isPlaying) EditorUtility.SetDirty(affectsO);
                                               if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(affectsO.scene);
                                           }
                                       });
                }
            }

        }
        #endregion // MODULE 1 - RotationDirection








        #region MODULE 2 - StringTypes
        class CustomModule_Example_StringTypes : EMX.CustomizationHierarchy.ExtensionInterface_CustomRightMod.Slot_2
        {

            [InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
            public static void SpikeNailRegistrationToImproveInitPerfomance() { new CustomModule_Example_StringTypes(); }



            public override string NameOfModule { get { return "UI Text"; } }
            static Color green = new Color(0, 1.0f, 0.4f, 0.2f);

            public override string ToString(GameObject o)
            {
                var component = EMX.Utility.GetComponentFast<Text>(o);
                return component ? component.text : "";
            }


            public override void Draw(Rect drawRect, GameObject o, bool mouseHover)
            {
                var component = EMX.Utility.GetComponentFast<Text>(o);
                //var component = o.GetComponent<Text>();
                if (component) EditorGUI.DrawRect(drawRect, green);
                else return;

                if (GUI.Button(drawRect, component.text, EMX.Utility.Styles.button))
                {
                    SHOW_StringInput(component.text,
                                      newText =>
                                      {
                                          if (!o) return;
                                          foreach (var affectsO in EMX.Utility.GetAffectsGameObjects(o))
                                          {
                                              component = affectsO.GetComponent<Text>();
                                              if (!component) continue;
                                              Undo.RecordObject(component, "Change Text");
                                              component.text = newText;
                                              if (!Application.isPlaying) EditorUtility.SetDirty(affectsO);
                                              if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(affectsO.scene);
                                          }

                                      });
                }
            }

        }
        #endregion // MODULE 2 - StringTypes









        #region MODULE 3 - StaticEditorFlags
        class CustomModule_Example_StaticEditorFlags : EMX.CustomizationHierarchy.ExtensionInterface_CustomRightMod.Slot_3
        {


            [InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
            public static void SpikeNailRegistrationToImproveInitPerfomance() { new CustomModule_Example_StaticEditorFlags(); }




            public override string NameOfModule { get { return "Static Editor Flags"; } }
            const float alpha = 1;
            static Color colorHalf = new Color(1, 1, 1, 0.4f);
            static Color red = new Color(1.0f, 0.4f, 0, alpha);
            static Color yellow = new Color(0.8f, 0.8f, 0, alpha);
            static Color blue = new Color(0, 0.4f, 1.0f, alpha);
            static StaticEditorFlags[] enabledFlags =
            {
        #if UNITY_2019_2_OR_NEWER
        StaticEditorFlags.ContributeGI,
        #else
        StaticEditorFlags.LightmapStatic,
        #endif
        StaticEditorFlags.NavigationStatic
    };
            static Color[] enabledFlagsColors = { yellow, blue };
            static int? cacheMask;
            static GUIContent content = new GUIContent();

            public override string ToString(GameObject o)
            {
                int result = 0;
                int inverse = (int)GameObjectUtility.GetStaticEditorFlags(o);
                var fl = (int)GameObjectUtility.GetStaticEditorFlags(o);
                for (int i = 0; i < enabledFlags.Length; i++)
                {
                    result |= (fl & (int)enabledFlags[i]) != 0 ? (1 << i) : 0;
                    inverse &= ~(int)enabledFlags[i];
                }
                if (inverse != 0) result += 1 << 10;
                return result == 0 ? "" : result.ToString();
                /*if (cacheMask == null) for (int i = 0 ; i < enabledFlags.Length ; i++) cacheMask = (cacheMask ?? 0) | (int)enabledFlags[i];
                return ((int)GameObjectUtility.GetStaticEditorFlags( o ) & cacheMask.Value).ToString();*/
            }

            Rect GetDot(Rect rect, int size)
            {
                rect.x += rect.width / 2 - size / 2;
                rect.y += rect.height / 2 - size / 2;
                rect.width = rect.height = size;
                return rect;
            }
            GUIStyle buttonStyle;

            public override void Draw(Rect drawRect, GameObject o, bool mouseHover)
            {
                EditorGUIUtility.AddCursorRect(drawRect, MouseCursor.Link);

                const int PAD = 1;
                var flags = GameObjectUtility.GetStaticEditorFlags(o);
                const StaticEditorFlags all = (StaticEditorFlags)int.MaxValue;
                drawRect.x += PAD;
                drawRect.y += PAD;
                drawRect.width = drawRect.width / (enabledFlags.Length + 1) - PAD * 2;
                drawRect.height -= PAD * 2;

                var enable = (flags & all) == all;
                content.text = enable ? "*" : "";
                content.tooltip = "Enable All StaticEditorFlags";

                if (buttonStyle == null)
                {
                    buttonStyle = new GUIStyle(EMX.Utility.Styles.button);
                    buttonStyle.active.textColor = buttonStyle.normal.textColor = Color.black;
                }

                var c = red;
                if (!o.activeInHierarchy) c *= colorHalf;
                if (enable) EditorGUI.DrawRect(drawRect, c);

                if (GUI.Button(drawRect, content, buttonStyle))
                {
                    foreach (var affectsO in EMX.Utility.GetAffectsGameObjects(o))
                    {
                        Undo.RecordObject(affectsO, "Change StaticEditorFlags");
                        GameObjectUtility.SetStaticEditorFlags(affectsO, (StaticEditorFlags)SwapBits((int)flags, (int)all));
                    };
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }

                if (mouseHover) EditorGUI.DrawRect(GetDot(drawRect, 3), c);


                for (int i = 0; i < enabledFlags.Length; i++)
                {
                    drawRect.x += drawRect.width + PAD * 2;
                    enable = (flags & enabledFlags[i]) == enabledFlags[i];
                    content.text = enable ? enabledFlags[i].ToString() : "";
                    content.tooltip = enabledFlags[i].ToString();

                    c = enabledFlagsColors[i];
                    if (!o.activeInHierarchy) c *= colorHalf;
                    if (enable) EditorGUI.DrawRect(drawRect, c);

                    if (GUI.Button(drawRect, content, buttonStyle))
                    {
                        foreach (var affectsO in EMX.Utility.GetAffectsGameObjects(o))
                        {
                            Undo.RecordObject(affectsO, "Change " + enabledFlags[i]);
                            GameObjectUtility.SetStaticEditorFlags(affectsO, (StaticEditorFlags)SwapBits((int)flags, (int)enabledFlags[i]));
                        };
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }

                    if (mouseHover) EditorGUI.DrawRect(GetDot(drawRect, 3), c);
                }
            }

            int SwapBits(int reference, int mask)
            {
                if ((reference & mask) == mask) reference &= ~mask;
                else reference |= mask;

                return reference;
            }

        }
        #endregion // MODULE 3 - StaticEditorFlags












        #region MODULE 6 - UI Graphic Caster Detector

        class UIGraphicCasterDetector : EMX.CustomizationHierarchy.ExtensionInterface_CustomRightMod.Slot_6
        {


            [InitializeOnLoadMethod] // You have to include this strange method added, this method added to avoid additional assembly scans on load. I'm sorry for inconvenience
            public static void SpikeNailRegistrationToImproveInitPerfomance() { new UIGraphicCasterDetector(); }





            public override string NameOfModule { get { return "UI GrapCaster Detector"; } }


            Rect GetDot(Rect rect, int size)
            {
                rect.x += rect.width / 2 - size / 2;
                rect.y += rect.height / 2 - size / 2;
                rect.width = rect.height = size;
                return rect;
            }

            public override void Draw(Rect drawRect, GameObject o, bool mouseHover)
            {

                if (!o.activeSelf) return;

                var i = EMX.Utility.GetComponentFast<Image>(o);
                if (i)
                {
                    drawRect.width = drawRect.height;
                    if (GUI.Button(drawRect, "", EMX.Utility.Styles.button))
                    {
                        Undo.RecordObject(i, "rt");
                        i.raycastTarget = !i.raycastTarget;
                        if (!Application.isPlaying) EditorUtility.SetDirty(i);
                        if (Event.current.control)
                        {
                            foreach (var item in o.GetComponentsInChildren<Image>(true))
                            {
                                Undo.RecordObject(item, "rt");
                                item.raycastTarget = i.raycastTarget;
                                if (!Application.isPlaying) EditorUtility.SetDirty(item);
                            }
                        }
                    }
                    var S = 4;
                    drawRect.x = drawRect.height / 2 - S / 2;
                    drawRect.y = drawRect.height / 2 - S / 2;
                    drawRect.width = S;
                    drawRect.height = S;
                    if (i.raycastTarget) EditorGUI.DrawRect(drawRect, Color.red);
                    if (mouseHover) EditorGUI.DrawRect(GetDot(drawRect, 2), Color.red);

                    /* if ( o.GetComponent<UIButtonPointerCheck>() && o.GetComponent<UIButtonPointerCheck>().targetButton && o.GetComponent<UIButtonPointerCheck>().targetButton.image != null && o.GetComponent<UIButtonPointerCheck>().targetButton.image.Length == 0 ) {
                         var b = o.GetComponent<UIButtonPointerCheck>().targetButton;
                         drawRect.x += sourceRect.height;
                         if ( b.GetComponent<Image>() ) {
                             if ( GUI.Button( drawRect , "" ,EMX.Utility.Styles.button) ) {
                                 b.image = new[] { b.GetComponent<Image>() };
                                 EditorUtility.SetDirty( b );
                             }
                             EditorGUI.DrawRect( drawRect , Color.green );
                         }
                     }*/
                }
                else if (EMX.Utility.GetComponentFast<Text>(o))
                {
                    var t = EMX.Utility.GetComponentFast<Text>(o);
                    if (t)
                    {
                        drawRect.width = drawRect.height;
                        if (GUI.Button(drawRect, "", EMX.Utility.Styles.button))
                        {
                            Undo.RecordObject(t, "rt");
                            t.raycastTarget = !t.raycastTarget;
                            if (!Application.isPlaying) EditorUtility.SetDirty(t);
                            if (Event.current.control)
                            {
                                foreach (var item in o.GetComponentsInChildren<Image>(true))
                                {
                                    Undo.RecordObject(item, "rt");
                                    item.raycastTarget = t.raycastTarget;
                                    if (!Application.isPlaying) EditorUtility.SetDirty(item);
                                }
                            }
                        }
                        var S = 4;
                        drawRect.x = drawRect.height / 2 - S / 2;
                        drawRect.y = drawRect.height / 2 - S / 2;
                        drawRect.width = S;
                        drawRect.height = S;
                        if (t.raycastTarget) EditorGUI.DrawRect(drawRect, Color.red);
                        if (mouseHover) EditorGUI.DrawRect(GetDot(drawRect, 2), Color.red);

                        /* if ( o.GetComponent<UIButtonPointerCheck>() && o.GetComponent<UIButtonPointerCheck>().targetButton.image.Length == 0 ) {
                             var b = o.GetComponent<UIButtonPointerCheck>().targetButton;
                             drawRect.x += sourceRect.height;
                             if ( b.GetComponent<Image>() ) {
                                 if ( GUI.Button( drawRect , "" ,EMX.Utility.Styles.button) ) {
                                     b.image = new[] { b.GetComponent<Image>() };
                                     EditorUtility.SetDirty( b );
                                 }
                                 EditorGUI.DrawRect( drawRect , Color.green );
                             }
                         }*/
                    }
                }/* else if ( EMX.Utility.GetComponentFast<TMPro.TextMeshProUGUI>( o ) ) {
                var t =  EMX.Utility.GetComponentFast <TMPro.TextMeshProUGUI>(o);
                if ( t ) {
                    var sourceRect =drawRect;
                    drawRect.width = drawRect.height;
                    if ( GUI.Button( drawRect , "",EMX.Utility.Styles.button ) ) {
                        Undo.RecordObject( t , "rt" );
                        t.raycastTarget = !t.raycastTarget;
                        EditorUtility.SetDirty( t );
                        if ( Event.current.control ) {
                            foreach ( var item in o.GetComponentsInChildren<Image>( true ) ) {
                                Undo.RecordObject( item , "rt" );
                                item.raycastTarget = t.raycastTarget;
                                EditorUtility.SetDirty( item );
                            }
                        }
                    }
                    var S = 4;
                    drawRect.x = drawRect.height / 2 - S / 2;
                    drawRect.y = drawRect.height / 2 - S / 2;
                    drawRect.width = S;
                    drawRect.height = S;
                    if ( t.raycastTarget ) EditorGUI.DrawRect( drawRect , Color.red );

                    / * if ( o.GetComponent<UIButtonPointerCheck>() && o.GetComponent<UIButtonPointerCheck>().targetButton.image.Length == 0 ) {
                         var b = o.GetComponent<UIButtonPointerCheck>().targetButton;
                         drawRect.x += sourceRect.height;
                         if ( b.GetComponent<Image>() ) {
                             if ( GUI.Button( drawRect , "",EMX.Utility.Styles.button ) ) {
                                 b.image = new[] { b.GetComponent<Image>() };
                                 EditorUtility.SetDirty( b );
                             }
                             EditorGUI.DrawRect( drawRect , Color.green );
                         }
                     }* /
                }
            }*/

            }



            public override string ToString(GameObject o)
            {
                if (EMX.Utility.GetComponentFast<Image>(o)) return EMX.Utility.GetComponentFast<Image>(o).raycastTarget ? "RT" : "";
                if (EMX.Utility.GetComponentFast<Text>(o)) return EMX.Utility.GetComponentFast<Text>(o).raycastTarget ? "RT" : "";
                // if ( HierarchyExtensions.Utilities.GetComponentFast<TMPro.TextMeshProUGUI>.Get( o ) ) return HierarchyExtensions.Utilities.GetComponentFast<TMPro.TextMeshProUGUI>.Get( o ).raycastTarget ? "RT" : "";
                return "";
            }
        }

        #endregion // MODULE 3 - StaticEditorFlags









    }


}
#endif
