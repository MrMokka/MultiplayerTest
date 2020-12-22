#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

//This class using only for the current editor session and objects will not save to the scene asset. 
//Just that the Unity engine requires that the MonoBehaviour scripts places outside the editor folder, even it using only for editor.

namespace EMX.HierarchyPlugin.Editor
{

	public class Folders
	{

		public const string PN_FOLDER = "HierarchyPro";
		public const string ASSET_NAME = PN_FOLDER;
		public const string PREFS_PATH = "EMX." + ASSET_NAME + "/";
		const string DEFAULT_PATH = "Assets/EMX/" + ASSET_NAME + "";

		// static bool hasInternal = false;
		//      public static string EMInternalFolder = null;
		//      public static string EMExternalFolder = null;
		static bool init = false;
		[NonSerialized]
		static ScriptableObject _icons;
		static internal ScriptableObject icons { get { return _icons ?? (_icons = AssetDatabase.LoadAssetAtPath<ScriptableObject>(_PluginInternalFolder + "/Editor/Icons/IconsArray.asset")); } }
		[NonSerialized]
		static bool wasForce = false;

		public static string UNITY_SYSTEM_PATH
		{
			get
			{
				return _UNITY_SYSTEM_PATH ?? (_UNITY_SYSTEM_PATH = Application.dataPath.Remove(Application.dataPath.Length - "Assets".Length));
			}
		}
		static string _UNITY_SYSTEM_PATH;

		public static string PluginInternalFolder
		{
			get
			{
				if (_PluginInternalFolder == null) _PluginInternalFolder = EditorPrefs.GetString(PREFS_PATH + "PluginInternalFolder", DEFAULT_PATH);
				if (!wasForce)
				{
					CheckFolders(!icons);
					wasForce = true;
				}
				return _PluginInternalFolder;
			}
			set
			{
				if (_PluginInternalFolder == value) return;
				EditorPrefs.SetString(PREFS_PATH + "PluginInternalFolder", _PluginInternalFolder = value);
			}
		}
		static string _PluginInternalFolder = null;
		public static string PluginExternalFolder
		{
			get
			{
				if (_PluginExternalFolder == null) _PluginExternalFolder = UNITY_SYSTEM_PATH + PluginInternalFolder;
				if (!wasForce)
				{
					CheckFolders(!icons);
					wasForce = true;
				}
				return _PluginExternalFolder;
			}
			set
			{
				if (_PluginExternalFolder == value) return;
				_PluginExternalFolder = value;
			}
		}
		static string _PluginExternalFolder = null;



		public static void CheckFolders(bool force = false)
		{
			if (!init || force)
			{
				init = true;

				string PAT = "EMX." + ASSET_NAME + ".Editor.asmdef";

				if (!File.Exists(_PluginInternalFolder + "/Editor/" + PAT))
				{
					var candidate = AssetDatabase.GetAllAssetPaths().Where(p => !string.IsNullOrEmpty(p) && p[p.Length - 1] == 'f').FirstOrDefault(a => a.EndsWith(PAT));
					if (candidate != null)
					{
						candidate = candidate.Replace('\\', '/');
						candidate = candidate.Remove(candidate.LastIndexOf('/'));
						candidate = candidate.Remove(candidate.LastIndexOf('/'));
						_PluginInternalFolder = candidate;
					}

					if (candidate == null)
					{
						throw new Exception("Cannot find hierarchy assembly: " + PAT);
					}
					else
					{
						var r = _PluginInternalFolder;
						_PluginExternalFolder = UNITY_SYSTEM_PATH + _PluginInternalFolder;
						if (!Directory.Exists(_PluginInternalFolder)) Directory.CreateDirectory(_PluginInternalFolder);
					}
					//if ( PluginInternalFolder == null ) PluginInternalFolder = DEFAULT_PATH;

				}


				/* if ( !hasInternal )
                 {
                     hasInternal = true;
                     EMInternalFolder = PluginInternalFolder.Remove( PluginInternalFolder.LastIndexOf( '/' ) );
                     EMExternalFolder = PluginExternalFolder.Remove( PluginExternalFolder.LastIndexOf( '/' ) );
                 }*/
			}
		}


	}
}
#endif