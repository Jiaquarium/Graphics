using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using static UnityEditor.EditorGUI;

namespace UnityEditor.Rendering.HighDefinition
{
    [CustomEditor(typeof(WaterSurface))]
    sealed class WaterSurfaceEditor : Editor
    {
        // Geometry parameters
        SerializedProperty m_Infinite;
        SerializedProperty m_HighFrequencyBands;
        SerializedProperty m_GeometryType;
        SerializedProperty m_Geometry;

        // CPU Simulation
        SerializedProperty m_CPUSimulation;
        SerializedProperty m_CPUFullResolution;
        SerializedProperty m_CPUEvaluateAllBands;

        // Simulation parameters
        SerializedProperty m_WaterMaxPatchSize;
        SerializedProperty m_Amplitude;
        SerializedProperty m_Choppiness;
        SerializedProperty m_TimeMultiplier;

        // Rendering parameters
        SerializedProperty m_CustomMaterial;
        SerializedProperty m_WaterSmoothness;

        // Refraction parameters
        SerializedProperty m_MaxRefractionDistance;
        SerializedProperty m_AbsorptionDistance;
        SerializedProperty m_RefractionColor;

        // Scattering parameters
        SerializedProperty m_ScatteringColor;
        SerializedProperty m_HeightScattering;
        SerializedProperty m_DisplacementScattering;
        SerializedProperty m_DirectLightTipScattering;
        SerializedProperty m_DirectLightBodyScattering;

        // Caustic parameters (Common)
        SerializedProperty m_Caustics;
        SerializedProperty m_CausticsIntensity;
        // Simulation Caustics
        SerializedProperty m_CausticsAlgorithm;
        SerializedProperty m_CausticsResolution;
        SerializedProperty m_CausticsBand;
        SerializedProperty m_CausticsVirtualPlaneDistance;

        // Procedural caustics
        SerializedProperty m_CausticsTiling;
        SerializedProperty m_CausticsSpeed;
        SerializedProperty m_CausticsPlaneBlendDistance;

        // Water masking
        SerializedProperty m_WaterMask;
        SerializedProperty m_WaterMaskExtent;
        SerializedProperty m_WaterMaskOffset;

        // Foam
        SerializedProperty m_SimulationFoamAmount;
        SerializedProperty m_SimulationFoamDrag;
        SerializedProperty m_SimulationFoamSmoothness;
        SerializedProperty m_FoamMask;
        SerializedProperty m_FoamMaskExtent;
        SerializedProperty m_FoamMaskOffset;

        // Wind
        SerializedProperty m_WindOrientation;
        SerializedProperty m_WindSpeed;
        SerializedProperty m_WindAffectCurrent;
        SerializedProperty m_WindFoamCurve;

        // Rendering
        SerializedProperty m_DecalLayerMask;
        SerializedProperty m_LightLayerMask;

        // Underwater
        SerializedProperty m_UnderWater;
        SerializedProperty m_VolumeBounds;
        SerializedProperty m_VolumeDepth;
        SerializedProperty m_VolumePriority;
        SerializedProperty m_TransitionSize;
        SerializedProperty m_AbsorbtionDistanceMultiplier;

