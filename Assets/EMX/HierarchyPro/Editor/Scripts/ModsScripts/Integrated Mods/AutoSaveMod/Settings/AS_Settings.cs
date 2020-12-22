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

	partial class EditorSettingsAdapter
	{


		internal int AS_SAVE_INTERVAL_IN_MIN { get { return Mathf.Clamp(GET("AS_SAVE_INTERVAL_IN_MIN", 5), 1, 60); } set { SET("AS_SAVE_INTERVAL_IN_MIN", value); } }
		internal bool AS_LOG { get { return GET("AS_LOG", false); } set { SET("AS_LOG", value);  } }

		internal int AS_SAVE_INTERVAL_IN_SEC
		{
			get { return (int)AS_SAVE_INTERVAL_IN_MIN * 60; }
			set { AS_SAVE_INTERVAL_IN_MIN = (value / 60); ; }
		}

		internal int AS_FILES_COUNT { get { return GET("AS_FILES_COUNT", 10); } set { SET("AS_FILES_COUNT", value);  } }
		internal string AS_LOCATION { get { return GET("AS_LOCATION", "AutoSave"); } set { SET("AS_LOCATION", value);  } }

		//Not Serialized



	}
}
