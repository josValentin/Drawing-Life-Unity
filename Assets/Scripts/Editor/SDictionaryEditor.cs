using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SKeyValuePair<,>), true)]
public class SKeyValuePairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keyProp = property.FindPropertyRelative("key");
        SerializedProperty valProp = property.FindPropertyRelative("value");

        // Obtener valor de la key como string
        string keyString = GetPropertyValueAsString(keyProp);
        if (string.IsNullOrEmpty(keyString))
            keyString = "";

        // Dibujar foldout usando la key como título
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            keyString,
            true
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Dibujar Key
            Rect keyRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUI.GetPropertyHeight(keyProp, true)
            );
            EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Key"), true);

            // Dibujar Value
            Rect valRect = new Rect(
                position.x,
                keyRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUI.GetPropertyHeight(valProp, true)
            );
            EditorGUI.PropertyField(valRect, valProp, new GUIContent("Value"), true);

            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // foldout siempre ocupa una línea

        if (property.isExpanded)
        {
            SerializedProperty keyProp = property.FindPropertyRelative("key");
            SerializedProperty valProp = property.FindPropertyRelative("value");

            height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(keyProp, true);
            height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(valProp, true);
        }

        return height;
    }

    // Convierte un SerializedProperty en string legible
    private string GetPropertyValueAsString(SerializedProperty prop)
    {
        if (prop == null) return string.Empty;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.Float: return prop.floatValue.ToString("0.###");
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Enum: return prop.enumDisplayNames[prop.enumValueIndex];
            case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue ? prop.objectReferenceValue.name : "None";
            default: return prop.displayName; // fallback
        }
    }
}

[CustomPropertyDrawer(typeof(SDictionary<,>), true)]
public class SDictionaryDrawer : PropertyDrawer
{
    private bool hasDuplicates;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty entries = property.FindPropertyRelative("entries");
        EditorGUI.PropertyField(position, entries, label, true);

        hasDuplicates = false;
        if (entries.isArray)
        {
            var keys = new HashSet<object>();
            for (int i = 0; i < entries.arraySize; i++)
            {
                var element = entries.GetArrayElementAtIndex(i);
                var keyProp = element.FindPropertyRelative("key");

                object keyObj = keyProp.propertyType switch
                {
                    SerializedPropertyType.String => keyProp.stringValue,
                    SerializedPropertyType.Integer => keyProp.intValue,
                    SerializedPropertyType.Boolean => keyProp.boolValue,
                    SerializedPropertyType.Float => keyProp.floatValue,
                    SerializedPropertyType.Enum => keyProp.enumNames[keyProp.enumValueIndex],
                    SerializedPropertyType.ObjectReference => keyProp.objectReferenceValue,
                    _ => null
                };

                if (keyObj != null && !keys.Add(keyObj))
                    hasDuplicates = true;
            }
        }

        if (hasDuplicates)
        {
            Rect helpRect = new Rect(
                position.x,
                position.y + EditorGUI.GetPropertyHeight(entries, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight * 2f
            );
            EditorGUI.HelpBox(helpRect, $"Duplicated Keys Found ({property.displayName})", MessageType.Error);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty entries = property.FindPropertyRelative("entries");
        float baseHeight = EditorGUI.GetPropertyHeight(entries, label, true);

        if (hasDuplicates)
            baseHeight += EditorGUIUtility.singleLineHeight * 2f + EditorGUIUtility.standardVerticalSpacing;

        return baseHeight;
    }
}

[CustomPropertyDrawer(typeof(SLDictionary<,>), true)]
public class SLDictionaryDrawer : SDictionaryDrawer
{
}