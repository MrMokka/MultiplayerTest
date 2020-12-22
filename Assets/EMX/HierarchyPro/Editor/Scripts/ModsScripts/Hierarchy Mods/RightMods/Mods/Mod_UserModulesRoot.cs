using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;




namespace EMX.HierarchyPlugin.Editor
{
	/*  class HierarchyCallBackModule : ICallBackModule
      {
          HierarchyCallBackModule(Adapter.Module module )
          {
              obj = module;
              typeFillter = module.GetType();
          }
    
          public object obj { get; set; }
          public Type typeFillter { get; set; }
          public FillterData.FillterData_Inputs callFromExternal_objects { get; set; }
          public void Draw(Rect rect, GameObject gameObject)
          {
              throw new NotImplementedException();
          }
      }
    */

	internal class Mod_UserModulesRoot : RightModBaseClass
	{


		internal override void Subscribe(EditorSubscriber sbs) { }
		internal override bool enableOverride()
		{
			return Assigned && adapter.par_e.USE_COMPONENTS_ICONS_MOD;
		}

		internal override string enableOverrideMessage()
		{
			return adapter.par_e.USE_COMPONENTS_ICONS_MOD ? " - Not Assigned" : " - Disabled in settings";
		}

		public Mod_UserModulesRoot(int restWidth, int sibbildPos, bool enable, PluginInstance adapter) : base(restWidth, sibbildPos, enable, adapter)
		{
			CustomHierarchyMod.m_OpenIntInput_W = SHOW_INT_W;
			CustomHierarchyMod.m_OpenIntInput = SHOW_INT;
			CustomHierarchyMod.m_OpenStringInput_W = SHOW_STRING_W;
			CustomHierarchyMod.m_OpenStringInput = SHOW_STRING;
			CustomHierarchyMod.m_OpenDropDownMenu = SHOW_DROPDOWN;
		}

		CustomHierarchyMod customModule;
		internal bool Assigned;
		internal string HeaderText_pref;
		internal string ContextHelper_pref;
		Rect? stateForDrag_B0;



		internal override object SetCustomModule(object _customModule)
		{
			var customModule = _customModule as CustomHierarchyMod;
			Assigned = customModule != null;
			this.customModule = customModule;

			if (Assigned)
			{
				HeaderText = "[C]" + customModule.NameOfModule;
				ContextHelper = ContextHelper_pref + " - " + customModule.NameOfModule;
			}

			else
			{
				HeaderText = HeaderText_pref;
				ContextHelper = ContextHelper_pref;
			}

			return this;
		}


