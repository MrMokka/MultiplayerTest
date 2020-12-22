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



namespace EMX.HierarchyPlugin.Editor.Mods
{


	class GameObjectCacher<T, X> where T : Component
	{
		Dictionary<int, T> dic = new Dictionary<int, T>();
		Dictionary<int, double> update = new Dictionary<int, double>();
		// const double UPDATETIME = 1;
		internal void Clear( )
		{
			dic.Clear();
			update.Clear();
		}
		internal T GetValue( HierarchyObject o )
		{
			if ( !o.go ) return null;

			if ( !dic.ContainsKey( o.id ) )
			{
				//var g = EditorUtility.InstanceIDToObject(instanceID);
				//if ( !g ) return null;
				var g2 =  o.go.GetComponent<T>();
				if ( g2 && g2 is X ) g2 = null;
				dic.Add( o.id, g2 );
				//update.Add( o.id, g2 == null ? EditorApplication.timeSinceStartup : -1 );
			}

			var result = dic[o.id];

			if ( !result )
			{
				dic.Remove( o.id );
				update.Remove( o.id );
				/*	if ( update[ o.id ] == -1 )
					{

					}
					 else
					{
						if (Math.Abs(EditorApplication.timeSinceStartup - update[instanceID]) > UPDATETIME)
						{
							var g = EditorUtility.InstanceIDToObject(instanceID);
							if (!g) return null;
							var g2 = ((GameObject) g).GetComponent<T>();
							dic[instanceID] = g2;
							update[instanceID] = g2 == null ? EditorApplication.timeSinceStartup : -1;
						}
					}*/
			}

			return result;
		}
	}


	class Mod_SpritesOrder : RightModBaseClass
	{
		internal override void Subscribe(EditorSubscriber sbs) {
			sbs.OnResetDrawStack += onResetDrawStack;
		}

		public Mod_SpritesOrder( int restWidth, int sibbildPos, bool enable, PluginInstance adapter ) : base( restWidth, sibbildPos, enable, adapter )
		{
			/* var loadAss = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
             if (loadAss == null || loadAss.Length == 0)
             {
                 EditorUtility.DisplayDialog("Error", "Error load M_SpritesOrder", "Ok");
                 return;
             }
             var tagManager = new SerializedObject(loadAss[0]);
             foreach (var p in tagManager.GetIterator())
             {
                 MonoBehaviour.print(((SerializedProperty)p).name);
            
             }*/
			/* var findProperty = tagManager.FindProperty("layers");
             if (findProperty == null)
             {
                 EditorUtility.DisplayDialog("Error", "Error load M_SpritesOrder", "Ok");
                 return;
             }
             for (int j = 8; j < findProperty.arraySize; j++)
             {
                 SerializedProperty layerSP = findProperty.GetArrayElementAtIndex(j);
                 if (layerSP.stringValue == "")
                 {
                     layerSP.stringValue = str;
                     tagManager.ApplyModifiedProperties();
                     break;
                 }
                 if (j == findProperty.arraySize - 1)
                 {
                     EditorUtility.DisplayDialog("Error", "No free slots", "Ok");
                     return;
                 }
             }*/

		}
		void onResetDrawStack( )
		{
			cache.Clear();
		}
		static PropertyInfo _findProperty;

