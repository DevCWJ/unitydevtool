﻿#if UNITY_EDITOR
using System;
using System.Reflection;

using CWJ;

using UnityEditor;

using UnityEngine;

namespace CWJ.EditorOnly
{
    public class ListDisplayAttributeControl : PropertyControl
    {
        private const string _invalidTypeWarning = "Invalid type for ListDisplay on field {0}: ListDisplay can only be applied to SerializedList or SerializedArray fields";

        private static IconButton _addButton = new IconButton(IconButton.Add, "Add an item to this list");
        private static IconButton _editButton = new IconButton(IconButton.Edit, "Edit this item");
        private static IconButton _removeButton = new IconButton(IconButton.Remove, "Remove this item from the list");

        private SerializedProperty _property;
        private GUIContent _label;
        private PropertyListControl _listControl = new PropertyListControl();
        private Type _assetListType;

        private static string GetOpenPreference(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetType().Name + "." + property.propertyPath + ".IsOpen";
        }

        public override void Setup(SerializedProperty property, FieldInfo fieldInfo)
        {
            _property = property.FindPropertyRelative("_items");

            if (_property == null || !_property.isArray)
            {
                Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
                _property = null;
            }
            else
            {
                var attribute = TypeHelper.GetAttribute<ListDisplayAttribute>(fieldInfo);

                if (attribute != null)
                {
                    _listControl.Setup(_property);
                    _assetListType = attribute.UseAssetPopup;

                    if (attribute.InlineChildren)
                        _listControl.MakeDrawable(DrawItemInline).MakeCustomHeight(GetItemInlineHeight);
                    else if (attribute.UseAssetPopup != null)
                        _listControl.MakeDrawable(DrawItemAsAssetPopup).MakeCustomHeight(GetItemAssetPopupHeight);

                    if (attribute.AllowAdd)
                        _listControl.MakeAddable(_addButton);

                    if (attribute.ShowEditButton)
                        _listControl.MakeEditable(_editButton);

                    if (attribute.AllowRemove)
                        _listControl.MakeRemovable(_removeButton);

                    if (attribute.AllowReorder)
                        _listControl.MakeReorderable();

                    if (attribute.AllowCollapse)
                        _listControl.MakeCollapsable(GetOpenPreference(property));

                    if (attribute.EmptyText != null)
                        _listControl.MakeEmptyLabel(new GUIContent(attribute.EmptyText));
                }
            }
        }

        public override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return _property != null ? _listControl.GetHeight() : EditorGUI.GetPropertyHeight(property, label);
        }

        public override void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_property != null)
            {
                // During drawing the text of label changes so Unity must internally pool or reuse it for each element. In
                // any case, making a copy of it fixes the problem.

                if (_label == null)
                    _label = new GUIContent(label);

                _listControl.Draw(position, _label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private float GetItemInlineHeight(int index)
        {
            var property = _property.GetArrayElementAtIndex(index);
            return InlineDisplayAttribute_Editor.GetHeight(property);
        }

        private void DrawItemInline(Rect position, SerializedProperty listProperty, int index)
        {
            var property = listProperty.GetArrayElementAtIndex(index);
            InlineDisplayAttribute_Editor.Draw(position, property, null);
        }

        private float GetItemAssetPopupHeight(int index)
        {
            return AssetPopupDrawer.GetHeight();
        }

        private void DrawItemAsAssetPopup(Rect position, SerializedProperty listProperty, int index)
        {
            var property = listProperty.GetArrayElementAtIndex(index);
            AssetPopupDrawer.Draw(position, GUIContent.none, property, _assetListType, true, false, true);
        }
    }

    [CustomPropertyDrawer(typeof(ListDisplayAttribute))]
    public class ListDisplayAttributeDrawer : ControlDrawer<ListDisplayAttributeControl>
    {
    }
}

#endif