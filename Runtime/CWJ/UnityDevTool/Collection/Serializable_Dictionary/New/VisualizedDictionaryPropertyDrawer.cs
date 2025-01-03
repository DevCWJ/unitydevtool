#if UNITY_2020_1_OR_NEWER && UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using CWJ.AccessibleEditor;

namespace CWJ.Collection
{
	using static DictionaryPropertyDrawerCore;

	[CustomPropertyDrawer(typeof(DictionaryVisualized<,>), true)]
	public class VisualizedDictionaryPropertyDrawer : PropertyDrawer_CWJ
	{
		static readonly float combinedPadding = lineHeight + standardVertSpace;

		public const string FieldName_keyCollision = "keyCollision";

		public override float GetHeight(SerializedProperty property, GUIContent label)
		{
			float propertyHeight = EditorGUIUtility.singleLineHeight;

			if (property.isExpanded)
			{
				var keyValueListProperty = property.FindPropertyRelative(FieldName_keyValues);

				foreach (var keyValue in GetKeyValueEnumerable(keyValueListProperty))
				{
					var keyProperty = keyValue.keyProperty;
					var valueProperty = keyValue.valueProperty;

					float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty, true);
					float valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty, true) : 0f;

					bool keyCanBeExpanded = CanPropertyBeExpanded(keyProperty);
					bool valueCanBeExpanded = valueProperty != null && CanPropertyBeExpanded(valueProperty);

					float lineH = 0f;
					if (valueProperty == null)
					{
						lineH = keyPropertyHeight;
					}
					else
					{
						if (!keyCanBeExpanded && valueCanBeExpanded)
						{
							// 여기서 DrawKeyValueField_ExpandValueOnly 로직 반영
							if (valueProperty.isExpanded)
							{
								// 확장된 상황
								float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;
								// lineHeight = keyHeight + (valueHeight + verticalSpacing)
								lineH = keyPropertyHeight + valuePropertyHeight + verticalSpacing;
							}
							else
							{
								lineH = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
							}
						}
						else
						{
							// 기존 DrawKeyValueField와 동일한 계산 로직
							lineH = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
						}
					}

					propertyHeight += lineH;
				}

				var conflictState = GetConflictState(property);
				if (conflictState.conflictIndex != -1)
				{
					propertyHeight += conflictState.conflictLineHeight;
				}
			}

			return propertyHeight;
		}


		private Action<int> buttonCallback = null;
		private int buttonActionIndex;

		public override bool DrawGUI(FieldInfo fieldInfo, Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
		{
			label = EditorGUI.BeginProperty(position, label, property);
			var headerPos = position;
			headerPos.height = EditorGUIUtility.singleLineHeight;
			if (property.isExpanded)
				headerPos.xMax -= s_buttonStyle.CalcSize(s_iconPlus).x;

			EditorGUI.PropertyField(headerPos, property, label, false);

			if (property.isExpanded)
			{
				SerializedProperty keyValueListProp = property.FindPropertyRelative(FieldName_keyValues);

				int keyValueCnt = keyValueListProp.GetSafelyLength();

				int[] nullKeyIndexes = ConvertIntArray(property.FindPropertyRelative(FieldName_NullKeyIndexes));
				int nullKeyLength = nullKeyIndexes.LengthSafe();

				int[] conflictOriginIndexes = ConvertIntArray(property.FindPropertyRelative(FieldName_ConflictOriginKeyIndexes));
				int conflictOriginLength = conflictOriginIndexes.LengthSafe();

				int[] conflictWarningIndexes = ConvertIntArray(property.FindPropertyRelative(FieldName_ConflictWarningKeyIndexes));
				int conflictWarningLength = conflictWarningIndexes.LengthSafe();
				Action<int, Rect> drawConflictInfo = null;

				if (conflictOriginLength > 0 || nullKeyLength > 0)
				{
					drawConflictInfo = (itemIndex, lineRect) =>
					{
						Rect iconRect = lineRect;
						GUIContent icon = null;

						if (conflictOriginIndexes.IsExists(itemIndex))
							icon = s_warningIcon_Origin;
						else if (conflictWarningIndexes.IsExists(itemIndex))
							icon = s_warningIcon_Conflict;
						else if (nullKeyIndexes.IsExists(itemIndex))
							icon = s_warningIcon_Null;

						if (icon != null)
						{
							var singleLineHeight = EditorGUIUtility.singleLineHeight;
							iconRect.size = new Vector2(singleLineHeight, singleLineHeight);
							GUI.Label(iconRect, icon);
						}
					};
				}

				var buttonPosition = position;
				buttonPosition.xMin = buttonPosition.xMax - ButtonWidth;
				buttonPosition.height = EditorGUIUtility.singleLineHeight;
				if (GUI.Button(buttonPosition, s_iconPlus, s_buttonStyle))
				{
					buttonCallback += keyValueListProp.InsertArrayElementAtIndex;
					buttonActionIndex = keyValueCnt;
				}

				EditorGUI.indentLevel++;
				var linePosition = position;
				linePosition.y += EditorGUIUtility.singleLineHeight;
				linePosition.xMax -= ButtonWidth;

				foreach (var keyValue in GetKeyValueEnumerable(keyValueListProp))
				{
					var keyProperty = keyValue.keyProperty;
					var valueProperty = keyValue.valueProperty;
					int i = keyValue.index;

					float lineHeight = DrawKeyValueLine(keyProperty, valueProperty, linePosition, i);

					buttonPosition = linePosition;
					buttonPosition.xMin = buttonPosition.xMax - ButtonWidth;
					buttonPosition.width = ButtonWidth;
					buttonPosition.x += ButtonWidth;
					buttonPosition.height = EditorGUIUtility.singleLineHeight;

					if (GUI.Button(buttonPosition, s_iconMinus, s_buttonStyle))
					{
						buttonCallback += (actionIndex) => DeleteArrayElementAtIndex(keyValueListProp, actionIndex);
						buttonActionIndex = i;
					}

					drawConflictInfo?.Invoke(i, linePosition);

					linePosition.y += lineHeight;
				}

				if (buttonCallback != null)
				{
					EditorGUI_CWJ.RemoveFocusFromText();
					buttonCallback.Invoke(buttonActionIndex);
					buttonCallback = null;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
			return property.isExpanded;
		}
	}
}
#endif