		private string[] sortingLayers
		{
			get
			{
				if ( _findProperty == null )
					_findProperty = typeof( InternalEditorUtility ).GetProperty( "sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic );
				return _findProperty.GetValue( null, null ) as string[];
			}
		}

		static GUIContent content = new GUIContent();
		static GameObjectCacher<Renderer, MeshRenderer> cache = new GameObjectCacher<Renderer, MeshRenderer>();
		static Rect RRR;



		public override void Draw( )
		{
			if ( !START_DRAW( drawRect, adapter.o ) ) return;



			// if (!r) return width;

			//MonoBehaviour.print(r.sortingLayerName);


			var r = cache.GetValue(adapter.o);
			var rect = drawRect;
			rect.width /= 3;

			DrawSortingOrder( rect, r, adapter.o );

			rect.x += rect.width;
			rect.width *= 2;


			DrawLayers( rect, r, adapter.o );


			END_DRAW( adapter.o );
		}















		void DrawSortingOrder( Rect drawRect, Renderer r, HierarchyObject _o )
		{
			var hasContent = false;

			if ( r )
			{
				content.text = r.sortingOrder.ToString();
				content.tooltip = "Order " + content.text;
				if ( content.text == "" ) content.text = "...Missing";
				if ( r.sortingOrder != 0 ) hasContent = true;
				/* var fs = Adapter.GET_SKIN().label.fontSize;
                 var al = Adapter.GET_SKIN().label.alignment;
                 Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleRight;
                 if ( !callFromExternal() ) Adapter.GET_SKIN().label.fontSize = adapter.FONT_8();
                 else Adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_8();*/

				/* GUI.enabled = r.gameObject.activeInHierarchy;
                 GUI.Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right );
                 GUI.enabled = true;*/
				Draw_Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right, true );

				/*   Adapter.GET_SKIN().label.alignment = al;
                   Adapter.GET_SKIN().label.fontSize = fs;*/
			}


			/*  var so = EditorGUI.IntField(drawRect, r.sortingOrder);
              if (so != r.sortingOrder)
              {
                  SetOrder(r, so);
              }*/

			/* if ( adapter.ModuleButton( drawRect, null, hasContent ) )
             {
            
            
            
             }*/
			Draw_ModuleButton( drawRect, content, BUTTON_ACTION_ORDER_HASH, hasContent );
		}



		GUIContent emptyContent = new GUIContent();

		void DrawLayers( Rect drawRect, Renderer r, HierarchyObject _o )
		{
			var hasContent = false;

			if ( r )
			{
				content.text = content.tooltip = r.sortingLayerName /* base.ContextHelper*/;
				// content.text = r.sortingLayerName;
				MT.GET_STRING( content, callFromExternal() ? 0 : adapter.par_e.RIGHT_SPRITEORDER_UPPERCASE );
				// if ( EVENT.type == EventType.Repaint )
				{
					RRR = drawRect;
					RRR.y += 1;
					RRR.height -= 2;
					RRR.width -= 1;

					Draw_Style( RRR, adapter.box, USE_GO: true ); //adapter.box
																  // adapter.box.Draw( RRR, "", false, false, false, false );
				}
				// MonoBehaviour.print(r.sortingLayerName);
				if ( r.sortingLayerName != "Default" )
				{ /*  var fs = Adapter.GET_SKIN().label.fontSize;
                      var al = Adapter.GET_SKIN().label.alignment;
                      Adapter.GET_SKIN().label.alignment = TextAnchor.MiddleLeft;
                      if ( !callFromExternal() ) Adapter.GET_SKIN().label.fontSize = adapter.FONT_8();
                      else Adapter.GET_SKIN().label.fontSize = adapter.WINDOW_FONT_8();*/
					// Adapter.GET_SKIN().label.fontSize = Hierarchy.FONT_8();

					/* GUI.enabled = r.gameObject.activeInHierarchy;
                     GUI.Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right );
                     GUI.enabled = true;*/

					Draw_Label( drawRect, content, !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right, true );


					hasContent = true;
					/*Adapter.GET_SKIN().label.alignment = al;
                    Adapter.GET_SKIN().label.fontSize = fs;*/
				}
				else //GUI.Label(drawRect, "-");
				{
					/* var a = adapter.label.alignment;
                    adapter.label.alignment = __Align;
                    GUI.Label( drawRect, "-", adapter.label );
                    adapter.label.alignment = a;*/
					Draw_Label( drawRect, "-", !callFromExternal() ? adapter.STYLE_LABEL_8_right : adapter.STYLE_LABEL_8_WINDOWS_right, true );
				}
			}

			//var bg = Adapter.GET_SKIN().button.active.background;
			// Adapter.GET_SKIN().button.active.background = Hierarchy.GetIcon("BUT");

			/*if ( adapter.ModuleButton( drawRect, null, hasContent ) )
            {
            
            }*/
			Draw_ModuleButton( drawRect, content, BUTTON_ACTION_LAYER_HASH, hasContent );

			//   Adapter.GET_SKIN().button.active.background = bg;
		}






















