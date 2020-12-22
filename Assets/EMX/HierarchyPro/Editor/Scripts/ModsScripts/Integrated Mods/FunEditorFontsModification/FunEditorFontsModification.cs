using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;



namespace EMX.HierarchyPlugin.Editor
{
    class FunEditorFontsModification
    {

        internal static void Modificate( int SIZE )
        {

            var fs = typeof(GUIStyle).GetProperty("fontSize");
            foreach ( var item in typeof( EditorWindow ).Assembly.GetTypes() )
            {
                foreach ( var f in item.GetFields( ~BindingFlags.Instance ) )
                {
                    // if ( !f.IsStatic ) continue;
                    if ( f.FieldType.Name == "GUIStyle" )
                    {
                        // if (f.FieldType.IsGenericParameter && f.FieldType.GetGenericParameterConstraints().Length != 0 ) continue; //TODO
                        //if (f.FieldType.IsConstructedGenericType) continue; //TODO
                        // Debug.Log(f.Name+ " " + item    );

                        try
                        {
                            var gs = f.GetValue(null);
                            if ( gs == null ) continue;
                            fs.SetValue( gs, SIZE );
                        }
                        catch { }
                    }
                }
            }
            bas = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.View" );
            view = typeof( EditorWindow ).Assembly.GetType( "UnityEditor.GUIView" );
            RepaintImmediately = view.GetMethod( "RepaintImmediately", ~(BindingFlags.Static | BindingFlags.GetField) );
            //  var p = bas.GetField("m_Parent", (BindingFlags)(-1));
            var m_Children = bas.GetField("m_Children",  ~(BindingFlags.Static | BindingFlags.InvokeMethod));

            ScriptableObject target = null;
            foreach ( var item in Resources.FindObjectsOfTypeAll<ScriptableObject>() )
            {
                if ( !bas.IsAssignableFrom( item.GetType() ) ) continue;
                target = item;
                break;
                // Replace( item.GetType(), item, 215 );
            }
            Scan( m_Children, target, SIZE );
            //  Rep( m_Children, target, SIZE );
            //   var sceneView = Resources.FindObjectsOfTypeAll<SceneView>().First();
            //  Debug.Log(target.GetType().Name);
            /* var par = p.GetValue(target);
             while(p.GetValue(target) != null ) par = p.GetValue(target);
             Debug.Log(par.GetType().Name);*/
        }
        static Type view, bas;
        static MethodInfo RepaintImmediately;
        static void Rep( FieldInfo m_Children, object o, int size )
        {

            // Replace( o.GetType(), o, size );
            if ( view.IsAssignableFrom( o.GetType() ) )
            {
                try
                {
                    RepaintImmediately.Invoke( o, null );
                }
                catch { }
            }
            var a = m_Children.GetValue(o) as object[];
            if ( a == null ) return;
            foreach ( var chld in a )
            {
                Rep( m_Children, chld, size );
            }
        }
        static void Scan( FieldInfo m_Children, object o, int size )
        {
            Replace( o.GetType(), o, size );

            var a = m_Children.GetValue(o) as object[];
            if ( a == null ) return;
            foreach ( var chld in a )
            {
                Scan( m_Children, chld, size );
            }
        }

        static void Replace( Type t, object o, int size )
        {
            if ( t == null ) return;
            var fs =    typeof(GUIStyle).GetProperty("fontSize");


            foreach ( var f in t.GetFields() )
            {
                if ( o == null && !f.IsStatic ) continue;
                if ( f.FieldType.Name == "GUIStyle" )
                {

                    var gs = f.GetValue(o);
                    if ( gs == null ) continue;
                    try
                    {
                        fs.SetValue( gs, size );
                    }
                    catch { }
                }
            }


            foreach ( var nst in t.GetNestedTypes( (BindingFlags)(-1) ) )
            {
                Replace( nst, null, size );
            }

            if ( o == null ) return;
            Replace( t.BaseType, o, size );
        }
    }
}
