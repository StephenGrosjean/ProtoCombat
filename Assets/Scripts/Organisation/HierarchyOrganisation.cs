using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class HierarchyOrganisation : MonoBehaviour {
#if UNITY_EDITOR
    private static Vector2 offset = new Vector2(0, 2);

    static HierarchyOrganisation() {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
        Color txtColor = Color.white;
        Color backgroundColor = new Color(.44f, .44f, .44f);


        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj != null) {
            if (obj.name.Contains("---")) {
                

                Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
                EditorGUI.DrawRect(selectionRect, backgroundColor);

                string newName = obj.name.Replace("---", "");

                EditorGUI.LabelField(offsetRect, newName, new GUIStyle() {
                    normal = new GUIStyleState() { textColor = txtColor },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    
                    
                    }
                    );
               
            }
        }
    }

#endif
}