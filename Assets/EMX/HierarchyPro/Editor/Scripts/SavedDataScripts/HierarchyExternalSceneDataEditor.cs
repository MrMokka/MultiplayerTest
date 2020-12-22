using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{




    [CustomEditor(typeof(HierarchyExternalSceneData))]
    class HierarchyExternalSceneDataEditor : UnityEditor.Editor
    {


        static HierarchyExternalSceneData copiedScriptableObject;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying) {
                GUILayout.Label("Not allow while application is playing");
                return; }

            if (GUILayout.Button("Copy", GUILayout.Height(30))) copiedScriptableObject = target as HierarchyExternalSceneData;
            GUI.enabled = copiedScriptableObject != null;
            var name = "Paste";
            if (copiedScriptableObject) name += " " + copiedScriptableObject.name;
            if (GUILayout.Button(name, GUILayout.Height(30)))
            {
                var temp = (target) as HierarchyExternalSceneData;
                if (temp)
                {

                    var cloned = copiedScriptableObject.types.Select(t => t.Clone()).ToArray();
                    temp.types = cloned;
                    var adapter = Root.p[0];

                    Undo.RecordObject(temp, "Paste DescriptionHelper");
                    HierarchyTempSceneData.RemoveCache();
                    Root.p[0].invoke_ReloadAfterAssetDeletingOrPasting();
                    //HierarchyExternalSceneData.();
                    EditorUtility.SetDirty(temp);
                 //   adapter.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                /* var path = AssetDatabase.GetAssetPath(target);
                 if (temp && !string.IsNullOrEmpty( path ))
                 {
                   AssetDatabase.DeleteAsset( path );
                   Hierarchy.M_Descript.RemoveIHashPropertY( temp );
                   AssetDatabase.CreateAsset( copiedScriptableObject, path );
                 }*/
            }

            GUILayout.Space(20);
            GUILayout.Label("Data:");
            DrawDefaultInspector();
            /*     GUI.enabled = true;
                  var temp = (target) as HierarchyExternalSceneData;
                  GUILayout.Space( 10 );
                  if ( temp && GUILayout.Button( "Apply to current Scene", GUILayout.Height( 30 ) ) )
                  {
                      for ( int i = 0 ; i < EditorSceneManager.sceneCount ; i++ )
                      {
                          var s = EditorSceneManager.GetSceneAt(i);
                          if ( !s.IsValid() || !s.isLoaded ) continue;
                          var d = Hierarchy.M_Descript.des(s.GetHashCode());
                          var assetPath = AssetDatabase.GetAssetPath(target);
                          assetPath = assetPath.Remove( assetPath.LastIndexOf( '.' ) );
                          var scenePath = Adapter.GetScenePath(s);
                          scenePath = scenePath.Remove( scenePath.LastIndexOf( '.' ) );
                          if ( d == null || !assetPath.EndsWith( scenePath ) ) continue;

                          Hierarchy.M_Descript.RemoveIHashPropertY( d );
                          Hierarchy.HierarchyAdapterInstance.SET_UNDO( d, "Apply to current Scene" );
                          Adapter.SET_HASH_WITHOUT_LOCALID( temp, d );
                          Hierarchy.HierarchyAdapterInstance.SetDirtyDescription( d, s );

                          Hierarchy.HierarchyAdapterInstance.EditorSceneManagerOnSceneOpening( null, OpenSceneMode.Single );
                      }
                  }*/
        }
    }
}
