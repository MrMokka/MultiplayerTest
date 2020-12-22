


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
	class AB_Cache : ScriptableObject
	{
	}
	[CustomEditor(typeof(AB_Cache))]
	class AboutCacheEditor : MainRoot
	{

		internal static string set_text = "About Cache";
		internal static string set_key = "";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			Draw.RESET();

			Draw.BACK_BUTTON();
			Draw.TOG_TIT(set_text);
			Draw.Sp(10);

			Draw.HELP_TEXTURE("HELP_CACHE_A");

			Draw.HELP("All cache stored in external folder, there are no data is saved in the scene, only temporarily for the current editor session and removing after closing the editor.", drawTog: true);
			Draw.Sp(10);
			Draw.HELP("You can add 'EMX/" + Root.PN_FOLDER + "/' to git ignore file, to ensure compatibility with people who does not own a " + Root.PN + ".", drawTog: true);
			Draw.HELP("You can add 'EMX/" + Root.PN_FOLDER + "/Editor/_SAVED_DATA' to git ignore file, to ensure compatibility with people with different scene settings.", drawTog: true);
			Draw.Sp(10);
			Draw.HELP("All data has an improved caching system to reach maximum performance in the editor during initialization.", drawTog: true);





			Draw.Sp(10);
			Draw.HRx2();

			//	GUI.Label(Draw.R, "However you may lost some data.");
			Draw.HELP("All data for each scene stored in separate file, you can find it in '_SAVED_DATA' folder. When data may lost? When you will rename, move or copy scene some data may be lost. But! There's a special methods that tracking scenes changes, but I think they may not always work.", drawTog: true);
			Draw.HELP_TEXTURE("HELP_CACHE_B");
			Draw.HELP("You can manualy rename, copy or move data files in '_SAVED_DATA'.", drawTog: true);
			Draw.HELP("You can copy data from another scene using special copy/paste buttons.", drawTog: true);

			Draw.Sp(10);
			Draw.HELP("There will be a special manager that will allow to recover or reasign each objects manually but only in the future. Anyway hope this versiou will work fine for you, because there is a completely different caching system compared to the previous version.", drawTog: true);


			Draw.Sp(10);
			Draw.HRx2();
			Draw.HELP("Yeah, all editor settings located in 'EMX/" + Root.PN_FOLDER + "/Editor/_SAVED_DATA/.EditorSettings/' so you can copy it to your other project using file browser.", drawTog: true);
			Draw.HELP("Yeah, and if you wanna reset settings to default just remove 'EMX/" + Root.PN_FOLDER + "/Editor/_SAVED_DATA/.EditorSettings/' .", drawTog: true);
			Draw.Sp(10);
			Draw.HRx2();
			Draw.HELP("If you have problems disabling any module, you can find a 'HardDisableMods.cs' script in the root, and replace the field value to 'false'.", drawTog: true);






		}
	}


}

