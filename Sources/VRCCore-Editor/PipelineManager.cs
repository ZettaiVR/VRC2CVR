using System;
using UnityEngine;

namespace VRC.Core
{

    public class APIUser 
    {
    }
    public class PipelineManager : MonoBehaviour
    {
        [HideInInspector]
        public bool launchedFromSDKPipeline;
        [HideInInspector]
        public bool completedSDKPipeline;
        [HideInInspector]
        public string blueprintId;
        [HideInInspector]
        public APIUser user;
        public ContentType contentType;
        [HideInInspector]
        [Obsolete("Property is not used.")]
        public string assetBundleUnityVersion;
        private Ownership owned;

        public enum ContentType
        {
            avatar,
            world,
        }

        private enum Ownership
        {
            Uninitialized,
            Querried,
            Owned,
            Unowned,
        }
    }
}