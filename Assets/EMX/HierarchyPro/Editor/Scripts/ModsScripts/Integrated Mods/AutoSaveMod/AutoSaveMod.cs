using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor.Mods
{





	internal class AutoSaveMod
	{


		static PluginInstance p { get { return Root.p[0]; } }


		static float AS_LAST_SAVE_TIME_IN_SEC
		{
			get { return SessionState.GetFloat("EMX_AS_LAST_SAVE_TIME_IN_SEC", -1); }
			set { SessionState.SetFloat("EMX_AS_LAST_SAVE_TIME_IN_SEC", value); }
		}
		static float AS_PLAY_LAUNCH_TIME
		{
			get { return SessionState.GetFloat("EMX_AS_PLAY_LAUNCH_TIME", -1); }
			set { SessionState.SetFloat("EMX_AS_PLAY_LAUNCH_TIME", value); }
		}

		static float lastSave
		{
			get { return AS_LAST_SAVE_TIME_IN_SEC; }
			set
			{
				if (AS_LAST_SAVE_TIME_IN_SEC != (value))
					AS_LAST_SAVE_TIME_IN_SEC = (value);
			}
		}
		static float EDITOR_TIMER
		{
			get { return (float)(EditorApplication.timeSinceStartup % 1000000); }
		}





		internal static void Subscribe(EditorSubscriber sbs)
		{
			sbs.OnUpdate += UpdateCS;
		}



		static string autoSaveFileName
		{
			get
			{
				if (!System.IO.Directory.Exists(Application.dataPath + "/" + p.par_e.AS_LOCATION))
				{
					System.IO.Directory.CreateDirectory(Application.dataPath + "/" + p.par_e.AS_LOCATION);
					AssetDatabase.Refresh();
				}
				//if (!AssetDatabase.IsValidFolder("Assets/" + AutoSaveFolder)) AssetDatabase.CreateFolder("Assets", AutoSaveFolder);
				var files = System.IO.Directory.GetFiles(Application.dataPath + "/" + p.par_e.AS_LOCATION).Select(f => f.Replace('\\', '/')).Where(f =>
				f.EndsWith(".unity") && f.Substring(f.LastIndexOf('/') + 1).StartsWith("AutoSave")).ToArray();
				if (files.Length == 0) return "AutoSave_00";

				var times = files.Select(System.IO.File.GetCreationTime).ToList();
				var max = times.Max();
				var ind = times.IndexOf(max);
				var count = 0;
				files = files.Select(n => n.Remove(n.LastIndexOf('.'))).ToArray();
				if (int.TryParse(files[ind].Substring(files[ind].Length - 2), out count))
				{
					count = (count + 1) % p.par_e.AS_FILES_COUNT;
					return "AutoSave_" + count.ToString("D2");
				}
				return "AutoSave_00";
			}
		}





		static float speeder = 0;

		public static void UpdateCS()
		{
			if (Application.isPlaying)
			{
				if (AS_PLAY_LAUNCH_TIME == -1) AS_PLAY_LAUNCH_TIME = EDITOR_TIMER;
				return;
			}

			if (AS_PLAY_LAUNCH_TIME != -1)
			{
				lastSave += (float)(EDITOR_TIMER - AS_PLAY_LAUNCH_TIME);
				AS_PLAY_LAUNCH_TIME = -1;
			}

			if (Mathf.Abs(speeder - EDITOR_TIMER) < 4) return;
			speeder = EDITOR_TIMER;

			if (Mathf.Abs(lastSave - (float)EDITOR_TIMER) >= p.par_e.AS_SAVE_INTERVAL_IN_SEC * 2)
			{
				lastSave = (float)EDITOR_TIMER;
				// resetSet();
			}

			//Debug.Log(lastSave + " : " +  (float)EDITOR_TIMER + "  : " +  p.par_e.AS_SAVE_INTERVAL_IN_SEC);
			if (Mathf.Abs(lastSave - (float)EDITOR_TIMER) >= p.par_e.AS_SAVE_INTERVAL_IN_SEC)
			{
				SaveScene();
			}
			//debug();
		}

	/*	static void debug()
		{
			var EDITOR_TIMER = 760f;
			var lastSave = 680f;
			var SSS = 35f;
			float dif = EDITOR_TIMER - lastSave - SSS;
			if (dif > 0)
			{
				int interator = 0;
				while (dif > 0)
				{
					lastSave = EDITOR_TIMER - dif;
					dif = EDITOR_TIMER - lastSave - SSS;
					interator++;
					if (interator > 15)
					{
						lastSave = EDITOR_TIMER;
						break;
					}
				}
			}
			Debug.Log(lastSave);
		}
		*/
		static void SaveScene()
		{

			var fn = autoSaveFileName;
			var relativeSavePath = "Assets/" + p.par_e.AS_LOCATION + "/";
			EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), relativeSavePath + fn + ".unity", true);
			var dif = (float)EDITOR_TIMER - lastSave - p.par_e.AS_SAVE_INTERVAL_IN_SEC;
			if ( dif > 0)
			{
				int interator = 0;
				while (dif > 0)
				{
					lastSave = (float)EDITOR_TIMER - dif;
					dif = (float)EDITOR_TIMER - lastSave - p.par_e.AS_SAVE_INTERVAL_IN_SEC;
					interator++;
					if (interator > 15)
					{
						lastSave = (float)EDITOR_TIMER;
						break;
					}
				}
			}
			else lastSave = (float)EDITOR_TIMER;

			if (p.par_e.AS_LOG)
			{
				Debug.Log("Auto-Save Current Scene: " + relativeSavePath + fn + ".unity"
					+ '\n' +
					lastSave + " : " + dif + " : " + p.par_e.AS_SAVE_INTERVAL_IN_SEC + " : " + EDITOR_TIMER);
			}
		}




	}
}
