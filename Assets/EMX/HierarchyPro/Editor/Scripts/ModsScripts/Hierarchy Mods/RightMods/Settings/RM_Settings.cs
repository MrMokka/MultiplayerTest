using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{
    public enum VerticesModuleTypeEnum
    {
        Triangles= 0,
        Vertices = 1,
        ChildCount = 2,
        TextureMemory = 3
    }

    partial class EditorSettingsAdapter
    {

        //MAIN
        internal bool RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS { get { return GET( "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS", false ); } set { SET( "RIGHT_DRAW_HYPHEN_FOR_EMPTY_LABELS", value ); p.RESET_DRAWSTACK(0); } }
        Color32 _pfc_def =new Color32( 180, 180, 180, 255 );
        internal Color RIGHT_LABELS_COLOR
        {
            get
            {
                if ( EditorGUIUtility.isProSkin ) return GET( "PLUGIN_LABEL_COLOR_PRO", _pfc_def );
                return GET( "PLUGIN_LABEL_COLOR_PRS", Color.black );
            }
            set
            {
                if ( EditorGUIUtility.isProSkin ) SET( "PLUGIN_LABEL_COLOR_PRO", value );
                else SET( "PLUGIN_LABEL_COLOR_PRS", value );
                p.RESET_DRAWSTACK(0);
            }
        }

        internal bool RIGHT_HEADER_BIND_TO_SCENE_LINE { get { return GET( "RIGHT_HEADER_BIND_TO_SCENE_LINE", false ); } set { SET( "RIGHT_HEADER_BIND_TO_SCENE_LINE", value ); p.RESET_DRAWSTACK(0); } }
        internal float RIGHT_BG_OPACITY { get { return GET( "RIGHT_BG_OPACITY", 0.45f ); } set { SET( "RIGHT_BG_OPACITY", value ); p.RESET_DRAWSTACK(0); } }
        internal int RIGHT_RIGHT_PADDING { get { return GET( "RIGHT_RIGHT_PADDING", 0 ); } set { SET( "RIGHT_RIGHT_PADDING", value ); p.RESET_DRAWSTACK(0); } } //  200, -100,

        internal bool RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER { get { return GET( "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER", false ); } set { SET( "RIGHT_RIGHT_PADDING_AFFECT_TO_SETACTIVE_AND_KEEPER", value ); p.RESET_DRAWSTACK(0); } }


        internal bool RIGHTDOCK_TEMPHIDE { get { return GET( "RIGHTDOCK_TEMPHIDE", false ); } set { SET( "RIGHTDOCK_TEMPHIDE", value ); p.RESET_DRAWSTACK(0); } }
        internal int RIGHTDOCK_TEMPHIDEMINWIDTH { get { return GET( "RIGHTDOCK_TEMPHIDEMINWIDTH", 300 ); } set { SET( "RIGHTDOCK_TEMPHIDEMINWIDTH", value ); p.RESET_DRAWSTACK(0); } } //  200, -100,

        internal int RIGHT_PADDING_LEFT_READABLE { get { return Math.Max( 150, GET( "RIGHT_PADDING_LEFT_READABLE", 250 ) ); } set { SET( "RIGHT_PADDING_LEFT_READABLE", value ); p.RESET_DRAWSTACK(0); } } //  200, -100,


        
        internal bool RIGHT_USE_CUSTOMMODULES { get { return GET( "RIGHT_USE_CUSTOMMODULES", true ); } set { SET( "RIGHT_USE_CUSTOMMODULES", value ); p.RESET_DRAWSTACK(0); } }
        

        //TAGS
        internal int RIGHT_TAGS_UPPERCASE { get { return GET( "RIGHT_TAGS_UPPERCASE", 0 ); } set { SET( "RIGHT_TAGS_UPPERCASE", value ); p.RESET_DRAWSTACK(0); } }
        internal int RIGHT_LAYERS_UPPERCASE { get { return GET( "RIGHT_LAYERS_UPPERCASE", 0 ); } set { SET( "RIGHT_LAYERS_UPPERCASE", value ); p.RESET_DRAWSTACK(0); } }
        internal int RIGHT_SPRITEORDER_UPPERCASE { get { return GET( "RIGHT_SPRITEORDER_UPPERCASE", 0 ); } set { SET( "RIGHT_SPRITEORDER_UPPERCASE", value ); p.RESET_DRAWSTACK(0); } }



        internal bool RIGHT_FREEZE_LOCK_SCENE_VIEW { get { return GET("RIGHT_FREEZE_LOCK_SCENE_VIEW", true); } set { SET("RIGHT_FREEZE_LOCK_SCENE_VIEW", value); p.RESET_DRAWSTACK(0); } }



        //MEMORY
        internal VerticesModuleTypeEnum RIGHT_MOD_VERTICES_SCAN_TYPE { get { return (VerticesModuleTypeEnum)GET( "RIGHT_MOD_VERTICES_SCAN_TYPE", 0 ); } set { SET( "RIGHT_MOD_VERTICES_SCAN_TYPE", (int)value ); p.RESET_DRAWSTACK(0); } }
        internal bool RIGHT_MOD_BROADCAST_ENABLED { get { return GET( "RIGHT_MOD_BROADCAST_ENABLED", false ); } set { SET( "RIGHT_MOD_BROADCAST_ENABLED", value ); p.RESET_DRAWSTACK(0); } }
        internal float RIGHT_MOD_BROADCASTING_PREFOMANCE { get { return GET( "RIGHT_MOD_BROADCASTING_PREFOMANCE", 30f ); } set { SET( "RIGHT_MOD_BROADCASTING_PREFOMANCE", value ); p.RESET_DRAWSTACK(0); } }
        internal int RIGHT_MOD_BROADCASTING_PREFOMANCE01
        {
            get { return Mathf.RoundToInt( (RIGHT_MOD_BROADCASTING_PREFOMANCE - 10f) / 2f ); }

            set { RIGHT_MOD_BROADCASTING_PREFOMANCE = Mathf.RoundToInt( value * 2 + 10 ); p.RESET_DRAWSTACK(0); }
        }




        //HIDE
        internal bool RIGHT_SHOWMODS_ONLY_IFHOVER { get { return GET( "RIGHT_SHOWMODS_ONLY_IFHOVER", false ); } set { SET( "RIGHT_SHOWMODS_ONLY_IFHOVER", value ); p.RESET_DRAWSTACK(0); } }
        internal bool RIGHT_LOCK_MODS_UNTIL_NOKEY { get { return (RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3) != 0; } }
        internal int RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX { get { return GET( "RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX", 0 ); } set { SET( "RIGHT_HIDEMODS_UNTIL_NOKEY_INDEX", value ); p.RESET_DRAWSTACK(0); } }
        internal string RIGHT_LOCK_MODS_UNTIL_NOKEY_TOSTRING
        {
            get
            {
                var new_i = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 3;
                var cts = new[] {"...", "'Alt'", "'Shift'", "'Ctrl'"};
                var key = cts[new_i];
                return key;
            }
        }
        internal bool RIGHT_LOCK_ONLY_IF_NOCONTENT
        {
            get { return (RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & 8) != 0; }
            set
            {
                if ( value ) RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX | 8;
                else RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX = RIGHT_LOCK_MODS_UNTIL_NOKEY_INDEX & ~8;
                p.RESET_DRAWSTACK(0);
            }
        }
        internal bool RIGHT_USE_HIDE_ISTEAD_LOCK { get { return GET( "RIGHT_USE_HIDE_ISTEAD_LOCK", true ); } set { SET( "RIGHT_USE_HIDE_ISTEAD_LOCK", value ); p.RESET_DRAWSTACK(0); } }

    }
}
