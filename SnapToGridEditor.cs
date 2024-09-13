using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class SnapToGridEditor : Editor
{
    private bool cloneInsteadOfMove = false;  // Variable to toggle cloning behavior
    
        public override void OnInspectorGUI()
        {
            Transform targetTransform = target as Transform;
    
            // Draw the default inspector
            DrawDefaultInspector();
    
            Renderer renderer = targetTransform.GetComponent<Renderer>();
            if (renderer == null)
            {
                EditorGUILayout.HelpBox("No Renderer found on the object. Cannot resize, duplicate, or rotate automatically.", MessageType.Warning);
                return;
            }
    
            Vector3 objectSize = renderer.bounds.size;
            Vector3 scale = targetTransform.localScale;
            float gridSize = 3.0f;  // Grid size for duplication and snapping
            float buttonWidth = EditorGUIUtility.currentViewWidth / 3;  // Width for most buttons
            float fullWidth = EditorGUIUtility.currentViewWidth;  // Full width for the snap button
    
            // Buttons for specific axis fitting
            DisplayFitToGridButtons(targetTransform, "X Axis Fit", ref scale.x, objectSize.x, buttonWidth);
            DisplayFitToGridButtons(targetTransform, "Z Axis Fit", ref scale.z, objectSize.z, buttonWidth);
    
            targetTransform.localScale = scale;
    
            // Toggle for move or clone
            cloneInsteadOfMove = EditorGUILayout.ToggleLeft("Clone Instead of Move", cloneInsteadOfMove, GUILayout.ExpandWidth(true));
    
            // Navigation and rotation buttons
            GUILayout.Label("Modify Object Position:");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ActionButton(targetTransform, "Up", new Vector3(0, 0, gridSize), buttonWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
    
            GUILayout.BeginHorizontal();
            ActionButton(targetTransform, "Left", new Vector3(-gridSize, 0, 0), buttonWidth);
            GUILayout.FlexibleSpace();
            ActionButton(targetTransform, "Right", new Vector3(gridSize, 0, 0), buttonWidth);
            GUILayout.EndHorizontal();
    
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ActionButton(targetTransform, "Down", new Vector3(0, 0, -gridSize), buttonWidth);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
    
            GUILayout.Label("Rotate Object:");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("90° Left", GUILayout.Width(buttonWidth)))
            {
                targetTransform.Rotate(0, -90, 0);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("90° Right", GUILayout.Width(buttonWidth)))
            {
                targetTransform.Rotate(0, 90, 0);
            }
            GUILayout.EndHorizontal();
    
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("45° Left", GUILayout.Width(buttonWidth)))
            {
                targetTransform.Rotate(0, -45, 0);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("45° Right", GUILayout.Width(buttonWidth)))
            {
                targetTransform.Rotate(0, 45, 0);
            }
            GUILayout.EndHorizontal();
    
            // Snap to grid button
            if (GUILayout.Button("Snap to Grid", GUILayout.Width(fullWidth)))
            {
                SnapToGrid(targetTransform, gridSize);
            }
        }
    
        void DisplayFitToGridButtons(Transform targetTransform, string label, ref float scaleDimension, float objectSizeDimension, float buttonWidth)
        {
            GUILayout.Label(label);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("3", GUILayout.Width(buttonWidth))) scaleDimension *= 3.0f / objectSizeDimension;
            if (GUILayout.Button("6", GUILayout.Width(buttonWidth))) scaleDimension *= 6.0f / objectSizeDimension;
            if (GUILayout.Button("9", GUILayout.Width(buttonWidth))) scaleDimension *= 9.0f / objectSizeDimension;
            GUILayout.EndHorizontal();
        }
    
        void ActionButton(Transform targetTransform, string direction, Vector3 offset, float buttonWidth)
        {
            if (GUILayout.Button(direction, GUILayout.Width(buttonWidth)))
            {
                if (cloneInsteadOfMove)
                {
                    GameObject clone = DuplicateObject(targetTransform);
                    targetTransform.position += offset;  // Move original object
                }
                else
                {
                    targetTransform.position += offset;
                }
            }
        }
    
        GameObject DuplicateObject(Transform original)
        {
            GameObject duplicate;
            if (PrefabUtility.IsPartOfAnyPrefab(original.gameObject))
            {
                duplicate = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(original.gameObject), original.parent);
                duplicate.transform.position = original.position;  // Clone remains in the original position
                duplicate.transform.rotation = original.rotation;
            }
            else
            {
                duplicate = Instantiate(original.gameObject, original.position, original.rotation, original.parent);
            }
            duplicate.name = original.name;  // Keep the same name
            return duplicate;
        }
    
        void SnapToGrid(Transform targetTransform, float gridSize)
        {
            Vector3 position = targetTransform.position;
            position.x = Mathf.Round(position.x / gridSize) * gridSize;
            position.z = Mathf.Round(position.z / gridSize) * gridSize;
            targetTransform.position = position;
        }
}