using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods
{



	public abstract class ExternalModRoot : EditorWindow
	{



		internal abstract void SubscribeEditorInstance(EditorSubscriber sbs);

		//internal ExternalWindowRoot targetWindow = null;
		internal virtual bool Alive()
		{
			return currentWindow;
		}

		internal void RepaintNow()
		{
			if (currentWindow) currentWindow.Repaint();
		}


		internal ExternalDrawContainer __controller;
		internal ExternalDrawContainer controller { get { return __controller ?? (__controller = new ExternalDrawContainer(pluginID)); } }
		Events.MouseRawUp _mouse_uo_helper;
		internal PluginInstance adapter { get { return Root.p[0]; } }
		internal Events.MouseRawUp mouse_uo_helper { get { return _mouse_uo_helper ?? (_mouse_uo_helper = new Events.MouseRawUp()); } }
		internal EditorWindow currentWindow;

		abstract internal int pluginID { get; }

		Vector2 size = new Vector2(300, 200);

		bool wasInit = false;
		internal void Init(bool skipPos = false)
		{
			if (wasInit) return;
			wasInit = true;

			if (!skipPos && position.x < 10 && position.y < 10)
			{
				position = new Rect(WinBounds.MAX_WINDOW_WIDTH.x + WinBounds.MAX_WINDOW_WIDTH.y / 2 - size.x / 2, WinBounds.MAX_WINDOW_HEIGHT.x + WinBounds.MAX_WINDOW_HEIGHT.y / 2 - size.y / 2, size.x, size.y);
			}
			//base.Init();
		}

		internal void PUSH_EVENT_HELPER_RAW()
		{
			controller.CHECK_MOUSE_UP();
			controller.EVENT_MOUSE_UP();
			var repaint = false;

			/*			if (controlIDsAndOnMouseUp.Count != 0)
						{
							foreach (var controlID in controlIDsAndOnMouseUp.Values.ToArray())
							{
								controlID();
							}

							controlIDsAndOnMouseUp.Clear();
							repaint = true;
						}*/


			if (repaint)
			{
				RepaintNow();
			}
		}

		internal void PUSH_ONMOUSEUP(Action ac)
		{
			mouse_uo_helper.PUSH_ONMOUSEUP(ac, this);
		}


		static Type _InspectorType;
		internal static Type InspectorType
		{
			get { if (_InspectorType == null) _InspectorType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow"); return _InspectorType; }
		}


		internal bool WAS_INIT = false;



		public void OnGUI()
		{
			if (Event.current.type == EventType.Layout) return;
			controller.tempRoot = this;
			if (!OnGUI_Check()) return;

			OnGUI_Draw();

			OnGUI_Post();
		}

		internal abstract void OnGUI_Draw();

		internal bool OnGUI_Check()
		{


			WAS_INIT = false;
			if (!currentWindow)
			{
				currentWindow = this;
				if (!currentWindow) return false;
				WAS_INIT = true;
			}


			if (!adapter.par_e.ENABLE_ALL)
			{
				GUI.Label(new Rect(0, 0, position.width, position.height), "" + Root.PN + " Disabled!", adapter.STYLE_LABEL_10_middle);
				return false;
			}




			return true;
		}

		internal void OnGUI_Post()
		{


			if ((Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout))
			{
				controller.EVENT_UPDATE();
			}

			if (Event.current.rawType == EventType.MouseUp)
			{
				controller.EVENT_MOUSE_UP();
			}



			if (controller.currentAction != null)
			{
				if (!controller.currentAction(false, adapter.deltaTime))
					Repaint();
			}


			if (Event.current.rawType == EventType.MouseUp)
			{
				controller.CHECK_MOUSE_UP();
			}



			if (mouse_uo_helper.Invoke())
			{
				RepaintNow();
				if (Event.current.isMouse) Event.current.Use();
			}



		}





		internal virtual void OnEnable()
		{
			Root.SET_EXTERNAl_MOD(this);

		}
		internal virtual void OnDisable()
		{
			Root.REMOVE_EXTERNAl_MOD(this);
		}


	}
}
