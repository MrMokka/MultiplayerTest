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
	class MainSettingsEnabler_Window : ScriptableObject
	{

		// [MenuItem( "Tools/Hierarchy/Select Main Settings", false, 10000 )]
		// [CreateAssetMenu(fileName = "New UI Manager", menuName = "Modern UI Pack/New UI Manager")]
		//[MenuItem("CONTEXT/GameObject/Select Main Settings")]
		//   [MenuItem("GameObject/Hierarchy/Select Main Settings", false, 10000)]


		internal static void Select<T>() where T : ScriptableObject
		{
			CheckSettings();
			Selection.objects = new[] { settings.First(s => s is T) };
		}


		static List<ScriptableObject> settings = new List<ScriptableObject>();
		const string ModsSettings = ""; //"Mods Settings/";
		internal static void CheckSettings()
		{
			settings.Clear();
			_Check<MainSettingsEnabler_Window>(Folders.PluginInternalFolder + "/Editor/Settings/MainSettingsEnabler.asset");
			_Check<MainSettingsParams_Window>(Folders.PluginInternalFolder + "/Editor/Settings/MainSettingsParams.asset");

			_Check<AS_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Integrated Mods/AutosaveSceneModSettings.asset");
			_Check<ST_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Integrated Mods/SnapTransformModSettings.asset");
			_Check<TB_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Integrated Mods/TopBarsModSettings.asset");
			_Check<RC_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Integrated Mods/RightClickMenuSettings.asset");



			_Check<SA_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Right Side Mods/GameObjectSetActiveModSettings.asset");
			_Check<RM_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Right Side Mods/RightHierarchyModsSettings.asset");
			_Check<IC_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Right Side Mods/IconsForComponentsModSettings.asset");

			_Check<PW_Window>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "Project Window Mods/ProjectFilesExtensionsSettings.asset");

			_Check<AB_Extensions>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "About/AboutExtensions.asset");
			_Check<AB_Cache>(Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "About/AboutCache.asset");
		}




		static void _Check<T>(string path) where T : ScriptableObject
		{
			//  ScriptableObject load ;
			// var asset = path.EndsWith(".asset") ? path.Remove(path.LastIndexOf('.')) : path;
			ScriptableObject load = AssetDatabase.LoadAssetAtPath<T>(path);
			if (!load)
			{
				var dir = Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "";// path.Remove(path.LastIndexOf('/'));
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
				var file = path.Substring(path.LastIndexOf('/') + 1);
				var finded = Directory.GetFiles(dir, "*.asset", SearchOption.AllDirectories).FirstOrDefault(f => f.EndsWith(file));
				if (!string.IsNullOrEmpty(finded))
				{
					finded = finded.Replace('\\', '/');
					finded = Folders.PluginInternalFolder + "/Editor/Settings/" + ModsSettings + "" + finded.Substring(finded.LastIndexOf("/Editor/Settings/") + "/Editor/Settings/".Length);
					load = AssetDatabase.LoadAssetAtPath<T>(finded);
				}
			}

			if (!load)
			{
				// Debug.Log(path);
				if (File.Exists(path)) File.Delete(path);
				var inst = CreateInstance(typeof(T));
				if (!Directory.Exists(path.Remove(path.LastIndexOf('/')))) Directory.CreateDirectory(path.Remove(path.LastIndexOf('/')));
				AssetDatabase.CreateAsset(load = inst, path);
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
			}
			settings.Add(load);
			/*if ( !sbs)
            {
                sbs = true;
               

            }*/
		}
		// static bool sbs = false;


	}


	[CustomEditor(typeof(MainSettingsEnabler_Window))]
	class MainSettingsEditor : MainRoot
	{

		public override VisualElement CreateInspectorGUI()
		{
			return base.CreateInspectorGUI();
		}
		public override void OnInspectorGUI()
		{
			// base.OnInspectorGUI();


			/*    if ( EditorGUILayout.BeginToggleGroup( "ASD", true ) )
                {

                }
                EditorGUILayout.EndToggleGroup();
                if ( EditorGUILayout.BeginFoldoutHeaderGroup( true, "ASD" ) )
                {

                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                if ( EditorGUILayout.ToggleLeft( "ASD", true ) )
                {

                }*/


			//GUI.Button( R, "gray line", s( "preToolbar2" ) );
			// GUI.Button( R, "3", s( "preToolbarLabel" ) );
			// GUI.Button( R, "border top", s( "footer" ) );
			// GUI.Button( R, "drop", s( "typeSelection" ) ); Sp( 10 );
			//   var r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

			Draw.RESET();


			GUI.Button(Draw.R2, "" + Root.PN + "", Draw.s("preToolbar"));


			Draw.TOG_TIT("Enable " + Root.PN + "", "ENABLE_ALL", (valBefore) =>
			 {
				 if (!valBefore.AS_BOOL && EditorUtility.DisplayDialog("" + Root.PN + "", "Do you want to enable " + Root.PN + " plugin?", "Yes - yes - yes", "No, I'm going to remove you, man")) return true;
				 if (valBefore.AS_BOOL && EditorUtility.DisplayDialog("" + Root.PN + "", "Do you want to disable " + Root.PN + " plugin?", "Yes, turn off this piece of 'cake'", "No, I have the wrong toggle")) return true;
				 return false;
			 },
			  (valAfter) =>
			  {
				  if (valAfter.AS_BOOL) Root.ENABLE();
				  if (!valAfter.AS_BOOL) Root.DISABLE();
			  });
			var C = GUI.color;
			var L = 0.5f;
			using (ENABLE.USE("ENABLE_ALL", 0))
			{
				GUI.color = C * Color.Lerp(new Color32(255, 150, 150, 255), Color.white, 0);
				// if (Draw.BUT("Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(); }
				Draw.TOG_TIT("Main Hierarchy Settings (Hierarchy Window)", rightOffset: 1); if (Draw.BUT(Draw.last, "Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_MAIN");
			}
			Draw.Sp(16);
			GUI.color = C;
			GUI.Button(Draw.R2, "Internal Mods", Draw.s("preToolbar"));
			GUI.color = C * Color.Lerp(new Color32(150, 200, 222, 255), Color.white, L);
			/*using (GRO.UP())
			{
				Draw.HELP("Other");
			}*/
			GUI.color = C * Color.Lerp(new Color32(150, 200, 222, 255), Color.white, L);
			using (ENABLE.USE("ENABLE_ALL", 0))
			{
				using (ENABLE.USE(Draw.TOG_TIT(TopBarsModSettingsEditor.set_text, TopBarsModSettingsEditor.set_key, rightOffset: 1))) if (Draw.BUT(Draw.last, TopBarsModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<TB_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_TB");
				using (ENABLE.USE((Draw.TOG_TIT(RightClickMenuSettingsEditor.set_text, RightClickMenuSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, RightClickMenuSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<RC_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_RC");
				using (ENABLE.USE((Draw.TOG_TIT(AutosaveSceneModSettingsEditor.set_text, AutosaveSceneModSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, AutosaveSceneModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<AS_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_AS");
				using (ENABLE.USE((Draw.TOG_TIT(SnapTransformModSettingsEditor.set_text, SnapTransformModSettingsEditor.set_key, rightOffset: 1, AreYouSureText: "Scripts compilation will start now. Are you sure?")
					))) if (Draw.BUT(Draw.last, SnapTransformModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<ST_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_SM");
			}





			Draw.Sp(16);
			GUI.color = C;
			GUI.Button(Draw.R2, "Right Side Mods", Draw.s("preToolbar"));
			GUI.color = C * Color.Lerp(new Color32(150, 222, 188, 255), Color.white, L);
			/*	using (GRO.UP())
				{
					Draw.HELP("Mods integrated into Hierarchy Window");
				}*/
			GUI.color = C * Color.Lerp(new Color32(150, 222, 188, 255), Color.white, L);
			using (ENABLE.USE("ENABLE_ALL", 0))
			{
				using (ENABLE.USE((Draw.TOG_TIT(GameObjectSetActiveModSettingsEditor.set_text, GameObjectSetActiveModSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, GameObjectSetActiveModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<SA_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_SA");
				using (ENABLE.USE((Draw.TOG_TIT(IconsforComponentsModSettingsEditor.set_text, IconsforComponentsModSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, IconsforComponentsModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<IC_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_IC");
				//Draw.TOG_TIT(HighlighterModSettingsEditor.set_text);
				using (ENABLE.USE((Draw.TOG_TIT(RightHierarchyModsSettingsEditor.set_text, RightHierarchyModsSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, RightHierarchyModsSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<RM_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_RM");

			}
			Draw.Sp(16);

		


			GUI.color = C;
			GUI.Button(Draw.R2, "Project Window Extensions", Draw.s("preToolbar"));
			using (ENABLE.USE("ENABLE_ALL", 0))
			{

				// if (Draw.BUT("Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(); }
				//using (ENABLE.USE((Draw.TOG_TIT(HighlighterManualModSettingsEditor.set_text, HighlighterManualModSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, HighlighterManualModSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<HLM_Window>(); }
				//	Draw.TOG_TIT("Project Window Settings", rightOffset: 1); if (Draw.BUT(Draw.last, "Main Hierarchy Settings")) { MainSettingsEnabler_Window.Select<MainSettingsParams_Window>(); }
					GUI.color = C * Color.Lerp(new Color32(150, 200, 222, 255), Color.white, L);

				using (ENABLE.USE((Draw.TOG_TIT(ProjectWindowSettingsEditor.set_text, ProjectWindowSettingsEditor.set_key, rightOffset: 1)))) if (Draw.BUT(Draw.last, ProjectWindowSettingsEditor.set_text + " Settings")) { MainSettingsEnabler_Window.Select<PW_Window>(); }
				Draw.HELP_TEXTURE(Draw.last, "IC_PV");
			}
			Draw.Sp(16);


			GUI.color = C;

		








			Draw.HRx4RED();
			GUI.color = C * Color.Lerp(new Color32(150, 150, 150, 255), Color.white, L);
			using (ENABLE.USE("ENABLE_ALL", 0))
			{
				Draw.TOG_TIT(AboutExtensionsEditor.set_text, rightOffset: 1); if (Draw.BUT(Draw.last, AboutExtensionsEditor.set_text)) { MainSettingsEnabler_Window.Select<AB_Extensions>(); }
				Draw.TOG_TIT(AboutCacheEditor.set_text, rightOffset: 1); if (Draw.BUT(Draw.last, AboutCacheEditor.set_text)) { MainSettingsEnabler_Window.Select<AB_Cache>(); }
			if (GUI.Button(Draw.R2, Draw.CONT( "Upgrade to Extended Version"))) Application.OpenURL("https://assetstore.unity.com/packages/slug/115861");
			}
			GUI.color = C;



			Draw.Sp(10);
			var a = GUI.color;
			var b = GUI.backgroundColor;
			GUI.backgroundColor = Color.clear;
			GUI.color *= new Color(1, 1, 1, 0.7f);
			if (bs == null)
			{
				bs = new GUIStyle(GUI.skin.button);
				bs.alignment = TextAnchor.MiddleLeft;
			}
			if (GUI.Button(Draw.R2, Draw.CONT( "         - Open Welcome Screen"), bs)) WelcomeScreen.Init(null);

			if (GUI.Button(Draw.R2, Draw.CONT("         - Open New Documentation: https://emem.store/HierarchyProDocumentation.pdf"), bs)) Application.OpenURL("https://emem.store/HierarchyProDocumentation.pdf");
			if (GUI.Button(Draw.R2, Draw.CONT("         - Open Legacy Wiki Page: https://emem.store/wiki"), bs)) Application.OpenURL("https://emem.store/wiki");
			GUI.color = a;
			GUI.backgroundColor = b;

			//   GUI.Button( Draw.R, "6", Draw.s( "insertionMarker" ) );
			//   GUI.Button( Draw.R, "7", Draw.s( "preBackground" ) ); Draw.Sp( 10 );

			/*   var toolbar = new Toolbar();
              root.Add(toolbar);

               TemplateContainer e1 = (EditorGUIUtility.Load("UXML/InspectorWindow/InspectorWindow.uxml") as VisualTreeAsset).CloneTree();
              e1.st*/
			/* e1.AddToClassList(InspectorWindow.s_MainContainerClassName);
             this.rootVisualElement.hierarchy.Add((VisualElement) e1);
             this.m_ScrollView = e1.Q<ScrollView>((string) null, (string) null);
             VisualElement e2 = this.rootVisualElement.Q((string) null, InspectorWindow.s_MultiEditClassName);
             e2.Query<TextElement>((string) null, (string) null).ForEach<string>((Func<TextElement, string>) (label => label.text = L10n.Tr(label.text)));
             e2.RemoveFromHierarchy();
             this.m_MultiEditLabel = e2;
             this.rootVisualElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
             this.rootVisualElement.AddStyleSheetPath("StyleSheets/InspectorWindow/InspectorWindow.uss");*/
		}


		GUIStyle bs;



	}





}