		void SetLayer( Renderer r, string sortingLayer )
		{
			var o = r.gameObject;
			if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
			{
				Undo.RecordObject( r, "Change sortingLayerName" );
				r.sortingLayerName = sortingLayer;
				adapter.SetDirty( r );
				adapter.MarkSceneDirty( o.scene );
				if ( adapter.par_e.ENABLE_OBJECTS_PING ) Tools.TRY_PING_OBJECT( o );
			}
			else
			{
				foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
				{
					var c = cache.GetValue(objectToUndo);
					if ( !c ) continue;
					Undo.RecordObject( c, "Change sortingLayerName" );
					c.sortingLayerName = sortingLayer;
					adapter.SetDirty( c );
					adapter.MarkSceneDirty( c.gameObject.scene );
					//  if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(objectToUndo);
				}
			}
		}

		void SetOrder( Renderer r, int order )
		{
			var o = r.gameObject;
			if ( adapter.ha.SELECTED_GAMEOBJECTS().All( selO => selO.go != o ) )
			{
				Undo.RecordObject( r, "Change sortingOrder" );
				r.sortingOrder = order;
				adapter.SetDirty( r );
				adapter.MarkSceneDirty( o.scene );
				if ( adapter.par_e.ENABLE_OBJECTS_PING ) Tools.TRY_PING_OBJECT( o );
			}
			else
			{
				foreach ( var objectToUndo in adapter.ha.SELECTED_GAMEOBJECTS() )
				{
					var c = cache.GetValue(objectToUndo);
					if ( !c ) continue;
					Undo.RecordObject( c, "Change sortingOrder" );
					c.sortingOrder = order;
					adapter.SetDirty( c );
					adapter.MarkSceneDirty( c.gameObject.scene );
					//  if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(objectToUndo);
				}
			}
		}


		bool Validate( GameObject o ) // return !string.IsNullOrEmpty(o.tag) && o.tag != "Untagged";
		{
			return cache.GetValue( Cache.GetHierarchyObjectByInstanceID( o ) );
		}
		bool Validate( HierarchyObject o ) // return !string.IsNullOrEmpty(o.tag) && o.tag != "Untagged";
		{
			return cache.GetValue( o );
		}

		bool Validate( HierarchyObject o, string filter ) // return !string.IsNullOrEmpty(o.tag) && o.tag != "Untagged";
		{
			var g = cache.GetValue(o);
			return g != null && g.sortingLayerName == filter;
		}

		bool Validate( HierarchyObject o, int sortingOrder ) // return !string.IsNullOrEmpty(o.tag) && o.tag != "Untagged";
		{
			var g = cache.GetValue(o);
			return g != null && g.sortingOrder == sortingOrder;
		}





		/* FillterData.Init(EVENT.mousePosition, SearchHelper, LayerMask.LayerToName(o.layer),
                     Validate(o) ?
                     CallHeaderFiltered(LayerMask.LayerToName(o.layer)) :
                     CallHeader(),
                     this);*/
		/** CALL HEADER */
		internal override Windows.SearchWindow.FillterData_Inputs CallHeader( )
		{
			var result = new Windows.SearchWindow.FillterData_Inputs(callFromExternal_objects)
			{
				Valudator = o => Validate(o) && (cache.GetValue(o).sortingLayerName != "Default" || cache.GetValue(o).sortingOrder != 0),
				SelectCompareString = (d, i) => { return cache.GetValue(d).sortingLayerName + " || " + cache.GetValue(d).sortingOrder.ToString(); },
				SelectCompareCostInt = (d, i) =>
				{
					var cost = i;
					cost += d.go.activeInHierarchy ? 0 : 100000000;
					return cost;
				}
			};
			return result;
		}

		internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( string filter )
		{
			var result = CallHeader();
			result.Valudator = s => Validate( s, filter );
			result.SelectCompareString = ( d, i ) => cache.GetValue( d ).sortingLayerName;
			return result;
		}

