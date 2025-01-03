﻿#if UNITY_EDITOR
using System.Linq;
using System.Collections.Generic;

using CWJ.AccessibleEditor.CustomDefine;
using CWJ.AccessibleEditor.DebugSetting;

using UnityEditor;
using UnityEngine;
using System;

#endif

namespace CWJ.AccessibleEditor
{
    public class DefineSymbolUtil
    {
        /// <summary>
        /// <para>Custom Define 추가</para>
        /// Reset에서 사용하는게 적당
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="enableDefault"></param>
        public static void AddCustomDefineSymbol(string symbolName, bool enableDefault = true)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildEventSystem.IsBuilding) return;

            if (string.IsNullOrEmpty(symbolName)) return;

            if (!CustomDefine_Window.ScriptableObj.isInitialized) CustomDefine_Window.UpdateSetting();

            if (symbolName.Equals(CWJ.SceneHelper.SceneEnumDefine.CWJ_SCENEENUM_ENABLED))
            {
                CustomDefine_Window.ScriptableObj.isSceneEnumSyncBackup = CustomDefine_Window.ScriptableObj.isSceneEnumSync = true;
            }
            else
            {
                int findIndex = CustomDefine_Window.ScriptableObj.symbolStructs.FindIndex((d) => d.symbolName.Equals(symbolName));
                if (findIndex < 0)
                {
                    CustomDefine_Window.ScriptableObj.WillAddDefine(symbolName, enableDefault);
                }
                //else
                //{
                //    CustomDefine_Window.ScriptableObj.SetDefineEnable(symbolName, enableDefault);
                //}
            }

            CustomDefine_Window.UpdateSettingAndOpen();
#endif
        }

        /// <summary>
        /// <para>Custom Define 제거</para>
        /// </summary>
        /// <param name="symbolName"></param>
        public static void RemoveCustomDefineSymbol(string symbolName)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildEventSystem.IsBuilding) return;

            if (string.IsNullOrEmpty(symbolName)) return;

            if (!CustomDefine_Window.ScriptableObj.isInitialized) CustomDefine_Window.UpdateSetting();

            int findIndex = CustomDefine_Window.ScriptableObj.symbolStructs.FindIndex((d) => d.symbolName.Equals(symbolName));
            if (findIndex < 0)
            {
                CustomDefine_Window.ScriptableObj.WillRemoveDefine(symbolName);
            }

            CustomDefine_Window.UpdateSettingAndOpen();
#endif
        }

