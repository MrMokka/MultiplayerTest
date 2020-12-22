using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{
	class LibraryFolderManager
	{

		internal static string ReadLibraryFile(string fileName)
		{
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library");
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "");
			if (!File.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName)) return "";
			return File.ReadAllText(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName);
		}

		internal static void WriteLibraryFile(string fileName, ref System.Text.StringBuilder content)
		{
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library");
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) Directory.CreateDirectory(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "");
			File.WriteAllText(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName, content.ToString());
		}

		internal static void RemoveLibraryFile(string fileName)
		{
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library")) return;
			if (!Directory.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "")) return;
			if (!File.Exists(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName)) return;
			File.Delete(Folders.UNITY_SYSTEM_PATH + "Library/" + Root.PN_FOLDER + "/" + fileName);
		}
	}
}
