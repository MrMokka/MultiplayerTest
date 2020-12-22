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
using UnityEditor.SceneManagement;

namespace EMX.HierarchyPlugin.Editor
{
	class HierarchyTempSceneDataPostprocessor : AssetPostprocessor
	{
		static Dictionary<char, char> numbs = Enumerable.Repeat(0, 10).Select((a, i) => i.ToString()[0]).ToDictionary(k => k, v => v);
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) // Debug.Log( "OnPostprocessAllAssets "  );
		{
			if (Root.p == null || Root.p.Length == 0 || Root.p[0] == null) return;
			if (!Root.p[0].par_e.ENABLE_ALL) return;
			foreach (var item in Root.p)
			{
				if (item == null) continue;
				item.invoke_ON_ASSET_IMPORT();
			}

			/*  string debug = "";
			  if (importedAssets.Length != 0) debug += ("--- importedAssets ---\n");
			  foreach (var item in importedAssets) debug += (item + "\n");
			  if (deletedAssets.Length != 0) debug += ("--- deletedAssets ---\n");
			  foreach (var item in deletedAssets) debug += (item + "\n");
			  if (movedAssets.Length != 0) debug += ("--- movedAssets ---\n");
			  foreach (var item in movedAssets) debug += (item + "\n");
			  if (movedFromAssetPaths.Length != 0) debug += ("--- movedFromAssetPaths ---\n");
			  foreach (var item in movedFromAssetPaths) debug += (item + "\n");
			  if (!string.IsNullOrEmpty(debug)) Debug.Log(debug);*/

			{
				bool hasScene = false;
				for (int i = 0; i < importedAssets.Length; i++) hasScene |= importedAssets[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase);
				if (!hasScene) for (int i = 0; i < movedAssets.Length; i++) hasScene |= movedAssets[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase);
				if (!hasScene) for (int i = 0; i < movedFromAssetPaths.Length; i++) hasScene |= movedFromAssetPaths[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase);
				if (hasScene)
				{
					Root.p[0].wasSceneMoved = true;
					/*	for (int i = 0; i < EditorSceneManager.sceneCount; i++)
					{
						var s = EditorSceneManager.GetSceneAt(i);
						if (!s.IsValid() || !s.isLoaded) continue;
						HierarchyTempSceneData.SaveOnScenePathChanged(s);
					}
					*/
				}
			}


			Root.p[0].RESET_DRAWSTACK(0);
			Root.p[0].RESET_DRAWSTACK(1);
			HierarchyObject._child_count.Clear();
			HierarchyObject._sibling_count.Clear();
			HierarchyObject._sibling_memory.Clear();
			Cache.ClearProejctObjects();

			TryFoundCopyedAssets(ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths);
			TryFoundMovedAssets(ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths);
			TryFoundDeletedAssets(ref importedAssets, ref deletedAssets, ref movedAssets, ref movedFromAssetPaths);





			//Initializator.Adapters[Initializator.PROJECT_NAME].ClearHierarchyObjects();
		}


