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
	class RC_Window : ScriptableObject
	{
	}


	[CustomEditor(typeof(RC_Window))]
	class RightClickMenuSettingsEditor : MainRoot
	{
		internal static string set_text = "Use RightClick GameObject Menu Extenstion Mod (Hierarchy Window)";
		internal static string set_key = "USE_RIGHT_CLICK_MENU_MOD";
		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			Draw.RESET();

			Draw.BACK_BUTTON();
			Draw.TOG_TIT(set_text, set_key);
			Draw.HELP("You cad add your buttons at the top bars, using EMX." + Root.CUST_NS + ", you can find examples in hierarchy folder");
			Draw.Sp(10);

			using (ENABLE.USE(set_key))
			{


				Draw.Sp(10);
				using (GRO.UP(0))
				{
					Draw.TOG("Add 'Open External Mods' items to menu", "DRAW_RIGHTCLIK_MENU_HOTBUTTONS", rov: Draw.R);

					Draw.TOG("Place menu item into one 'HerarchyUltra' tab", "RCGO_MENU_PLACE_TO_HIERFOLDER", rov: Draw.R);

					Draw.TOG("Use hotkeys", "RCGO_MENU_USE_HOTKEYS", rov: Draw.R);
					using (ENABLE.USE("RCGO_MENU_USE_HOTKEYS", 0))
					{

						GUI.Label(Draw.R, "Other windows that will catch hotkeys events");

						var htks = p.par_e.CUSTOMMENU_HOTKEYS_WINDOWS;

						var oldw1 = htks.ContainsKey("SceneView");
						var w1 = GUI.Toolbar(Draw.R, oldw1 ? 1 : 0, new[] { "No", "SceneView" }, EditorStyles.toolbarButton) == 1;
						var oldw2 = htks.ContainsKey("GameView");
						var w2 = GUI.Toolbar(Draw.R, oldw2 ? 1 : 0, new[] { "No", "GameView" }, EditorStyles.toolbarButton) == 1;
						var oldw3 = htks.ContainsKey("Inspector");
						var w3 = GUI.Toolbar(Draw.R, oldw3 ? 1 : 0, new[] { "No", "Inspector" }, EditorStyles.toolbarButton) == 1;

						if (w1 != oldw1 || w2 != oldw2 || w3 != oldw3)
						{
							var res = new Dictionary<string, bool>();
							if (w1) res.Add("SceneView", true);
							if (w2) res.Add("GameView", true);
							if (w3) res.Add("Inspector", true);
							p.par_e.CUSTOMMENU_HOTKEYS_WINDOWS = res;
						}
					}


					Draw.HRx4RED();
					GUI.Label(Draw.R, "Quick tips:");
					Draw.HELP_TEXTURE("HELP_RIGHT_MENU");
					Draw.HELP("Use the right mouse button on the gameobject to open a special menu for quick access to functions.", drawTog: true);
					Draw.HELP("All hotkeys work only for special windows, that means if you switch to the animator window, hotkeys for hierarchy are automatically disabled.", drawTog: true);

					Draw.HRx2();
					Draw.HELP("You can add your own menu items using 'EMX." + Root.CUST_NS + ".ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);

					if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[0] }; }
					

				}
			}
		}
	}

}