		internal Windows.SearchWindow.FillterData_Inputs CallHeaderFiltered( int sortingOrder )
		{
			var result = CallHeader();
			result.Valudator = s => Validate( s, sortingOrder );
			result.SelectCompareString = ( d, i ) => cache.GetValue( d ).sortingOrder.ToString();
			return result;
		}

		/** CALL HEADER */
		/*
        
                    internal override bool CallHeader(out GameObject[] obs, out int[] contentCost)
                    {
                        obs = Utilities.AllSceneObjects().Where(o => Validate(o)).Where(r => cache.GetValue(r.GetInstanceID()).sortingLayerName != "Default" || cache.GetValue(r.GetInstanceID()).sortingOrder != 0).ToArray();
                        contentCost = obs
                           .Select((d, i) => new { cache.GetValue(d.GetInstanceID()).sortingLayerName, startIndex = i, obj = d, cache.GetValue(d.GetInstanceID()).sortingOrder })
        
                           .OrderBy(d => d.sortingLayerName).ThenBy(d => d.sortingOrder)
                            .Select((d, i) => {
                                var cost = i;
                                cost += d.obj.activeInHierarchy ? 0 : 100000000;
                                return new { d.startIndex, cost = cost };
                            })
                           .OrderBy(d => d.startIndex)
                           .Select(d => d.cost).ToArray();
                        return true;
                    }
        
                    internal void CallHeaderFiltered(out GameObject[] obs, out int[] contentCost, string filter)
                    {
                        obs = Utilities.AllSceneObjects().Where(s => Validate(s, filter)).ToArray();
                        contentCost = obs
                           .Select((d, i) => new { name = cache.GetValue(d.GetInstanceID()).sortingLayerName, startIndex = i, obj = d })
                           .OrderBy(d => d.name)
                           .Select((d, i) => {
                               var cost = i;
                               cost += d.obj.activeInHierarchy ? 0 : 100000000;
                               return new { d.startIndex, cost = cost };
                           })
                           .OrderBy(d => d.startIndex)
                           .Select(d => d.cost).ToArray();
                    }
        
                    internal void CallHeaderFiltered(out GameObject[] obs, out int[] contentCost, int sortingOrder)
                    {
                        obs = Utilities.AllSceneObjects().Where(s => Validate(s, sortingOrder)).ToArray();
                        contentCost = obs
                           .Select((d, i) => new { name = cache.GetValue(d.GetInstanceID()).sortingOrder, startIndex = i, obj = d })
                           .OrderBy(d => d.name)
                           .Select((d, i) => {
                               var cost = i;
                               cost += d.obj.activeInHierarchy ? 0 : 100000000;
                               return new { d.startIndex, cost = cost };
                           })
                           .OrderBy(d => d.startIndex)
                           .Select(d => d.cost).ToArray();
                    }*/
		DrawStackMethodsWrapper __BUTTON_ACTION_LAYER_HASH = null;

		DrawStackMethodsWrapper BUTTON_ACTION_LAYER_HASH
		{
			get { return __BUTTON_ACTION_LAYER_HASH ?? (__BUTTON_ACTION_LAYER_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION_LAYER )); }
		}