        void OnEnable()
        {
            var o = new PropertyFetcher<WaterSurface>(serializedObject);

            // Geometry parameters
            m_Infinite = o.Find(x => x.infinite);
            m_HighFrequencyBands = o.Find(x => x.highFrequencyBands);
            m_GeometryType = o.Find(x => x.geometryType);
            m_Geometry = o.Find(x => x.geometry);

            // CPU Simulation
            m_CPUSimulation = o.Find(x => x.cpuSimulation);
            m_CPUFullResolution = o.Find(x => x.cpuFullResolution);
            m_CPUEvaluateAllBands = o.Find(x => x.cpuEvaluateAllBands);

            // Band definition parameters
            m_WaterMaxPatchSize = o.Find(x => x.waterMaxPatchSize);
            m_Amplitude = o.Find(x => x.amplitude);
            m_Choppiness = o.Find(x => x.choppiness);
            m_TimeMultiplier = o.Find(x => x.timeMultiplier);

            // Rendering parameters
            m_CustomMaterial = o.Find(x => x.customMaterial);
            m_WaterSmoothness = o.Find(x => x.waterSmoothness);

            // Refraction parameters
            m_AbsorptionDistance = o.Find(x => x.absorptionDistance);
            m_MaxRefractionDistance = o.Find(x => x.maxRefractionDistance);
            m_RefractionColor = o.Find(x => x.refractionColor);

            // Scattering parameters
            m_ScatteringColor = o.Find(x => x.scatteringColor);
            m_HeightScattering = o.Find(x => x.heightScattering);
            m_DisplacementScattering = o.Find(x => x.displacementScattering);
            m_DirectLightTipScattering = o.Find(x => x.directLightTipScattering);
            m_DirectLightBodyScattering = o.Find(x => x.directLightBodyScattering);

            // Caustic parameters
            m_Caustics = o.Find(x => x.caustics);
            m_CausticsIntensity = o.Find(x => x.causticsIntensity);
            m_CausticsAlgorithm = o.Find(x => x.causticsAlgorithm);

            // Simulation caustics
            m_CausticsResolution = o.Find(x => x.causticsResolution);
            m_CausticsBand = o.Find(x => x.causticsBand);
            m_CausticsVirtualPlaneDistance = o.Find(x => x.virtualPlaneDistance);

            // Procedural caustics
            m_CausticsTiling = o.Find(x => x.causticsTiling);
            m_CausticsSpeed = o.Find(x => x.causticsSpeed);
            m_CausticsPlaneBlendDistance = o.Find(x => x.causticsPlaneBlendDistance);

            // Foam
            m_SimulationFoamAmount = o.Find(x => x.simulationFoamAmount);
            m_SimulationFoamDrag = o.Find(x => x.simulationFoamDrag);
            m_SimulationFoamSmoothness = o.Find(x => x.simulationFoamSmoothness);
            m_FoamMask = o.Find(x => x.foamMask);
            m_FoamMaskExtent = o.Find(x => x.foamMaskExtent);
            m_FoamMaskOffset = o.Find(x => x.foamMaskOffset);

            // Water masking
            m_WaterMask = o.Find(x => x.waterMask);
            m_WaterMaskExtent = o.Find(x => x.waterMaskExtent);
            m_WaterMaskOffset = o.Find(x => x.waterMaskOffset);

            // Wind parameters
            m_WindOrientation = o.Find(x => x.windOrientation);
            m_WindSpeed = o.Find(x => x.windSpeed);
            m_WindFoamCurve = o.Find(x => x.windFoamCurve);
            m_WindAffectCurrent = o.Find(x => x.windAffectCurrent);

            // Rendering
            m_DecalLayerMask = o.Find(x => x.decalLayerMask);
            m_LightLayerMask = o.Find(x => x.lightLayerMask);

            // Underwater
            m_UnderWater = o.Find(x => x.underWater);
            m_VolumeBounds = o.Find(x => x.volumeBounds);
            m_VolumeDepth = o.Find(x => x.volumeDepth);
            m_VolumePriority = o.Find(x => x.volumePrority);
            m_TransitionSize = o.Find(x => x.transitionSize);
            m_AbsorbtionDistanceMultiplier = o.Find(x => x.absorbtionDistanceMultiplier);
        }

        // CPU Simulation
        static public readonly GUIContent k_CPUSimulation = EditorGUIUtility.TrTextContent("Enable", "When enabled, HDRP will evaluate the water simulation on the CPU for C# script height requests. Enabling this will significantly increase the CPU cost of the feature.");
        static public readonly GUIContent k_CPUFullResolution = EditorGUIUtility.TrTextContent("Full Resolution", "Specifies if the CPU simulation should be evaluated at full or half resolution. When in full resolution, the visual fidelity will be higher but the cost of the simulation will increase.");
        static public readonly GUIContent k_CPUEvaluateAllBands = EditorGUIUtility.TrTextContent("Evaluate all bands", "Specifies if the CPU simulation should evaluate all four band (when active) or should limit itself to the first two bands. A higher band count will allow for a higher visual fidelity but the cost of the simulation will increase.");

