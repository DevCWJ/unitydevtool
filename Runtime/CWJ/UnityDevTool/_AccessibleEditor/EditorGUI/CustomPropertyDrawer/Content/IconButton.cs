﻿#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace CWJ.EditorOnly
{
    public class IconButton : StaticContent
    {
        public const string Add = "Toolbar Plus";
        public const string CustomAdd = "Toolbar Plus More";
        public const string Remove = "Toolbar Minus";
        public const string Edit = "UnityEditor.InspectorWindow";
        public const string Expanded = "IN foldout focus on";
        public const string Collapsed = "IN foldout focus";

        public string IconName;
        public string Tooltip;

        public IconButton(string iconName, string tooltip = "")
        {
            IconName = iconName;
            Tooltip = tooltip;
        }

        protected override GUIContent Create()
        {
            var image = !string.IsNullOrEmpty(IconName) ? EditorGUIUtility.IconContent(IconName)?.image : null;
            return new GUIContent(image, Tooltip);
        }
    }
}

#endif