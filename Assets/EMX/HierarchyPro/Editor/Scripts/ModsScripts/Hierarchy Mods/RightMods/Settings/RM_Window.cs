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
	class RM_Window : ScriptableObject
	{


	}

	[CustomEditor(typeof(RM_Window))]
	class RightHierarchyModsSettingsEditor : MainRoot
	{
		internal static string set_text = "Use Right Hierarchy Mods (Hierarchy Window)";
		internal static string set_key = "USE_RIGHT_ALL_MODS";

		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			Draw.RESET();

			//   GUI.Button( Draw.R2, "xxx", Draw.s( "preToolbar" ) );
			// GUI.Button( Draw.R, "Common Settings", Draw.s( "insertionMarker" ) );
			Draw.BACK_BUTTON();
			Draw.TOG_TIT(set_text, set_key);
			Draw.Sp(10);

			using (ENABLE.USE(set_key))
			{


				///     Draw.FIELD( "Header bg opacity", "RIGHT_BG_OPACITY", 0, 1 );
				Draw.FIELD("Modules bg opacity", "RIGHT_BG_OPACITY", 0, 1);
				Draw.COLOR("Labels color", "RIGHT_LABELS_COLOR");
				Draw.FIELD("Right mod labels font size '10'", "PLUGIN_LABELS_FONT_SIZE", 4, 60, overrideObject: p.par_e.HIER_WIN_SET);//
				Draw.TOG("Draw hyphen '-' for empty labels", "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS");

				Draw.FIELD("Right Padding", "RIGHT_RIGHT_PADDING", -100, 200, "px");
				Draw.TOG("Right Padding affect SetActive and PlayModeKeeper mods", "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER");

				Draw.TOG("Auto hiding modules if window width less than...", "RIGHTDOCK_TEMPHIDE");
				using (ENABLE.USE("RIGHTDOCK_TEMPHIDE")) Draw.FIELD("If width <", "RIGHTDOCK_TEMPHIDEMINWIDTH", 100, 500, "px");




				Draw.Sp(10);
				//MAIN
				using (GRO.UP(0))
				{
					GUI.Label(Draw.R, Draw.CONT("Tags/Layers/SpritesLayers mods")); Draw.Sp(2); //, Draw.s( "preBackground" )
					Draw.TOG("Tags displays only uppercase chars", "RIGHT_TAGS_UPPERCASE");
					Draw.TOG("Layers displays only uppercase chars", "RIGHT_LAYERS_UPPERCASE");
					Draw.TOG("SpritesLayers displays only uppercase chars", "RIGHT_SPRITEORDER_UPPERCASE");
					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}
				Draw.Sp(10);
				//MAIN
				using (GRO.UP(0))
				{
					GUI.Label(Draw.R, Draw.CONT("Freeze mod")); Draw.Sp(2); //, Draw.s( "preBackground" )
					Draw.TOG("Lock selection in scene view", "RIGHT_FREEZE_LOCK_SCENE_VIEW");
					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}


				Draw.Sp(10);

				using (GRO.UP(0))
				{
					GUI.Label(Draw.R, Draw.CONT("Textures Memory mod")); Draw.Sp(2); //, Draw.s( "preBackground" )
					Draw.TOG("Enable broadcast scan", "RIGHT_MOD_BROADCAST_ENABLED");
					Draw.HELP("This means that the parent objects will display the total value of all children. It also counts the count of same textures and models, which will help to identify objects that are inefficient using unique models or textures with a large amount of memory");
					Draw.HELP("You can enable/disable broadcast using LeftClick in hierarchy window");
					Draw.FIELD("Broadcasting Performance", "RIGHT_MOD_BROADCASTING_PREFOMANCE01", 5, 95, "%");
					Draw.HELP("(High values may reduce performance)");
				}

				Draw.Sp(10);
				using (GRO.UP(0))
				{
					GUI.Label(Draw.R, Draw.CONT("Hiding mods")); Draw.Sp(2); //, Draw.s( "preBackground" )

					
					RightAutoHider();
					// Draw.TOG( "Lock modules if no special key is pressed", "RIGHT_SPRITEORDER_UPPERCASE" ); //RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX
					using (ENABLE.USE("RIGHT_LOCK_MODS_UNTIL_NOKEY"))
					{
						if (p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT && p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK)
						{
							p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK = p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT = false;
						}
						using (ENABLE.USE("RIGHT_USE_HIDE_ISTEAD_LOCK", 0, true))
						{
							Draw.TOG("Lock only with empty content", "RIGHT_LOCK_ONLY_IF_NOCONTENT");
							if (!p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT)
							{
								Draw.HELP(LOCK_CONTENT[p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3] + " key selected to access locked modules without content");
							}
						}
						using (ENABLE.USE("RIGHT_LOCK_ONLY_IF_NOCONTENT", 0, true))
						{
							Draw.TOG("Hide and lock instead of just locking", "RIGHT_USE_HIDE_ISTEAD_LOCK");
							if (p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && !p.par_e.RIGHT_LOCK_ONLY_IF_NOCONTENT)
							{
								Draw.HELP(LOCK_CONTENT[p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3] + " key selected to display modules");
							}
						}
					}


					using (ENABLE.USE("RIGHT_USE_HIDE_ISTEAD_LOCK", 0, true))
					{
						Draw.TOG("Right mods shows only if mouse hovers", "RIGHT_SHOWMODS_ONLY_IFHOVER");
						if (p.par_e.RIGHT_SHOWMODS_ONLY_IFHOVER && !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK)
							Draw.HELP("You can press 'ALT' to display all modules");
					}
					// Draw.TOOLBAR( new[] { "TopBar", "Scene Name", "Bottom", "Disable" }, "HOTBUTTONS_BAR_PLACE" );
				}

				Draw.Sp(10);

				using (GRO.UP(0)) Draw.TOG_TIT("Use custom modules", "RIGHT_USE_CUSTOMMODULES");
				Draw.Sp(10);
				using (GRO.UP(0))
				{
					Draw.TOG_TIT("Search Window");
					Draw.FIELD("Input window width factor", "ADDITIONA_INPUT_WINDOWS_WIDTH", 0.5f, 5, "x");
					Draw.FIELD("Search window width factor", "ADDITIONA_SEARCH_WINDOWS_WIDTH", 0.5f, 5, "x");
					Draw.TOG("Snap search to left when opening", "BIND_SEARCH_TO_LEFT");
					Draw.TOG("Include disabled objects", "SEARCH_SHOW_DISABLED_OBJECT");
					Draw.TOG("Search only inside the current root", "SEARCH_USE_ROOT_ONLY");
					Draw.HELP("Objects located outside the root of selected object will not be included");
					Draw.HELP("You can also select an object then hold control and then click an item to search, in this case, the search will inside the selected object only");
				}

				Draw.Sp(10);
				using (GRO.UP(0))
				{
					Draw.TOG("Draw External Mods HotButtons on Hierarchy Header", "DRAW_HEADER_HOTBUTTONS", rov: Draw.R);
					if (p.par_e.DRAW_HEADER_HOTBUTTONS) Draw.FIELD("Hierarchy Header Buttons Size", "HEADER_HOTBUTTON_SEZE", 3, 60, "px");
				}

			}




			Draw.HRx4RED();
			GUI.Label(Draw.R, "Quick tips:");
			Draw.HELP_TEXTURE("HELP_RIGHTMOD");
			Draw.HELP("You can change columns width or order.", drawTog: true);
			Draw.HELP("Use left-click to open menu to use special functions of enable/disable mods.", drawTog: true);

			Draw.HELP_TEXTURE("HELP_SEARCH");
			Draw.HELP("Use right-click to open special search window.", drawTog: true);
			Draw.HELP("If you right-click on the header rect, it will display all objects with any not empty content for that mod.", drawTog: true);
			Draw.HELP("If you right-click on the content rect, it will display all objects with the same content.", drawTog: true);
			Draw.HELP("If you select a one root object and then use ctrl+right-click on any child content, the search window will only scan the child content of the selected object.", drawTog: true);


			Draw.HRx2();
			Draw.HELP("You can add your own mod using 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", drawTog: true);
			//Draw.HELP("You can add your own items using 'ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
			if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[1] }; }



		}
		string[] LOCK_CONTENT = { "Disabled", "Alt", "Shift", "Ctrl" };
		void RightAutoHider()
		{
			Rect _R = Draw.R;
			// _R = EditorGUILayout.GetControlRect();
			// _R.width -= 80;
			GUI.Label(_R, "Lock modules if no special key is pressed:");
			//  R.x += R.width;
			//  R.width = 80;
			_R = Draw.R;
			var old_i = p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3;
			var new_i = GUI.Toolbar(_R, old_i, LOCK_CONTENT, EditorStyles.miniButton);
			if (new_i != old_i)
			{
				p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = (p.par_e.RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & ~3) | new_i;
			}

			//    TOOLTIP( R2 , "If a module already has a content, you shouldn't use a key to change them." );

			//             var   lineRect = EditorGUILayout.GetControlRect( );
			//             var new_S_HideRightIfNoFunction = TOOGLE_POP( ref lineRect , "Hide <b>Right Bar</b> if " + key + " not pressed" , _S_HideRightIfNoFunction ? 1 : 0 , "Show Always" , "Hide" ) == 1;
			//             GUILayout.Space( EditorGUIUtility.singleLineHeight );
			//             lineRect = EditorGUILayout.GetControlRect();
			//             var new_S_HideBttomIfNoFunction = TOOGLE_POP( ref lineRect , "Hide <b>Bottom Bar</b> if " + key + " not pressed" , _S_HideBttomIfNoFunction ? 1 : 0 , "Show Always" , "Hide" ) == 1;
			//             GUILayout.Space( EditorGUIUtility.singleLineHeight );
			//             GUI.enabled = on;
			//
			//             if ( new_A != par.USE_BUTTON_TO_INTERACT_AHC || _S_HideRightIfNoFunction != new_S_HideRightIfNoFunction || _S_HideBttomIfNoFunction != new_S_HideBttomIfNoFunction ) {
			//                 par.USE_BUTTON_TO_INTERACT_AHC = new_A;
			//                 _S_HideRightIfNoFunction = new_S_HideRightIfNoFunction;
			//                 _S_HideBttomIfNoFunction = new_S_HideBttomIfNoFunction;
			//                 DRAW_STACK.ValueChanged();
			//             }
		}
	}
}
