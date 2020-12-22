#define USE2017
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{

    internal partial class PluginInstance
    {





        internal void SetDirty(UnityEngine.Object o)
        {
            if (Application.isPlaying) return;

            //MonoBehaviour.print("SET");
            //Hierarchy.SetDirty(o);
            EditorUtility.SetDirty(o);
        }
        internal void MarkSceneDirty(Scene s)     //  if (Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty || IS_PROJECT()) return;
        {
            if (s.IsValid()) return;
            if (Application.isPlaying || pluginID != 0) return;
            if (s.IsValid() && s.isLoaded && !s.isDirty) EditorSceneManager.MarkSceneDirty(s);
			//Debug.Log("ASD");
            //#EMX_TODO SaveAssets
            // MarkSceneDirty(s.GetHashCode());
        }
        internal void MarkSceneDirty(int s)
        {
            if (Application.isPlaying || pluginID != 0) return;
            var getS = Tools.GET_SCENE_BY_ID(s);
            if (getS.IsValid() && getS.isLoaded && !getS.isDirty) EditorSceneManager.MarkSceneDirty(getS);
			//Debug.Log("ASD");
            //#EMX_TODO SaveAssets
        }




        /*  internal void DrawTexture( Rect rect, Texture whiteTexture, ScaleMode StretchToFill, bool al, float ac, Color col, float border, float rad )
          {
              GUI.DrawTexture( rect, whiteTexture, StretchToFill, al, ac, col, border, rad );
          }*/




        static GUIContent __ModuleButtonContent = new GUIContent();
        public bool SimpleButton(Rect drawRect, GUIContent content, GUIStyle style = null)
        {
            style = style ?? button;
            return gl._DrawButton(drawRect, content, style);
        }

        public bool ModuleButton(Rect drawRect, GUIContent content, bool hasContent, GUIStyle style = null)
        {

            if (style == null)
            {
               
                {
                    style = button;
                }
            }

            if (content == null)
            {

                {
                    content = __ModuleButtonContent;
                }
            }


            // int controlId = GUIUtility.GetControlID("Button".GetHashCode(), FocusType.Passive, drawRect);
            //if(Event.current.type == EventType.Repaint) style.Draw(drawRect, content, controlId, false, drawRect.Contains(Event.current.mousePosition));
            //return false;





            if (par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX == 0 || par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT && hasContent)
            { //  return GUI.Button( drawRect , content );
                return gl._DrawButton(drawRect, content, style);
            }

            bool Allow;

            switch (par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3)
            {
                case 1:
                    Allow = EVENT.alt;
                    break;

                case 2:
                    Allow = EVENT.shift;
                    break;

                case 3:
                    Allow = EVENT.control;
                    break;

                // default: return GUI.Button( drawRect , content );
                default: return gl._DrawButton(drawRect, content, style);
            }

            if (!Allow)
            { // GUI.enabled = false; GUI.Button( drawRect , content , buttonStyle ); GUI.enabled = true;
                GUI.enabled = false;
                gl._DrawButton(drawRect, content, style);
                GUI.enabled = true;
                return false;
            }

            // Adapter.DrawRect(drawRect, new Color(1, 1, 1, 0.05f));
            // return GUI.Button( drawRect , content );
            return gl._DrawButton(drawRect, content, style);
        }


    }
}
