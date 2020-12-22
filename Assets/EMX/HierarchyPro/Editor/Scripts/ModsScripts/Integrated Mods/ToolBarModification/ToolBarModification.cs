using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace EMX.CustomizationHierarchy
{
	public class ExtensionInterface_TopBarOnGUI
	{
		/// <summary>
		/// Rect - windowSize;
		/// </summary>
		static public Action<Rect> onLeftLayoutGUI;
		/// <summary>
		/// Rect - windowSize;
		/// </summary>
		static public Action<Rect> onRightLayoutGUI;
	}
}
namespace EMX.HierarchyPlugin.Editor.Mods
{



	//   [InitializeOnLoad]
	class ToolBarModification
	{

		PluginInstance p;
		internal HotButtons hotButtons;
		internal LayoutsMod layoutsMod;
		internal ToolBarModification(PluginInstance p)
		{
			this.p = p;
			hotButtons = new HotButtons(p);
			layoutsMod = new LayoutsMod(p);
		}



		// static object chld;

		// static ScriptableObject last;
		//  static EventInfo   disable;
		object oldtb;
		Type tb;
		FieldInfo gettb;
		object gettvvalue
		{
			get
			{
				var _tb = tb ?? (tb = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar") ?? throw new Exception("Toolbar")) ?? throw new Exception("Toolbar");
				return (gettb ?? (gettb = _tb.GetField("get", ~(BindingFlags.Instance)))).GetValue(null);
			}
		}
		void Update()
		{

			var newtb = gettvvalue;
			if (!ReferenceEquals(newtb, oldtb))
			{
				oldtb = newtb;
				if (!p.par_e.DRAW_TOPBAR_LAYOUTS_BAR || !p.par_e.ENABLE_ALL) return;
				//Debug.Log("A" + ReferenceEquals(newtb, oldtb));
				_install(false, false);
			}

			if (!installed) return;
			reinstall();
		}
		bool installed = false;
		Action reinstall = null;

		internal void Remove(EditorSubscriber sbs)
		{
			if (!oldAssigned) return;
			oldAssigned = false;
			Install(sbs, false, true);
		}
		bool oldAssigned = false;
		object oldContainer = null;
		static object lastMain = null;
		internal void Install(EditorSubscriber sbs, bool resinstall, bool remove = false)
		{

			if (sbs != null)
			{
				sbs.OnUpdate += Update;
				if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR && p.par_e.ENABLE_ALL) layoutsMod.Subscribe(sbs);
			}

