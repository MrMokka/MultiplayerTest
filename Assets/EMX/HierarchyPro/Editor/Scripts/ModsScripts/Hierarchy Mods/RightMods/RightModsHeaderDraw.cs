using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEngine.UIElements;



namespace EMX.HierarchyPlugin.Editor.Mods
{


	class ModSavedData
	{
		PluginInstance p;
		string key;
		int defSibling;
		int defWidth;
		bool defEnabled;
		internal ModSavedData(string key, PluginInstance p, int defSibling, int defWidth, bool defEnabled)
		{
			this.key = key;
			this.p = p;
			this.defSibling = defSibling;
			this.defWidth = defWidth;
			this.defEnabled = defEnabled;
		}

		internal void SetToDefault()
		{
			sibling = defSibling;
			width = defWidth;
			enabled = defEnabled;
		}
		internal bool NowDefault()
		{
			return defEnabled == enabled && sibling == defSibling;
		}
		internal int sibling { get { return Math.Max(0, p.par_e.GET(key + "_sibling", defSibling)); } set { p.par_e.SET(key + "_sibling", value); } }
		internal int width { get { return Math.Max(RightModsManager.MIN_MOD_WIDTH, p.par_e.GET(key + "_width", defWidth)); } set { p.par_e.SET(key + "_width", value); } }
		internal bool enabled
		{
			get { return p.par_e.GET(key + "_enabled", defEnabled); }
			set
			{
				if (enabled == value) return;
				p.par_e.SET(key + "_enabled", value);
				Root.p[0].modsController.REBUILD_PLUGINS(false);
			}
		}
	}

	partial class RightModsManager
	{


