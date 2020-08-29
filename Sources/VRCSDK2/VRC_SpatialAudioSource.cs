using UnityEngine;

namespace VRCSDK2
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class VRC_SpatialAudioSource : MonoBehaviour
    {
        public float Gain = 10f;
        public float Far = 40f;
        public bool EnableSpatialization = true;
        public float Near;
        public float VolumetricRadius;
        public bool UseAudioSourceVolumeCurve;
    }
}