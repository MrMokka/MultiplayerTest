#if UNITY_EDITOR

using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Examples.HierarchyPlugin
{
	public class HierarchyExample_DrawInHier_Scene : MonoBehaviour
	{


		[DRAW_IN_HIER] //FIELD
		public GameObject go_field_a = null;

		[DRAW_IN_HIER] //FIELD
		public GameObject go_field_b = null;

		[DRAW_IN_HIER] //PROPERTY
		public int health_prop { get { return 1; } }

		[DRAW_IN_HIER] //PROPERTY
		public int speed_prop { get { return _speed_prop; } set { _speed_prop = value; Debug.Log("Speed setted to: " + value); } }
		int _speed_prop = 50;

		[DRAW_IN_HIER] //METHOD
		void changeTarget()
		{
			Debug.Log("Target Changed");
		}

		[DRAW_IN_HIER] //ENUM
		internal EntType type_enum = EntType.Mice;
		internal enum EntType { Dog, Cat, Mice }

	}
}
#endif