		Rect firstDrawRect;
		void AllocateRightMods(Rect lineRect)
		{

			if (p.EVENT.type == EventType.Layout) return;

			if (!p.window.oldPositionWidth.HasValue) p.window.oldPositionWidth = p.window.position.width;
			if (p.window.oldPositionWidth != p.window.position.width && p.EVENT.type == EventType.Repaint)
			{
				p.window.oldPositionWidth = p.window.position.width;
				for (int i = 0; i < rightMods.Length; i++)
				{
					p.window.modulesPosesX[i] = -1;
				}
			}

			///disable
			p.window.modsCount = 0;
			bool skip = false;
			for (int i = 0; i < rightMods.Length; i++)
			{
				skip = false;
				if (!rightMods[i].savedData.enabled)
				{
					skip = true;
				}
				if (rightMods[i] is Mod_UserModulesRoot)
				{
					if (!p.par_e.RIGHT_USE_CUSTOMMODULES) skip = true;
					else
					{
						var um = (Mod_UserModulesRoot)rightMods[i];
						if (!um.Assigned) skip = true;
					}
				}
				if (skip)
				{
					p.window.tempModsData[i].enabled = false;
					p.window.modulesPosesX[i] = -1;
					continue;
				}

				p.window.tempModsData[i].enabled = true;
				///if (p.window.tempModsData[ i ].sibling == 2) Debug.Log(p.window.tempModsData[ i ].width + " " + p.EVENT.type);
				///
				p.window.tempModsData[i].sibling = rightMods[i].savedData.sibling;
				p.window.tempModsData[i].width = rightMods[i].savedData.width;
				p.window.tempModsData[i].targetMod = rightMods[i];
				p.window.tempModsData[i].width = Math.Max(rightMods[i].savedData.width, MIN_MOD_WIDTH);
				p.window.tempModsData[i].i_targetMod = i;

				var T = p.window.tempModsData[p.window.modsCount];
				p.window.tempModsData[p.window.modsCount] = p.window.tempModsData[i];
				p.window.tempModsData[i] = T;
				p.window.modsCount++;
			}
			p.window.tempModsData[rightMods.Length].enabled = false;

			///sort
			for (int i = 0; i < p.window.modsCount - 1; i++)
			{
				for (int x = 0; x < p.window.modsCount - 1; x++)
				{
					if (p.window.tempModsData[x].sibling == p.window.tempModsData[x + 1].sibling)
					{
						var cs = p.window.tempModsData[x].sibling;
						var cm = p.window.tempModsData[x].targetMod;
						for (int z = 0; z < p.window.modsCount - 1; z++)
						{
							if (p.window.tempModsData[x].sibling >= cs && p.window.tempModsData[x].targetMod != cm)
							{
								p.window.tempModsData[x].sibling++;
								p.window.tempModsData[x].targetMod.savedData.sibling++;
							}
						}

					}
					if (p.window.tempModsData[x].sibling > p.window.tempModsData[x + 1].sibling)
					{
						var m = p.window.tempModsData[x + 1];
						p.window.tempModsData[x + 1] = p.window.tempModsData[x];
						p.window.tempModsData[x] = m;
					}
				}


			}
			int checkSib = -1;
			for (int i = 0; i < p.window.modsCount - 1; i++)
			{
				if (p.window.tempModsData[i].sibling < checkSib) throw new Exception(p.window.tempModsData[i].targetMod + " siblings sorting error " + p.window.tempModsData[i].sibling);
				checkSib = p.window.tempModsData[i].sibling;
			}

			//assign data
			var width = lineRect.x + lineRect.width;
			firstDrawRect = lineRect;
			firstDrawRect.x = firstDrawRect.x + firstDrawRect.width - p.rightOffset;
			for (int i = 0; i < p.window.modsCount; i++)
			{
				p.window.tempModsData[i].enabled = false;

				firstDrawRect.width = p.window.tempModsData[i].width;
				firstDrawRect.x = width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING;
				firstDrawRect.x -= firstDrawRect.width;





				if (p.window.modulesPosesX[p.window.tempModsData[i].i_targetMod] == -1)
					p.window.modulesPosesX[p.window.tempModsData[i].i_targetMod] = (int)firstDrawRect.x;

				//EVENTS ANIMTION
				if (p.window.mouseEvent != null || p.window.mouseEventDrag != null ||
					 p.window.modulesPosesX[p.window.tempModsData[i].i_targetMod] != firstDrawRect.x)
				{
					// CurrentRectInit( p.window, p.window.tempModsData[ i ], currentRect );
					//  if ( p.EVENT.type == EventType.Repaint )
					{
						var WR = firstDrawRect;
						float vx = p.window.modulesPosesX[p.window.tempModsData[i].i_targetMod];
						///var v = WR;
						///v.x = width - v.x;
						//WR.x = width - WR.x;
						if (p.EVENT.type == EventType.Repaint) vx = Mathf.MoveTowards(vx, WR.x, p.deltaTime * 1500.6f);
						bool rep = false;
						if (vx != WR.x) rep = true;
						//WR.x = width - WR.x;
						//v.x = width - v.x;
						p.window.modulesPosesX[p.window.tempModsData[i].i_targetMod] = vx;
						WR.x = vx;
						firstDrawRect = WR;
						if (rep && (p.EVENT.type == EventType.Repaint))
						{
							//p.RESET_DRAWSTACK(p.window.pluginID);
							//p.RepaintWindow(p.window.pluginID, true);
							p.RepaintWindowInUpdate_PlusResetStack(p.window.pluginID);
						}

					}
				}
				else
				{
					///p.window.tempModsData[ i ].rect = firstDrawRect;
				}

				firstDrawRect = ClipMINSizeRect((Rect)firstDrawRect, width);
				if (firstDrawRect.width < 1) continue;
				p.window.tempModsData[i].enabled = true;

				p.window.tempModsData[i].rect = firstDrawRect;

				bool fade_narrow = (firstDrawRect.x < p.par_e.RIGHT_PADDING_LEFT_READABLE);
				p.window.tempModsData[i].fade_narrow = fade_narrow;
				//	var w = p.window.tempModsData[ i ].rect;
				//	EditorGUI.DrawRect( new Rect( w.x + 2, w.y + 2, w.width - 4, w.height - 4 ), Color.white );

				p.rightOffset += p.window.tempModsData[i].rect.width;



			}
		}

