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
	class TB_Window : ScriptableObject
	{


	}


	[CustomEditor(typeof(TB_Window))]
	class TopBarsModSettingsEditor : MainRoot
	{

		internal static string set_text = "Use TopBar (ToolBar)";
		internal static string set_key = "USE_TOPBAR_MOD";
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

				using (GRO.UP(0))
				{
					Draw.TOG("Swap left and right areas", "TOPBAR_SWAP_LEFT_RIGHT");
				}

				string LEFT = "Left";
				string RIGHT=  "Right";
				if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT)
				{
					var t = LEFT;
					LEFT = RIGHT;
					RIGHT = t;
				}


				Draw.Sp(10);
				Draw.HRx2();
				GUI.Label(Draw.R, ""+ LEFT+" Area:");
				using (GRO.UP())
				{

					Draw.FIELD("" + LEFT + " area min border offset", "TOPBAR_LEFT_MIN_BORDER_OFFSET", -500, 500);
					Draw.FIELD("" + LEFT + " area max border offset", "TOPBAR_LEFT_MAX_BORDER_OFFSET", -500, 500);

					Draw.TOG("Draw Layouts Tab", "DRAW_TOPBAR_LAYOUTS_BAR");
					using (ENABLE.USE("DRAW_TOPBAR_LAYOUTS_BAR"))
					{
						Draw.TOG("Draw cross for selected layout", "TOPBAR_LAYOUTS_DRAWCROSS");
						Draw.TOG("AutoSave selected layout", "TOPBAR_LAYOUTS_AUTOSAVE");
						using (ENABLE.USE("TOPBAR_LAYOUTS_AUTOSAVE"))
						{
							Draw.TOG("Disalbe autosave for internal layout", "TOPBAR_LAYOUTS_SAVE_ONLY_CUSTOM");
							Draw.HELP("Be careful, because you can rewrite even a default layout, however you can of course reset it");
						}
					}

					Draw.TOG("Use Custom " + LEFT + " Side Buttons", "DRAW_TOPBAR_CUSTOM_LEFT");
					Draw.HELP("You can add your buttons at the top bars, using EMX." + Root.CUST_NS + "");

				}


				Draw.Sp(10);
				Draw.HRx2();
				GUI.Label(Draw.R, "" + RIGHT + " Area:");
				using (GRO.UP())
				{

					Draw.FIELD("" + RIGHT + " area min border offset", "TOPBAR_RIGHT_MIN_BORDER_OFFSET", -500, 500);
					Draw.FIELD("" + RIGHT + " area max border offset", "TOPBAR_RIGHT_MAX_BORDER_OFFSET", -500, 500);

					Draw.TOG("Draw External Mods HotButtons on TopBar", "DRAW_TOPBAR_HOTBUTTONS");
					using (ENABLE.USE("DRAW_TOPBAR_HOTBUTTONS")) Draw.FIELD("TopBar Buttons Size", "TOPBAR_HOTBUTTON_SIZE", 3, 60, "px");
					Draw.TOG("Use Custom " + RIGHT + " Side Buttons", "DRAW_TOPBAR_CUSTOM_RIGHT");
					Draw.HELP("You can add your buttons at the top bars, using EMX." + Root.CUST_NS + "");
				}
				//	Draw.Sp(10);




				Draw.HRx4RED();


				GUI.Label(Draw.R, "Quick tips:");
				Draw.HELP_TEXTURE("HELP_LAYOUT");
				Draw.HELP("Use the right mouse button to open a special menu for quick access to functions.", drawTog: true);
				Draw.HELP("You can use left mouse button to drag button to change position, or use middle button to remove.", drawTog: true);
				Draw.HRx2();
				Draw.HELP("You can add your own OnGUI function using 'ExtensionInterface_TopBarOnGUI'.", drawTog: true);
				if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[3] }; }

				//HOT BUTTONS



			}
		}
	}
}
