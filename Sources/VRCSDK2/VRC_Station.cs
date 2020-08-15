using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRCSDK2
{
    public class VRC_PlayerApi : MonoBehaviour
    { 
    }

    public class VRC_ObjectApi : MonoBehaviour
    {
        public void ClaimControl(VRC_PlayerApi player)
        {
        }
    }
    public class VRC_Station : MonoBehaviour
    {
        public VRC_Station.Mobility PlayerMobility = VRC_Station.Mobility.Immobilize;
        public bool canUseStationFromStation = true;
        public bool seated = true;
        [Obsolete("Please set the PlayerMobility value instead")]
        private bool? shouldImmobolizePlayer;
        public RuntimeAnimatorController animatorController;
        public bool disableStationExit;
        public Transform stationEnterPlayerLocation;
        public Transform stationExitPlayerLocation;
        public VRC_ObjectApi controlsObject;
        public enum Mobility
        {
            Mobile,
            Immobilize,
            ImmobilizeForVehicle,
        }

    }
}