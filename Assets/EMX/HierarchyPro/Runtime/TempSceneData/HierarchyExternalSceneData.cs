#if UNITY_EDITOR
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
using UnityEditor.SceneManagement;
using System.Text;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{
	public enum SaverType { ModFreezer = 0, ModDescription = 1, ModManualHighligher = 2, Bookmarks = 3, ModPlayKeeper = 4, SceneHierarchyExands = 5, ModManualIcons = 6, PresetsManager = 7 }

	[Serializable]
	public class SavedObjectData
	{


		public SavedObjectData(int id_in_external_heap) { this.id_in_external_heap = id_in_external_heap; }

		public int saverType;
		public int id_in_external_heap;

		public string indentifierVersion;
		public string identifierTypetargetObjectIdtargetPrefabId;
		public string namesPathBackup;
		public string siblingsPathBackup;
		/* public string globalid;
       public int identifierType;
        public string targetObjectId;
        public string targetPrefabId;*/




		public string GetGuidData()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(1.ToString());
			sb.AppendLine(saverType.ToString());
			sb.AppendLine(id_in_external_heap.ToString());
			sb.AppendLine(indentifierVersion);
			sb.AppendLine(identifierTypetargetObjectIdtargetPrefabId);
			sb.AppendLine(namesPathBackup);
			sb.AppendLine(siblingsPathBackup);
			return sb.ToString();
		}
		public void SetGuidData(string s)
		{
			StringReader sr = new StringReader(s);
			sr.ReadLine(); //version
			saverType = int.Parse(sr.ReadLine());
			id_in_external_heap = int.Parse(sr.ReadLine());

			indentifierVersion = sr.ReadLine();
			identifierTypetargetObjectIdtargetPrefabId = sr.ReadLine();
			namesPathBackup = sr.ReadLine();
			siblingsPathBackup = sr.ReadLine();
		}

		//DATA
		public bool boolValue;
		public string stringValue;
		public HighlighterExternalData[] highLighterData = new HighlighterExternalData[0];
		public IconExternalData[] iconData = new IconExternalData[0];
		public SavedObjectData Clone()
		{
			var res = (SavedObjectData)this.MemberwiseClone();
			return res;
		}
	}
	[Serializable]
	public class SavedObjectDataByType
	{
		public SavedObjectData[] data = new SavedObjectData[0];

		public SavedObjectDataByType Clone()
		{
			var res = (SavedObjectDataByType)MemberwiseClone();
			if (data != null) res.data = data.Select(d => d.Clone()).ToArray();
			return res;
		}
	}

	[Serializable]
	public class BookMarkCategory_Saved
	{

		public Color bgColor = Color.white;
		public string category_name = null;
		public List<HierExpands_Saved> buttons = new List<HierExpands_Saved>();

		/*	[SerializeField]
			int __dictionaryKey = 100;
			public int dictionaryKey
			{
				get { return Mathf.Clamp(__dictionaryKey, 100, 1000); }
				set
				{
					if (value < 100 || value > 1000) throw new Exception(value.ToString());
					__dictionaryKey = value;
				}
			}*/
	}
	[Serializable]
	public class ScenesTab_Saved
	{
		public bool pin;
		public string[] path;
		public string[] guid;
	}

	[Serializable]
	public class HierExpands_Saved
	{
		public string name;
		public int[] ids_in_external_heap;
	}

	public class HierarchyExternalSceneData : ScriptableObject
	{


		[SerializeField]
		public SavedObjectDataByType[] types;
		[SerializeField]
		public List<BookMarkCategory_Saved> BookMarks_InternalGlobal = new List<BookMarkCategory_Saved>();
		[SerializeField]
		public List<HierExpands_Saved> HierExpands_InternalGlobal = new List<HierExpands_Saved>();

		[NonSerialized]
		public static bool SkipSetDirty;


		// Dictionary<SaverType, SavedObjectData[]> _LoadObjects = new Dictionary<SaverType, SavedObjectData[]>
		public static SavedObjectData[] LoadObjects(SaverType type, Scene scene)
		{

			var file = GetHierarchyExternalSceneData(scene);
			/*if (type == SaverType.ModDescription)
			{
				Debug.Log("ASD");
			}*/
			var t = (int)type;
			if (file.types == null) file.types = new SavedObjectDataByType[0];
			if (t >= file.types.Length) Array.Resize(ref file.types, t + 1);
			if (file.types[t] == null) file.types[t] = new SavedObjectDataByType();
			var r = file.types[t];
			if (r.data == null) r.data = new SavedObjectData[0];
			return r.data;

		}
		public static void WriteObjects(SaverType type, Scene scene, SavedObjectData[] objects)
		{
			//Debug.Log("ASD");
			//	if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("External data, requires saving the scene before applying any changes")) return;
			var file = GetHierarchyExternalSceneData(scene);
			var t = (int)type;
			if (file.types == null) file.types = new SavedObjectDataByType[0];
			if (t >= file.types.Length) Array.Resize(ref file.types, t + 1);
			if (file.types[t] == null) file.types[t] = new SavedObjectDataByType();
			var r = file.types[t];
			r.data = objects;
			SetDirtyFile(file);
			// if ( r.data == null ) r.data = new SavedObjectData[ 0 ];
			// return r.data;
		}


		public static HierarchyExternalSceneData GetHierarchyExternalSceneData(Scene scene)
		{


			var scene_path = GetScenePath(scene);
			// if ( !descriptionWasSave.ContainsKey( sceneID ) ) descriptionWasSave.Add( sceneID, false );
			var result = GetProjectHash(scene, ref scene_path);
			if (!result)
			{
				TryCreateBackupForCache(scene);
				// if (  )
				// {
				// AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				// }

				result = CreateInstance<HierarchyExternalSceneData>();
				var path = GetStoredDataPathInternal(scene);
				var folder = path.Remove(path.LastIndexOf('/'));
				if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
				AssetDatabase.CreateAsset(result, path);
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
			}
			return result;
		}


		public static void Undo(Scene scene, string v)
		{
			//EMX_TODO - add cache cleaner for undo events
			//	/var file = GetHierarchyExternalSceneData(scene);
			//UnityEditor.Undo.RecordObject(file, v);
		}
		public static void SetDirtyFile(HierarchyExternalSceneData file)
		{
			if (SkipSetDirty) return;
			UnityEditor.EditorUtility.SetDirty(file);
			SaveAssets();
			//Debug.Log("ASD");
		}
		public static void SetDirtyFile(Scene scene)
		{
			if (SkipSetDirty) return;
			var file = GetHierarchyExternalSceneData(scene);
			UnityEditor.EditorUtility.SetDirty(file);
			SaveAssets();
			//	Debug.Log("ASD");
		}
		public static void SaveAssets()
		{
			if (SkipSetDirty) return;
			//#EMX_TODO SaveAssets
			//if (!Application.isPlaying && !SceneManager.GetActiveScene().isDirty) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
			//AssetDatabase.SaveAssets();
			//Debug.Log("ASD");
		}

		public static bool TryCreateBackupForCache(Scene s)
		{
			return TryCreateBackupForCache(s.path);
		}

		public static bool TryCreateBackupForCache(string scene)
		{
			var oldName = GetStoredDataPathExternal(scene);
			bool reload = false;
			if (System.IO.File.Exists(oldName + ".backup"))
			{
				System.IO.File.Copy(oldName + ".backup", oldName);
				reload = true;
			}

			if (System.IO.File.Exists(oldName + ".meta" + ".backup"))
			{
				System.IO.File.Copy(oldName + ".meta" + ".backup", oldName + ".meta");
				reload = true;
			}

			if (reload)
			{

				return true;
			}

			return false;
		}
		// const string FOLDER = "/Editor/SavedData/ScenesData/";
		const string SCENED_DATA_FOLDER = "/Editor/_SAVED_DATA/_SCENES/";
		public static HierarchyExternalSceneData GetProjectHash(Scene s, ref string scene_path)
		{
			var path = Folders.PluginInternalFolder + SCENED_DATA_FOLDER + scene_path.Remove(scene_path.LastIndexOf('.')) + ".asset";
			return AssetDatabase.LoadAssetAtPath<HierarchyExternalSceneData>(path);
		}



		static public string GetScenePath(Scene s)
		{
			var p = s.path;

			if (string.IsNullOrEmpty(p)) return "untitled.unity";

			return p;
		}
		public static string GetStoredDataPathInternal(Scene s)
		{
			return Folders.PluginInternalFolder + SCENED_DATA_FOLDER + GetScenePath(s).Remove(GetScenePath(s).LastIndexOf('.')) + ".asset";
		}
		public static string GetStoredDataPathInternal(string p)
		{
			if (!p.ToLower().EndsWith(".unity")) return null;

			p = p.Remove(p.Length - ".unity".Length);
			return Folders.PluginInternalFolder + SCENED_DATA_FOLDER + p + ".asset";
		}

		public static string GetStoredDataPathExternal(Scene s)
		{
			var path = Folders.UNITY_SYSTEM_PATH;

			if (!path.EndsWith("/")) path += '/';

			return path + GetStoredDataPathInternal(s);
		}
		public static string GetStoredDataPathExternal(string s)
		{
			var path = Folders.UNITY_SYSTEM_PATH;

			if (!path.EndsWith("/")) path += '/';

			return path + GetStoredDataPathInternal(s);
		}










	}



}

#endif