using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
	class AS_Window : ScriptableObject
	{
	}

	[CustomEditor(typeof(AS_Window))]
	class AutosaveSceneModSettingsEditor : MainRoot
	{

		internal static string set_text = "Use Autosave Scene Every n'min Mod (Background)";
		internal static string set_key = "USE_AUTOSAVE_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			Draw.RESET();

			Draw.BACK_BUTTON();
			Draw.TOG_TIT(set_text, set_key);
			Draw.Sp(10);

			using (ENABLE.USE(set_key))
			{


				Draw.FIELD("Save Every (Minutes)", "AS_SAVE_INTERVAL_IN_MIN", 1, 60);
				Draw.FIELD("Maximum Files Version", "AS_FILES_COUNT", 1, 99);
				Draw.STRING("Auto-Save Location:", "AS_LOCATION");
				EditorGUILayout.LabelField("Assets/" + Root.p[0].par_e.AS_LOCATION);

				Draw.TOG("Log", "AS_LOG");

				Draw.HELP("Autosave pauses the time during the PlayMode, for example, you set save every 5 minutes, and 2 minutes left to save, you will play 15 minutes and when you stop, 2 minutes will also be left to save");

			}
		}
	}
}
