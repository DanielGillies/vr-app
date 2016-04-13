/*
* MPMP
* Copyright © 2016 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace monoflow
{

    public class MPMP_Editor_Utils 
    {

        [MenuItem(MPMP.MENUITEM_NEW_MPMP_VIDEO_SYNCHRONIZER)]
        static void _NewMPMP_MPMP_VideoSynchronizer()
        {
            //Debug.Log("_NewMPMP");
            GameObject go = new GameObject("MPMP.VideoSynchronizer");
            go.AddComponent<MPMP_VideoSynchronizer>();
        }



        #region SEEK_TOLERANCE

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IPHONE
#endif
        private static bool prefsLoaded = false;
        // The Preferences
        private static bool boolPreferenceMPMP_seekWithTolerance = false;

        [PreferenceItem("MPMP")]
        private static void CustomPreferencesGUI()
        {
            if (!prefsLoaded)
            {
                boolPreferenceMPMP_seekWithTolerance = EditorPrefs.GetBool("MPMP_seek_tolerance_key", false);
                prefsLoaded = true;
            }
            EditorGUIUtility.labelWidth =220f;
            //EditorGUILayout.LabelField("Enable or disable the ");
            boolPreferenceMPMP_seekWithTolerance = EditorGUILayout.Toggle("Seeking with tolerance on iOS/OSX: ", boolPreferenceMPMP_seekWithTolerance);

            if (GUI.changed)
            {
                EditorPrefs.SetBool("MPMP_seek_tolerance_key", boolPreferenceMPMP_seekWithTolerance);
                if (boolPreferenceMPMP_seekWithTolerance) {
                    MountScriptingDefineSymbolToAllTargets("SEEK_TOLERANCE");
                } else
                {
                    UnMountScriptingDefineSymbolToAllTargets("SEEK_TOLERANCE");
                }
            }


        }
#endregion


        public static void MountScriptingDefineSymbolToAllTargets(string defineSymbol)
        {
            foreach (BuildTargetGroup _group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (_group == BuildTargetGroup.Unknown) continue;

                List<string> _defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group).Split(';').Select(d => d.Trim()).ToList();

                if (!_defineSymbols.Contains(defineSymbol))
                {
                    _defineSymbols.Add(defineSymbol);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(_group, string.Join(";", _defineSymbols.ToArray()));
                }
            }
        }

        public static void UnMountScriptingDefineSymbolToAllTargets(string defineSymbol)
        {
            foreach (BuildTargetGroup _group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (_group == BuildTargetGroup.Unknown) continue;

                List<string> _defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_group).Split(';').Select(d => d.Trim()).ToList();

                if (_defineSymbols.Contains(defineSymbol))
                {
                    _defineSymbols.Remove(defineSymbol);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(_group, string.Join(";", _defineSymbols.ToArray()));
                }
            }
        }





    }


  



}
