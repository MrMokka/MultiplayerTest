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


    partial class HierarchyCommonData : ScriptableObject
    {



        [SerializeField]
        List<string> PlayModeSaverPersistScripts = new List<string>();
        /* [SerializeField]
         List<CustromIconData> HidedComponentsIconsValues = new List<CustromIconData>();*/
        internal Dictionary<string, MonoScript> _HasPlayModePersistScripts;
        [NonSerialized] Dictionary<GameObject, bool> _dataKeeperObjects = null;
        Dictionary<GameObject, bool> dataKeeperObjects { get { return _dataKeeperObjects ?? (_dataKeeperObjects = new Dictionary<GameObject, bool>()); } }


        internal List<string> PlayModeSaverPersistScripts_GET
        {
            get { return PlayModeSaverPersistScripts; }
            set
            {
                PlayModeSaverPersistScripts = value;
                _HasPlayModePersistScripts = null;
                _dataKeeperObjects = null;
            }
        }



        internal Dictionary<string, MonoScript> GetPlayModeSaverPersistScriptList()
        {
            if (_HasPlayModePersistScripts == null)
            {
                // for ( int i = 0, L = Math.Min( PlayModePersistScripts.Count, HasCustomIconValues.Count ) ; i < L ; i++ )
                _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                for (int i = 0, L = PlayModeSaverPersistScripts.Count; i < L; i++)
                {

                    var path = AssetDatabase.GUIDToAssetPath(HasCustomIconKeys[i]);
                    if (string.IsNullOrEmpty(path)) continue;
                    var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (!mono) continue;
                    if (!_HasPlayModePersistScripts.ContainsKey(mono.GetClass().FullName))
                    {
                        _HasPlayModePersistScripts.Add(mono.GetClass().FullName, mono);
                    }
                }
            }
            return _HasPlayModePersistScripts;
        }



        internal bool HasPlayModeSaverPersistScript(Component comp)
        {

            if (_HasPlayModePersistScripts == null)
            {
                // for ( int i = 0, L = Math.Min( PlayModePersistScripts.Count, HasCustomIconValues.Count ) ; i < L ; i++ )
                _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                for (int i = 0, L = PlayModeSaverPersistScripts.Count; i < L; i++)
                {

                    var path = AssetDatabase.GUIDToAssetPath(HasCustomIconKeys[i]);
                    if (string.IsNullOrEmpty(path)) continue;
                    var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    if (!mono) continue;
                    if (!_HasPlayModePersistScripts.ContainsKey(mono.GetClass().FullName))
                    {
                        _HasPlayModePersistScripts.Add(mono.GetClass().FullName, mono);
                    }
                }
            }
            if (!_HasPlayModePersistScripts.ContainsKey(comp.GetType().FullName)) return false;
            return true;
        }
     /*   internal void SetPlayModeSaverPersistScript(Component comp, bool value)
        {
            Undo.RecordObject(this, "SetHasCustomIcon");
            if (value)
            {
                if (_HasPlayModePersistScripts != null && _HasPlayModePersistScripts.ContainsKey(comp.GetType().FullName)) return;
                if (_HasPlayModePersistScripts == null) _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                _HasPlayModePersistScripts.Add(comp.GetType().FullName, MonoScript.FromMonoBehaviour(comp as MonoBehaviour));
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                PlayModeSaverPersistScripts.Add(guid);
            }
            else
            {
                if (_HasPlayModePersistScripts == null) _HasPlayModePersistScripts = new Dictionary<string, MonoScript>();
                _HasPlayModePersistScripts.Remove(comp.GetType().FullName);
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(comp as MonoBehaviour)));
                while (true)
                {
                    var i = PlayModeSaverPersistScripts.IndexOf(guid);
                    if (i == -1) break;
                    PlayModeSaverPersistScripts.RemoveAt(i);
                }
            }
            EditorUtility.SetDirty(this);
        }*/


        internal bool DataKeeper_IsObjectIncluded(HierarchyObject o)
        {
            if (!dataKeeperObjects.ContainsKey(o.go))
            {
                //  var comps = HierarchyExtensions.Utilities.GetComponentFast<Component>.GetAll(o);
                var comps = o.GetComponents();
                var contains = false;

                for (int i = 0; i < comps.Length; i++)
                {
                    if (!comps[i]) continue;

                    if (HasPlayModeSaverPersistScript((comps[i])))
                    {
                        contains = true;
                        break;
                    }
                }

                dataKeeperObjects.Add(o.go, contains);
            }

            return dataKeeperObjects[o.go];
        }








    }
}
