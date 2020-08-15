using UnityEngine;

namespace VRCSDK2
{
    public class VRC_AvatarDescriptor : MonoBehaviour
    {
        public Vector3 ViewPosition = new Vector3(0.0f, 1.6f, 0.2f);
        public bool ScaleIPD = true;
        public string MouthOpenBlendShapeName = "Facial_Blends.Jaw_Down";
        public Vector3 portraitCameraPositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
        public Quaternion portraitCameraRotationOffset = Quaternion.AngleAxis(180f, Vector3.up);
        public string Name;
        public VRC_AvatarDescriptor.AnimationSet Animations;
        public AnimatorOverrideController CustomStandingAnims;
        public AnimatorOverrideController CustomSittingAnims;
        public VRC_AvatarDescriptor.LipSyncStyle lipSync;
        public Transform lipSyncJawBone;
        public SkinnedMeshRenderer VisemeSkinnedMesh;
        public string[] VisemeBlendShapes;
        [HideInInspector]
        public object apiAvatar;
        public string unityVersion;

        public enum AnimationSet
        {
            Male,
            Female,
            None,
        }

        public enum LipSyncStyle
        {
            Default,
            JawFlapBone,
            JawFlapBlendShape,
            VisemeBlendShape,
        }

        public enum Viseme
        {
            sil,
            PP,
            FF,
            TH,
            DD,
            kk,
            CH,
            SS,
            nn,
            RR,
            aa,
            E,
            ih,
            oh,
            ou,
            Count,
        }
    }
}