		static void TryFoundCopyedAssets(ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths)
		{
			if (importedAssets.Length != 0)
			{
				for (int i = 0; i < importedAssets.Length; i++)
				{
					if (importedAssets[i].EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
					{
						var estim = importedAssets[i].Remove(importedAssets[i].Length - ".unity".Length);
						//DETECT COPY SCENE
						if (numbs.ContainsKey(estim[estim.Length - 1]))
						{
							var lastind = estim.LastIndexOf(" ");
							int ind = -1;
							if (estim.Length - lastind <= 3 && estim.Length - 1 != lastind && int.TryParse(estim.Remove(lastind + 1), out ind))
							{
								/*var numb = estim.Substring( estim.LastIndexOf( " " ) + 1);
                                var numbInt  = -1;
                                var tryCompleted = false;
                                if ( int.TryParse( numb , out numbInt ) ) {

                                    numbInt--;
                                    if ( numbInt >= 0 ) {
                                        var oldPath = estim.Remove( estim.LastIndexOf( " " ) )( numbInt == 0 ? "" : (" "+ numbInt)) + ".unity";
                                        var sa = AssetDatabase.LoadAssetAtPath<SceneAsset>( oldPath );
                                        if ( sa ) {
                                            var newPathExternal = Hierarchy.HierarchyAdapterInstance.GetStoredDataPathExternal(importedAssets[i]);
                                            if ( File.Exists( newPathExternal ) ) {
                                                if ( Hierarchy.M_Descript.TryCreateBackupForCache( Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( importedAssets[i] ) ) ) {
                                                    AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
                                                    Adapter.RequestScriptReload();
                                                }
                                            }
                                            AssetDatabase.CopyAsset( Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( oldPath ) ,
                                   Hierarchy.HierarchyAdapterInstance.GetStoredDataPathInternal( importedAssets[i] ) );
                                            tryCompleted = true;
                                        }
                                    }


                                }

                                if ( !tryCompleted )*/
								{
									var p = estim.Remove(estim.LastIndexOf(" "));
									var oldPath = p + ".unity";
									SceneAsset sa = null;

									if (File.Exists(oldPath)) sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(oldPath); //p
									else
									{
										for (int z = 0; z < ind; z++)
										{
											if (File.Exists(p + " " + ind.ToString() + ".unity"))
											{
												sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(p + " " + ind.ToString() + ".unity"); //unity
												break;
											}

										}
									}
									// FOUND COPYED SCENE
									if (sa)
									{
										var newPathExternal = HierarchyExternalSceneData.GetStoredDataPathExternal(importedAssets[i]);
										var newPathInternal = HierarchyExternalSceneData.GetStoredDataPathInternal(importedAssets[i]);

										if (File.Exists(newPathExternal))
										{
											if (HierarchyExternalSceneData.TryCreateBackupForCache(newPathInternal))
											{
												//   AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
												//  Adapter.RequestScriptReload();
											}
											File.Delete(newPathExternal);
											if (File.Exists(newPathExternal + ".meta")) File.Delete(newPathExternal + ".meta");
										}
										else
										{
											if (!Directory.Exists(newPathExternal.Remove(newPathExternal.LastIndexOf('/'))))
												Directory.CreateDirectory(newPathExternal.Remove(newPathExternal.LastIndexOf('/')));
										}
										AssetDatabase.CopyAsset(HierarchyExternalSceneData.GetStoredDataPathInternal(oldPath), newPathInternal);
									}
								}
							}
						}
					}
				}
			}
		}

		static void TryFoundMovedAssets(ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths)
		{
			if (movedAssets.Length != 0)
			{
				bool wasBackUp = false;

				for (int i = 0; i < movedAssets.Length; i++)
				{
					if (movedAssets[i].ToLower().EndsWith(".unity"))
					{
						var newPathExternal = HierarchyExternalSceneData.GetStoredDataPathExternal(movedAssets[i]);
						var newPathInternal = HierarchyExternalSceneData.GetStoredDataPathInternal(movedAssets[i]);

						if (File.Exists(newPathExternal))
						{
							if (HierarchyExternalSceneData.TryCreateBackupForCache(newPathInternal)) //HierarchyExternalSceneData.GetStoredDataPathInternal(movedAssets[i])
							{
								//  AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
								//  Adapter.RequestScriptReload();
								wasBackUp = true;
							}
							File.Delete(newPathExternal);
							if (File.Exists(newPathExternal + ".meta")) File.Delete(newPathExternal + ".meta");
						}
						else
						{
							if (!Directory.Exists(newPathExternal.Remove(newPathExternal.LastIndexOf('/'))))
								Directory.CreateDirectory(newPathExternal.Remove(newPathExternal.LastIndexOf('/')));
						}

						AssetDatabase.MoveAsset(HierarchyExternalSceneData.GetStoredDataPathInternal(movedFromAssetPaths[i]), newPathInternal);
					}
				}

				if (wasBackUp) { }
			}
		}

		static void TryFoundDeletedAssets(ref string[] importedAssets, ref string[] deletedAssets, ref string[] movedAssets, ref string[] movedFromAssetPaths)
		{
			if (deletedAssets.Length != 0)
			{
				foreach (var item in Root.p)
				{
					if (item == null) continue;
					item.invoke_ReloadAfterAssetDeletingOrPasting();
					//    Hierarchy.HierarchyAdapterInstance.Again_Reloder_UsingWhenCopyPastOrAssets();
				}
			}
		}
	}


}
