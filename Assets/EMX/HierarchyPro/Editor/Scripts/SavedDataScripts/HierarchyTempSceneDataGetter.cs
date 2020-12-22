//#define DEBUG_ONLY
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
using System.Text;

namespace EMX.HierarchyPlugin.Editor
{

	class HierarchyTempSceneDataGetter
	{




		internal static void TryToInitBookOrExpand(SaverType type, Scene scene)
		{

			var temp_sd = HierarchyTempSceneData.InstanceFast(scene);
			if (type == SaverType.Bookmarks)
			{
				if (temp_sd.BookMarkCategory_Temp_WasInit) return;
				temp_sd.BookMarkCategory_Temp_WasInit = true;
				var _all = GetAllObjectData(type, scene);
				var all = _all.Values.ToDictionary(v => v.id_in_external_heap, v => v);
				var glob = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene).BookMarks_InternalGlobal;
				if (temp_sd.BookMarkCategory_Temp == null) temp_sd.BookMarkCategory_Temp = new List<BookMarkCategory_Temp>();
				else temp_sd.BookMarkCategory_Temp.Clear();
				foreach (var g in glob)
				{
					var cat = new BookMarkCategory_Temp();
					cat.category_name = g.category_name;
					cat.bgColor = g.bgColor;
					foreach (var b in g.buttons)
					{
						var res = new HierExpands_Temp();
						foreach (var id in b.ids_in_external_heap)
						{
							if (!all.ContainsKey(id)) continue;
							if (!all[id].target) continue;
							res.targets.Add(all[id].target);
						}
						if (res.targets.Count == 0) continue;
						cat.targets.Add(res);
					}
					if (cat.targets.Count == 0) continue;
					temp_sd.BookMarkCategory_Temp.Add(cat);
				}

				if (temp_sd.BookMarkCategory_Temp.Count == 0)
				{
					temp_sd.BookMarkCategory_Temp.Add(new BookMarkCategory_Temp()
					{
						category_name = "Default"

					});
				}
			}
			else if (type == SaverType.SceneHierarchyExands)
			{
				if (temp_sd.HierExpands_Temp_WasInit) return;
				temp_sd.HierExpands_Temp_WasInit = true;
				var _all = GetAllObjectData(type, scene);
				var all = _all.Values.ToDictionary(v => v.id_in_external_heap, v => v);
				var glob = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene).HierExpands_InternalGlobal;
				if (temp_sd.HierExpands_Temp == null) temp_sd.HierExpands_Temp = new List<HierExpands_Temp>();
				else temp_sd.HierExpands_Temp.Clear();
				foreach (var g in glob)
				{
					var res = new HierExpands_Temp();
					string first = "";
					foreach (var id in g.ids_in_external_heap)
					{
						if (!all.ContainsKey(id)) continue;
						res.targets.Add(all[id].target);
						first = all[id].stringValue;
					}
					if (res.targets.Count == 0) continue;
					res.name = first;
					temp_sd.HierExpands_Temp.Add(res);
				}
			}
			else
			{
				throw new Exception(type.ToString());
			}
		}

		internal static void SaveBookOrExpand(SaverType type, Scene scene)
		{

			var temp_sd = HierarchyTempSceneData.InstanceFast(scene);

			Dictionary<int, TempSceneObjectPTR> data = new Dictionary<int, TempSceneObjectPTR>();

			if (type == SaverType.Bookmarks)
			{
				//if (!temp_sd.BookMarkCategory_Temp_WasInit) TryToInitBookOrExpand(type, scene);
				foreach (var item in temp_sd.BookMarkCategory_Temp) foreach (var b in item.targets) foreach (var t in b.targets)
						{
							if (data.ContainsKey(t.GetInstanceID())) continue;
							data.Add(t.GetInstanceID(), new TempSceneObjectPTR(t, -1));
						}


				HierarchyTempSceneData._SetAllObjectData(type, scene, data);
				TEMP_SAVE_TO_FILE(type, scene);
				data = GetAllObjectData(type, scene); //#EMX_TODO maybe remove
				if (data.Count != 0 && data.First().Value.id_in_external_heap == -1) throw new Exception("ASD");

				var ex_sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
				ex_sd.BookMarks_InternalGlobal.Clear();
				foreach (var item in temp_sd.BookMarkCategory_Temp)
				{
					var cat = new BookMarkCategory_Saved();
					cat.category_name = item.category_name;
					cat.bgColor = item.bgColor;
					foreach (var b in item.targets)
						cat.buttons.Add(new HierExpands_Saved() { ids_in_external_heap = b.targets.Select(t => data[t.GetInstanceID()].id_in_external_heap).ToArray() });
					ex_sd.BookMarks_InternalGlobal.Add(cat);
				}


			
				//temp_sd.BookMarkCategory_Temp_WasInit = false;
			}
			else if (type == SaverType.SceneHierarchyExands)
			{
				//if (!temp_sd.HierExpands_Temp_WasInit) TryToInitBookOrExpand(type, scene);
				foreach (var item in temp_sd.HierExpands_Temp) foreach (var t in item.targets)
					{
						if (data.ContainsKey(t.GetInstanceID())) continue;
						data.Add(t.GetInstanceID(), new TempSceneObjectPTR(t, -1));
					}

				HierarchyTempSceneData._SetAllObjectData(type, scene, data);
				TEMP_SAVE_TO_FILE(type, scene);
				data = GetAllObjectData(type, scene); //#EMX_TODO maybe remove
				if (data.Count != 0 && data.First().Value.id_in_external_heap == -1) throw new Exception("ASD");

				var ex_sd = HierarchyExternalSceneData.GetHierarchyExternalSceneData(scene);
				ex_sd.HierExpands_InternalGlobal.Clear();
				foreach (var item in temp_sd.HierExpands_Temp)
				{
					if (item.targets.Count == 0) continue;
					ex_sd.HierExpands_InternalGlobal.Add(new HierExpands_Saved()
					{
						name = item.name,
						ids_in_external_heap = item.targets.Select(t => data[t.GetInstanceID()].id_in_external_heap).ToArray()
					});
				}

				//temp_sd.HierExpands_Temp_WasInit = false;
			}
			else
			{
				throw new Exception(type.ToString());
			}




		}

		public static void SetAllObjectDataAndSave(SaverType type, Scene scene, Dictionary<int, TempSceneObjectPTR> data, bool save = true)
		{
			HierarchyTempSceneData._SetAllObjectData(type, scene, data);
			if (!save) HierarchyExternalSceneData.SkipSetDirty = true;
			Exception f_ex = null;
			try
			{
				TEMP_SAVE_TO_FILE(type, scene);
			}
			catch (Exception ex)
			{
				f_ex = ex;
			}
			if (!save) HierarchyExternalSceneData.SkipSetDirty = false;
			if (f_ex != null) throw new Exception(f_ex.Message + '\n' + f_ex.StackTrace);
			//if (save) 
			//else Root.p[0].RESET_DRAWSTACK();
		}








		static Dictionary<int, Dictionary<int, TempSceneObjectPTR>> _od_DA;
		static Dictionary<int, TempSceneObjectPTR> _od_DB;

		internal static void ClearSceneCache(SaverType type, Scene scene)
		{
			var t = (int)type;
			HierarchyTempSceneData.__clearcache(ref scene, t);
			if (!HierarchyTempSceneData._GetObjectData.TryGetValue(scene.GetHashCode(), out _od_DA) || !_od_DA.TryGetValue(t, out _od_DB)) return;
			HierarchyTempSceneData._GetObjectData[scene.GetHashCode()].Remove(t);

		}

		//ModManualHighligher
		internal static TempSceneObjectPTR GetObjectData(SaverType type, HierarchyObject _o)
		{

			// if (_o.pluginID == 1)
			if (_o.pluginID == 1)
			{
				if (type != SaverType.ModManualHighligher && type != SaverType.ModManualIcons) throw new Exception("Project type exception");
				return ProjectWindowObjectsData.Instance().GetObjectData(_o);
			}
			// if (_o.pluginID == 1)



			var t = (int)type;
			if (HierarchyTempSceneData._GetObjectData.TryGetValue(_o.scene, out _od_DA) && _od_DA.TryGetValue(t, out _od_DB))
			{
				if (!_od_DB.TryGetValue(_o.id, out HierarchyTempSceneData.tryGet)) return null;
				return HierarchyTempSceneData.tryGet;
			}
			HierarchyTempSceneData.tryGet = null;
			var scene = _o.go.scene;
			if (!scene.IsValid()) return null;
			HierarchyTempSceneData.__(ref scene, t);

			return GetObjectData(type, _o);
		}
		internal static TempSceneObjectPTR GetObjectData(SaverType type, UnityEngine.Object _o, Scene scene)
		{
			var t = (int)type;
			if (HierarchyTempSceneData._GetObjectData.TryGetValue(scene.GetHashCode(), out _od_DA) && _od_DA.TryGetValue(t, out _od_DB))
			{
				if (!_od_DB.TryGetValue(_o.GetInstanceID(), out HierarchyTempSceneData.tryGet)) return null;
				return HierarchyTempSceneData.tryGet;
			}
			HierarchyTempSceneData.tryGet = null;
			//var scene = _o.go.scene;
			if (!scene.IsValid()) return null;
			HierarchyTempSceneData.__(ref scene, t);

			return GetObjectData(type, _o, scene);
		}



		internal static Dictionary<int, TempSceneObjectPTR> GetAllObjectData(SaverType type, Scene scene)
		{
			return HierarchyTempSceneData.GetAllObjectData(type, scene);
		}

		/*	internal static Dictionary<int, TempSceneObjectPTR> GetAllObjectData(int type, Scene scene)
			{
				return HierarchyTempSceneData.GetAllObjectData(type, scene);
			}*/

		/*	internal static void SetAllObjectData(SaverType type, Scene scene, Dictionary<int, TempSceneObjectPTR> data)
			{
				HierarchyTempSceneData._SetAllObjectData(type, scene, data);
				TEMP_SAVE_TO_FILE(type, scene);
			}*/
		/*	internal static Dictionary<int, TempSceneObjectPTR> GetAllObjectData(SaverType type, Scene scene, int category)
			{
				return HierarchyTempSceneData.GetAllObjectData(type, scene, int category);
			}*/



		static Dictionary<int, TempSceneObjectPTR> getDic;
		static TempSceneObjectPTR tryGet_get;
		static void _get_or_minus1(SaverType type, HierarchyObject _o)
		{
			var t = (int)type;
			tryGet_get = GetObjectData(type, _o);
			if (tryGet_get == null)
			{
				if (!HierarchyTempSceneData._GetObjectData.ContainsKey(_o.scene))
				{
					HierarchyTempSceneData._GetObjectData.Add(_o.scene, new Dictionary<int, Dictionary<int, TempSceneObjectPTR>>());
					HierarchyTempSceneData._GetScenePath.Add(_o.scene, new Dictionary<int, string>());
				}
				if (!HierarchyTempSceneData._GetObjectData[_o.scene].ContainsKey(t))
					HierarchyTempSceneData._GetObjectData[_o.scene].Add(t, new Dictionary<int, TempSceneObjectPTR>());
				if (!HierarchyTempSceneData._GetScenePath[_o.scene].ContainsKey(t))
					HierarchyTempSceneData._GetScenePath[_o.scene].Add(t, _o.go.scene.path);

				HierarchyTempSceneData._GetObjectData[_o.scene][t].Add(_o.id, tryGet_get = new TempSceneObjectPTR(_o.go, -1));
			}
		}
		static void _get_or_minus1(SaverType type, UnityEngine.Object _o, Scene scene)
		{
			var t = (int)type;
			var sh = scene.GetHashCode();
			tryGet_get = GetObjectData(type, _o, scene);
			if (tryGet_get == null)
			{
				if (!HierarchyTempSceneData._GetObjectData.ContainsKey(sh))
				{
					HierarchyTempSceneData._GetObjectData.Add(sh, new Dictionary<int, Dictionary<int, TempSceneObjectPTR>>());
					HierarchyTempSceneData._GetScenePath.Add(sh, new Dictionary<int, string>());
				}
				if (!HierarchyTempSceneData._GetObjectData[sh].ContainsKey(t))
					HierarchyTempSceneData._GetObjectData[sh].Add(t, new Dictionary<int, TempSceneObjectPTR>());
				if (!HierarchyTempSceneData._GetScenePath[sh].ContainsKey(t))
					HierarchyTempSceneData._GetScenePath[sh].Add(t, scene.path);

				HierarchyTempSceneData._GetObjectData[sh][t].Add(_o.GetInstanceID(), tryGet_get = new TempSceneObjectPTR(_o, -1));
			}
		}


		//ModManualHighligher
		static internal bool SetObjectData(SaverType type, HierarchyObject _o, TempSceneObjectPTR value, bool skipSave = false)
		{

			// if (_o.pluginID == 1)
			if (_o.pluginID == 1)
			{
				if (type != SaverType.ModManualHighligher && type != SaverType.ModManualIcons) throw new Exception("Project type exception");
				return ProjectWindowObjectsData.Instance().SetObjectData(_o, value);
			}
			// if (_o.pluginID == 1)
			//Debug.Log(value.iconData[0].icon_guid);

			if (!_o.go.scene.IsValid()) { Debug.LogWarning("Warning scene is !IsValid"); return false; }

			if (type != SaverType.ModManualIcons && type != SaverType.ModManualHighligher) throw new Exception("type != SaverType.ModManualHighligher");

			_get_or_minus1(type, _o);


			if (type == SaverType.ModManualIcons) tryGet_get.iconData = value.iconData;
			if (type == SaverType.ModManualHighligher) tryGet_get.highLighterData = value.highLighterData;

			if (!skipSave) TEMP_SAVE_TO_FILE(type, _o);


			return true;
		}
		static internal bool SetObjectDataAsComponent(SaverType type, UnityEngine.Object _o, Scene scene, bool value)
		{
			if (!scene.IsValid()) { Debug.LogWarning("Warning scene is !IsValid"); return false; }


			_get_or_minus1(type, _o, scene);
			var res = tryGet_get.boolValue != value || tryGet_get.id_in_external_heap == -1;
			if (res) tryGet_get.boolValue = value;



			//if (!skipSave)
			TEMP_SAVE_TO_FILE(type, scene);


			return true;
		}

		static internal bool SetObjectData(SaverType type, HierarchyObject _o, bool value, bool skipSave = false)
		{
			if (!_o.go.scene.IsValid()) { Debug.LogWarning("Warning scene is !IsValid"); return false; }
			_get_or_minus1(type, _o);
			var res = tryGet_get.boolValue != value || tryGet_get.id_in_external_heap == -1;
			if (res) tryGet_get.boolValue = value;
			/* if ( res )
             {

             }*/
			if (!skipSave && res) TEMP_SAVE_TO_FILE(type, _o);


			return res;
		}
		static internal bool SetObjectData(SaverType type, HierarchyObject _o, string value, bool skipSave = false)
		{
			if (!_o.go.scene.IsValid()) { Debug.LogWarning("Warning scene is !IsValid"); return false; }
			// Undo.RecordObject(tryGet_get, type.ToString() );
			_get_or_minus1(type, _o);
			var res = tryGet_get.stringValue != value || tryGet_get.id_in_external_heap == -1;
			if (res) tryGet_get.stringValue = value;



			if (!skipSave && res) TEMP_SAVE_TO_FILE(type, _o);
			/*   if ( res )
               {

               }*/
			return res;
		}
		static internal bool SetObjectData(SaverType type, HierarchyObject _o, List<int> value, bool skipSave = false)
		{
			if (!_o.go.scene.IsValid()) { Debug.LogWarning("Warning scene is !IsValid"); return false; }
			// Undo.RecordObject(tryGet_get, type.ToString() );
			if (value.Count == 0)
			{
				return RemoveObjectData(type, _o);
			}

			_get_or_minus1(type, _o);
			// var res = tryGet_get.stringValue != value;
			var res = tryGet_get.SetString(value) || tryGet_get.id_in_external_heap == -1;

			if (!skipSave && res) TEMP_SAVE_TO_FILE(type, _o);


			return res;
			/*   if ( res )
               {

               }*/
			// return res;
		}

		/* var t = (int)type;
         if ( !_GetObjectData.TryGetValue( t ^ _o.scene, out getDic ) ) _GetObjectData.Add( t ^ _o.scene, getDic = new Dictionary<int, TempSceneObjectPTR>() );
         if ( !getDic.TryGetValue( _o.id, out tryGet ) ) getDic.Add( _o.id, tryGet = new TempSceneObjectPTR() );
         tryGet.boolValue = value;*/



		/*

        {
            if ( !_GetObjectData[ t ^ _o.scene ].TryGetValue( _o.id, out tryGet ) ) return null;
            return tryGet;
        }*/

		//


		//ModManualHighligher
		static internal bool RemoveObjectData(SaverType type, HierarchyObject _o)
		{


			// if (_o.pluginID == 1)
			if (_o.pluginID == 1)
			{
				if (type != SaverType.ModManualHighligher && type != SaverType.ModManualIcons) throw new Exception("Project type exception");
				return ProjectWindowObjectsData.Instance().RemoveObjectData(_o);
			}
			// if (_o.pluginID == 1)


			/* if ( !_o.go.scene.IsValid() ) { Debug.LogWarning( "Warning scene is !IsValid" ); return; }

           var t = (int)type;
             var tryGet = GetObjectData(type, _o);
             if ( tryGet == null ) return;
             tryGet.Remove( _o.id );*/


			var t = (int)type;
			if (!HierarchyTempSceneData._GetObjectData.ContainsKey(_o.scene)) return false;
			if (!HierarchyTempSceneData._GetObjectData[_o.scene].TryGetValue(t, out getDic)) return false;
			var res = getDic.Remove(_o.id);

			if (res) TEMP_SAVE_TO_FILE(type, _o);

			return res;
			/*

            {
                if ( !_GetObjectData[ t ^ _o.scene ].TryGetValue( _o.id, out tryGet ) ) return null;
                return tryGet;
            }*/

			//
		}















		static void TEMP_SAVE_TO_FILE(SaverType type, HierarchyObject _o)
		{
			HierarchyTempSceneData.SaveToExternalFile(type, _o.go.scene);
			//switch
			Root.p[0].RESET_DRAWSTACK(0);
		}


		internal static void TEMP_SAVE_TO_FILE(SaverType type, Scene scene)
		{
			HierarchyTempSceneData.SaveToExternalFile(type, scene);
			//switch
			Root.p[0].RESET_DRAWSTACK(0);
		}




		internal static void SetUndo(string undoScring)
		{
			//Undo.RecordObject( this, undoScring );
		}

		internal static void SetDirty()
		{
			//if ( Application.isPlaying ) return;
			// Root.p[ 0 ].SetDirty( this );
		}

	}
}