#if UNITY_EDITOR
        public static bool IsContains(string defineName)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Contains(defineName);
        }

        public static string GetCurrentSymbols(bool isOnlyCWJDefine = false)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if(buildTargetGroup == BuildTargetGroup.Unknown)
            {
                AccessibleEditorUtil.OpenBuildSettings();
                buildTargetGroup = BuildTargetGroup.Standalone;
            }
            string symbolsStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return !isOnlyCWJDefine ? symbolsStr : string.Join(";", symbolsStr.Split(';').FindAll((s) => s.Contains(nameof(CWJ)) || s.Equals("UNITY_ASSERTIONS")));
        }

        public static string[] GetCurrentSymbolsToArray(bool isOnlyCWJDefine = false) => GetCurrentSymbols(isOnlyCWJDefine).Split(';');

        public static bool RegistSymbolsToAllTargets(bool isAdd, bool isPrintLog, params string[] symbols)
        {
            if (isAdd) return AddSymbolsToAllTargets(isPrintLog, addSymbols: symbols);
            else return RemoveSymbolsFromAllTargets(isPrintLog, removeSymbols: symbols);
        }

        public static bool AddSymbolsToAllTargets(bool isPrintLog, params string[] addSymbols)
        {
            return AddSymbolsToAllTargets(isPrintLog, addSymbolList: addSymbols);
        }

        public static bool AddSymbolsToAllTargets(bool isPrintLog, IList<string> addSymbolList)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildEventSystem.IsBuilding) return false;

            if (addSymbolList == null || addSymbolList.Count == 0) return false;

            if (addSymbolList.Count > 1)
            {
                addSymbolList = addSymbolList.Distinct().ToArray();
            }

            var added = new System.Collections.Generic.List<string>();

            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (!IsValidBuildTargetGroup(group)) continue;

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToHashSet();

                string[] adds = addSymbolList.Where((s) => defineSymbols.Add(s)).ToArray();

                bool changed = adds.Length > 0;

                if (changed)
                {
                    added.AddRange(adds);
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (System.Exception)
                    {
                        typeof(CWJDefineSymbols).PrintLogWithClassName("Could not add compile defines for build target group: " + group, UnityEngine.LogType.Error, isPreventStackTrace: true);
                        continue;
                        //throw;
                    }
                }
            }

            bool isChanged = added.Count > 0;

            if (isPrintLog && isChanged)
            {
                typeof(CWJDefineSymbols).PrintLogWithClassName($"<b>Included defines</b> ({ string.Join(";", added.Distinct())}) " +
                                                               $"\nto <i>Scripting Define Symbols</i> for all build platforms.\n\n\nCurrent Define Symbols : {string.Join("\n", GetCurrentSymbolsToArray())}", UnityEngine.LogType.Log, isBigFont: false, isPreventStackTrace: true);
            }

            return isChanged;
        }

        public static bool RemoveSymbolsFromAllTargets(bool isPrintLog, params string[] removeSymbols)
        {
            return RemoveSymbolsFromAllTargets(isPrintLog, removeSymbolList: removeSymbols);
        }

        public static bool RemoveSymbolsFromAllTargets(bool isPrintLog, IList<string> removeSymbolList)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildEventSystem.IsBuilding) return false;

            if (removeSymbolList == null || removeSymbolList.Count == 0) return false;

            if (removeSymbolList.Count > 1)
            {
                removeSymbolList = removeSymbolList.Distinct().ToArray();
            }

            var removeds = new System.Collections.Generic.List<string>();

            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (!IsValidBuildTargetGroup(group)) continue;

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').Select(d => d.Trim()).ToHashSet();

                string[] removes = removeSymbolList.Where((s) => defineSymbols.Remove(s)).ToArray();

                bool changed = removes.Length > 0;

                if (changed)
                {
                    removeds.AddRange(removes);
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (System.Exception)
                    {
                        typeof(CWJDefineSymbols).PrintLogWithClassName("Could not remove compile defines for build target group: " + group, UnityEngine.LogType.Error, isPreventStackTrace: true);
                        continue;
                        //throw;
                    }
                }
            }

            bool isChanged = removeds.Count > 0;

            if (isPrintLog && isChanged)
            {
                typeof(CWJDefineSymbols).PrintLogWithClassName(
                    message:
                    $"<b>Excluded defines</b> ({string.Join(";", removeds.Distinct())})\nto <i>Scripting Define Symbols</i> for all build platforms.\n\n\nCurrent Define Symbols : {string.Join("\n", GetCurrentSymbolsToArray())}",
                    UnityEngine.LogType.Log, isBigFont: false, isPreventStackTrace: true);
            }

            return isChanged;
        }

        public static bool RemoveAndAddSymbolsFromAllTargets(bool isPrintLog, IList<string> removeSymbolList, IList<string> addSymbolList)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || BuildEventSystem.IsBuilding) return false;

            if (removeSymbolList.Count > 1)
                removeSymbolList = removeSymbolList.Distinct().ToArray();

            if (addSymbolList.Count > 1)
                addSymbolList = addSymbolList.Distinct().ToArray();

            bool isChanged = false;

            foreach (BuildTargetGroup group in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (!IsValidBuildTargetGroup(group)) continue;

                var lastDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
                var defineSymbols = lastDefineSymbols.Where(s => !string.IsNullOrWhiteSpace(s)).Select(d => d.Trim()).ToHashSet();

                string[] removes = removeSymbolList.Where((s) => defineSymbols.Remove(s)).ToArray();
                bool changed = removes.Length > 0;

                string[] adds = addSymbolList.Where((s) => defineSymbols.Add(s)).ToArray();
                changed |= adds.Length > 0;

                var setDefineSymbols = defineSymbols.ToArray();

                if (setDefineSymbols.Length == lastDefineSymbols.Length && ArrayUtil.ArrayEquals(setDefineSymbols, lastDefineSymbols))
                {
                    continue;
                }

                if (changed)
                {
                    if (!isChanged) isChanged = true;
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", setDefineSymbols));
                    }
                    catch (System.Exception)
                    {
                        typeof(CWJDefineSymbols).PrintLogWithClassName("Could not modifiy compile defines for build target group: " + group,
                                                                       UnityEngine.LogType.Error, isPreventStackTrace: true);
                        continue;
                        //throw;
                    }
                }
            }

            if (isPrintLog && isChanged)
            {
                DisplayDialogUtil.DisplayDialog<CWJDefineSymbols>(
                    message:
                    $"<b>Modified defines</b><Loading takes about 1~2 min. If you just look at the two #shorts, the loading will be over>\n로딩 1~2분 걸림. 유튜브쇼츠 두개보고오면 끝나있을거임\nChange to Define Symbols : \n{string.Join("\n", GetCurrentSymbolsToArray())}\nto <i>Scripting Define Symbols</i> for all build platforms.",
                    isError: false);
            }

            return isChanged;
        }

        static List<string> addSymbols = new List<string>();
        static List<string> removeSymbols = new List<string>();

        public static void AddSymbolsStack(params string[] adds)
        {
            AddSymbolsStack(addList: adds);
        }
        public static void AddSymbolsStack(IList<string> addList)
        {
            addSymbols.AddRange(addList);
        }

        public static void RemoveSymbolsStack(params string[] removes)
        {
            RemoveSymbolsStack(removeList: removes);
        }
        public static void RemoveSymbolsStack(IList<string> removeList)
        {
            removeSymbols.AddRange(removeList);
        }

        public static void InvokeRegistStackList()
        {
            int removeCnt = removeSymbols.Count;
            int addCnt = addSymbols.Count;
            if (addCnt > 0 && removeCnt > 0)
            {
                RemoveAndAddSymbolsFromAllTargets(true, removeSymbols, addSymbols);
            }
            else
            {
                if (removeCnt > 0)
                {
                    RemoveSymbolsFromAllTargets(true, removeSymbols);
                }
                else if (addCnt > 0)
                {
                    AddSymbolsToAllTargets(true, addSymbols);
                }
            }

        }

        const string UnityVersion_5_6 = "5.6";
        private static bool IsValidBuildTargetGroup(BuildTargetGroup group)
        {
            if (group == BuildTargetGroup.Unknown || IsObsolete(group))
                return false;

            if (UnityEngine.Application.unityVersion.StartsWith(UnityVersion_5_6)) //내 라이브러리 쓰려면 Unity 버전 5.대는 지양하는게 좋음 최소 2017이상
            {
                if ((int)(object)group == 27)
                    return false;
            }

            return true;
        }

        private static bool IsObsolete(System.Enum value)
        {
            int enumInt = (int)(object)value;

            if (enumInt == 4 || enumInt == 14)
                return false;

            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());
            System.ObsoleteAttribute[] attributes = (System.ObsoleteAttribute[])field.GetCustomAttributes(typeof(System.ObsoleteAttribute), false);
            return attributes.Length > 0;
        }

#endif
    }
}
