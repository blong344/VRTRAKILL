﻿using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.VRIK.Armature
{
    internal class Leggature
    {
        public Transform GameObjectT { get; set; }
        public Transform Thigh { get; set; }
        public Transform Calf { get; set; }
        public Transform Foot { get; set; }
        public Transform Heel { get; set; }
        public Transform Toe { get; set; }

        public static Leggature MRV1Preset(Transform T)
        {
            Leggature L = new Leggature();
            L.GameObjectT = T;
            L.Thigh = L.GameObjectT;
            L.Calf = L.Thigh.GetChild(0);
            L.Foot = L.Calf.GetChild(0);
            L.Heel = L.Foot.GetChild(0);
            L.Toe = L.Foot.GetChild(1);
            return L;
        }
    }
}