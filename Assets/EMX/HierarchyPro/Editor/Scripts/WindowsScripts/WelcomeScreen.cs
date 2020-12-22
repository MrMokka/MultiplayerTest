using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{


	class WelcomeScreen : EditorWindow
	{
		const int buttonW = 180;

		public static void Init(Rect? _source) // var w = GetWindow<WelcomeScreen>( "Post Presets - Welcome Screen" , true, Params.WindowType,)
		{
			var source = _source ?? new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
			var w = GetWindow<WelcomeScreen>(true, "" + Root.PN + " - Welcome", true);
			var thisR = new Rect(0, 0, buttonW + (Screen.currentResolution.width < 1500 ? 430 : Screen.currentResolution.width < 2100 ? 750 : 1000), Math.Max(source.height,
				Math.Min(Screen.currentResolution.height, 1080) - 80));
			thisR.x = source.x + source.width / 2 - thisR.width / 2;
			thisR.y = source.y + source.height / 2 - thisR.height / 2;
			w.position = thisR;
		}


		void drawTexture(Texture2D texture)
		{
			var dif = Mathf.Clamp((position.width - buttonW) / texture.width, 0.01f, 1);
			var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.RoundToInt(texture.height * dif * 0.95f)));
			rect.height += 2;
			GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
		}

		Vector2 scrollPos;
		Dictionary<int, List<Texture2D>> t = new Dictionary<int, List<Texture2D>>();
		char[] parts = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
		GUIStyle __emptyStyle;

		GUIStyle emptyStyle
		{
			get
			{
				return __emptyStyle ?? (__emptyStyle = new GUIStyle()
				{
					active = new GUIStyleState() { background = GUI.skin.box.normal.background ?? GUI.skin.box.normal.scaledBackgrounds[0] },
					clipping = TextClipping.Overflow,
					alignment = TextAnchor.MiddleLeft,
					border = new RectOffset(5, 5, 5, 5)
				});
			}
		}




		string[] Chapters = { "HELLO", "HIGHLIGHTER", "OBJECT MENU", "HEADER", "RIGHT BAR", "COMPONENTS ICONS", "EXTERNAL MODS", "SEARCH", "CACHE", "SNAP" };
		string[] Icons = { "WELCOME_HI", "WELCOME_01_HIGH", "WELCOME_02_RC_MENU", "WELCOME_03_HEADER", "WELCOME_04_RIGHTBAR", "WELCOME_05_COMP_ICONS", "WELCOME_06_EXTERNAL", "WELCOME_07_SEARCH", "WELCOME_08_CACHE", "WELCOME_09_SNAP" };
		GUIStyle label;
		void LegacyButton(string t, int ind)
		{
			var R = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f), GUILayout.Width(buttonW));
			R.width *= 0.8f;
			/*R.x += 16 * t.Count( c => c == '/' );
            if ( t.IndexOf( '/' ) != -1 ) t = t.Substring( t.LastIndexOf( '/' ) + 1 );
            t = "└ " + t;*/
			if (GUI.Button(R, "", emptyStyle))
			{
				currentWelcomeChapter = ind;
				scrollPos = Vector2.zero;
			}

			if (ind == currentWelcomeChapter && Event.current.type == EventType.Repaint)
			{
				GUI.skin.box.Draw(R, true, true, true, true);
				var r = R;
				r.x += r.width;
				r.width = 3;
				EditorGUI.DrawRect(r, Color.red);
			}
			if (label == null)
			{
				label = new GUIStyle(GUI.skin.label);
				label.fontSize += 2;
				label.fontStyle = FontStyle.Bold;
			}
			GUI.Label(R, t, label);
			EditorGUIUtility.AddCursorRect(R, MouseCursor.Link);
		}

		static int currentWelcomeChapter = 0;

		//  Rect lastPosition;
		private void OnGUI()
		{

			scrollPos.y = Mathf.RoundToInt(scrollPos.y);


			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical(GUILayout.Width(buttonW));
			GUILayout.FlexibleSpace();
			for (int i = 0; i < Chapters.Length; i++)
			{
				LegacyButton(Chapters[i], i);
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			GUILayout.BeginVertical();

			//  GUILayout.Label( Chapters[currentWelcomeChapter] );
			//   lastPosition = GUILayoutUtility.GetLastRect();
			// if ( !t.ContainsKey( currentWelcomeChapter ) )
			// {
			//     var res = new List<Texture2D>();
			//     var p = 0;
			//     while ( true )
			//     {
			//         var path = Adapter.HierAdapter.PluginInternalFolder + "/Documentations/Welcome Screen/Hierarchy-PRO---Welcome-Screen---S0" + currentWelcomeChapter + parts[p++] + ".png";
			//         var load = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			//         if ( !load ) break;
			//         res.Add( load );
			//     }
			//
			//     t.Add( currentWelcomeChapter, res );
			// }

			scrollPos = GUILayout.BeginScrollView(scrollPos);
			//for ( int i = 0 ; i < t[ currentWelcomeChapter ].Count ; i++ ) drawTexture( t[ currentWelcomeChapter ][ i ] );

			var texture = Root.icons.GetHelpTexture(Icons[currentWelcomeChapter], 1);
			var dif = Mathf.Clamp((position.width - buttonW) / texture.width, 0.01f, 1);
			var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.RoundToInt(texture.height * dif * 0.95f)));
			rect.height += 2;
			var color = Color.white;
			Root.p[0].gl._DrawTexture_ForExternalWindow(rect, texture, ref color);
			GUILayout.EndScrollView();

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();

			GUILayout.Space(10);
			if (GUILayout.Button("Close", GUILayout.Height(50)))
			{
				Close();
			}
		}

		// public bool Shown { get { return true; } }
	}




}
