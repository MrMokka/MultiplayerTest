
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
	class AB_Extensions : ScriptableObject
	{
	}
	[CustomEditor(typeof(AB_Extensions))]
	class AboutExtensionsEditor : MainRoot
	{

		internal static string set_text = "About what you can extend here";
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


			Draw.Sp(10);
			Draw.HRx2();
			using (GRO.UP(0))
			{
				Draw.TOG_TIT( RightClickMenuSettingsEditor.set_text.Substring(4));
				Draw.HELP("You can add your own custom menu item using 'EMX." + Root.CUST_NS + ".ExtensionInterface_RightClickOnGameObjectMenuItem'.", drawTog: true);
				if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[0] }; }
				if (Draw.BUT("Open " + RightClickMenuSettingsEditor.set_text.Substring(4) + " Settings")) { MainSettingsEnabler_Window.Select<RC_Window>(); }
			}

			Draw.Sp(10);
			Draw.HRx2();
			using (GRO.UP(0))
			{
				Draw.TOG_TIT(RightHierarchyModsSettingsEditor.set_text.Substring(4));
				Draw.HELP("You can add your own custom mod using 'EMX." + Root.CUST_NS + ".ExtensionInterface_CustomRightMod.Slot_1'.", drawTog: true);
				if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[1] }; }
				if (Draw.BUT("Open " + RightHierarchyModsSettingsEditor.set_text.Substring(4) + " Settings")) { MainSettingsEnabler_Window.Select<RM_Window>(); }
			}
			Draw.Sp(10);
			Draw.HRx2();
			using (GRO.UP(0))
			{
				Draw.TOG_TIT(TopBarsModSettingsEditor.set_text.Substring(4));
				Draw.HELP("You can add your own OnGUI function using 'ExtensionInterface_TopBarOnGUI'.", drawTog: true);
				if (Draw.BUT("Select Example Script")) { Selection.objects = new[] { Root.icons.example_folders[3] }; }
				if (Draw.BUT("Open " + TopBarsModSettingsEditor.set_text.Substring(4) + " Settings")) { MainSettingsEnabler_Window.Select<TB_Window>(); }
			}
			Draw.Sp(10);
			Draw.HRx2();
			using (GRO.UP(0))
			{
				Draw.TOG_TIT(IconsforComponentsModSettingsEditor.set_text.Substring(4));
				Draw.HELP("You can add [DRAW_IN_HIER] attribute to any: public, private or internal fields.", drawTog: true);
				if (Draw.BUT("Select Example Scene")){	Selection.objects = new[] { Root.icons.example_folders[2] };	}
				if (Draw.BUT("Open " + IconsforComponentsModSettingsEditor.set_text.Substring(4) + " Settings")) { MainSettingsEnabler_Window.Select<IC_Window>(); }
			}



		}
	}


}

