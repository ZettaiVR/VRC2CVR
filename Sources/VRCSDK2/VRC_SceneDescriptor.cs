// Decompiled with JetBrains decompiler
// Type: VRCSDK2.VRC_SceneDescriptor
// Assembly: VRCSDK2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=67033c44591afb45
// MVID: E8444E77-B495-41ED-8853-2167A02BAA4B
// Assembly location: E:\Unity\New1_2017\Assets\VRCSDK\Dependencies\VRChat\VRCSDK2.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRCSDK2
{
    public class VRC_SceneDescriptor : MonoBehaviour
    {
        public SpawnOrder spawnOrder = SpawnOrder.Random;
        public float RespawnHeightY = -100f;
        public RespawnHeightBehaviour ObjectBehaviourAtRespawnHeight = RespawnHeightBehaviour.Destroy;
        public float VoiceFalloffRangeNear = 4f;
        public float VoiceFalloffRangeFar = 350f;
        [HideInInspector]
        public bool autoSpatializeAudioSources = true;
        [HideInInspector]
        public Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
        [HideInInspector]
        public Vector3 SpawnPosition = new Vector3(0.0f, 0.0f, 0.0f);
        public List<GameObject> DynamicPrefabs = new List<GameObject>();
        public List<Material> DynamicMaterials = new List<Material>();
        [Range(33f, 150f)]
        public int UpdateTimeInMS = 33;
        public Vector3 portraitCameraPositionOffset = new Vector3(0.0f, 0.0f, 0.0f);
        public Quaternion portraitCameraRotationOffset = Quaternion.AngleAxis(180f, Vector3.up);
        public Transform[] spawns;
        public SpawnOrientation spawnOrientation;
        public GameObject ReferenceCamera;
        public bool ForbidUserPortals;
        public bool UseCustomVoiceFalloffRange;
        [HideInInspector]
        public bool[] layerCollisionArr;
        [HideInInspector]
        public int capacity;
        [HideInInspector]
        public bool contentSex;
        [HideInInspector]
        public bool contentViolence;
        [HideInInspector]
        public bool contentGore;
        [HideInInspector]
        public bool contentOther;
        [HideInInspector]
        public bool releasePublic;
        public string unityVersion;
        [Obsolete("Property is not used.")]
        [HideInInspector]
        public string Name;
        [HideInInspector]
        [Obsolete("Property is not used.")]
        public bool NSFW;
        [HideInInspector]
        public Transform SpawnLocation;
        [HideInInspector]
        public float DrawDistance;
        [HideInInspector]
        public bool useAssignedLayers;
        private static Dictionary<string, GameObject> sDynamicPrefabs;
        private static Dictionary<string, Material> sDynamicMaterials;
        [HideInInspector]
        public Texture2D[] LightMapsNear;
        [HideInInspector]
        public Texture2D[] LightMapsFar;
        [HideInInspector]
        public LightmapsMode LightMode;
        [HideInInspector]
        public Color RenderAmbientEquatorColor;
        [HideInInspector]
        public Color RenderAmbientGroundColor;
        [HideInInspector]
        public float RenderAmbientIntensity;
        [HideInInspector]
        public Color RenderAmbientLight;
        [HideInInspector]
        public AmbientMode RenderAmbientMode;
        [HideInInspector]
        public SphericalHarmonicsL2 RenderAmbientProbe;
        [HideInInspector]
        public Color RenderAmbientSkyColor;
        [HideInInspector]
        public bool RenderFog;
        [HideInInspector]
        public Color RenderFogColor;
        [HideInInspector]
        public FogMode RenderFogMode;
        [HideInInspector]
        public float RenderFogDensity;
        [HideInInspector]
        public float RenderFogLinearStart;
        [HideInInspector]
        public float RenderFogLinearEnd;
        [HideInInspector]
        public float RenderHaloStrength;
        [HideInInspector]
        public float RenderFlareFadeSpeed;
        [HideInInspector]
        public float RenderFlareStrength;
        [HideInInspector]
        public Cubemap RenderCustomReflection;
        [HideInInspector]
        public DefaultReflectionMode RenderDefaultReflectionMode;
        [HideInInspector]
        public int RenderDefaultReflectionResolution;
        [HideInInspector]
        public int RenderReflectionBounces;
        [HideInInspector]
        public float RenderReflectionIntensity;
        [HideInInspector]
        public Material RenderSkybox;
        [HideInInspector]
        public object apiWorld;


        public enum SpawnOrder
        {
            First,
            Sequential,
            Random,
            Demo,
        }

        public enum SpawnOrientation
        {
            Default,
            AlignPlayerWithSpawnPoint,
            AlignRoomWithSpawnPoint,
        }

        public enum RespawnHeightBehaviour
        {
            Respawn,
            Destroy,
        }

    }
}
