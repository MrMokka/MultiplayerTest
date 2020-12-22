using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace EMX.HierarchyPlugin.Editor.Settings
{
    class MainRoot : UnityEditor.Editor
    {

        
        internal PluginInstance p { get { return Root.p[ 0 ]; } }


        CLASS_GROUP __GROUP;

        internal CLASS_GROUP GRO
        {
            get { return __GROUP ?? (__GROUP = new CLASS_GROUP() { A = p }); }
        }

        CLASS_ENALBE __ENABLE;

        internal CLASS_ENALBE ENABLE
        {
            get { return __ENABLE ?? (__ENABLE = new CLASS_ENALBE() { A = p }); }
        }

    }
}