		void CallFilterWindow(HierarchyObject _o)
		{
			var mp = new MousePos(Event.current.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
			var o = _o.go;


			Windows.SearchWindow.Init(mp, HeaderText + " '" + (customModule.ToString(o) ?? "-") + "'", (customModule.ToString(o) ?? "-"),
				  Validate(o) ? CallHeaderFiltered(customModule.ToString(o)) : CallHeader(),
				  this, Root.p[0].firstWindow(0), _o);
		}

		static Rect tr;

		public override void Draw()
		{
			if (!Assigned) return;

			//if (Adapter.OPT_EV_BR(Event.current)) return 0;

			var o = adapter.o.go;
			//  bool down = false;
			//  if (Event.current.rawType == EventType.MouseUp && Event.current.button == 1) down = true;

			/*  if (stateForDrag_B0.HasValue)
              {
                  //var screen = EditorGUIUtility.GUIToScreenPoint(new Vector2(stateForDrag_B0.Value.x, stateForDrag_B0.Value.y));
                 // var tt = new Rect(screen.x, screen.y, stateForDrag_B0.Value.width, stateForDrag_B0.Value.height);
                  var pos = (Event.current.mousePosition);
            
                  if (Event.current.rawType == EventType.MouseUp  )
                  {
                      //  Debug.Log(Event.current.rawType + " " + stateForDrag_B0.Value + " " + Event.current.mousePosition);
            
                      // Debug.Log(tt + " " + screen);
            
                      if (stateForDrag_B0.Value.Contains(pos))
                      //     if (stateForDrag_B0.Value.Contains(Event.current.mousePosition))
                      {
                          Adapter.EventUse();
                          CallFilterWindow(o);
                      }
                      stateForDrag_B0 = null;
                  }
            
            
            
                  if (stateForDrag_B0.HasValue && stateForDrag_B0.Value.Contains(pos))
                  {
                      // Debug.Log(Event.current.rawType + " " + stateForDrag_B0.Value + " " + Event.current.mousePosition);
            
                      if (EditorGUIUtility.isProSkin) GUI.DrawTexture(stateForDrag_B0.Value, Adapter.GET_SKIN().button.active.background);
                      else GUI.DrawTexture(stateForDrag_B0.Value, Texture2D.whiteTexture);
                  }
              }
            
            
              //  Undo.RecordObject(o, "GameObject SetActive");
            
            
              if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && drawRect.Contains(Event.current.mousePosition))
              {
            
                  stateForDrag_B0 = drawRect;
                  // adapter.bottomInterface.
                  Adapter.EventUse();
                  //  down = true;
              }*/


			/*  if (Event.current.type == EventType.MouseDrag && Event.current.button == 1 && drawRect.Contains(Event.current.mousePosition))
              {
                  Adapter.EventUse();
              }*/


			/* if (tR.Contains(Event.current.mousePosition))
             {
            
                 if (tR.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown)
                 {
                     //EditorUtility.SetObjectEnabled( markedObjects[ instanceID ], !markedObjects[ instanceID ].activeInHierarchy );
                     /*   if (objectIsHiddenAndLock)
                        {
                            // o.SetActive(!o.activeSelf);
                        }
                        else#1#
            
                     if (Event.current.button == 0)
                     {
                         var targetO = new[] { o };
                         var sel = SELECTED_GAMEOBJECTS();
                         if (sel.Contains(o) /*&& Event.current.control#1#)
                         {
                             // targetO = sel.Where(g => g.GetComponentsInParent<Transform>(true).Count(p => sel.Contains(p.gameObject)) == 1).Select(g => g.gameObject).ToArray();
                             targetO = Utilities.GetOnlyTopObjects(sel);
                             /*  for (int i = 0; i < targetArray.Count; i++)
                               {
            
                               } #1#
                         }
                     }
                 }
             }*/
			/* if ((down ) || stateForDrag_B0.HasValue)
             {
                 if (adapter.ModuleButton(drawRect, ""))
                 {
                     CallFilterWindow(o);
                 }
             }*/


			if (Event.current.button == adapter.MOUSE_BUTTON_1)
			{
				tr.Set(drawRect.x, drawRect.y - 1, drawRect.width, drawRect.height);

				if (adapter.ModuleButton(tr, null, true))
				{
					CallFilterWindow(adapter.o);
				}
			}


			if (customModule != null)
			{
				/*   if ( HierarchyExtensions.Styles.button == null )
				   {
					   HierarchyExtensions.Styles.button = new GUIStyle( adapter.STYLE_DEFBUTTON );
					   HierarchyExtensions.Styles.button.alignment = TextAnchor.MiddleLeft;
					   HierarchyExtensions.Styles.button.clipping = TextClipping.Clip;
				   }*/

				// HierarchyExtensions.Styles.button.normal.textColor = adapter.button.normal.textColor;




				/*HierarchyExtensions.Styles.button.fontSize = adapter.STYLE_DEFBUTTON.fontSize;
                    var b = Adapter.GET_SKIN().button.normal.textColor;
                var a = Adapter.GET_SKIN().button.alignment;
                Adapter.GET_SKIN().button.normal.textColor = Adapter.GET_SKIN().label.normal.textColor;
                Adapter.GET_SKIN().button.alignment = TextAnchor.MiddleLeft;*/

				/// GUI.BeginClip( drawRect );
				/// drawRect.x = 0;
				///  drawRect.y = 0;

				if (Event.current.button != 1 || !Event.current.isMouse)
				{
					//try
					{
						customModule.Draw(drawRect, o, adapter.hashoveredItem && adapter.hoverID == adapter.o.id);
					}

				/*	catch (Exception ex) // adapter.logProxy.LogWarning( "CustomModule: " + ex.Message + " " + ex.StackTrace );
					{ // Debug.LogWarning( "CustomModule: " + ex.Message + " " + ex.StackTrace );
						LogProxy.LogError("CustomModule: " + ex.Message + " " + ex.StackTrace);
					}*/
				}

				///  GUI.EndClip();

				/*  Adapter.GET_SKIN().button.normal.textColor = b;
                  Adapter.GET_SKIN().button.alignment = a;*/
			}


		}




		static void SHOW_INT(int i, Action<int> action)
		{
			SHOW_INT_W("New Value", i, action);
		}

		static void SHOW_INT_W(string windowName, int i, Action<int> action)
		{ /* if ( Event.current == null ) {
			     LOGERROR();
			     return;
			 }*/

			Action result = () =>
			{
				Action<string> convertAction = (str) =>
				{
					int reslt;

					if (int.TryParse(str, out reslt)) action(reslt);
				};

				var adapter = Root.p[0];

				var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, true, adapter);
				// var pos = InputData.WidnwoRect(FocusRoot.WidnwoRectType.Clamp, Event.current.mousePosition, 190, 68, Hierarchy.adapter);
				Windows.InputWindow.InitTeger(pos, windowName, Root.p[0].firstWindow(0), convertAction, null, i.ToString());
			};

			if (Event.current == null)
			{
				Root.p[0].PUSH_GUI_ONESHOT(0,result);
				// Initializator.AdaptersByName[ Initializator.HIERARCHY_NAME ].GUI_ONESHOTPUSH( result );
				//LOGERROR();
				//  return;
			}

			else
			{
				result();
			}
		}