		internal float propWidth { get { return EditorGUIUtility.singleLineHeight; } }
		Rect opasRect;
		Color col04 = new Color(1, 1, 1, 0.4f);
		Rect DrawHeader(Rect lineRect)
		{

			// var width = lineRect.x + lineRect.width;
			// var  _dr = lineRect;
			//  _dr.x = _dr.x + _dr.width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING;

			var old_guicolor = GUI.color;
			var old_contentcolor = GUI.contentColor;
			Rect? lastRect = null;


			//FADE SCENE RECT
			if (p.par_e.RIGHT_BG_OPACITY > 0.01f && GUI.color.a == 0)
			{

				var HIDE_MODULES = p.modsController.rightModsManager.CheckSpecialButtonIfRightHidingEnabled();
				if (!HIDE_MODULES)
				{
					var c = Colors.SceneColor;
					c.a = p.par_e.RIGHT_BG_OPACITY;
					opasRect.x = lineRect.x + lineRect.width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING;
					opasRect.width = lineRect.x + lineRect.width - opasRect.x;
					opasRect.y = p._first_FullLineRect.y;
					opasRect.height = p._last_FullLineRect.y + p._last_FullLineRect.height - p._first_FullLineRect.y;

					if (!p.par_e.USE_DINAMIC_BATCHING)
					{
						var gc = GUI.color;
						gc.a = 1;
						GUI.color = gc;
						EditorGUI.DrawRect(opasRect, c);
						gc.a = 0;
						GUI.color = gc;
					}
					else
					{
						p.gl.DrawFade(opasRect, c);
					}
				}
			}
			// var w = p.window;
			for (int i = 0; i < p.window.modsCount; i++)
			{

				if (!p.window.tempModsData[i].enabled) break; //throw new Exception( "mode disabled" );
				if (p.window.tempModsData[i].width == 0) throw new Exception(" p.window.tempModsData[i].width == 0");
				var targetMod = p.window.tempModsData[i].targetMod;


				bool fade_narrow = p.window.tempModsData[i].fade_narrow;
				var modRect = p.window.tempModsData[i].rect;
				lastRect = modRect;






				if (fade_narrow)
				{
					Debug.Log(targetMod);
					var c = GUI.color;
					c.a = Mathf.Lerp(0, 1, modRect.width / p.window.tempModsData[i].width);
					GUI.color = c;
					c = GUI.contentColor;
					c.a = Mathf.Lerp(0, 1, modRect.width / p.window.tempModsData[i].width);
					GUI.contentColor = c;
				}




				targetMod.CLIP_VECTOR.Set(modRect.x, modRect.width);

				// GUI.BeginClip( (Rect)_dr );
				// _dr.x = 0;
				//  _dr.y = 0;

				if (targetMod.headOverrideTexture != null && GUI.color.a != 0)
				{
					/*  var oldC = GUI.color;
						var c = GUI.color;
						c.a = 0.4f;
						GUI.color = c;*/
					EditorGUI.DrawRect(modRect, targetMod.headOverrideTexture.Value * col04);
					/*  GUI.color = oldC;*/
				}


				if (targetMod.HeaderTexture2D != null)
				{
					//tCont.image = null;
					//  tCont.text = "";
					tCont.tooltip = targetMod.ContextHelper;
					if (p.par_e.RIGHT_MOD_BROADCAST_ENABLED) tCont.tooltip += "\nEnabled Broadcast Memory Optimizer!\n(This may reduce performance)";
					//  GUI.Label( drawRect, tCont, STYLE_LABEL_10_middle );
					Root.SetMouseTooltip(tCont, (Rect)modRect);

					var oldW = modRect.width;
					var oldH = modRect.height;
					modRect.height = modRect.width = Mathf.Min(13, modRect.width);
					modRect.x += (oldW - modRect.width) / 2;
					modRect.y += (oldH - modRect.width) / 2;

					bool needBack = false;
					if (!EditorGUIUtility.isProSkin && targetMod is Mod_Freeze)
					{
						colCache = GUI.color;
						GUI.color = grayFreezee * old_guicolor;
						needBack = true;
					}

					p.gl._DrawTexture(modRect, p.GetNewIcon(NewIconTexture.RightMods, targetMod.HeaderTexture2D));
					if (needBack) GUI.color = old_guicolor * colCache;
				}
				else
				{
					tCont.image = null;
					tCont.text = targetMod.HeaderText;
					tCont.tooltip = targetMod.ContextHelper;
					if (p.par_e.RIGHT_MOD_BROADCAST_ENABLED) tCont.tooltip += "\nEnabled Broadcast Memory Optimizer!\n(This may reduce performance)";
					////	var rrr = modRect;
					////	rrr.width += 100;
					////	rrr.x -= 50;
					GUI.Label(modRect, tCont, p.STYLE_LABEL_10_middle_clip);
					//  Debug.Log(p.STYLE_LABEL_10_middle_clip.fontSize + " " +p.FONT_10());
				}



				//DRAG AREA;
				{
					var drag_r = lastRect ?? modRect;
					drag_r.x += 6;
					drag_r.width -= 9;
					EditorGUIUtility.AddCursorRect(drag_r, MouseCursor.MoveArrow);
					if (drag_r.Contains(p.EVENT.mousePosition) && p.EVENT.type == EventType.MouseDown && p.EVENT.button == 0 && p.window.mouseEventDrag == null && p.window.mouseEvent == null)
					{
						p.window.captured_mod = targetMod;
						p.window.captured_mod_arr = i;
						///Debug.Log( p.window.captured_mod_arr + " " + p.window.tempModsData[ p.window.captured_mod_arr + 1 ].width );
						Tools.EventUse();
						var startRect = modRect;
						var startMouse = GUIUtility.GUIToScreenPoint(p.EVENT.mousePosition);
						var startX = startRect.x;
						//p.window.DragRect = startRect;
						var w = p.window;
						//mouseEventW = window();
						//   Debug.Log(startX + " " + GUIUtility.GUIToScreenPoint(p.EVENT.mousePosition));
						p.window.mouseEventDrag = (b, deltaTime) =>
						 {

							 w.DragRect = startRect;
							 float DIF = -(int)startMouse.x + (int)GUIUtility.GUIToScreenPoint(Event.current.mousePosition).x;
							 w.DragRect.x = DIF + startX;
							 //  Debug.Log(-(int)startMouse.x + (int)GUIUtility.GUIToScreenPoint( p.EVENT.mousePosition ).x);
							 //  LogProxy.Log(DragRect.x + " " + startRect.x);
							 /*  var max = rightMods.Max(m => m.savedData.sibling);
								 // if (DragRect.x + startRect.width * 0.65f < startRect.x)
								 var upnext = w.captured_mod.savedData.sibling;
								 var cached_upnext = upnext;
								 while ( cached_upnext < max )
								 {
										 cached_upnext++;
										 var f = rightMods.FirstOrDefault(m => m.savedData.sibling == cached_upnext);
										 if ( f == null || !f.savedData.enabled ) continue;
										 upnext = cached_upnext;
										 break;
								 }

								 //  if (DragRect.x > startRect.x + startRect.width * 0.65f)
								 var downnext = w.captured_mod.savedData.sibling;
								 var cached_downnext = downnext;
								 while ( cached_downnext > 0 )
								 {
										 cached_downnext--;
										 var f = rightMods.FirstOrDefault(m => m.savedData.sibling == cached_downnext);
										 if ( f == null || !f.savedData.enabled ) continue;
										 downnext = cached_downnext;
										 // startRect.x += curMod.width;
										 break;
								 }

								 var next = w.captured_mod.savedData.sibling;
								 if ( upnext != w.captured_mod.savedData.sibling && w.DragRect.x - startRect.x < -rightMods.First( m => m.savedData.sibling == upnext ).savedData.width * 0.6f ) next = upnext;
								 if ( downnext != w.captured_mod.savedData.sibling && w.DragRect.x - startRect.x > +rightMods.First( m => m.savedData.sibling == downnext ).savedData.width * 0.6f ) next = downnext;



								 if ( next != w.captured_mod.savedData.sibling ) //  LogProxy.Log(next);
								 {
										 // __modulesOrdered = null;

										 // lastEditTime = EditorApplication.timeSinceStartup;
										 var target = rightMods.First(m => m.savedData.sibling == next);

										 if ( next < w.captured_mod.savedData.sibling ) startRect.x += target.savedData.width;
										 else startRect.x -= target.savedData.width;

										 target.savedData.sibling = w.captured_mod.savedData.sibling;
										 w.captured_mod.savedData.sibling = next;
								 }*/
							 //   var upnext = w.tempModsData[w.captured_mod_arr].sibling;
							 //  var downnext = w.tempModsData[w.captured_mod_arr].sibling;
							 TempModData? nextMod = null;
							 const float M = 0.6f;
							 bool move_left, move_right;
							 if (move_right = (w.captured_mod_arr != 0 && DIF > +w.tempModsData[w.captured_mod_arr - 1].width * M))
								 nextMod = w.tempModsData[w.captured_mod_arr - 1];
							 if (move_left = (
									w.captured_mod_arr < w.tempModsData.Length - 1
									&& w.tempModsData[w.captured_mod_arr + 1].enabled
									&& DIF < -w.tempModsData[w.captured_mod_arr + 1].width * M))
								 nextMod = w.tempModsData[w.captured_mod_arr + 1];
							 // Debug.Log( DIF + " " + w.captured_mod_arr + " " + w.tempModsData[ w.captured_mod_arr + 1 ].width );
							 /*Debug.Log( DIF + " " +  (-w.tempModsData[ w.captured_mod_arr + 1 ].width * M) + " " + 
									   w.tempModsData[ w.captured_mod_arr - 1 ].width * M );*/
							 if (nextMod.HasValue)
							 {

								 var o = nextMod.Value.targetMod.savedData.sibling;
								 nextMod.Value.targetMod.savedData.sibling = w.tempModsData[w.captured_mod_arr].targetMod.savedData.sibling;
								 w.tempModsData[w.captured_mod_arr].targetMod.savedData.sibling = o;
								 // var increase = nextMod.Value.sibling >  w.tempModsData[w.captured_mod_arr].sibling;
								 if (move_left)
								 {
									 /// Debug.Log( "ASD" );
									 ///startRect.x -= nextMod.Value.width;
									 startMouse.x -= nextMod.Value.width;
									 startX -= nextMod.Value.width;
									 w.captured_mod_arr++;
								 }
								 if (move_right)
								 {
									 /// startRect.x += w.tempModsData[ w.captured_mod_arr ].width;
									 startMouse.x += nextMod.Value.width;
									 startX += nextMod.Value.width;
									 w.captured_mod_arr--;
								 }
								 // Debug.Log( DIF + " " + w.captured_mod_arr + " " + w.tempModsData[ w.captured_mod_arr + 1 ].width );
							 }

							 if (Event.current.delta.x != 0)
							 {
								 w.allow = false;
							 }

							 if (b) //MonoBehaviour.print("ASD");
							 {
								 if (startMouse == GUIUtility.GUIToScreenPoint(Event.current.mousePosition))
								 {
									 w.mouseEventDrag = null;
									 LeftClickOnRightModsHeaderMenu.Open(false, menu =>
															   {
																   menu.AddItem(new GUIContent("[ Hide This Module ☓ ]"), false, () =>
																					  {
																						  //   captured_mod.CreateUndo();
																						  w.captured_mod.savedData.enabled = false;
																						  //   captured_mod.SetDirty();
																					  });
																   menu.AddSeparator("");
															   }
															 );
									 /*var menu = new GenericMenu();
									 menu.AddSeparator("");
									 menu.ShowAsContext();*/
								 }

								 //  w.captured_mod.SetDirty();
							 }
						 };
						p.RepaintWindow(p.window.pluginID, true);
					}
					if (p.window.mouseEvent != null && p.window.captured_mod == targetMod) p.gl.DRAW_TAP_GLOW(modRect);
					if (p.window.mouseEventDrag != null && p.window.captured_mod == targetMod) p.gl.DRAW_TAP_GLOW(p.window.DragRect);
				}


				//RESIZE AREA;
				{
					var resize_r = lastRect ?? modRect;
					resize_r.width = 1;
					Rect line_r = resize_r;
					var oh = line_r.height;
					line_r.height /= 1.5f;
					line_r.y += (oh - line_r.height) / 2;
					///if ( GUI.color.a != 0 ) EditorGUI.DrawRect( line_r, Color.gray );
					resize_r.x -= 4;
					resize_r.width = 10;
					var _dr_rightmax = resize_r.x + resize_r.width;

					EditorGUIUtility.AddCursorRect((Rect)resize_r, MouseCursor.SplitResizeLeftRight);
					if (p.window.mouseEvent != null && p.window.captured_mod != null)
					{
						var cr = resize_r;
						cr.width = 200;
						cr.height = 200;
						cr.x = p.EVENT.mousePosition.x - 100;
						cr.y = p.EVENT.mousePosition.y - 100;

						EditorGUIUtility.AddCursorRect(cr, MouseCursor.SplitResizeLeftRight);
					}

					if (resize_r.Contains(p.EVENT.mousePosition) && p.EVENT.type == EventType.MouseDown && p.EVENT.button == 0 && p.window.mouseEventDrag == null && p.window.mouseEvent == null)
					//if (GUI.Button(drawRect, "") && p.EVENT.button == 0)
					{
						p.window.allow = true;
						p.window.captured_mod = targetMod;
						//  curMod.CreateUndo();
						Tools.EventUse();

						// if (init != null) Undo.RecordObject(init, "Change Hierarchy");
						var startMouse = GUIUtility.GUIToScreenPoint(p.EVENT.mousePosition);
						//var startWidth = (int)modRect.width;
						var w = p.window;
						var startWidth = (int)w.captured_mod.savedData.width;
						p.window.mouseEvent = (b, deltaTime) =>
						 {
							 //

							 var dif = (int)startMouse.x - (int)GUIUtility.GUIToScreenPoint(Event.current.mousePosition).x;
							 p.RESET_DRAWSTACK(w.pluginID);
							 w.captured_mod.savedData.width = dif + startWidth;
							 if (w.captured_mod.savedData.width > _dr_rightmax - 20) w.captured_mod.savedData.width = (int)_dr_rightmax - 20;

							 if (w.captured_mod.savedData.width < MIN_MOD_WIDTH) w.captured_mod.savedData.width = MIN_MOD_WIDTH;
							 // if ( b ) w.captured_mod.SetDirty();
							 if (!b && Event.current.delta.x != 0)
							 {
								 w.allow = false;
							 }
						 };
						p.RepaintWindow(p.window.pluginID, true);
					}
				}




				//MENU OPEN
				{
					//_dr.x = CurrentRect( w, p.window.tempModsData[ i ] ).x;
					// _dr.y = 0;



					if (modRect.Contains(p.EVENT.mousePosition) && p.EVENT.type == EventType.MouseDown && p.EVENT.button == p.MOUSE_BUTTON_1 && p.window.mouseEventDrag == null && p.window.mouseEvent == null)
					{
						Tools.EventUse();

						var captureRect = modRect;
						var captureModule = targetMod;
						var w = p.window;
						if (!p.window.rightModIndexAndOnMouseUp.ContainsKey(captureModule.savedData.sibling))
						{
							//  PUSH_EVENT_HELPER_RAW();
							p.PUSH_ONMOUSEUP(w.pluginID, p.window.EVENT_HELPER_ONUP, p.window.Instance);

							p.window.rightModIndexAndOnMouseUp.Add(captureModule.savedData.sibling, () =>
							{
								if (captureRect.Contains(p.EVENT.mousePosition))
								{
									w.rightModIndexAndOnMouseUp.Remove(captureModule.savedData.sibling);
									p.RepaintWindowInUpdate(w.pluginID);

									Tools.EventUse();

									bool error = false;


									if (!error)
									{
										var capturedModule = captureModule;

										captureModule.callFromExternal_objects = null;
										var result = captureModule.CallHeader();

										if (result != null)
										{
											var mp = new MousePos(Event.current.mousePosition, MousePos.Type.Search_356_0, true, p);
											Windows.SearchWindow.Init(
																				mp, capturedModule.SearchHelper, "All", /*topGui*/result, capturedModule, w, null);
										}
									}
								}
							});
						}

						p.RepaintWindow(p.window.pluginID, true);
					}
					//  if (GUIUtility.hotControl != 0)

					if (p.EVENT.type == EventType.Repaint && GUI.color.a != 0 && p.window.rightModIndexAndOnMouseUp.ContainsKey(targetMod.savedData.sibling))
					{
						// button.Draw( (Rect)_dr, "", true, true, false, true );
						p.gl.DRAW_TAP_GLOW(modRect);
					}
				}




				GUI.color = old_guicolor;
				GUI.contentColor = old_contentcolor;


			}


			var _dr = firstDrawRect;
			var prop_position_x = lineRect.x + lineRect.width - p.rightOffset - p.par_e.RIGHT_RIGHT_PADDING;
			_dr.x = prop_position_x - propWidth;
			_dr.width = propWidth;
			///prop_position_x += +propWidth ;

			/*
						var _dr =  firstDrawRect;
						if ( lastRect.HasValue )
						{
							_dr = lastRect.Value;
							_dr.x -= _dr.width;
						}
						_dr.width = EditorGUIUtility.singleLineHeight;
						_dr.x -= _dr.width;*/
			/////////////////////
			//PROP BUTTON
			/////////////////////
			Rect proprect;
			{
				var pRect = _dr;

				EditorGUIUtility.AddCursorRect(pRect, MouseCursor.Link);



				//FADE SCENE RECT
				if (p.par_e.RIGHT_BG_OPACITY > 0.01f && GUI.color.a != 0)
				{
					var c = Colors.SceneColor;
					c.a = p.par_e.RIGHT_BG_OPACITY;
					EditorGUI.DrawRect((Rect)pRect, c);
				}


				proprect = pRect;
				if (proprect.height > EditorGUIUtility.singleLineHeight)
				{
					proprect.y += (proprect.height - EditorGUIUtility.singleLineHeight) / 2;
					proprect.height = EditorGUIUtility.singleLineHeight;
				}

				if (GUI.color.a != 0)
					p.gl._DrawTexture(proprect, p.GetNewIcon(NewIconTexture.RightMods, "PROP"));


				if (pRect.Contains(p.EVENT.mousePosition) && p.EVENT.type == EventType.MouseDown && p.EVENT.button == 0 && p.window.mouseEventDrag == null && p.window.mouseEvent == null)
				//if (GUI.Button(drawRect, "") && p.EVENT.button == 0)
				{ // MonoBehaviour.print(currentRect);


					p.window.captured_mod = null;

					p.window.allow = true;
					var startMouse = GUIUtility.GUIToScreenPoint(p.EVENT.mousePosition);
					//	var prop_position_x = lineRect.x + lineRect.width - Mathf.Max(0, p.rightOffset + p.par_e.RIGHT_RIGHT_PADDING ) ;//GET_PADING(headerRect.x + headerRect.width);
					//    var startWidth = p.par_e.RIGHT_PADDING_LEFT_READABLE ;//GET_PADING(headerRect.x + headerRect.width);
					var captureWidth = lineRect.x + lineRect.width;
					var w = p.window;

					p.window.mouseEvent = (b, deltaTime) =>
						{
							if (!b)
							{
								if (Event.current.delta.x != 0)
								{
									w.allow = false;
									p.RESET_DRAWSTACK(w.pluginID);
									p.RepaintWindow(w.pluginID, true);

								}
							}
							if (!w.allow)
							{
								p.par_e.RIGHT_PADDING_LEFT_READABLE = Mathf.RoundToInt(prop_position_x - ((int)startMouse.x - (int)GUIUtility.GUIToScreenPoint(p.EVENT.mousePosition).x));
								if (p.par_e.RIGHT_PADDING_LEFT_READABLE > captureWidth) p.par_e.RIGHT_PADDING_LEFT_READABLE = Mathf.RoundToInt(captureWidth);
								//  Debug.Log(p.par_e.RIGHT_PADDING_LEFT_READABLE);
							}
							if (b)
							{
								if (startMouse == GUIUtility.GUIToScreenPoint(Event.current.mousePosition))
								{
									w.mouseEventDrag = null;
									LeftClickOnRightModsHeaderMenu.Open(true);
								}
								//  Hierarchy.RepaintWindow();
							}
						};
					p.RepaintWindow(p.window.pluginID, true);
				}
			}

			//	if (p.par_e.DRAW_HEADER_HOTBUTTONS) p.modsController.toolBarModification.hotButtons.DrawButtonsOnHierarchyHeader(proprect);

			//OTHER
			if (p.window.mouseEvent != null && GUI.color.a != 0 && !p.window.allow)
			{
				var dr = _dr;
				// dr.width = window().position.width;
				dr.y = p._first_FullLineRect.y;
				dr.height = p._last_FullLineRect.y + p._last_FullLineRect.height - p._first_FullLineRect.y;

				dr.x += propWidth;
				dr.width = lineRect.x + lineRect.width;
				//  dr.x += dr.width;

				if (p.window.captured_mod == null) Colors.SelectRect(dr, 0.15f);

				//var oldC = GUI.color;
				// GUI.color = Color.red;
				// GUI.color *= ;
				dr.x = p.par_e.RIGHT_PADDING_LEFT_READABLE;// - dr.width;
				dr.width = 1;
				EditorGUI.DrawRect(dr, Color.red /** new Color32( 255, 90, 80, 255 ) */);
				// GUI.DrawTexture( (Rect)dr, Texture2D.whiteTexture );
				// GUI.color = oldC;
				// GUI.DrawTexture(dr, );
			}

			if (p.window.mouseEvent != null && GUI.color.a != 0 && p.window.captured_mod == null)
			{
				p.gl.DRAW_TAP_GLOW(_dr);
			}

			//  Debug.Log(GUI.depth);



			if (_dr.Contains(p.EVENT.mousePosition) && p.EVENT.type == EventType.MouseDown && p.EVENT.button == 1 && p.window.mouseEventDrag == null && p.window.mouseEvent == null)
			{
				var captureRect = _dr;
				if (!p.window.rightModIndexAndOnMouseUp.ContainsKey(999))
					p.window.rightModIndexAndOnMouseUp.Add(999, () =>
					{
						if (captureRect.Contains(p.EVENT.mousePosition))
						{
							LeftClickOnRightModsHeaderMenu.Open(false);
						}
					});
			}

			// Button( _dr, PropContent );
			Root.SetMouseTooltip(PropContent, _dr);
			// if ( p.EVENT.type == EventType.Repaint ) button.Draw( _dr, PropContent, false, p.window.rightModIndexAndOnMouseUp.ContainsKey( 999 ), false, false );
			if (p.window.rightModIndexAndOnMouseUp.ContainsKey(999)) p.gl.DRAW_TAP_GLOW(_dr);




			var headerRect = lineRect;
			headerRect.width = headerRect.x + headerRect.width - _dr.x;
			headerRect.x = _dr.x;
			return headerRect;
			/* if (GUI.Button(currentRect, )) {
					 if (allow) ShowCategoryList(true);
			 }*/
			/*  if ( needReapaint )
				{
						RepaintWindow( true );
				}*/

		}



		private Rect ClipMINSizeRect(Rect mod, float fullWidth)
		{
			float leftPad = p.par_e.RIGHT_PADDING_LEFT_READABLE;
			if (p.par_e.RIGHT_PADDING_LEFT_READABLE > fullWidth) leftPad = fullWidth;
			if (mod.x < leftPad)
			{
				var difToRight = leftPad - mod.x;
				mod.width = Math.Max(0, mod.width - difToRight);
				mod.x += difToRight;
			}
			return mod;
		}


	}
}
