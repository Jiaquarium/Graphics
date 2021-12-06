namespace UnityEditor.Experimental.Rendering
{
    internal class SerializedProbeTouchupVolume
    {
        internal SerializedProperty size;
        internal SerializedProperty intensityScale;
        internal SerializedProperty invalidateProbes;
        internal SerializedObject serializedObject;

        internal SerializedProbeTouchupVolume(SerializedObject obj)
        {
            serializedObject = obj;

            size = serializedObject.FindProperty("size");
            intensityScale = serializedObject.FindProperty("intensityScale");
            invalidateProbes = serializedObject.FindProperty("invalidateProbes");
        }

        internal void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
