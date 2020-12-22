using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{
    class LeftClickOnRightModsHeaderMenu
    {

        internal static void SHOW_HIER_SETTINGS_GENERICMENU( )
        {
            Settings.MainSettingsEnabler_Window.Select<Settings.MainSettingsEnabler_Window>();
        }




        static PluginInstance p { get { return Root.p[ 0 ]; } }

        internal static void Open( bool addMenu = false, Action<GenericMenu> topBuilder = null )
        {
            var menu = new GenericMenu();

            //  if (addMenu)
            var pid = p.pluginID;

            if ( topBuilder != null ) topBuilder( menu );

            GUIContent cont = null;
            bool haveDisable = false;


            foreach ( var mod in p.modsController.rightModsManager.rightMods )
            {
                if ( mod.savedData.sibling == -1 ) continue;

                var wasEnable = mod.savedData.enabled;
                var captured_mod = mod;
                // if (mod.HeaderTexture2D != null) cont = new GUIContent(GetIcon(mod.HeaderTexture2D));
                /*else*/

                if ( !mod.enableOverride() )
                {
                    if ( string.IsNullOrEmpty( mod.enableOverrideMessage() ) )
                    {
                        continue;
                        /*menu.AddDisabledItem(new GUIContent(mod.ContextHelper.ToString() + " (Pro Only)")); */
                    }

                    else
                    {
                        if ( !haveDisable ) menu.AddDisabledItem( new GUIContent( mod.ContextHelper.ToString() + " " + mod.enableOverrideMessage() ) );

                        haveDisable = true;
                    }
                }

                else
                {
                    var userType = typeof(Mod_UserModulesRoot);
                    var txt = userType.IsAssignableFrom(mod.GetType()) /* is Adapter.M_UserModulesRoot*/ ? mod.ContextHelper : mod.HeaderText;

                    if ( mod.savedData.enabled || userType.IsAssignableFrom( mod.GetType() ) /* is M_UserModulesRoot */) cont = new GUIContent( txt.ToString() );
                    else cont = new GUIContent( "[ " + txt.ToString() + " ]" );

                    menu.AddItem( cont, mod.savedData.enabled, ( ) =>
                     {
                         captured_mod.savedData.enabled = !wasEnable;
                         p.RepaintWindow(pid,true);
                     } );
                }
            }

            menu.AddSeparator( "" );
            //  var mp = EditorGUIUtility.GUIToScreenPoint( Event.current.mousePosition);
            var pos = new MousePos(null, MousePos.Type.ModulesListWindow_380_700, false, p); //new Rect(Event.current.mousePosition.x - 190, Event.current.mousePosition.y, 0, 0)


            menu.AddItem( new GUIContent( "Use Custom Modules" ), p.par_e.RIGHT_USE_CUSTOMMODULES, ( ) => { p.par_e.RIGHT_USE_CUSTOMMODULES = !p.par_e.RIGHT_USE_CUSTOMMODULES; } );
            var w = p.window;
            menu.AddItem( new GUIContent( "[ Open Modules Table ☷ ]" ), false, ( ) => { Windows.ModulesWindow.Init( pos, w ); } );

            //* Static Members **/
            CONTEXTMENU_STATICMODULES( menu );

            menu.AddSeparator( "" );

            ADD_LAYOUTS( ref menu );

            menu.AddSeparator( "" );

            {
                menu.AddItem( new GUIContent( "Auto-Hide If Width < " + p.par_e.RIGHTDOCK_TEMPHIDEMINWIDTH ),
                    p.par_e.RIGHTDOCK_TEMPHIDE, ( ) =>
                    {
                        p.par_e.RIGHTDOCK_TEMPHIDE = !p.par_e.RIGHTDOCK_TEMPHIDE;
                    } );

                //  if ( IS_PROJECT() )
                {
                    menu.AddItem( new GUIContent( "'*.*' Display files extension" ), p.par_e.DRAW_EXTENSION_IN_PROJECT && p.par_e.DRAW_EXTENSION_IN_PROJECT, ( ) =>
                       {
                           if ( !p.par_e.DRAW_EXTENSION_IN_PROJECT ) p.par_e.DRAW_EXTENSION_IN_PROJECT = true;
                           p.par_e.DRAW_EXTENSION_IN_PROJECT = !p.par_e.DRAW_EXTENSION_IN_PROJECT;
                       } );
                }

                if ( EditorSceneManager.sceneCount < 2 )
                    menu.AddItem( new GUIContent( "Bind Header To The Top" ), p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE, ( ) =>
                       {
                           p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE = !p.par_e.RIGHT_HEADER_BIND_TO_SCENE_LINE;
                       } );
                else
                    menu.AddDisabledItem( new GUIContent( "Bind Header To The Top" ) );


                menu.AddSeparator( "" );
            }
            menu.AddItem( new GUIContent( "Open Settings" ), false, ( ) =>
               {
                   SHOW_HIER_SETTINGS_GENERICMENU();
               } );


            menu.AddSeparator( "" );

            menu.AddItem( new GUIContent( "Revival Cache in Project" ), false, ( ) =>
               {

                   var s = EditorSceneManager.GetActiveScene();
                   var sp= s.path;
                   var p = HierarchyExternalSceneData.GetProjectHash(s,ref sp);
                   Selection.objects = new[] { p };
                   //var d = MOI.des(-1);
                   //Selection.objects = new[] { d as UnityEngine.Object };
               } );




            if ( !addMenu )
            { /*   menu.AddSeparator("");
			       // menu.AddDisabledItem(new GUIContent("Click on the item for more options"));
			       // menu.AddDisabledItem(new GUIContent("Click below to get more options.."));
			       menu.AddDisabledItem(new GUIContent("Click on the object's line to configure"));*/
            }
            else
            {
                menu.AddSeparator( "" );
                menu.AddDisabledItem( new GUIContent( "Drag this icon to change size of right bar" ) );
            }


            menu.ShowAsContext();
            // EventUse();
        }






        static void CONTEXTMENU_STATICMODULES( GenericMenu menu )
        {
            menu.AddSeparator( "" );
            var HierarchyAdapterInstance = Root.p[0];

            GUIContent cont = null;
            /*  if ( HierarchyAdapterInstance.par.DataKeeperParams.ENABLE )
                  cont = new GUIContent( HierarchyAdapterInstance.modules.First( m => m is M_PlayModeKeeper ).HeaderText.ToString() );
              else*/
         

            /* if ( HierarchyAdapterInstance.par.DataKeeperParams.ENABLE )
                 cont = new GUIContent( HierarchyAdapterInstance.modules.First( m => m is M_SetActive ).HeaderText.ToString() );
             else*/
            cont = new GUIContent( "- " + p.modsController.setActiveMod.HeaderText.ToString() + " -" );
            menu.AddItem( cont, p.par_e.USE_SETACTIVE_MOD, ( ) =>
             {
                 p.par_e.USE_SETACTIVE_MOD = !p.par_e.USE_SETACTIVE_MOD;
             } );
            if ( !p.par_e.USE_SETACTIVE_MOD )
            {
                cont = new GUIContent( "- SetActive Module Style" );
                menu.AddDisabledItem( cont );
            }
            else
            {
                /*  cont = new GUIContent( "- SetActive Module Style/Left Small" );
                  menu.AddItem( cont, HierarchyAdapterInstance.SETACTIVE_POSITION == 1, ( ) =>
                   {
                       HierarchyAdapterInstance.SETACTIVE_POSITION = 1;
                   } );
                   */

                cont = new GUIContent( "- SetActive Module Style/Right Default" );
                menu.AddItem( cont, p.par_e.SET_ACTIVE_POSITION == 0, ( ) =>
                 {
                     p.par_e.SET_ACTIVE_POSITION = 0;
                 } );

                cont = new GUIContent( "- SetActive Module Style/Right Small" );
                menu.AddItem( cont, p.par_e.SET_ACTIVE_POSITION == 2, ( ) =>
                 {
                     p.par_e.SET_ACTIVE_POSITION = 2;
                 } );

                menu.AddSeparator( "- SetActive Module Style/" );

                cont = new GUIContent( "- SetActive Module Style/Contrast Style" );
                menu.AddItem( cont, p.par_e.SET_ACTIVE_STYLE == 1, ( ) =>
                 {
                     p.par_e.SET_ACTIVE_STYLE = 1 - p.par_e.SET_ACTIVE_STYLE;
                 } );
            }
        }


        static void ADD_LAYOUTS( ref GenericMenu menu )
        {
            var current_state = TakeModulesSnapShot();
            var saved_states = saved_states_get;
            var findIndex = saved_states.FindIndex(s => s.data == current_state.data);

            const string CATEGORY = "- LAYOUTS/";

            var pos = new MousePos(Event.current.mousePosition, MousePos.Type.Input_128_68, false, p);

            var w = p.window;
            //  var pos = InputData.WidnwoRect(FocusRoot.WidnwoRectType.Clamp, Event.current.mousePosition, 128, 68, this);
            if ( findIndex != -1 ) menu.AddDisabledItem( new GUIContent( CATEGORY + "Add" ) );
            else
                menu.AddItem( new GUIContent( CATEGORY + "Add" ), false, ( ) =>
                   {
                       Windows.InputWindow.Init( pos, "New Item", w, ( str ) =>
                 {
                     if ( string.IsNullOrEmpty( str ) ) return;

                     str = str.Trim();
                     current_state.name = str;
                     saved_states.Add( current_state );
                     saved_states_get = saved_states;
                 }, textInputSet: "MyWorkflow" );
                   } );

            if ( findIndex == -1 ) menu.AddDisabledItem( new GUIContent( CATEGORY + "Remove" ) );
            else
                menu.AddItem( new GUIContent( CATEGORY + "Remove" ), false, ( ) =>
                   {
                       saved_states.RemoveAt( findIndex );
                       saved_states_get = saved_states;
                   } );

            if ( saved_states.Count != 0 ) menu.AddSeparator( CATEGORY );
            var pid = p.pluginID;

            for ( int i = 0 ; i < saved_states.Count ; i++ )
            {
                var captureName = saved_states[i].name;
                var captureSnap = saved_states[i].data;
                menu.AddItem( new GUIContent( CATEGORY + captureName ), findIndex == i, ( ) => { SetDirtyModulesSnapShots( captureName, captureSnap , pid); } );
            }


            menu.AddSeparator( CATEGORY );

            menu.AddItem( new GUIContent( CATEGORY + "- Default" ), NOW_DEFAULT(), ( ) =>
                    {
                        SET_TO_DEFAULT();
                        p.RepaintWindow(pid, true );
                    } );

            menu.AddItem( new GUIContent( CATEGORY + "- Show All" ), p.modsController.rightModsManager.rightMods.All( m => m.savedData.enabled ) && p.par_e.USE_PLAYMODE_SAVER_MOD, ( ) =>
                 {
                     foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = true;
                     p.par_e.USE_PLAYMODE_SAVER_MOD = true;
                     p.RepaintWindow(pid, true );
                 } );
            menu.AddItem( new GUIContent( CATEGORY + "- Hide All" ), p.modsController.rightModsManager.rightMods.All( m => !m.savedData.enabled ) && !p.par_e.USE_PLAYMODE_SAVER_MOD, ( ) =>
                {
                    foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = false;
                    p.par_e.USE_PLAYMODE_SAVER_MOD = false;
                    //   modules.First( m => m is IModuleOnnector_M_CustomIcons ).enable = true;
                    //   modules.First( m => m is IModuleOnnector_M_CustomIcons ).enable = true;
                    p.RepaintWindow(pid, true );
                } );
        }

        static bool NOW_DEFAULT( )
        {
            var modules = p.modsController.rightModsManager.rightMods;
            foreach ( var module in modules ) if ( !module.savedData.NowDefault() ) return false;
            if ( p.par_e.USE_PLAYMODE_SAVER_MOD ) return false;
            return true;
        }
        static internal void SET_TO_DEFAULT( )
        {
            var modules = p.modsController.rightModsManager.rightMods;
            foreach ( var module in modules )
            {
                if ( module.savedData.sibling == -1 ) continue;

                module.savedData.SetToDefault();
                // Debug.Log( DefaulTypes[0] + " " + module.GetType().FullName );
                // if ( DefaulTypes.Any( d => d == (module.GetType().FullName) ) ) module.enable = true;
                // else module.enable = false;
            }
            p.par_e.USE_PLAYMODE_SAVER_MOD = false;
            p.par_e.USE_SETACTIVE_MOD = true;
            // p.par_e.RIGHT_PADDING_LEFT_READABLE
        }


        class snap_res
        {
            internal string name;
            internal string data;
        }
        static char[] trim= {'\n','\r',' '};
        static snap_res TakeModulesSnapShot( )
        {

            var final = "";
            var modules = p.modsController.rightModsManager.rightMods;
            for ( int i = 0 ; i < modules.Length ; i++ )
            {
                var result = "";
                result += modules[ i ].MODULE_KEY + "#";
                result += modules[ i ].savedData.enabled + "#";
                result += modules[ i ].savedData.sibling + "#";
                result += modules[ i ].savedData.width + "\n\r";
                final += result;
            }
            final += "KEEPER#" + p.par_e.USE_PLAYMODE_SAVER_MOD + "\n\r";
            final += "SETACTIVE#" + p.par_e.USE_SETACTIVE_MOD + "\n\r";

            return new snap_res() { name = "", data = final.Trim( trim ) };
        }

        static void SetDirtyModulesSnapShots( string name, string data , int pid)
        {

            foreach ( var module in p.modsController.rightModsManager.rightMods ) module.savedData.enabled = false;
            p.par_e.USE_PLAYMODE_SAVER_MOD = false;
            p.par_e.USE_SETACTIVE_MOD = false;

            var rioghtMods = p.modsController.rightModsManager.rightMods.ToDictionary(m=>m.MODULE_KEY,v=>v);

            foreach ( var _i in data.Split( '\n' ) )
            {
                var i = _i.Trim(trim);

                if ( i.StartsWith( "KEEPER#" ) ) p.par_e.USE_PLAYMODE_SAVER_MOD = bool.Parse( i.Split( '#' )[ 1 ].Trim( trim ) );
                else if ( i.StartsWith( "SETACTIVE#" ) ) p.par_e.USE_SETACTIVE_MOD = bool.Parse( i.Split( '#' )[ 1 ].Trim( trim ) );
                else
                {
                    var read = i.Split('#').Select(s=>s.Trim(trim)).ToArray();
                    if ( rioghtMods.ContainsKey( read[ 0 ] ) )
                    {
                        var m = rioghtMods[read[0]];
                        m.savedData.enabled = bool.Parse( read[ 1 ] );
                        m.savedData.sibling = int.Parse( read[ 2 ] );
                        m.savedData.width = int.Parse( read[ 3 ] );
                    }
                }
            }

            p.RepaintWindow(pid, true );
            // RepaintAllViews();
        }


        /* int GetCountSnapShots()
         {
             return EditorPrefs.GetInt( pluginname + "/Layouts/SnapsCount", 0 );
         }*/

        static List<snap_res> saved_states_get
        {
            get
            {
                var r = p.par_e.GET("RIGHT_MODULES_STATES","");
                //  var snaps = EditorPrefs.GetString(pluginname + "/Layouts/SnapsSer", null);
                if ( !string.IsNullOrEmpty( r ) )
                {
                    List<snap_res> res = new List<snap_res>();
                    foreach ( var s in r.Split( '*' ) )
                    {
                        var read = s.Trim(trim).Split('$');
                        var c = new snap_res();
                        c.name = read[ 0 ].Trim( trim );
                        c.data = read[ 1 ].Trim( trim );
                        res.Add( c );
                    }
                    return res;
                }
                return new List<snap_res>();
            }

            set
            {
                // var ser = SERIALIZE_SINGLE(value);
                //  EditorPrefs.SetString( pluginname + "/Layouts/SnapsSer", ser );
                p.par_e.SET( "RIGHT_MODULES_STATES", value.Select( v => v.name + "$\n\r" + v.data ).Aggregate( ( a, b ) => a + "*\n\r" + b ) );
            }
        }

    }
}
