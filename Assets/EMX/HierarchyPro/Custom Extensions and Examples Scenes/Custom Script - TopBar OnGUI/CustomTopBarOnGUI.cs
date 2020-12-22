#if UNITY_EDITOR

using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Examples.HierarchyPlugin
{
	public class CustomTopBarOnGUI
	{

		// Try to copy it or just uncomment

		/*
		[InitializeOnLoadMethod]
		static void CreateEvents() //should be a static
		{
			EMX." + Root.CUST_NS + ".ExtensionInterface_TopBarOnGUI.onLeftLayoutGUI += OnLeftLayoutGUI;
			EMX." + Root.CUST_NS + ".ExtensionInterface_TopBarOnGUI.onRightLayoutGUI += OnRightLayoutGUI;
		}

		static void OnLeftLayoutGUI(Rect rect)
		{
			if (GUILayout.Button("GO_LEFT LAYOUT", GUILayout.ExpandHeight(true)))
			{
				// Something
			}
		}

		static void OnRightLayoutGUI(Rect rect)
		{
			if (GUILayout.Button("GO_RIGHT LAYOUT", GUILayout.ExpandHeight(true)))
			{
				// Something
			}
		}
		*/

	}
}
#endif
