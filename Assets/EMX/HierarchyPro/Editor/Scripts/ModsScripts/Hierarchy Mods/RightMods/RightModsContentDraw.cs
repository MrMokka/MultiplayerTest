using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor.Mods
{


	partial class RightModsManager
	{

		Color oldG1;
		Color oldG2;
		void DrawModulesContent()
		{
			//??
			//button.normal.textColor *= o.Active() ? Color.white : new Color( 1, 1, 1, 0.5f );

			if (p.EVENT.type == EventType.Layout) return;

			var HIDE_MODULES = p.modsController.rightModsManager.CheckSpecialButtonIfRightHidingEnabled();
			if (HIDE_MODULES) return;



			// var width = selectionRect.x + selectionRect.width;

			var sel = p.par_e.RIGHT_SHOWMODS_ONLY_IFHOVER ? p.ha.IsSelected(p.o.id) : false;

			if (p.par_e.RIGHT_SHOWMODS_ONLY_IFHOVER && !p.par_e.RIGHT_USE_HIDE_ISTEAD_LOCK && !p.EVENT.alt && p.hashoveredItem && p.hoverID != p.o.id && !sel) return;

			for (int i = 0; i < p.window.modsCount; i++)
			{

				// foreach ( var drawModule in modulesOrdered )
				//SKIP

				if (!p.window.tempModsData[i].enabled) break;/// throw new Exception( "mod disable content" );

				var targetMod = p.window.tempModsData[i].targetMod;

				/*  currentRect.width = Math.Max( drawModule.width, defWDTH );
                  currentRect.x -= currentRect.width;
                  var MIN = par.PADDING_LEFT;
                  if ( w != null && MIN > width - RIGHTPAD ) MIN = width - RIGHTPAD;
                  bool fade = (currentRect.x < MIN);
                  currentRect = ClipMINSizeRect( currentRect, width );
                  if ( currentRect.width < 2 ) continue;*/
				//  FadeRect(currentRect, par.HEADER_OPACITY ?? DefaultBGOpacity);


				var headerRect = p.window.tempModsData[i].rect;
				var fade = p.window.tempModsData[i].fade_narrow;

				if (fade)
				{
					oldG1 = GUI.color;
					oldG2 = GUI.contentColor;
					var c = GUI.color;
					var t = headerRect.width / targetMod.savedData.width * 2;
					c.a = Mathf.SmoothStep(0, 1, t);
					GUI.color = c;
					c = GUI.contentColor;
					c.a = Mathf.SmoothStep(0, 1, t);
					GUI.contentColor = c;
				}


				//DRAG
				//if ( p.window.CurrentRectContainsKey( w, drawModule ) ) drawRect.x = CurrentRect( w, drawModule ).x;
				headerRect.y = p.fullLineRect.y;

				targetMod.callFromExternal_objects = null;
				targetMod.drawRect = headerRect;
				targetMod.Draw();

				if (fade)
				{
					GUI.color = oldG1;
					GUI.contentColor = oldG2;
				}

				if (targetMod.CURRENT_STACK != null) throw new Exception("Cache not finilizing " + targetMod);

			} // foreach

		}









		//VERTICAL LINES
		Color32 S_COL1 = new Color32(8, 8, 8, 50);
		Color32 S_COL2 = new Color32(132, 132, 132, 50);

		Color32 S_COL3 = new Color32(8, 8, 8, 20);
		Color32 S_COL4 = new Color32(255, 255, 255, 50);
		Rect vR;
		void DrawVerticalLines()
		{

			vR.y = p._first_FullLineRect.y;
			vR.height = p._last_FullLineRect.y + p._last_FullLineRect.height - p._first_FullLineRect.y;
			vR.width = 1;

			for (int i = 0; i < p.modsController.rightModsManager.rightMods.Length; i++)
			{

				// foreach ( var drawModule in modulesOrdered )
				//SKIP
					/* drawRect.width = 2;

				GUI.DrawTexture(drawRect, GetIcon("SEPARATOR"));
				GUI.DrawTexture(drawRect, GetIcon("SEPARATOR")); */
					// Adapter.DrawRect( drawRect, S_COL3 );
					// Adapter.DrawRect( drawRect, S_COL4 );
				if (!p.window.tempModsData[i].enabled) break;

				vR.x = p.window.tempModsData[i].rect.x;


				if (!EditorGUIUtility.isProSkin)
				{
					vR.x -= 1;
					EditorGUI.DrawRect(vR, S_COL3);
					vR.x += 1;
					EditorGUI.DrawRect(vR, S_COL4);
				}
				else
				{
					vR.x -= 1;
					EditorGUI.DrawRect(vR, S_COL1);
					vR.x += 1;
					EditorGUI.DrawRect(vR, S_COL2);
				}

				if (i == 0)
				{
					vR.x += p.window.tempModsData[i].rect.width;
					if (!EditorGUIUtility.isProSkin)
					{
						vR.x -= 1;
						EditorGUI.DrawRect(vR, S_COL3);
						vR.x += 1;
						EditorGUI.DrawRect(vR, S_COL4);
					}
					else
					{
						vR.x -= 1;
						EditorGUI.DrawRect(vR, S_COL1);
						vR.x += 1;
						EditorGUI.DrawRect(vR, S_COL2);
					}
				}
			}


		}
	}
}