		/** SHOW_INT */
		static void SHOW_STRING(string s, Action<string> action)
		{
			SHOW_STRING_W("New Value", s, action);
		}

		static void SHOW_STRING_W(string windowName, string s, Action<string> action)
		{
			Action result = () =>
			{
				//    var adapter = Initializator.AdaptersByName[Initializator.HIERARCHY_NAME];
				var adapter = Root.p[0];

				var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_190_68, true, adapter);
				// var pos = InputData.WidnwoRect(FocusRoot.WidnwoRectType.Clamp, Event.current.mousePosition, 190, 68, Hierarchy.adapter);
				Windows.InputWindow.Init(pos, windowName, Root.p[0].firstWindow(0), action, null, s);
			};

			if (Event.current == null)
			{
				// Initializator.AdaptersByName[ Initializator.HIERARCHY_NAME ].GUI_ONESHOTPUSH( result );
				Root.p[0].PUSH_GUI_ONESHOT(0,result);
				//LOGERROR();
				//  return;
			}

			else
			{
				result();
			}
		}

		/** SHOW_STRING */
		static void LOGERROR()
		{
			LogProxy.LogWarning("Input windows can only be called from the OnGUI method");
		}


		static void SHOW_DROPDOWN(int i1, string[] strings, Action<int> arg3, Action<string> arg4)
		{
			if (Event.current == null)
			{
				LOGERROR();
				return;
			}


			GenericMenu menu = new GenericMenu();

			for (int i = 0; i < strings.Length; i++)
			{
				var ind = i;
				menu.AddItem(new GUIContent(strings[i]), i == i1, () =>
				{
					try
					{
						arg3(ind);
					}

					catch (Exception ex)
					{
						LogProxy.LogError("Changing Index: " + ex.Message + " " + ex.StackTrace);
					}
				});
			}

			// var adapter = Initializator.AdaptersByName[Initializator.HIERARCHY_NAME];
			var adapter = Root.p[0];


			if (arg4 != null)
			{
				menu.AddSeparator("");
				var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_128_68, true, adapter);
				//  var pos = InputData.WidnwoRect(FocusRoot.WidnwoRectType.Clamp, Event.current.mousePosition, 128, 68, Hierarchy.adapter);
				menu.AddItem(new GUIContent("New Item"), false, () =>
				{
					Windows.InputWindow.Init(pos, "New Item", Root.p[0].firstWindow(0), (str) =>
					{
					  if (string.IsNullOrEmpty(str)) return;

					  str = str.Trim();

					  try
					  {
						  arg4(str);
					  }

					  catch (Exception ex)
					  {
						  LogProxy.LogError("Adding Item: " + ex.Message + " " + ex.StackTrace);
					  }
				  });
				});
			}


			menu.ShowAsContext();
			Tools.EventUse();
		}







		bool Validate(GameObject o)
		{
			return !string.IsNullOrEmpty(customModule.ToString(o));
		}



		/** CALL HEADER */
		internal override Windows.SearchWindow.FillterData_Inputs CallHeader()
		{
			var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = (oo) => Validate(oo.go),
				SelectCompareString = (d, i) => customModule.ToString(d.go),
				SelectCompareCostInt = (d, i) =>
				{
					var cost = i;
					cost += d.go.activeInHierarchy ? 0 : 100000000;
					return cost;
					/*var cost = customModule.ToString(d.go).GetHashCode();
                    unchecked
                    {
                        cost += d.Validate() ? 0 : 1000000000;
                        if (cost < 0) cost = int.MaxValue;
                    }
                    return cost;*/
				}
			};
			return result;
		}

		internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered(string filter)
		{
			var result = CallHeader();
			result.Valudator = s => Validate(s.go) && customModule.ToString(s.go) == filter;
			return result;
		}
		/** CALL HEADER */

	}
}
