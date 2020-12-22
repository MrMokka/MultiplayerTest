
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
	class HotButtons
	{

		PluginInstance p;
		internal HotButtons(PluginInstance _p)
		{
			p = _p;
		}

		GUIStyle _toolBarStyle;
		internal GUIStyle toolBarStyle
		{
			get
			{
				if (_toolBarStyle == null)
				{
					_toolBarStyle = new GUIStyle(EditorStyles.toolbarButton);
					_toolBarStyle.padding = new RectOffset();
					_toolBarStyle.fixedHeight = 0;
					_toolBarStyle.stretchHeight=  true;
				}

				return _toolBarStyle;
			}
		}

		GUIContent content = new GUIContent();
		internal void DrawButtonsOnTopBar()
		{
			foreach (var item in externalMod_Buttons)
			{
				var r = EditorGUILayout.GetControlRect(GUILayout.Width(p.par_e.TOPBAR_HOTBUTTON_SIZE), GUILayout.ExpandHeight(true));
				r.y += Mathf.RoundToInt((r.height - p.par_e.TOPBAR_HOTBUTTON_SIZE) / 2);
				r.height = p.par_e.TOPBAR_HOTBUTTON_SIZE;
				r.width = p.par_e.TOPBAR_HOTBUTTON_SIZE + 2;
				content.tooltip = item.text + "\n- Left-click to open window\n- Right-click to open fast context menu";
				//   GUI.DrawTexture(r, item.icon());
				// if (GUI.Button(r, content, p.STYLE_DEFBUTTON)) item.release(Event.current.button);
				content.image = Root.p[0].GetOldIcon(item.icon()).texture;
				if (GUI.Button(r, content, toolBarStyle)) item.release(Event.current.button, item.text);
				//	GUILayout.Space(2);
			}
		}
		/*internal void DrawButtonsOnHierarchyHeader(Rect startRect)
		{
			foreach (var item in externalMod_Buttons)
			{
				var r = startRect;
				r.y += Mathf.RoundToInt((r.height - p.par_e.HEADER_HOTBUTTON_SEZE) / 2);
				r.height = p.par_e.HEADER_HOTBUTTON_SEZE;
				r.x -= r.width;
				content.tooltip = item.text + "\n- Left click to open window\n- Right click to open fast context menu";
				content.image = null;
				GUI.DrawTexture(r, Root.p[0].GetOldIcon(item.icon()).texture );
				if (GUI.Button(r, content, p.STYLE_DEFBUTTON)) item.release(Event.current.button, item.text);
				startRect.x -= r.width + 1;
			}
		}*/

		/*	internal void Subscribe(EditorSubscriber sbs)
			{
			}*/

		GUIContent hot_content = new GUIContent();

		private void DrawHotButtonsInHierarchy()
		{
			var proprect = p.fullLineRect;
			proprect.y = 0;
			if (p.modsController.rightModsManager.headerEventsBlockRect.HasValue)
			{
				proprect.y = p.modsController.rightModsManager.headerEventsBlockRect.Value.y;
				proprect.x = proprect.x + proprect.width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING - p.modsController.rightModsManager.propWidth;
			}
			else
			{
				proprect.x = proprect.x + proprect.width - p.WIN_SET.LINE_HEIGHT * 2;
			}
			proprect.width = p.par_e.HEADER_HOTBUTTON_SEZE;
			if (p.par_e.DRAW_HEADER_HOTBUTTONS) //&& GUI.color.a != 0
			{//HOTS 

				var hot_r = proprect;
				//	hot_r.x -= hot_r.width;
				hot_r.width = p.par_e.HEADER_HOTBUTTON_SEZE;
				hot_r.x -= hot_r.width;
				hot_r.x -= 6;
				foreach (var item in p.modsController.toolBarModification.hotButtons.externalMod_Buttons)
				{


					//Root.SetMouseTooltip(item.text + "\n- Left click to open window\n- Right click to open fast context menu", hot_r);
					var draw_hot = hot_r;

					draw_hot.y += (draw_hot.height - p.par_e.HEADER_HOTBUTTON_SEZE) / 2;
					draw_hot.width = draw_hot.height = p.par_e.HEADER_HOTBUTTON_SEZE;
					/*	const int SH = 2;
                        draw_hot.x += SH;
                        draw_hot.y += SH;
                        draw_hot.width -= 2 * SH;
                        draw_hot.height -= 2 * SH;*/
					hot_content.tooltip = item.text + "\n- Left-click to open window\n- Right-click to open fast context menu";
					//hot_content.image = Root.p[0].GetOldIcon(item.icon()).texture;
					if (GUI.Button(draw_hot, hot_content)) item.release(Event.current.button, item.text);
					//GUI.DrawTexture(draw_hot, Root.p[0].GetOldIcon(item.icon()).texture);
					if (Event.current.type == EventType.Repaint) p.gl._DrawTexture(draw_hot, Root.p[0].GetOldIcon(item.icon()));
					hot_r.x -= hot_r.width;
					//	GUILayout.Space(2);
				}

			}
		}


		internal List<ExternalMod_Button> externalMod_Buttons = new List<ExternalMod_Button>();
		internal void RegistrateButton(EditorSubscriber sbs)
		{
			if (sbs.ExternalMod_Buttons.Count > 0) sbs.BuildedOnGUI_last += DrawHotButtonsInHierarchy;
			this.externalMod_Buttons = sbs.ExternalMod_Buttons.OrderBy(b => b.priority).ToList();
		}
	}
}
