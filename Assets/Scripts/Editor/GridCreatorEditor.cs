using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridCreator))]
public class GridCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridCreator t_gridCreator = (GridCreator)target;

        if (!t_gridCreator)
            return;

        GUILayout.Space(25);

        if (GUILayout.Button("Create Grid"))
            t_gridCreator.CreateGrid();

        GUILayout.Space(7);

        if (GUILayout.Button("Recreate Grid"))
            t_gridCreator.RecreateGrid();

        GUILayout.Space(7);

        if (GUILayout.Button("Delete Grid"))
            t_gridCreator.DeleteGrid();
    }
}