        static public readonly GUIContent k_Amplitude = EditorGUIUtility.TrTextContent("Amplitude", "Sets the normalized (between 0.0 and 1.0) amplitude of each simulation band (from lower to higher frequencies).");
        static public readonly GUIContent k_Choppiness = EditorGUIUtility.TrTextContent("Choppiness", "Sets the choppiness factor the waves. Higher values combined with high wind speed may introduce visual artifacts.");
        static public readonly GUIContent k_TimeMultiplier = EditorGUIUtility.TrTextContent("Time Multiplier", "Sets the speed of the water simulation. This allows to slow down the wave's speed or to accelerate it.");
        static public readonly GUIContent k_WaterSmoothness = EditorGUIUtility.TrTextContent("Water Smoothness", "Controls the smoothness used to render the water surface.");
        static public readonly GUIContent k_MaxRefractionDistance = EditorGUIUtility.TrTextContent("Maximum Refraction Distance", "Controls the maximum distance in meters used to clamp the under water refraction depth. Higher value increases the distortion amount.");
        static public readonly GUIContent k_AbsorptionDistance = EditorGUIUtility.TrTextContent("Absorption Distance", "Controls the approximative distance in meters that the camera can perceive through a water surface. This distance can vary widely depending on the intensity of the light the object receives.");

        static public readonly GUIContent k_HeightScattering = EditorGUIUtility.TrTextContent("Height Scattering", "Controls the intensity of the height based scattering. The higher the vertical displacement, the more the water receives scattering. This can be adjusted for artistic purposes.");
        static public readonly GUIContent k_DisplacementScattering = EditorGUIUtility.TrTextContent("Displacement Scattering", "Controls the intensity of the displacement based scattering. The bigger horizontal displacement, the more the water receives scattering. This can be adjusted for artistic purposes.");
        static public readonly GUIContent k_DirectLightTipScattering = EditorGUIUtility.TrTextContent("Direct Light Tip Scattering", "Controls the intensity of the direct light scattering on the tip of the waves. The effect is more perceivable at grazing angles.");
        static public readonly GUIContent k_DirectLightBodyScattering = EditorGUIUtility.TrTextContent("Direct Light Body Scattering", "Controls the intensity of the direct light scattering on the body of the waves. The effect is more perceivable at grazing angles.");

        static public readonly GUIContent k_CausticsBand = EditorGUIUtility.TrTextContent("Caustics Band", "Controls which band is used for the caustics evaluation.");

        static public readonly GUIContent k_SimulationFoamSmoothness = EditorGUIUtility.TrTextContent("Simulation Foam Smoothness", "Controls the simulation foam smoothness.");
        static public readonly GUIContent k_SimulationFoamDrag = EditorGUIUtility.TrTextContent("Simulation Foam Drag", "Controls the life span of the surface foam. A higher value will cause the foam to persist longer and leave a trail.");
        static public readonly GUIContent k_SimulationFoamAmount = EditorGUIUtility.TrTextContent("Simulation Foam Amount", "Controls the simulation foam amount. Higher values generate larger foam patches. Foam presence is highly dependent on the wind speed and chopiness values.");

        static public readonly GUIContent k_WindSpeed = EditorGUIUtility.TrTextContent("Wind Speed", "Controls the wind speed in kilometers per hour.");
        static public readonly GUIContent k_WindAffectsCurrent = EditorGUIUtility.TrTextContent("Wind Affects current", "Controls the proportion in which the wind affects the current of the water.");

        void SanitizeVector4(SerializedProperty property, float minValue, float maxValue)
        {
            Vector4 vec4 = property.vector4Value;
            vec4.x = Mathf.Clamp(vec4.x, minValue, maxValue);
            vec4.y = Mathf.Clamp(vec4.y, minValue, maxValue);
            vec4.z = Mathf.Clamp(vec4.z, minValue, maxValue);
            vec4.w = Mathf.Clamp(vec4.w, minValue, maxValue);
            property.vector4Value = vec4;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
            if (!currentAsset?.currentPlatformRenderPipelineSettings.supportWater ?? false)
            {
                EditorGUILayout.Space();
                HDEditorUtils.QualitySettingsHelpBox("The current HDRP Asset does not support Water Surfaces.", MessageType.Error,
                    HDRenderPipelineUI.Expandable.Water, "m_RenderPipelineSettings.supportWater");
                return;
            }

            bool highFrequencyBands = false;
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_HighFrequencyBands);
                highFrequencyBands = m_HighFrequencyBands.boolValue;

