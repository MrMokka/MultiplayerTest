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
		public List<ScenesTab_Saved> ScenesTabs = new List<ScenesTab_Saved>();


		const string TypeName = "HierarchyCommonData.asset";
		internal static Func<HierarchyCommonData> Instance = () =>
		{

			Folders.CheckFolders();
			return (Instance = () =>
		  {
			  if (_Instance) return _Instance;
			  var g = EditorPrefs.GetInt(Folders.PREFS_PATH + "|SObjGUID" + TypeName, -1);
			  if (g != -1 && (InternalEditorUtility.GetObjectFromInstanceID(g) as HierarchyCommonData))
			  {
				  Folders.CheckFolders(true);
				  return (_Instance = InternalEditorUtility.GetObjectFromInstanceID(g) as HierarchyCommonData);
			  }

			  var ASSET_PATH = Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/" + TypeName;

			  var loaded = AssetDatabase.LoadAssetAtPath<HierarchyCommonData>(ASSET_PATH);
			  if (!loaded)
			  {
				  if (!Directory.Exists(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/")) Directory.CreateDirectory(Folders.PluginInternalFolder + "/Editor/_SAVED_DATA/");

				  var preCache = ScriptableObject.CreateInstance<HierarchyCommonData>();
				  preCache.hideFlags = HideFlags.DontSaveInBuild;

				  if (File.Exists(ASSET_PATH)) File.Delete(ASSET_PATH);
				  AssetDatabase.CreateAsset(preCache, ASSET_PATH);
				  //HierarchyExternalSceneData.SaveAssets();
				  AssetDatabase.SaveAssets();

				  AssetDatabase.ImportAsset(ASSET_PATH, ImportAssetOptions.ForceUpdate);

				  loaded = preCache;
			  }

			  EditorPrefs.SetInt(Folders.PREFS_PATH + "|SObjGUID" + TypeName, loaded.GetInstanceID());
			  return (_Instance = loaded);
		  })();
		};
		static HierarchyCommonData _Instance;








		internal void SetUndo(string undoScring)
		{
			if (Root.p[0].par_e.SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER) return;
			Undo.RecordObject(this, undoScring);
		}

		internal new void SetDirty()
		{
			if (Root.p[0].par_e.SAVE_HIGHLIGHTER_SETS_TO_HIDENFOLDER)
			{
				/*    if ( _GetFakeClass[ adapter.pluginID ] == null ) return;
                    var result = new System.Text.StringBuilder();
                    result.AppendLine( "Initialized" );
                    result.AppendLine( _GetFakeClass[ adapter.pluginID ].Initialized.ToString() );
                    foreach ( var item in _GetFakeClass[ adapter.pluginID ]._colorFilters )
                    {
                        result.AppendLine( "_colorFilters" );
                        item.SaveToString( ref result );
                    } //0

                    Adapter.WriteLibraryFile( adapter.pluginID == Initializator.HIERARCHY_ID ? modName0 : modName1, ref result );*/
				throw new Exception("ASD");
			}
			else
			{
				// if ( _GetFakeClass[ adapter.pluginID ] != null )
				// {
				//    _getAssetInstance( adapter ).Initialized = _GetFakeClass[ adapter.pluginID ].Initialized;
				//     _getAssetInstance( adapter )._colorFilters = _GetFakeClass[ adapter.pluginID ]._colorFilters;
				//     Adapter.SetDirty( _getAssetInstance( adapter ) );
				// }
				// Root.p[0].SetDirty(this);
				if (!Application.isPlaying) EditorUtility.SetDirty(this);
				HierarchyExternalSceneData.SaveAssets();
			}
		}
	}
}