			_install(resinstall, remove);

		}
		Action customGui;
		void _install(bool resinstall, bool remove)
		{

			if (resinstall)
			{
				lastMain = null;
			}
			if (remove)
			{
				lastMain = null;
				SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
			}

			if (ReferenceEquals(lastMain, gettvvalue)) return;
			//Debug.Log(ReferenceEquals(lastMain, m));
			lastMain = gettvvalue;

			var b = typeof(EditorWindow).Assembly.GetType("UnityEditor.View") ?? throw new Exception("View");
			var v = typeof(EditorWindow).Assembly.GetType("UnityEditor.GUIView") ?? throw new Exception("GUIView");
			var tb = typeof(EditorWindow).Assembly.GetType("UnityEditor.Toolbar") ?? throw new Exception("Toolbar");
			var ll = tb.GetProperty("lastLoadedLayoutName", ~(BindingFlags.Instance)) ?? throw new Exception("lastLoadedLayoutName");
			var p3 = b.GetProperty("position", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("position");
#if UNITY_2020_1_OR_NEWER
			var be = v.GetProperty("windowBackend", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("windowBackend");
			var t2 = typeof(UnityEditor.UIElements.Toolbar).Assembly.GetType("UnityEditor.UIElements.DefaultWindowBackend") ?? throw new Exception("DefaultWindowBackend");
			var r = t2.GetField("imguiContainer", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("imguiContainer");
			var ongui_type = r.FieldType;
			Func<object, object> get_r = (w) =>
			 {
				 var def_w = be.GetValue(w, null);
				 return r.GetValue(def_w);
			 };
			Action<object, object> set_r = (w, s) =>
			{
				var def_w = be.GetValue(w, null);
				r.SetValue(def_w, s);
			};
#else
			var r = v.GetProperty("imguiContainer", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("imguiContainer");
			var ongui_type = r.PropertyType;
			Func<object, object> get_r = (w) =>
			{
				return r.GetValue(w, null);
			};
			Action<object, object> set_r = (w, s) =>
			{
				 r.SetValue(w, s, null);
			};
			//var pp = v.GetProperty("panel", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("panel");
			//var mp = v.GetField("m_Panel", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_Panel");
#endif
			var ongui = ongui_type.GetField("m_OnGUIHandler", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_OnGUIHandler");
			var c = b.GetField("m_Children", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("m_Children");
			ScriptableObject m = null;
			List<ScriptableObject> targets = new List<ScriptableObject>();
			foreach (var item in Resources.FindObjectsOfTypeAll<ScriptableObject>())
			{
				if (!b.IsAssignableFrom(item.GetType())) continue;
				if (item.GetType().Name != "MainView") continue;
				// var test = (object[])c.GetValue(item);
				// var es = test[0] as ScriptableObject;
				// if ( !es ) continue;
				/* var test = (object[])c.GetValue(item);
                 if ( resinstall && r.GetValue( test[ 0 ], null ) == null )
                 {
                     Debug.Log( "skip" );
                     continue;
                 }*/
				targets.Add(item);
				//m = item;
				break;
			}

			installed = false;
			//  Debug.Log( targets.Count );
			if (targets.Count == 0)
			{
				if (remove) throw new Exception("Remove Exception");
				EditorApplication.delayCall += () =>
				{
					Install(null, true);
				};
				return;
			}

			//  var OnDisable= t[ 0 ].GetType().GetMethod("OnDisable",  ~(BindingFlags.InvokeMethod | BindingFlags.Static));
			//  var OnEnable=  t[ 0 ].GetType().GetMethod("OnEnable",  ~(BindingFlags.InvokeMethod | BindingFlags.Static));

			/*  EditorApplication.CallbackFunction up = null;
              up = () =>
              {
                  if (r.GetValue(t[0], null) == null)
                  {
                      EditorApplication.update -= up;
                      // Debug.Log(last);
                      //  Debug.Log((bool)last);
                      // if (disable != null) disable.DynamicInvoke(null);
                      // Application.Run(exFormAsObj);
                      //  disable.
                      // if ( last ) OnDisable.Invoke( last, null );
                      Install(null, true);
                  }
              };
              EditorApplication.update += up;*/


			m = targets[0];
			var t = (object[])c.GetValue(m);

			// Debug.Log( "install" );
			object inst;
			MethodInfo OldOnGUI = null;
			if (!remove)
			{

				installed = true;
				reinstall = () =>
				{
					if (get_r(t[0]) == null)
					{
						Install(null, true);
					}
				};



				customGui = () =>
				{
					//Debug.Log(Event.current.type);

					(OldOnGUI ?? (OldOnGUI = t[0].GetType().GetMethod("OldOnGUI", ~(BindingFlags.GetProperty | BindingFlags.Static)))).Invoke(t[0], null);

					// GUI.Label(new Rect(0,0,20,20), "ASD");
					float o1 = 416f;
					var _p = (Rect)p3.GetValue(t[0], null);
					float shrink = (_p.width - 1000) / 920 * 50;
					int _bp = Mathf.RoundToInt((float)((_p.width - 140.0) / 2.0));
					var _b1 = new Rect(o1, 0, _bp - o1 - shrink - 10, _p.height);
					_b1.x += p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET;
					_b1.width -= p.par_e.TOPBAR_LEFT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_LEFT_MAX_BORDER_OFFSET;

					GUILayout.BeginArea(_b1);
					GUILayout.BeginHorizontal();
					if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT) { if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers(); }
					else if (p.par_e.DRAW_TOPBAR_HOTBUTTONS) hotButtons.DrawButtonsOnTopBar();
					/*  if (GUILayout.Button("save" , GUILayout.ExpandHeight(true)))
					  {
						  chld = c.GetValue(m);
					  }
					  if (GUILayout.Button("load" , GUILayout.ExpandHeight(true)))
					  {
						  c.SetValue(m, chld);
						  Install(true);
						  InternalEditorUtility.RepaintAllViews();
					  }*/
					if (p.par_e.DRAW_TOPBAR_CUSTOM_LEFT) if (EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onLeftLayoutGUI != null) EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onLeftLayoutGUI(_p);
					GUILayout.EndHorizontal();
					GUILayout.EndArea();


					int o2 = Mathf.RoundToInt((float)((_p.width + 40) / 2.0) + shrink);
					o2 += 16;
					var _b2 = new Rect(o2, 0, (_p.width - 850) / 2.2f - shrink, _p.height);
					_b2.x += p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET;
					_b2.width -= p.par_e.TOPBAR_RIGHT_MIN_BORDER_OFFSET - p.par_e.TOPBAR_RIGHT_MAX_BORDER_OFFSET;

					GUILayout.BeginArea(_b2);
					GUILayout.BeginHorizontal();
					if (p.par_e.TOPBAR_SWAP_LEFT_RIGHT) { if (p.par_e.DRAW_TOPBAR_HOTBUTTONS) hotButtons.DrawButtonsOnTopBar(); }
					else if (p.par_e.DRAW_TOPBAR_LAYOUTS_BAR) layoutsMod.DrawLayers();
					if (p.par_e.DRAW_TOPBAR_CUSTOM_RIGHT) if (EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onRightLayoutGUI != null) EMX.CustomizationHierarchy.ExtensionInterface_TopBarOnGUI.onRightLayoutGUI(_p);
					GUILayout.EndHorizontal();
					GUILayout.EndArea();

				};

				try
				{
				}
				catch { }
				// last = t[ 0 ] as ScriptableObject;
				inst = Activator.CreateInstance(ongui_type, customGui);

				if (!oldAssigned)
				{
					oldAssigned = true;
					oldContainer = get_r(t[0]);
				}
			}
			else
			{
				installed = false;
				reinstall = null;
				customGui = () =>
				{
					(OldOnGUI ?? (OldOnGUI = t[0].GetType().GetMethod("OldOnGUI", ~(BindingFlags.GetProperty | BindingFlags.Static)))).Invoke(t[0], null);
				};
				inst = Activator.CreateInstance(ongui_type, customGui);
				//inst = oldContainer;
			}



			if (get_r(t[0]) == null) set_r(t[0], inst);
			// OnDisable.Invoke( t[ 0 ], null );


			/*  var d = Delegate.CreateDelegate(typeof(Action), null, OnDisable);
               MethodInfo addHandler = disable.GetAddMethod();
   object[] addHandlerArgs = { d };
   addHandler.Invoke(t[ 0 ], addHandlerArgs);*/
			//  var old = mp.GetValue( t[ 0 ] );
			//  if ( old != null ) old.GetType().BaseType.GetMethods().First( q => q.Name == "Dispose" && q.GetParameters().Length == 0 ).Invoke( old, null );

			// pp.GetValue( t[ 0 ], null );

			//#EMX_TODO kMax Count Callback registaiotion
			var toolbar_integrated = SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
			LayoutsMod.InitProperties();

			//bool needRegistrate = false;
			//Debug.Log((string)ll.GetValue(null, null));
			{

				//var old = mp.GetValue(t[0]);
				//if (old != null)
				{
					//owner = old.GetType().BaseType.GetProperty("ownerObject", ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(old, null) as ScriptableObject ?? throw new Exception("ownerObject");
					//var utiputy = typeof(UnityEngine.UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.UIElementsUtility") ?? throw new Exception("UIElementsUtility");
					//var rmw = utiputy.GetMethod("RemoveCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RemoveCachedPanel");
					//rmw.Invoke(null, new object[] { owner.GetInstanceID() });
					//needRegistrate = true;
				}
				SessionState.SetBool("EXM_TOOLBAR_BOOL", true);
				SessionState.SetString("EXM_TOOLBAR_LASTPATH", (string)ll.GetValue(null, null));
			}


#if !UNITY_2020_1_OR_NEWER
			//mp.SetValue(t[0], null);
#endif

			var cont = get_r(t[0]);
			var ac = (Action)ongui.GetValue(cont);
			ac = customGui;
			ongui.SetValue(cont, ac);

			//r.SetValue(t[0], inst, null);


			if (!toolbar_integrated || (string)ll.GetValue(null, null) != SessionState.GetString("EXM_TOOLBAR_LASTPATH", ""))
			{
				if (inst != null)
				{
					//UnityEngine.UIElements.VisualElementExtensions.StretchToParentSize(inst as UnityEngine.UIElements.VisualElement);
					//(r.PropertyType.GetField("useOwnerObjectGUIState", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("useOwnerObjectGUIState")).SetValue(inst, true);
					//(r.PropertyType.BaseType.GetProperty("viewDataKey", ~(BindingFlags.InvokeMethod | BindingFlags.Static)) ?? throw new Exception("viewDataKey")).SetValue(inst, "Dockarea", null);
				}
			}


			//var panel =
#if !UNITY_2020_1_OR_NEWER
			//pp.GetValue(t[0], null);
#endif

			//	if (needRegistrate)
			{
				/*	owner = panel.GetType().BaseType.GetProperty("ownerObject", ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(panel, null) as ScriptableObject ?? throw new Exception("ownerObject");
					var utiputy = typeof(UnityEngine.UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.UIElementsUtility") ?? throw new Exception("UIElementsUtility");
					var rmw = utiputy.GetMethod("RemoveCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RemoveCachedPanel");
					rmw.Invoke(null, new object[] { owner.GetInstanceID() });
					var add = utiputy.GetMethod("RegisterCachedPanel", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance)) ?? throw new Exception("RegisterCachedPanel");
					add.Invoke(null, new object[] { owner.GetInstanceID(), panel });*/
			}

			if (remove)
			{
				lastMain = null;
				SessionState.GetBool("EXM_TOOLBAR_BOOL", false);
			}
			/*   if ( mp.GetValue( t[ 0 ] ) != null )
               {
                   Debug.Log( "ASD" );
                   var a = mp.GetValue( t[ 0 ] );
                   var re = a.GetType().GetProperty("visualTree",  ~(BindingFlags.InvokeMethod | BindingFlags.Static)).GetValue(a);
                   re.GetType().GetMethod( "Insert", ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static) ).Invoke( re, new object[] { 0, inst } );
               }*/
			/*
            var pd = v.GetMethod("UpdateDrawChainRegistration",  ~(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static)  );

            pd.Invoke( t[ 0 ], new object[] { true } );

            var ev = typeof( UnityEngine.UIElements.VisualElement ).Assembly.GetType( "UnityEngine.UIElements.Panel" );
            var bef = ev.GetField("BeforeUpdaterChange",  ~(BindingFlags.InvokeMethod | BindingFlags.Instance) );
            var aft = ev.GetField("AfterUpdaterChange",  ~(BindingFlags.InvokeMethod | BindingFlags.Instance) );
            bef.SetValue( null, ((Action)bef.GetValue( null )) + (( ) => { pd.Invoke( t[ 0 ], new object[] { false } ); }) );
            bef.SetValue( null,  (Action)(( ) => { Debug.Log("QWE"); }) );
            aft.SetValue( null, ((Action)aft.GetValue( null )) + (( ) => { pd.Invoke( t[ 0 ], new object[] { true } ); }) );*/
			// OnEnable.Invoke( t[ 0 ], null );
			//r.SetValue( t[ 0 ], inst, null );
		}

		ScriptableObject owner;



		/*    public static void SaveGUI()
    {
      UnityEditor.SaveWindowLayout.Show(WindowLayout.FindMainView().screenPosition);
    }

    public static void DeleteGUI()
    {
      DeleteWindowLayout.Show(WindowLayout.FindMainView().screenPosition);
    }
*/
	}
}