                EditorGUILayout.PropertyField(m_Infinite);
                using (new IndentLevelScope())
                {
                    if (!m_Infinite.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_GeometryType);
                        if ((WaterSurface.WaterGeometryType)m_GeometryType.enumValueIndex == WaterSurface.WaterGeometryType.Custom)
                            EditorGUILayout.PropertyField(m_Geometry);
                    }
                }
            }

            EditorGUILayout.LabelField("CPU Simulation", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                // Display the CPU simulation check box, but only make it available if the asset allows it
                bool cpuSimSupported = currentAsset.currentPlatformRenderPipelineSettings.waterCPUSimulation;
                using (new EditorGUI.DisabledScope(!cpuSimSupported))
                {
                    EditorGUILayout.PropertyField(m_CPUSimulation, k_CPUSimulation);
                    using (new IndentLevelScope())
                    {
                        if (m_CPUSimulation.boolValue)
                        {
                            if (currentAsset.currentPlatformRenderPipelineSettings.waterSimulationResolution == WaterSimulationResolution.Low64)
                            {
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    // When in 64, we always show that we are running the CPU simulation at full res.
                                    bool fakeToggle = true;
                                    EditorGUILayout.Toggle(k_CPUFullResolution, fakeToggle);
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(m_CPUFullResolution, k_CPUFullResolution);
                            }

                            if (!highFrequencyBands)
                            {
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    // When we only have 2 bands, we should evaluate all bands
                                    bool fakeToggle = true;
                                    EditorGUILayout.Toggle(k_CPUEvaluateAllBands, fakeToggle);
                                }
                            }
                            else
                            {
                                EditorGUILayout.PropertyField(m_CPUEvaluateAllBands, k_CPUEvaluateAllBands);
                            }
                        }
                    }
                }

                // Redirect to the asset if disabled
                if (!cpuSimSupported)
                {
                    HDEditorUtils.QualitySettingsHelpBox("Enable 'CPU Simulation' in your HDRP Asset if you want to replicate the water simulation on CPU. There is a performance cost of enabling this option.",
                        MessageType.Info, HDRenderPipelineUI.Expandable.Water, "m_RenderPipelineSettings.waterCPUSimulation");
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.LabelField("Simulation", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_WaterMaxPatchSize);
                m_WaterMaxPatchSize.floatValue = Mathf.Clamp(m_WaterMaxPatchSize.floatValue, 25.0f, 10000.0f);

                using (new IndentLevelScope())
                {
                    if (m_HighFrequencyBands.boolValue)
                    {
                        EditorGUI.BeginChangeCheck();
                        m_Amplitude.vector4Value = EditorGUILayout.Vector4Field(k_Amplitude, m_Amplitude.vector4Value);
                        if (EditorGUI.EndChangeCheck())
                            SanitizeVector4(m_Amplitude, 0.0f, 1.0f);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector2 amplitude2D = new Vector2(m_Amplitude.vector4Value.x, m_Amplitude.vector4Value.y);
                        amplitude2D = EditorGUILayout.Vector2Field(k_Amplitude, amplitude2D);
                        m_Amplitude.vector4Value = new Vector4(amplitude2D.x, amplitude2D.y, m_Amplitude.vector4Value.z, m_Amplitude.vector4Value.w);
                        if (EditorGUI.EndChangeCheck())
                            SanitizeVector4(m_Amplitude, 0.0f, 1.0f);
                    }
                }

                m_Choppiness.floatValue = EditorGUILayout.Slider(k_Choppiness, m_Choppiness.floatValue, 0.0f, 1.0f);
                m_TimeMultiplier.floatValue = EditorGUILayout.Slider(k_TimeMultiplier, m_TimeMultiplier.floatValue, 0.0f, 10.0f);
            }

            EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_CustomMaterial);
                // Water Smoothness from 0.0f to 0.99f
                m_WaterSmoothness.floatValue = EditorGUILayout.Slider(k_WaterSmoothness, m_WaterSmoothness.floatValue, 0.0f, 0.99f);
            }

            EditorGUILayout.LabelField("Refraction", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_RefractionColor);
                m_MaxRefractionDistance.floatValue = EditorGUILayout.Slider(k_MaxRefractionDistance, m_MaxRefractionDistance.floatValue, 0.0f, 3.5f);
                m_AbsorptionDistance.floatValue = EditorGUILayout.Slider(k_AbsorptionDistance, m_AbsorptionDistance.floatValue, 0.0f, 100.0f);
            }

            EditorGUILayout.LabelField("Scattering", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_ScatteringColor);
                m_HeightScattering.floatValue = EditorGUILayout.Slider(k_HeightScattering, m_HeightScattering.floatValue, 0.0f, 1.0f);
                m_DisplacementScattering.floatValue = EditorGUILayout.Slider(k_DisplacementScattering, m_DisplacementScattering.floatValue, 0.0f, 1.0f);
                m_DirectLightTipScattering.floatValue = EditorGUILayout.Slider(k_DirectLightTipScattering, m_DirectLightTipScattering.floatValue, 0.0f, 1.0f);
                m_DirectLightBodyScattering.floatValue = EditorGUILayout.Slider(k_DirectLightBodyScattering, m_DirectLightBodyScattering.floatValue, 0.0f, 1.0f);
            }

            EditorGUILayout.LabelField("Caustics", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_Caustics);
                if (m_Caustics.boolValue)
                {
                    EditorGUILayout.PropertyField(m_CausticsIntensity);
                    m_CausticsIntensity.floatValue = Mathf.Max(m_CausticsIntensity.floatValue, 0.0f);

                    EditorGUILayout.PropertyField(m_CausticsPlaneBlendDistance);
                    m_CausticsPlaneBlendDistance.floatValue = Mathf.Max(m_CausticsPlaneBlendDistance.floatValue, 0.0f);

                    EditorGUILayout.PropertyField(m_CausticsAlgorithm);

                    if ((WaterSurface.WaterCausticsType)m_CausticsAlgorithm.enumValueIndex == WaterSurface.WaterCausticsType.Simulation)
                    {
                        using (new IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(m_CausticsResolution);
                            m_CausticsBand.intValue = EditorGUILayout.IntSlider(k_CausticsBand, m_CausticsBand.intValue, 0, highFrequencyBands ? 3 : 1);

                            EditorGUILayout.PropertyField(m_CausticsVirtualPlaneDistance);
                            m_CausticsVirtualPlaneDistance.floatValue = Mathf.Max(m_CausticsVirtualPlaneDistance.floatValue, 0.001f);
                        }
                    }
                    else
                    {
                        using (new IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(m_CausticsTiling);
                            m_CausticsTiling.floatValue = Mathf.Max(m_CausticsTiling.floatValue, 0.001f);
                            EditorGUILayout.PropertyField(m_CausticsSpeed);
                            m_CausticsSpeed.floatValue = Mathf.Clamp(m_CausticsSpeed.floatValue, 0.0f, 100.0f);
                        }
                    }
                }
            }

            EditorGUILayout.LabelField("Masking", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_WaterMask);
                if (m_WaterMask.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(m_WaterMaskExtent);
                    EditorGUILayout.PropertyField(m_WaterMaskOffset);
                }
            }

            EditorGUILayout.LabelField("Foam", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                // Surface foam
                m_SimulationFoamAmount.floatValue = EditorGUILayout.Slider(k_SimulationFoamAmount, m_SimulationFoamAmount.floatValue, 0.0f, 1.0f);
                m_SimulationFoamDrag.floatValue = EditorGUILayout.Slider(k_SimulationFoamDrag, m_SimulationFoamDrag.floatValue, 0.0f, 1.0f);
                m_SimulationFoamSmoothness.floatValue = EditorGUILayout.Slider(k_SimulationFoamSmoothness, m_SimulationFoamSmoothness.floatValue, 0.0f, 1.0f);

                // Foam masking
                EditorGUILayout.PropertyField(m_FoamMask);
                if (m_FoamMask.objectReferenceValue != null)
                {
                    using (new IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_FoamMaskExtent);
                        EditorGUILayout.PropertyField(m_FoamMaskOffset);
                    }
                }
            }

            EditorGUILayout.LabelField("Wind", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_WindOrientation);
                m_WindSpeed.floatValue = EditorGUILayout.Slider(k_WindSpeed, m_WindSpeed.floatValue, 0.0f, 100.0f);
                m_WindAffectCurrent.floatValue = EditorGUILayout.Slider(k_WindAffectsCurrent, m_WindAffectCurrent.floatValue, 0.0f, 1.0f);
                EditorGUILayout.PropertyField(m_WindFoamCurve);
            }

            // Decal controls
            EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);
            if (HDRenderPipeline.currentPipeline != null && HDRenderPipeline.currentPipeline.currentPlatformRenderPipelineSettings.supportDecals)
            {
                bool decalLayerEnabled = false;
                using (new IndentLevelScope())
                {
                    decalLayerEnabled = HDRenderPipeline.currentPipeline.currentPlatformRenderPipelineSettings.supportDecalLayers;
                    using (new EditorGUI.DisabledScope(!decalLayerEnabled))
                    {
                        EditorGUILayout.PropertyField(m_DecalLayerMask);
                    }
                }

                if (!decalLayerEnabled)
                {
                    HDEditorUtils.QualitySettingsHelpBox("Enable 'Decal Layers' in your HDRP Asset if you want to control which decals affect water surfaces. There is a performance cost of enabling this option.",
                        MessageType.Info, HDRenderPipelineUI.Expandable.Decal, "m_RenderPipelineSettings.supportDecalLayers");
                    EditorGUILayout.Space();
                }
            }

            if (HDRenderPipeline.currentPipeline != null)
            {
                bool lightLayersEnabled = HDRenderPipeline.currentPipeline.currentPlatformRenderPipelineSettings.supportLightLayers;
                using (new IndentLevelScope())
                {
                    using (new EditorGUI.DisabledScope(!lightLayersEnabled))
                    {
                        EditorGUILayout.PropertyField(m_LightLayerMask);
                    }
                }

                if (!lightLayersEnabled)
                {
                    HDEditorUtils.QualitySettingsHelpBox("Enable 'Light Layers' in your HDRP Asset if you want defined which lights affect water surfaces. There is a performance cost of enabling this option.",
                        MessageType.Info, HDRenderPipelineUI.Expandable.Lighting, "m_RenderPipelineSettings.supportLightLayers");
                    EditorGUILayout.Space();
                }
            }

            // Under Water Rendering
            EditorGUILayout.LabelField("Underwater", EditorStyles.boldLabel);
            using (new IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_UnderWater);
                using (new IndentLevelScope())
                {
                    if (m_UnderWater.boolValue)
                    {
                        // Bounds data
                        if (!m_Infinite.boolValue)
                        {
                            EditorGUILayout.PropertyField(m_VolumeBounds);
                            m_VolumeBounds.floatValue = Mathf.Max(m_VolumeBounds.floatValue, 0.0f);
                        }
                        else
                            EditorGUILayout.PropertyField(m_VolumeDepth);

                        // Priority
                        EditorGUILayout.PropertyField(m_VolumePriority);
                        m_VolumePriority.intValue = m_VolumePriority.intValue > 0 ? m_VolumePriority.intValue : 0;

                        // Transition size
                        EditorGUILayout.PropertyField(m_TransitionSize);
                        m_TransitionSize.floatValue = Mathf.Max(m_TransitionSize.floatValue, 0.0f);

                        // View distance
                        EditorGUILayout.PropertyField(m_AbsorbtionDistanceMultiplier);
                        m_AbsorbtionDistanceMultiplier.floatValue = Mathf.Max(m_AbsorbtionDistanceMultiplier.floatValue, 0.0f);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        // Anis 11/09/21: Currently, there is a bug that makes the icon disappear after the first selection
        // if we do not have this. Given that the geometry is procedural, we need this to be able to
        // select the water surfaces.
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawGizmosSelected(WaterSurface waterSurface, GizmoType gizmoType)
        {
        }
    }
}
