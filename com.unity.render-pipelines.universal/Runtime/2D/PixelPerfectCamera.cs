using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Experimental.Rendering.Universal;
using System;

namespace UnityEngine.Experimental.Rendering.Universal
{   
    /// <summary>
    /// The Pixel Perfect Camera component ensures your pixel art remains crisp and clear at different resolutions, and stable in motion.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Rendering/2D/Pixel Perfect Camera (Experimental Custom)")]
    [RequireComponent(typeof(Camera))]
    [MovedFrom("UnityEngine.Experimental.Rendering.LWRP")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html?subfolder=/manual/2d-pixelperfect.html%23properties")]
    public class PixelPerfectCamera : MonoBehaviour, IPixelPerfectCamera
    {
        //------------------------------------------------------------------------------
        // Custom
        public bool forceAspect { get { return m_ForceAspect; } set { m_ForceAspect = value; } }
        public double targetAspectWidth { get { return m_TargetAspectWidth; } set { m_TargetAspectWidth = value; } }
        public double targetAspectHeight { get { return m_TargetAspectHeight; } set { m_TargetAspectHeight = value; } }

        public Vector2Int pixelScreenSize { get { return m_PixelScreenSize; } set { m_PixelScreenSize = value; } }
        //------------------------------------------------------------------------------
        
        /// <summary>
        /// Match this value to to the Pixels Per Unit values of all Sprites within the Scene.
        /// </summary>
        public int assetsPPU { get { return m_AssetsPPU; } set { m_AssetsPPU = value > 0 ? value : 1; } }

        /// <summary>
        /// The original horizontal resolution your Assets are designed for.
        /// </summary>
        public int refResolutionX { get { return m_RefResolutionX; } set { m_RefResolutionX = value > 0 ? value : 1; } }

        /// <summary>
        /// Original vertical resolution your Assets are designed for.
        /// </summary>
        public int refResolutionY { get { return m_RefResolutionY; } set { m_RefResolutionY = value > 0 ? value : 1; } }

        /// <summary>
        /// Set to true to have the Scene rendered to a temporary texture set as close as possible to the Reference Resolution,
        /// while maintaining the full screen aspect ratio. This temporary texture is then upscaled to fit the full screen.
        /// </summary>
        public bool upscaleRT { get { return m_UpscaleRT; } set { m_UpscaleRT = value; } }

        /// <summary>
        /// Set to true to prevent subpixel movement and make Sprites appear to move in pixel-by-pixel increments.
        /// Only applicable when upscaleRT is false.
        /// </summary>
        public bool pixelSnapping { get { return m_PixelSnapping; } set { m_PixelSnapping = value; } }

        /// <summary>
        /// Set to true to crop the viewport with black bars to match refResolutionX in the horizontal direction.
        /// </summary>
        public bool cropFrameX { get { return m_CropFrameX; } set { m_CropFrameX = value; } }

        /// <summary>
        /// Set to true to crop the viewport with black bars to match refResolutionY in the vertical direction.
        /// </summary>
        public bool cropFrameY { get { return m_CropFrameY; } set { m_CropFrameY = value; } }

        /// <summary>
        /// Set to true to expand the viewport to fit the screen resolution while maintaining the viewport's aspect ratio.
        /// Only applicable when both cropFrameX and cropFrameY are true.
        /// </summary>
        public bool stretchFill { get { return m_StretchFill; } set { m_StretchFill = value; } }

        /// <summary>
        /// Ratio of the rendered Sprites compared to their original size (readonly).
        /// </summary>
        public int pixelRatio
        {
            get
            {
                if (m_CinemachineCompatibilityMode)
                {
                    if (m_UpscaleRT)
                        return m_Internal.zoom * m_Internal.cinemachineVCamZoom;
                    else
                        return m_Internal.cinemachineVCamZoom;
                }
                else
                {
                    return m_Internal.zoom;
                }
            }
        }

        public int cinemachineVCamZoom
        {
            get => m_Internal.cinemachineVCamZoom;
        }

        public double TargetAspect
        {
            get => targetAspectWidth / targetAspectHeight;
        }

        /// <summary>
        /// Round a arbitrary position to an integer pixel position. Works in world space.
        /// </summary>
        /// <param name="position"> The position you want to round.</param>
        /// <returns>
        /// The rounded pixel position.
        /// Depending on the values of upscaleRT and pixelSnapping, it could be a screen pixel position or an art pixel position.
        /// </returns>
        public Vector3 RoundToPixel(Vector3 position)
        {
            float unitsPerPixel = m_Internal.unitsPerPixel;
            if (unitsPerPixel == 0.0f)
                return position;

            Vector3 result;
            result.x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
            result.y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
            result.z = Mathf.Round(position.z / unitsPerPixel) * unitsPerPixel;

            return result;
        }

        /// <summary>
        /// Find a pixel-perfect orthographic size as close to targetOrthoSize as possible. Used by Cinemachine to solve compatibility issues with Pixel Perfect Camera.
        /// https://github.com/Unity-Technologies/com.unity.cinemachine/blob/master/Runtime/Behaviours/CinemachinePixelPerfect.cs
        /// </summary>
        /// <param name="targetOrthoSize">Orthographic size from the live Cinemachine Virtual Camera.</param>
        /// <returns>The corrected orthographic size.</returns>
        public float CorrectCinemachineOrthoSize(float targetOrthoSize)
        {
            m_CinemachineCompatibilityMode = true;

            if (m_Internal == null)
                return targetOrthoSize;
            else
                return m_Internal.CorrectCinemachineOrthoSize(targetOrthoSize);
        }

        [SerializeField] int m_AssetsPPU = 100;
        [SerializeField] int m_RefResolutionX = 320;
        [SerializeField] int m_RefResolutionY = 180;
        [SerializeField] bool m_UpscaleRT;
        [SerializeField] bool m_PixelSnapping;
        [SerializeField] bool m_CropFrameX;
        [SerializeField] bool m_CropFrameY;
        [SerializeField] bool m_StretchFill;

        //------------------------------------------------------------------------------
        // Custom
        [SerializeField] bool m_ForceAspect;
        [SerializeField] double m_TargetAspectWidth;
        [SerializeField] double m_TargetAspectHeight;
        [SerializeField] Vector2Int m_PixelScreenSize;
        //------------------------------------------------------------------------------

        Camera m_Camera;
        PixelPerfectCameraInternal m_Internal;
        bool m_CinemachineCompatibilityMode;

        internal bool isRunning
        {
            get
            {
#if UNITY_EDITOR
                return (Application.isPlaying || runInEditMode) && enabled;
#else
                return enabled;
#endif
            }
        }

        internal FilterMode finalBlitFilterMode
        {
            get
            {
                if (!isRunning)
                    return FilterMode.Bilinear;
                else
                    return m_Internal.useStretchFill ? FilterMode.Bilinear : FilterMode.Point;
            }
        }

        internal Vector2Int offscreenRTSize
        {
            get
            {
                if (!isRunning)
                    return Vector2Int.zero;
                else
                    return new Vector2Int(m_Internal.offscreenRTWidth, m_Internal.offscreenRTHeight);
            }
        }

        Vector2Int cameraRTSize
        {
            get
            {
                var targetTexture = m_Camera.targetTexture;
                return targetTexture == null ? new Vector2Int(Screen.width, Screen.height) : new Vector2Int(targetTexture.width, targetTexture.height);
            }
        }

        // Snap camera position to pixels using Camera.worldToCameraMatrix.
        void PixelSnap()
        {
            Vector3 cameraPosition = m_Camera.transform.position;
            Vector3 roundedCameraPosition = RoundToPixel(cameraPosition);
            Vector3 offset = roundedCameraPosition - cameraPosition;
            offset.z = -offset.z;
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(-offset, Quaternion.identity, new Vector3(1.0f, 1.0f, -1.0f));

            m_Camera.worldToCameraMatrix = offsetMatrix * m_Camera.transform.worldToLocalMatrix;
        }

        void Awake()
        {
            m_Camera = GetComponent<Camera>();
            m_Internal = new PixelPerfectCameraInternal(this);

            m_Internal.originalOrthoSize = m_Camera.orthographicSize;

            // Case 1249076: Initialize internals immediately after the scene is loaded,
            // as the Cinemachine extension may need them before OnBeginContextRendering is called.
            // If forcing Aspect Ratio, use the adjusted RT Size.
            Vector2Int rtSize = new Vector2Int();
            Rect rect = new Rect();
            CalculateCameraProperties(ref rect, ref rtSize);
            
            pixelScreenSize = new Vector2Int(m_Camera.pixelWidth, m_Camera.pixelHeight);
        }

        void Update()
        {
            pixelScreenSize = new Vector2Int(m_Camera.pixelWidth, m_Camera.pixelHeight);
        }

        void OnBeginContextRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            // If forcing Aspect Ratio, use the adjusted RT Size.
            Vector2Int rtSize = new Vector2Int();
            Rect rect = new Rect();
            CalculateCameraProperties(ref rect, ref rtSize);
            
            PixelSnap();

            if (m_Internal.useOffscreenRT)
                m_Camera.pixelRect = m_Internal.CalculateFinalBlitPixelRect(rtSize.x, rtSize.y);
            else if (forceAspect)
                m_Camera.pixelRect = rect;
            else
                m_Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

            // In Cinemachine compatibility mode the control over orthographic size should
            // be given to the virtual cameras, whose orthographic sizes will be corrected to
            // be pixel-perfect. This way when there's blending between virtual cameras, we
            // can have temporary not-pixel-perfect but smooth transitions.
            if (!m_CinemachineCompatibilityMode)
            {
                m_Camera.orthographicSize = m_Internal.orthoSize;
            }
        }

        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            // Custom: Causes jittering noticeable on Player sprite when in motion.
            // if (camera == m_Camera)
            //     UnityEngine.U2D.PixelPerfectRendering.pixelSnapSpacing = m_Internal.unitsPerPixel;
        }

        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            // Custom: Causes jittering noticeable on Player sprite when in motion.
            // if (camera == m_Camera)
            //     UnityEngine.U2D.PixelPerfectRendering.pixelSnapSpacing = 0.0f;
        }

        private void CalculateCameraProperties(ref Rect rect, ref Vector2Int rtSize)
        {
            if (forceAspect)
            {
                rect = GetAspectEnforcedRect(rect);
                rtSize.x = (int)rect.width;
                rtSize.y = (int)rect.height;
            }
            else
                rtSize = cameraRTSize;
            
            m_Internal.CalculateCameraProperties(rtSize.x, rtSize.y);
        }

        /// <summary>
        /// Get a rect with the implied letterboxing / pillarboxing.
        /// </summary>
        private Rect GetAspectEnforcedRect(Rect rect)
        {
            double windowAspect = (double)Screen.width / (double)Screen.height;

            // Current viewport height should be scaled by this amount.
            double scaleHeight = windowAspect / TargetAspect;

            int pixelHeight;
            int pixelWidth;

            // Letterbox
            if (scaleHeight < 1.0d)
            {
                double rawPixelHeight = scaleHeight * (double)Screen.height;
                pixelHeight = (int)Math.Round(rawPixelHeight, MidpointRounding.AwayFromZero);
                pixelWidth = Screen.width;

                rect.x = 0f;
                rect.y = (float)Math.Round(((int)Screen.height - pixelHeight) / 2f, MidpointRounding.AwayFromZero);
                rect.width = pixelWidth;
                rect.height = pixelHeight;
            }
            // Pillarbox
            else if (scaleHeight > 1.0d)
            {
                double scaleWidth = 1.0d / scaleHeight;
                pixelHeight = Screen.height;
                double rawPixelWidth = scaleWidth * (double)Screen.width;
                pixelWidth = (int)Math.Round(rawPixelWidth, MidpointRounding.AwayFromZero);

                rect.x = (float)Math.Round(((int)Screen.width - pixelWidth) / 2f, MidpointRounding.AwayFromZero);
                rect.y = 0f;
                rect.width = pixelWidth;
                rect.height = pixelHeight;
            }
            // No Cropping Necessary
            else
            {
                rect.x = 0f;
                rect.y = 0f;
                rect.width = Screen.width;
                rect.height = Screen.height;
            }

            return rect;
        }

        void OnEnable()
        {
            // If forcing Aspect Ratio, use the adjusted RT Size.
            Vector2Int rtSize = new Vector2Int();
            Rect rect = new Rect();
            CalculateCameraProperties(ref rect, ref rtSize);
            
            m_CinemachineCompatibilityMode = false;

            RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
#endif
        }

        internal void OnDisable()
        {
            RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;

            m_Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            m_Camera.orthographicSize = m_Internal.originalOrthoSize;
            m_Camera.ResetWorldToCameraMatrix();

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeChanged;
#endif
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // Show on-screen warning about invalid render resolutions.
        void OnGUI()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !runInEditMode)
                return;