		void BUTTON_ACTION_LAYER( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			//var o = _o.go;
			var r = cache.GetValue(_o);
#pragma warning disable
			var content = data.content;
#pragma warning restore
			if ( r && EVENT.button == adapter.MOUSE_BUTTON_0)
			{
				var l = sortingLayers;

				// var select = -1;
				// var ordered = l.OrderBy(f => f.Key).Select(f => f.Value).ToArray();
				var oldSelect = content.text;
				Action<int> Callback = (res) =>
				{
					SetLayer(r, l[res]);
                    /*  Undo.RecordObject(o, "Change tag");
                      o.tag = l[res];
                      Hierarchy.SetDirty(o);
                      if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
                };


				GenericMenu menu = new GenericMenu();

				for ( int i = 0 ; i < l.Length ; i++ )
				{
					var ind = i;
					content.text = l[ i ];
					// menu.AddItem(new GUIContent(content), GET_STRING(content.text, adapter.par.UPPER_SORT) == oldSelect, () => Callback(ind));
					menu.AddItem( new GUIContent( content ), content.text == oldSelect, ( ) => Callback( ind ) );
				}

				menu.AddSeparator( "" );

				/*    menu.AddItem(new GUIContent("Show 'Tags And Layers' Settings"), false, () => {
                      Selection.objects = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                  });
                  menu.AddSeparator("");*/

				var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_128_68, !callFromExternal(), adapter);
				//  var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 128, 68, adapter );
				var w = adapter.window;
				menu.AddItem( new GUIContent( "+ Assign a New SortingLayer" ), false, ( ) =>
				   {
					   Windows.InputWindow.Init( pos, "New SortingLayer name's", w, ( str ) =>
					 {
						 if ( string.IsNullOrEmpty( str ) ) return;
						 str = str.Trim();
						 var lowwer = l.Select(ord => ord.ToLower()).ToList();
						 var ind = lowwer.IndexOf(str.ToLower());
						 if ( ind != -1 )
						 {
							 SetLayer( r, l[ ind ] );
							 /* Undo.RecordObject(o, "Change tag");
							  o.tag = l[ind];
							  Hierarchy.SetDirty(o);
							  if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
						 }
						 else
						 {
							 var UpdateSortingLayersOrder = typeof(InternalEditorUtility).GetMethod("UpdateSortingLayersOrder", (BindingFlags) (-1));
							 var AddSortingLayer = typeof(InternalEditorUtility).GetMethod("AddSortingLayer", (BindingFlags) (-1));
							 var SetSortingLayerName = typeof(InternalEditorUtility).GetMethod("SetSortingLayerName", (BindingFlags) (-1));
							 var GetSortingLayerCount = typeof(InternalEditorUtility).GetMethod("GetSortingLayerCount", (BindingFlags) (-1));

							 var count = GetSortingLayerCount.Invoke(null, null);
							 AddSortingLayer.Invoke( null, null );
							 SetSortingLayerName.Invoke( null, new[] { count, str } );
							 UpdateSortingLayersOrder.Invoke( null, null );
							 /*
																 var loadAss = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
																 if (loadAss == null || loadAss.Length == 0)
																 {
																	 EditorUtility.DisplayDialog("Error", "Error M_SpritesOrder layers", "Ok");
																	 return;
																 }
																 var tagManager = new SerializedObject(loadAss[0]);
																 var findProperty = tagManager.FindProperty("m_SortingLayers");
																 if (findProperty == null)
																 {
																	 EditorUtility.DisplayDialog("Error", "Error M_SpritesOrder layers", "Ok");
																	 return;
																 }
																 findProperty.InsertArrayElementAtIndex(findProperty.arraySize);
																 SerializedProperty layerSP = findProperty.GetArrayElementAtIndex(findProperty.arraySize - 1);
																 layerSP.stringValue = str;
																 tagManager.ApplyModifiedProperties();*/
							 /**  UnityEditorInternal.InternalEditorUtility.AddTag(str);  */

							 SetLayer( r, str );
							 /*  Undo.RecordObject(o, "Change tag");
							   o.tag = str;
							   Hierarchy.SetDirty(o);
							   if (Hierarchy.par.ENABLE_PING_Fix) Tools.TRY_PING_OBJECT(o);*/
						 }
					 } );
				   } );

				menu.AddSeparator( "" );
				menu.AddItem( new GUIContent( "Show only uppercase letters" ), adapter.par_e.RIGHT_SPRITEORDER_UPPERCASE != 0, ( ) =>
				   {
					   adapter.par_e.RIGHT_SPRITEORDER_UPPERCASE = 1 - adapter.par_e.RIGHT_SPRITEORDER_UPPERCASE;
					   //adapter.SavePrefs();
				   } );
				menu.AddSeparator( "" );
				menu.AddItem( new GUIContent( "Show 'Tags And Layers' Settings" ), false, ( ) =>
				   {
					   Selection.objects = AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/TagManager.asset" );
					   Tools.FocusToInspector();
				   } );


				menu.ShowAsContext();
				Tools.EventUse();
			}


			if ( EVENT.button == adapter.MOUSE_BUTTON_1)
			{
				Tools.EventUse();
				/*     int[] contentCost = new int[0];
                     GameObject[] obs = new GameObject[0];
                
                     if (r && Validate(r.gameObject))
                     {
                         if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFiltered(out obs, out contentCost, r.sortingLayerName);
                     } else
                     {
                         CallHeader(out obs, out contentCost);
                     }
                
                     FillterData.Init(EVENT.mousePosition, SearchHelper, "'sortingLayerName' = " + (r ? r.sortingLayerName.ToString() : "All assigned"), obs, contentCost, null, this);
                */

				var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
				Windows.SearchWindow.Init( mp, SearchHelper, "'sortingLayerName' = " + (r ? r.sortingLayerName.ToString() : "All assigned"),
					r && Validate( r.gameObject ) ? CallHeaderFiltered( r.sortingLayerName ) : CallHeader(),
					this, adapter.window, _o );
				// EditorGUIUtility.ic
			}
		}

