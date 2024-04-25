using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CustomEditorForRenderPipeline(typeof(MonoBehaviour),typeof(UniversalRenderPipelineAsset))]
public class ScreenVisualizer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


    }
}