#endif

            Color oldColor = GUI.color;
            GUI.color = Color.red;

            Vector2Int renderResolution = Vector2Int.zero;
            renderResolution.x = m_Internal.useOffscreenRT ? m_Internal.offscreenRTWidth : m_Camera.pixelWidth;
            renderResolution.y = m_Internal.useOffscreenRT ? m_Internal.offscreenRTHeight : m_Camera.pixelHeight;

            if (renderResolution.x % 2 != 0 || renderResolution.y % 2 != 0)
            {
                string warning = string.Format("Rendering at an odd-numbered resolution ({0} * {1}). Pixel Perfect Camera may not work properly in this situation.", renderResolution.x, renderResolution.y);
                GUILayout.Box(warning);
            }

            var targetTexture = m_Camera.targetTexture;
            Vector2Int rtSize = targetTexture == null ? new Vector2Int(Screen.width, Screen.height) : new Vector2Int(targetTexture.width, targetTexture.height);

            if (rtSize.x < refResolutionX || rtSize.y < refResolutionY)
            {
                GUILayout.Box("Target resolution is smaller than the reference resolution. Image may appear stretched or cropped.");
            }

            GUI.color = oldColor;
        }

#endif

#if UNITY_EDITOR
        void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
        {
            // Stop running in edit mode when entering play mode.
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
            {
                runInEditMode = false;
                OnDisable();
            }
        }

#endif
    }
}
