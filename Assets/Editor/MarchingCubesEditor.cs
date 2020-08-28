using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



namespace MarchingCode.MarchingCubes.Assets.Codebase
{
    [CustomEditor(typeof(MarchingCubes))]
    public class MarchingCubesEditor : Editor
    {
        MarchingCubes original;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            original = (MarchingCubes)target;

            if (GUILayout.Button("Reload"))
            {
                original.ReloadMesh();
            }
        }

        private void OnSceneGUI()
        {

        }
    }
}
