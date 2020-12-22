using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EMX.HierarchyPlugin.Editor
{

    class HierarchyTempGlobalData : ScriptableObject
    {


        const string  TypeName ="HierarchyTempData.asset";
        internal static Func<HierarchyTempGlobalData> Instance = () =>
        {
            Folders.CheckFolders();
            return  ( Instance = ()=>
            {
                if (_Instance) return _Instance;
                var g = EditorPrefs.GetInt(Folders.PREFS_PATH + "|SObjGUID" + TypeName, -1);
                if (g != -1 && (InternalEditorUtility.GetObjectFromInstanceID( g ) as HierarchyTempGlobalData))
                {
                    Folders.CheckFolders(true);
                    return (_Instance = InternalEditorUtility.GetObjectFromInstanceID( g ) as HierarchyTempGlobalData);
                }


                var preCache = ScriptableObject.CreateInstance<HierarchyTempGlobalData>();
                preCache.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;

                EditorPrefs.SetInt(Folders.PREFS_PATH + "|SObjGUID" + TypeName, preCache.GetInstanceID());
                return (_Instance = preCache);
            })();
        };
        static   HierarchyTempGlobalData _Instance;




    }


}
