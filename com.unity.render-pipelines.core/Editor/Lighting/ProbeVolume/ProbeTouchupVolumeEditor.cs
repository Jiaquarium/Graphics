using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEditorInternal;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.Rendering
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProbeTouchupVolume))]
    internal class ProbeTouchupVolumeEditor : Editor
    {
        internal static class Styles
        {
            internal static readonly GUIContent s_Size = new GUIContent("Size", "Modify the size of this Probe Volume. This is independent of the Transform's Scale.");
            internal static readonly GUIContent s_IntensityScale = new GUIContent("Probe Intensity Scale", "A scale to be applied to all probes that fall within this probe touchup volume.");
            internal static readonly GUIContent s_InvalidateProbes = new GUIContent("Invalidate Probes", "Set all probes falling within this probe touchup volume to invalid.");

            internal static readonly Color k_GizmoColorBase = new Color32(222, 132, 144, 255);

            internal static readonly Color[] k_BaseHandlesColor = new Color[]
            {
                k_GizmoColorBase,
                k_GizmoColorBase,
                k_GizmoColorBase,
                k_GizmoColorBase,
                k_GizmoColorBase,
                k_GizmoColorBase
            };
        }


        SerializedProbeTouchupVolume m_SerializedTouchupVolume;
        internal const EditMode.SceneViewEditMode k_EditShape = EditMode.SceneViewEditMode.ReflectionProbeBox;

        static HierarchicalBox _ShapeBox;
        static HierarchicalBox s_ShapeBox
        {
            get
            {
                if (_ShapeBox == null)
                    _ShapeBox = new HierarchicalBox(Styles.k_GizmoColorBase, Styles.k_BaseHandlesColor);
                return _ShapeBox;
            }
        }

        protected void OnEnable()
        {
            m_SerializedTouchupVolume = new SerializedProbeTouchupVolume(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            ProbeVolume probeVolume = target as ProbeVolume;

            var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            if (renderPipelineAsset != null && renderPipelineAsset.GetType().Name == "HDRenderPipelineAsset")
            {
                serializedObject.Update();

                if (!ProbeReferenceVolume.instance.isInitialized || !ProbeReferenceVolume.instance.enabledBySRP)
                {
                    EditorGUILayout.HelpBox("The probe volumes feature is disabled. The feature needs to be enabled in the HDRP Settings and on the used HDRP asset.", MessageType.Warning, wide: true);
                    return;
                }
                EditorGUILayout.PropertyField(m_SerializedTouchupVolume.invalidateProbes, Styles.s_InvalidateProbes);
                EditorGUI.BeginDisabledGroup(m_SerializedTouchupVolume.invalidateProbes.boolValue);
                EditorGUILayout.PropertyField(m_SerializedTouchupVolume.intensityScale, Styles.s_IntensityScale);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.HelpBox("Probe Volumes is not a supported feature by this SRP.", MessageType.Error, wide: true);
            }

            m_SerializedTouchupVolume.Apply();
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void DrawGizmosSelected(ProbeTouchupVolume touchupVolume, GizmoType gizmoType)
        {
            using (new Handles.DrawingScope(Matrix4x4.TRS(touchupVolume.transform.position, touchupVolume.transform.rotation, Vector3.one)))
            {
                // Bounding box.
                s_ShapeBox.center = Vector3.zero;
                s_ShapeBox.size = touchupVolume.size;
                s_ShapeBox.baseColor = new Color(0.75f, 0.2f, 0.18f);
                s_ShapeBox.DrawHull(true);
            }
        }

        protected void OnSceneGUI()
        {
            ProbeTouchupVolume touchupVolume = target as ProbeTouchupVolume;

            //important: if the origin of the handle's space move along the handle,
            //handles displacement will appears as moving two time faster.
            using (new Handles.DrawingScope(Matrix4x4.TRS(Vector3.zero, touchupVolume.transform.rotation, Vector3.one)))
            {
                //contained must be initialized in all case
                s_ShapeBox.center = Quaternion.Inverse(touchupVolume.transform.rotation) * touchupVolume.transform.position;
                s_ShapeBox.size = touchupVolume.size;

                s_ShapeBox.monoHandle = false;
                EditorGUI.BeginChangeCheck();
                s_ShapeBox.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects(new Object[] { touchupVolume, touchupVolume.transform }, "Change Probe Touchup Volume Bounding Box");

                    touchupVolume.size = s_ShapeBox.size;
                    Vector3 delta = touchupVolume.transform.rotation * s_ShapeBox.center - touchupVolume.transform.position;
                    touchupVolume.transform.position += delta; ;
                }
            }
        }
    }
}
