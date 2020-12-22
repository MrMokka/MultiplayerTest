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
using UnityEditor.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{


	class Draw
	{

		static Dictionary<string, FIELD_SETTER> _setterCache_null = new Dictionary<string, FIELD_SETTER>();
		static Dictionary<object, Dictionary<string, FIELD_SETTER>> _setterCache_dic = new Dictionary<object, Dictionary<string, FIELD_SETTER>>();
		internal static FIELD_SETTER GetSetter(string name, /*PluginInstance A, bool UsePar = true, */Action<FIELD_SETTER> ac = null, Func<FIELD_SETTER, bool> valudate = null, object overrideObject = null)
		{

			var _setterCache = _setterCache_null;
			if (overrideObject != null)
			{
				if (!_setterCache_dic.ContainsKey(overrideObject)) _setterCache_dic.Add(overrideObject, new Dictionary<string, FIELD_SETTER>());
				_setterCache = _setterCache_dic[overrideObject];
			}


			if (!_setterCache.ContainsKey(name))
			{
				// var type = UsePar ? A.par_e.GetType() : A.GetType();
				var type = (overrideObject ?? Root.p[0].par_e).GetType();
				var p = type.GetProperty(name, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
				var res = new FIELD_SETTER() { overrideObject = overrideObject };

				if (p != null)
				{
					res.isprop = true;
					res.prop = p;
				}

				else
				{
					var f = type.GetField(name, ~(BindingFlags.InvokeMethod | BindingFlags.Static));
					res.field = f;
				}

				if (res.field == null && res.prop == null) throw new Exception(name + " field not found\n");

				if (ac != null) res.onChanged = ac;
				if (valudate != null) res.onValidateChange = valudate;

				// res.UsePar = UsePar;
				_setterCache.Add(name, res);
			}

			_setterCache[name].A = Root.p[0];
			return _setterCache[name];
		}



		static GUIStyle _b;

		internal static void BACK_BUTTON()
		{
			var C = GUI.color;
			GUI.color = C * Color.Lerp(new Color32(255, 150, 150, 255), Color.white, 0);
			if (_b == null)
			{
				_b = new GUIStyle(GUI.skin.button);
				_b.alignment = TextAnchor.MiddleLeft;
			}
			_b.fontSize = GUI.skin.button.fontSize;
			if (Draw.BUT("<--  Back To Main Settings", _b)) { MainSettingsEnabler_Window.Select<MainSettingsEnabler_Window>(); }
			GUI.color = C;
			Draw.Sp(10);
		}
		[NonSerialized]
		internal static int groupIndex = 0;
		[NonSerialized]
		internal static float padding = 0;

		internal static void RESET()
		{
			groupIndex = 0;
			padding = 0;
		}
		static GUIContent _calcContent = new GUIContent();
		static Rect CALC_R(GUIStyle s, string t)
		{
			_calcContent.text = t;
			var h = s.CalcHeight(_calcContent, EditorGUIUtility.currentViewWidth - CALC_PADDING - 16);
			var r = _getRerct(GUILayout.Height(h));
			return r;
		}
		static Rect _getRerct(GUILayoutOption gUILayoutOption = null)
		{
			var res = gUILayoutOption != null ? EditorGUILayout.GetControlRect(gUILayoutOption) : EditorGUILayout.GetControlRect();
			res.x = 0;
			res.width = EditorGUIUtility.currentViewWidth - 16;
			res.x += padding;
			res.width -= Math.Min(20, padding) + padding;
			return last = res;
		}
		static float CALC_PADDING { get { return padding + Math.Min(20, padding); } }
		static GUIContent ec = new GUIContent();
		internal static Rect R05 { get { return _getRerct(GUILayout.Height(Mathf.RoundToInt(EditorGUIUtility.singleLineHeight * 0.2f))); } }
		internal static Rect R15 { get { return _getRerct(GUILayout.Height(Mathf.RoundToInt(EditorGUIUtility.singleLineHeight * 1.5f))); } }
		internal static Rect R { get { return _getRerct(); } }
		internal static Rect R2 { get { return _getRerct(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)); } }
		internal static Rect RH(float h) { return _getRerct(GUILayout.Height(h)); }
		internal static Rect last;
		internal static Rect lastPlus
		{
			get
			{
				var r = last;
				r.x += 16;
				return r;
			}
		}
		static bool hover { get { return last.Contains(Event.current.mousePosition); } }
		static bool press { get { return Event.current.button == 0 && Event.current.isMouse; } }
		internal static Rect Sp(float sp)
		{
			//  GUILayout.Space( sp );
			return _getRerct(GUILayout.Height(sp));
			//GUILayout.Space( sp );
		}
		static Dictionary<string, GUIStyle> _styles = new Dictionary<string, GUIStyle>();
		static Type t;
		internal static GUIStyle s(string style)
		{
			if (_styles.ContainsKey(style)) return _styles[style];
			if (t == null)
			{
				t = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow+Styles");
				if (t == null)
				{
					t = typeof(EditorWindow).Assembly.GetType("UnityEditor.PropertyEditor+Styles");
					//if (t == null) throw new Exception("ASD");
				}
			}
			if (t == null)
				_styles.Add(style, EditorStyles.toggle);
			else
			{
				var l = new GUIStyle(t.GetField(style, ~BindingFlags.Instance).GetValue(null) as GUIStyle);
				if (style == "addComponentArea")
				{
					l.fixedHeight = 0;
					l.stretchHeight = true;
					l.padding.left = 16 + 32;
				}
				else if (style == "addComponentButtonStyle")
				{
					l = new GUIStyle(EditorStyles.toolbarButton);
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else if (style == "preToolbar")
				{
					l.fixedWidth = 0;
					l.stretchWidth = true;
					l.fixedHeight = 0;
					l.stretchHeight = true;
				}
				else
				{
					l.padding.left = 16;
				}
				l.fixedWidth = 0;
				l.stretchWidth = true;
				l.fixedHeight = 0;
				l.stretchHeight = true;
				l.alignment = TextAnchor.MiddleLeft;
				_styles.Add(style, l);
			}
			return _styles[style];
		}
		internal static bool TOG_TIT(string text, string KEY, Func<FIELD_SETTER, bool> valudate = null, Action<FIELD_SETTER> onChanged = null, float rightOffset = 0, Rect? rov = null, object overrideObject = null, string AreYouSureText = null)
		{
			//   var setter = GetSetter( KEY );
			var setter = valudate == null ? Draw.GetSetter(KEY, overrideObject: overrideObject) : Draw.GetSetter(KEY, onChanged, valudate: valudate, overrideObject: overrideObject);
			var en = setter.AS_BOOL;
			var oldP = Draw.padding;
			Draw.padding = 0;
			var r = rov ?? R2;
			if (rightOffset != 0/* && setter.AS_BOOL*/)
			{
				r.width -= last.height;
				last.width -= last.height;
			}
			if (GUI.Button(r, CONT(text), s("addComponentArea")))
			{
				if (AreYouSureText == null || EditorUtility.DisplayDialog(AreYouSureText, AreYouSureText, "Yes", "Cancel")) setter.AS_BOOL = !setter.AS_BOOL;
			}
			if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(lastPlus, ec, hover, false, en, false);
			Draw.padding = oldP;
			return setter.AS_BOOL;
		}
		internal static void TOG_TIT(string text, float rightOffset = 0)

		{
			//   var setter = GetSetter( KEY );
			var oldP = Draw.padding;
			Draw.padding = 0;
			var r = R2;
			if (rightOffset != 0)
			{
				r.width -= last.height;
				last.width -= last.height;
			}
			if (GUI.Button(r, CONT(text), s("addComponentArea"))) { };//setter.AS_BOOL = !setter.AS_BOOL;
																	  //  if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(lastPlus, ec, hover, false, en, false);
			Draw.padding = oldP;
		}
		internal static bool BUT(Rect r, string text)
		{
			if (text.StartsWith("Use ")) text = text.Substring(4);
			r.x += r.width;
			r.width = r.height;

			if (GUI.Button(r, Draw.CONT("-->", text))) return true;
			return false;
		}
		internal static bool BUT(string text, GUIStyle st = null)
		{
			var r = R15;
			r.x += 40;
			r.width -= 80;
			if (st != null)
			{
				if (GUI.Button(r, Draw.CONT(text), st)) return true;
			}
			else
			{
				if (GUI.Button(r, Draw.CONT(text))) return true;
			}
			return false;
		}
		internal static bool TOG_TIT(Rect r, string text, bool val)
		{
			if (GUI.Button(r, CONT(text), s("addComponentArea"))) val = !val;
			r.x += 16;
			if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(r, ec, r.Contains(Event.current.mousePosition), false, val, false);
			return val;
		}
		internal static bool TOG(Rect r, string text, bool val)
		{
			/*  if (GUI.Button(r, CONT(text), s("addComponentArea"))) val = !val;
              r.x += 16;
              if (Event.current.type == EventType.Repaint) EditorStyles.toggle.Draw(r, ec, r.Contains(Event.current.mousePosition), false, val, false);*/
			val = EditorGUI.ToggleLeft(r, CONT(text), val);
			return val;
		}
		internal static void TOG(string text, string KEY, Func<FIELD_SETTER, bool> valudate = null, Action<FIELD_SETTER> onChanged = null, Rect? rov = null, object overrideObject = null)
		{
			var setter = valudate == null ? Draw.GetSetter(KEY, overrideObject: overrideObject) : Draw.GetSetter(KEY, onChanged, valudate: valudate, overrideObject: overrideObject);


			//if ( Draw.padding > 20 )
			{
				var res = EditorGUI.ToggleLeft(rov ?? R15, CONT(text), setter.AS_BOOL);
				if (res != setter.AS_BOOL) setter.AS_BOOL = res;
				//if ( GUI.Button( R, text, EditorStyles.toggle ) ) setter.AS_BOOL = !setter.AS_BOOL;
				// if ( Event.current.type == EventType.Repaint ) EditorStyles.toggle.Draw( lastPlus, ec, hover, false, setter.AS_BOOL, false );
				return;
			}

			/*   var en = setter.AS_BOOL;
               var lr = R15;
               var rr = last;
               //  rr.width *= 0.25f;
               // lr.width *= 0.75f;
               rr.width = 70;
               lr.width -= rr.width;
               rr.x += lr.width;
               var c =GUI.color;
               GUI.color = new Color( 0, 0, 0, 0 );
               if ( GUI.Button( lr, Draw.CONT( text ), s( "addComponentButtonStyle" ) ) ) { setter.AS_BOOL = true; }
               if ( GUI.Button( rr, Draw.CONT( "Off" ), s( "addComponentButtonStyle" ) ) ) { setter.AS_BOOL = false; }
               if ( Event.current.type == EventType.Repaint )
               {
                   GUI.color = c;
                   // if ( !en && !lr.Contains( Event.current.mousePosition ) ) GUI.color = c * new Color( 1, 1, 1, 0.4f );
                   //  else GUI.color = c;
                   s( "addComponentButtonStyle" ).Draw( lr, text, lr.Contains( Event.current.mousePosition ), en, false, false );
                   //  if ( en && !rr.Contains( Event.current.mousePosition ) ) GUI.color = c * new Color( 1, 1, 1, 0.4f );
                   //  else GUI.color = c;
                   s( "addComponentButtonStyle" ).Draw( rr, "Off", rr.Contains( Event.current.mousePosition ), !en, false, false );
               }
               GUI.color = c;*/
		}
		static GUIStyle tb;
		internal static void TOOLBAR(string[] textArray, string KEY, float height = -1, object overrideObject = null, Rect? rect = null, string[] tooltips = null)
		{
			var setter = GetSetter(KEY, overrideObject: overrideObject);
			var b = (int)setter.value;//+ (int) stack[i].offset;
			var _R = rect ?? (height == -1 ? Draw.R15 : Draw.RH(height));
			if (tb == null)
			{
				tb = new GUIStyle(EditorStyles.toolbarButton);
				tb.fixedWidth = 0;
				tb.stretchWidth = true;
				tb.stretchHeight = true;
			}
			tb.fixedHeight = _R.height;

			var r = _R;
			r.width /= textArray.Length;
			var newb = b;
			for (int i = 0; i < textArray.Length; i++)
			{

				int controlId = GUIUtility.GetControlID(FocusType.Passive, r);
				var cont = tooltips == null ? Draw.CONT(textArray[i]) : Draw.CONT(textArray[i], tooltips[i]);
				if (GUI.Button(r, cont)) newb = i;
				if (Event.current.type == EventType.Repaint) tb.Draw(r, Draw.CONT(textArray[i]), controlId, i == b, r.Contains(Event.current.mousePosition));
				r.x += r.width;

			}

			//var newb = GUI.Toolbar(_R, b, textArray.Select(Draw.CONT).ToArray(), tb, GUI.ToolbarButtonSize.Fixed);// EditorStyles.miniButton);
			// EditorGUIUtility.AddCursorRect( _R, MouseCursor.Link );

			if (b != newb)
			{
				var res = newb;//- (int) stack[i].offset;
				/*  if ( stack[ i ].conform == null || stack[ i ].conform( res ) )
                  {
                      stack[ i ].setter.value = res;
                      ValueChanged();
                  }*/
				setter.value = res;
			}

		}
		static char[] trim = new char[] { ':' };
		internal static void FIELD(string text, string KEY, float min, float max, string postfix = null, object overrideObject = null)
		{

			var setter = GetSetter(KEY, overrideObject: overrideObject);

			var _R = Draw.R;
			var _r = _R;
			_R.width /= 1.5f;
			GUI.Label(_R, Draw.CONT(text.Trim(trim) + ":"));
			_R.x += _R.width;
			_R.width = _r.width - _R.width;

			var F = setter.type == typeof(float);
			var FX = setter.type == typeof(float?);
			// var I = setter.type == typeof(int);
			var IX = setter.type == typeof(int?);

			if (F || FX)
			{
				var v = (FX ? ((float?)setter.value).Value : (float)setter.value);
				var newv = Mathf.Clamp(S_FLOAT_FIELD(_R, v, null), min, max);
				if (v != newv) setter.value = FX ? (float?)(newv) : newv;
			}
			else
			{

				var v = (IX ? ((int?)setter.value).Value : (int)setter.value);
				var newv = Mathf.Clamp(S_INT_FIELD(_R, v, null), Mathf.RoundToInt(min), Mathf.RoundToInt(max));
				if (v != newv) setter.value = IX ? (int?)(newv) : newv;
			}
			if (!string.IsNullOrEmpty(postfix))
			{
				_R.x += _R.width;
				_R.width = 25;
				GUI.Label(_R, postfix);
			}

		}

		internal static float FIELD(string text, float v, float min, float max)
		{
			var _R = Draw.R;
			var _r = _R;
			_R.width /= 1.5f;
			GUI.Label(_R, Draw.CONT(text.Trim(trim) + ":"));
			_R.x += _R.width;
			_R.width = _r.width - _R.width;

			var newv = Mathf.Clamp(S_FLOAT_FIELD(_R, v, null), min, max);
			return newv;
		}

		internal static void STRING(string text, string KEY)
		{

			var setter = GetSetter(KEY);

			var _R = Draw.R;
			var _r = _R;
			_R.width /= 1.5f;
			GUI.Label(_R, Draw.CONT(text.Trim(trim) + ":"));
			_R.x += _R.width;
			_R.width = _r.width - _R.width;

			var v = (string)setter.value;
			var newv = EditorGUI.TextField(_R, v);
			if (v != newv)
			{
				newv = newv.Replace('\\', '/');
				var res = newv.ToCharArray().Intersect(System.IO.Path.GetInvalidPathChars()).ToArray();
				setter.value = res.Length == 0 ? "" : res.Select(c => c.ToString()).Aggregate((a, b) => a + b);
			}

		}


		internal static GUIContent CONT(string text, string tooltip)
		{
			var c = new GUIContent();
			c.text = text;
			c.tooltip = tooltip;
			return c;
		}

		static char[] trim2 = { ' ', '-' };
		internal static GUIContent CONT(string text)
		{
			var c = new GUIContent();
			c.tooltip = c.text = text;
			c.tooltip = c.tooltip.Trim(trim2);
			return c;
		}

		internal static float S_FLOAT_FIELD(Rect rect, float value, string postFix = null)
		{
			var crop = rect;
			crop.width /= 2;
			crop.x += crop.width;
			value = EditorGUI.FloatField(crop, value);

			var ac = GUI.color;
			GUI.color *= Color.clear;
			value = EditorGUI.FloatField(new Rect(0, rect.y, rect.x * 4, rect.height), "ASD", value);
			GUI.color = ac;

			// PREPARE_TEXTFIELD();
			/*  var crop = rect;
             crop.width /= 2;
              var crop1 = crop;

              crop.x += crop.width;
              GUI.SetNextControlName( "MyTextField" );


              if ( GUI.enabled ) value = EditorGUI.FloatField( crop, value );

              if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
              {
                  if ( GUI.enabled )
                  {
                      GUI.BeginClip( crop1 );
                      value = EditorGUI.FloatField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                      GUI.EndClip();
                  }
              }

              if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if (!EditorGUIUtility.isProSkin)
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
              }
              */
			// RESTORE_TEXTFIELD();

			return value;
		}


		/*  internal int S_INT_FIELD( string title , int value ) {
              var R = EditorGUILayout.GetControlRect();
              // var R = GetControlRect(false, GUILayout.Height(20));
              return S_INT_FIELD( R , title , value );
          }
          internal int S_INT_FIELD( Rect rect , string title , int value ) {

              PREPARE_TEXTFIELD();
              var result = EditorGUI.IntField(rect, title, value);
              RESTORE_TEXTFIELD();

              return result;
          }*/
		internal static int S_INT_FIELD(Rect rect, int value, string postFix = null)
		{

			var crop = rect;
			crop.width /= 2;
			crop.x += crop.width;
			value = EditorGUI.IntField(crop, value);

			var ac = GUI.color;
			GUI.color *= Color.clear;
			value = EditorGUI.IntField(new Rect(0, rect.y, rect.x * 4, rect.height), "ASD", value);
			GUI.color = ac;


			//  var crop1 = crop;

			// GUI.SetNextControlName( "MyTextField" );

			// if ( GUI.enabled ) 
			// value = EditorGUI.IntField( rect, value );

			/*    if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
                {
                    if ( GUI.enabled )
                    {
                        GUI.BeginClip( crop1 );
                        value = EditorGUI.IntField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                        GUI.EndClip();
                    }
                }*/

			/*  if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if ( !EditorGUIUtility.isProSkin )
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
                  // RESTORE_TEXTFIELD();
              }*/


			// PREPARE_TEXTFIELD();
			/*  var crop = rect;
              crop.width /= 2;
              var crop1 = crop;

              crop.x += crop.width;
              GUI.SetNextControlName( "MyTextField" );

              if ( GUI.enabled ) value = EditorGUI.IntField( crop, value );

              if ( GUI.GetNameOfFocusedControl() != "MyTextField" )
              {
                  if ( GUI.enabled )
                  {
                      GUI.BeginClip( crop1 );
                      value = EditorGUI.IntField( new Rect( 0, 0, crop1.width, rect.height ), " ", value );
                      GUI.EndClip();
                  }
              }

              if ( Event.current.type == EventType.Repaint
                  && GUI.GetNameOfFocusedControl() != "MyTextField" )
              { //if ( !EditorGUIUtility.isProSkin )
                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );

                  (Root.p[ 0 ]).GET_SKIN().textField.Draw( rect, new GUIContent( value.ToString() + (postFix ?? "") ), false, false, false, false );
                  // RESTORE_TEXTFIELD();
              }*/

			return value;
		}


		internal static void SLIDER()
		{
			/*  var R = GetControlRect();
                     var b = (stack[i].IsNullable ? ((int?) stack[i].setter.value).Value : (int) stack[i].setter.value) + (int) stack[i].offset;
                     var newb = (A.S_SLIDER(R, TOGGLE_PTR + stack[i].text, b, (int) stack[i].min, (int) stack[i].max));

                     if (b != newb)
                     {
                         if (stack[i].IsNullable) stack[i].setter.value = (int?) (newb - (int) stack[i].offset);
                         else stack[i].setter.value = newb - (int) stack[i].offset;

                         ValueChanged();
                     }*/

			throw new Exception();
		}


		internal static void COLOR(string text, string KEY, object overrideObject = null)
		{
			// var R = stack[i].UseLastRect ? lastRect : GetControlRect();
			var _R = R15;
			// if ( stack[ i ].UseLastRect ) _R.width += 90;

			_R.width -= 85;

			if (text != null) GUI.Label(_R, CONT(text.Trim() + ":"));
			var setter = GetSetter(KEY, overrideObject: overrideObject);
			var b = (Color)setter.value;
			var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), text, b);

			if (b != newb)
			{
				setter.value = newb;
			}
		}


		internal static Color COLOR(Rect r, Color oldColor)
		{
			var _R = r;
			_R.width -= 85;
			var newb = Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), null, oldColor);
			return newb;
		}

		internal static Color COLOR(Rect r, GUIContent text, Color oldColor)
		{
			var _R = r;
			_R.width -= 85;
			//	if (text != null) GUI.Label(_R, CONT(text.Trim() + ":"));
			GUI.Label(_R, text);
			return Tools.PICKER(new Rect(_R.x + _R.width, _R.y - 3, 85, 23), text.tooltip, oldColor);
		}
		static Color oldc;
		internal static void HELP(string text, Color? c = null, bool drawTog = false)
		{
			//  EditorGUI.LabelField( R, text, s( "previewMiniLabel" ) );
			var _s = s("previewMiniLabel");
			_s.wordWrap = true;

			if (c.HasValue)
			{
				oldc = GUI.color;
				GUI.color *= c.Value;
			}
			if (drawTog) text = "· " + text;
			var ca = _s.normal.textColor;
			if (!EditorGUIUtility.isProSkin) _s.normal.textColor = new Color32(20, 20, 20, 255);
			EditorGUI.TextArea(CALC_R(_s, text), text, _s);
			_s.normal.textColor = ca;
			if (c.HasValue) GUI.color = oldc;
			// GUI.Label( CALC_R( _s, text ), text, _s );

		}

		internal static void HELP_TEXTURE(string v)
		{


			Draw.Sp(10);
			var t = Root.icons.GetHelpTexture(v);
			var rect = RH(t.height);
			rect.x += (rect.width - t.width) / 2;
			rect.width = t.width;

			if (EditorGUIUtility.currentViewWidth < rect.width)
			{
				rect.x += (rect.width - EditorGUIUtility.currentViewWidth) / 2;
				rect.width = Mathf.RoundToInt(EditorGUIUtility.currentViewWidth);
			}
			rect.x = Mathf.RoundToInt(rect.x);

			Color c = Color.white;
			if (!GUI.enabled) c.a = 0.4f;
			Root.p[0].gl._DrawTexture_ForExternalWindow(rect, t, ref c);
			Draw.Sp(10);
		}

		internal static void HELP_TEXTURE(Rect r, string v)
		{
			var t = Root.icons.GetHelpTexture(v);
			r.x += r.width - t.width;
			r.width = t.width;
			r.y -= 1;
			r.height = t.height;

			Color c = Color.white;
			if (!GUI.enabled) c.a = 0.4f;
			Root.p[0].gl._DrawTexture_ForExternalWindow(r, t, ref c);
		}


		internal static void HRx4RED()
		{
			Sp(4);

			Draw.HRx2();
			//EditorGUI.DrawRect(Draw.R2, Color.red);
			var c = GUI.color;
			GUI.color = Color.red;
			Draw.HRx2();
			GUI.color = c;
			/*	var r = R05;
				if (Event.current.type == EventType.Repaint)
					s("dragHandle").Draw(r, ec, false, false, false, false);*/
			//Sp(12);
			Draw.HRx2();
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp(4);
		}

		internal static void HRx2()
		{
			Sp(4);
			var r = R05;
			if (Event.current.type == EventType.Repaint)
				s("dragHandle").Draw(r, ec, false, false, false, false);
			Sp(12);
			/*   r = R05;
              if ( Event.current.type == EventType.Repaint )
                  s( "dragHandle" ).Draw( r, ec, false, false, false, false );*/
			Sp(4);
		}
		internal static void EXPAND(string text)
		{
			GUI.Button(R, text, s("preDropDown"));
		}

		internal static Rect Grow(Rect p, int v)
		{
			v = -v;
			p.x += v;
			p.y += v;
			p.width -= v * 2;
			p.height -= v * 2;
			return p;
		}
	}












	internal class CLASS_ENALBE
	{
		internal class dsp : IDisposable
		{
			internal PluginInstance A;
			internal float usePadding;
			internal bool enableStack;
			internal Color col;

			public void Dispose()
			{

				Draw.padding -= 20;
				Draw.padding -= usePadding;
				GUI.color = col;
				GUI.enabled = enableStack;// A.DRAW_STACK.ENABLE_RESTORE(); //ENABLE
			}
		}

		internal PluginInstance A;
		internal dsp USE(string setter)
		{
			return USE(Draw.GetSetter(setter));
		}
		internal dsp USE(string setter, object overrideObject)
		{
			return USE(Draw.GetSetter(setter, overrideObject: overrideObject));
		}
		internal dsp USE(string setter, bool inverce = false, object overrideObject = null)
		{
			return USE(Draw.GetSetter(setter, overrideObject: overrideObject), inverce);
		}
		internal dsp USE(FIELD_SETTER setter, bool inverce = false)
		{
			Draw.padding += 20;
			var o = new dsp() { A = A, enableStack = GUI.enabled, col = GUI.color };
			var was = GUI.enabled;
			GUI.enabled &= inverce ? !setter.AS_BOOL : setter.AS_BOOL;
			if (!GUI.enabled && was) GUI.color *= new Color(1, 1, 1, 0.4f);
			//   A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			return o;
		}

		internal dsp USE(string setter, float padding, bool inverce = false, object overrideObject = null)
		{
			return USE(Draw.GetSetter(setter, overrideObject: overrideObject), padding, inverce);
		}

		internal dsp USE(FIELD_SETTER setter, float padding, bool inverce = false)
		{
			// A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			// A.DRAW_STACK.PADDING_SET( padding );
			//  setter.value

			// Draw.padding += 20;
			padding -= 20;
			var res = USE(setter, inverce);
			res.usePadding = padding;
			Draw.padding += padding;
			return res;
			//  return new dsp() { A = A, usePadding = true };
		}

		internal dsp USE(bool v)
		{
			var o = new dsp() { A = A, enableStack = GUI.enabled, col = GUI.color };
			o.usePadding = -20;
			var was = GUI.enabled;
			GUI.enabled &= v;
			if (!GUI.enabled && was) GUI.color *= new Color(1, 1, 1, 0.4f);
			//   A.DRAW_STACK.ENABLE_SET( setter ); //ENABLE
			return o;
		}
	}




	internal class CLASS_GROUP
	{
		internal class dsp : IDisposable
		{
			// internal bool UseSearchSet;
			internal PluginInstance A;
			internal CLASS_GROUP c;
			internal float p;
			internal int groupIndex;
			public void Dispose()
			{
				// A.DRAW_STACK.END_GROUP();
				GUILayout.EndVertical();
				Draw.padding -= p;
				var res = Draw.Sp(5);
				if (res.y != 0)
				{
					if (!lastRect.ContainsKey(groupIndex)) lastRect.Add(groupIndex, Rect.zero);
					lastRect[groupIndex] = res;
				}

				//   if ( UseSearchSet ) A.DRAW_STACK.SEARCH_SET( null ); //SEARCH
			}
		}
		Rect firstRect;
		static Dictionary<int, Rect> lastRect = new Dictionary<int, Rect>();
		// Rect lastRect;
		internal PluginInstance A;
		static GUIContent ec = new GUIContent();

		internal dsp UP(float padding = 20)
		{
			// A.DRAW_STACK.BEGIN_GROUP();
			Draw.groupIndex++;
			var p = Draw.Sp(5);
			firstRect = p;
			if (Event.current.type == EventType.Repaint)
			{
				if (lastRect.ContainsKey(Draw.groupIndex))
				{
					var l = lastRect[Draw.groupIndex];
					p.height = l.y + l.height - p.y;
					p.x = 0;
					p.width = EditorGUIUtility.currentViewWidth - 16;
					// GUI.skin.box.Draw( p, ec, false, false, false, false );

					Draw.s("preToolbar").Draw(p, ec, false, false, false, false);
					//      p.x += 12;
					//  p.width -= 24;
					//  Draw.s( "preBackground" ).Draw( Draw.Grow(p, -3), ec, false, false, false, false );
				}
			}
			var c = GUI.color;
			GUI.color *= new Color(1, 1, 1, 0.5f);


			GUILayout.BeginVertical(); //m_ProgressBarBack  m_Tooltip
			GUI.color = c;
			Draw.padding += padding;
			// groupBegin = true;
			return new dsp() { A = A, c = this, p = padding, groupIndex = Draw.groupIndex };
		}

		/* internal dsp UP( string searchSet )
         {
             A.DRAW_STACK.SEARCH_SET( searchSet ); //SEARCH
             A.DRAW_STACK.BEGIN_GROUP();
             return new dsp() { A = A, UseSearchSet = true };
         }*/

	}


	internal class FIELD_SETTER
	{


		internal static void ValueChanged(PluginInstance A)
		{
			//  A.SavePrefs();
			//  A.RESET_DRAW_STACKS();
			Cache.ClearHierarchyObjects(false);
			//  A.RepaintWindowInUpdate();
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}


		internal bool isprop;
		internal FieldInfo field;
		internal PropertyInfo prop;
		internal PluginInstance A;
		internal bool UsePar = true;
		internal Func<FIELD_SETTER, bool> onValidateChange;
		internal Action<FIELD_SETTER> onChanged;
		internal object overrideObject = null;

		// object cachedValue
		internal object value
		{
			get
			{
				if (UsePar) return isprop ? prop.GetValue(overrideObject ?? A.par_e, null) : field.GetValue(overrideObject ?? A.par_e);
				else return isprop ? prop.GetValue(A, null) : field.GetValue(A);
			}

			set
			{
				if (this.value == value) return;


				if (onChanged != null && !onValidateChange(this)) return;

				if (UsePar)
				{
					object t = overrideObject ?? A.par_e;

					if (isprop) prop.SetValue(t, value, null);
					else field.SetValue(t, value);

					//A.par_e = (EditorSettingsAdapter)t;
				}

				else
				{
					if (isprop) prop.SetValue(A, value, null);
					else field.SetValue(A, value);
				}

				if (onChanged != null) onChanged(this);

				ValueChanged(A);
			}
		}
		internal Type type { get { return this.isprop ? this.prop.PropertyType : this.field.FieldType; } }
		public bool AS_BOOL
		{
			get
			{
				var t = this.isprop ? this.prop.PropertyType : this.field.FieldType;
				bool res = false;

				if (t == typeof(bool))
				{
					res |= (bool)this.value;
					//if ( this2 != null ) res |= (bool)this2.value;
				}

				else if (t == typeof(bool?))
				{
					res |= (bool?)this.value ?? true;
					// if ( this2 != null ) res |= (bool?)this2.value ?? true;
				}

				else if (t == typeof(int))
				{
					res |= (int)this.value != 0;
					// if ( this2 != null ) res |= (int)this2.value == 1;
				}

				else if (t == typeof(int?))
				{
					res |= ((int?)this.value ?? 0) != 0;
					//if ( setter2 != null ) res |= ((int?)setter2.value ?? 0) == 1;
				}
				return res;
			}
			set
			{
				var t = this.isprop ? this.prop.PropertyType : this.field.FieldType;
				if (t == typeof(bool)) this.value = value;
				else if (t == typeof(bool?)) this.value = (bool?)value;
				else if (t == typeof(int)) this.value = value ? 1 : 0;
				else if (t == typeof(int?)) this.value = (int?)(value ? 1 : 0);
			}
		}
	}
}
