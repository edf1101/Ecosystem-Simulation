using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;



[CustomEditor(typeof(TerrainGenerator))]
class DecalMeshHelperEditor : Editor
{
    SerializedProperty m_strength;
    void OnEnable()
    {
        
        m_strength = serializedObject.FindProperty("strength");
    }

    public override void OnInspectorGUI() // all used to test terrain gen stuff in editor
    {
        TerrainGenerator myTarget = (TerrainGenerator)target;
        GUILayout.Label(" Terrain");
        if (GUILayout.Button("Fully Generate Terrain"))
            myTarget.generateTerrain();
        if (GUILayout.Button("Partial Generate Terrain"))
            myTarget.partialGenerateTerrain();

        GUILayout.Label(" ");
        GUILayout.Label(" ");
        GUILayout.Label(" ");
        GUILayout.Label("Perlin Noise");
        if (GUILayout.Button("Generate Noise"))
            myTarget.GenerateNoise();
       //
        GUILayout.Label("");
        GUILayout.Label("Base Mesh");

        if (GUILayout.Button("Cheap Mesh Gen"))
        {
            myTarget.GenCheapPoints();
            myTarget.TriangulateMesh();
        }
        if (GUILayout.Button("Expensive Mesh Gen"))
        {
            myTarget.GenExpensivePoints();
            myTarget.TriangulateMesh();
        }
        GUILayout.Label("");
        GUILayout.Label("Mesh height");
        EditorGUILayout.PropertyField(m_strength, new GUIContent("Strength"));
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Apply HeightMap"))
        {
           myTarget.ApplyHeight();

        }
        GUILayout.Label("");
        GUILayout.Label("Rivering");
        
        if (GUILayout.Button("Create  Rivers"))
        {
            myTarget.createRivers();

        }
        GUILayout.Label("");
        GUILayout.Label("Biomes");

        if (GUILayout.Button("Create Biomes"))
        {
            myTarget.createBiomes();

        }
        GUILayout.Label("");
        GUILayout.Label("Colors");

        if (GUILayout.Button("Assign Cols"))
        {
            myTarget.assignCols();

        }
        GUILayout.Label("");
        GUILayout.Label("Sea");
        if (GUILayout.Button("Gen Sea"))
        {
            myTarget.seaGen();

        }
        GUILayout.Label("");
        GUILayout.Label("Objects");
        if (GUILayout.Button("Test"))
        {
            myTarget.objTest();

        }
        if (GUILayout.Button("Add trees"))
        {
            myTarget.addTrees();

        }
        if (GUILayout.Button("Add flowers"))
        {
            myTarget.addFlowers();

        }

    }
}
#endif