		DrawStackMethodsWrapper __BUTTON_ACTION_ORDER_HASH = null;

		DrawStackMethodsWrapper BUTTON_ACTION_ORDER_HASH
		{
			get { return __BUTTON_ACTION_ORDER_HASH ?? (__BUTTON_ACTION_ORDER_HASH = new DrawStackMethodsWrapper( BUTTON_ACTION_ORDER )); }
		}

		void BUTTON_ACTION_ORDER( Rect worldOffset, Rect inputRect, DrawStackMethodsWrapperData data, HierarchyObject _o )
		{
			//	var o = _o.go;
			var r = cache.GetValue(_o);
#pragma warning disable
			var content = data.content;
#pragma warning restore
			if ( r && EVENT.button == adapter.MOUSE_BUTTON_0)
			{
				var pos = new MousePos(EVENT.mousePosition, MousePos.Type.Input_190_68, !callFromExternal(), adapter);
				//  var pos = InputData.WidnwoRect(!callFromExternal(), EVENT.mousePosition, 190, 68, adapter );
				/*   var lowwer = ;
                   var ind = lowwer.IndexOf(str.ToLower());
                   string sendText = null;
                   if (ind != -1)
                   {
                       list.a
                   }*/

				Action<string> act = (str) =>
				{
					int pars = 0;
					int.TryParse(str, out pars);
					SetOrder(r, pars);

                    //Hierarchy.MarkSceneDirty(o.scene);
                    //var lowwer = list.listValues.Select(ord => ord.ToLower()).ToList();
                    // var ind = lowwer.IndexOf(str.ToLower());
                    /*  if (ind != -1)
                      {
                          Undo.RecordObject(o, "Change description");
                          list.a
                          //o.tag = l[ind];
                          Hierarchy.SetDirty(o);
                          Tools.TRY_PING_OBJECT(o);
                      }
                      else
                      {*/


                    //  }
                };
				Windows.InputWindow.InitTeger( pos, "Sorting Order", adapter.window, act, null, r.sortingOrder.ToString() );
			}

			if ( EVENT.button == adapter.MOUSE_BUTTON_1)
			{
				Tools.EventUse();
				/*      int[] contentCost = new int[0];
                      GameObject[] obs = new GameObject[0];
                
                      if (r && Validate(r.gameObject))
                      {
                          if (EditorSceneManager.GetActiveScene().rootCount != 0) CallHeaderFiltered(out obs, out contentCost, r.sortingOrder);
                      } else
                      {
                          CallHeader(out obs, out contentCost);
                      }
                
                      FillterData.Init(EVENT.mousePosition, SearchHelper, "'sortingOrder' = " + (r ? r.sortingOrder.ToString() : "All assigned"), obs, contentCost, null, this);
                */
				var mp = new MousePos(EVENT.mousePosition, MousePos.Type.Search_356_0, !callFromExternal(), adapter);
				Windows.SearchWindow.Init( mp, SearchHelper, "'sortingOrder' = " + (r ? r.sortingOrder.ToString() : "All assigned"),
					r && Validate( r.gameObject ) ? CallHeaderFiltered( r.sortingOrder ) : CallHeader(),
					this, adapter.window, _o );
				// EditorGUIUtility.ic
			}
		}











	}
}
