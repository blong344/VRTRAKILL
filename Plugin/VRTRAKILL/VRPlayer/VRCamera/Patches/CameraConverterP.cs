﻿using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

using Valve.VR;
namespace Plugin.VRTRAKILL.VRPlayer.VRCamera.Patches
{
    [HarmonyPatch] internal class CameraConverterP
    {
        // ty huskvr you pretty
        public static GameObject Container;
        public static Camera DesktopWorldCam, DesktopUICam, ThirdPersonCam;

        [HarmonyPrefix] [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))] static void Containerize()
        {
            Container = new GameObject("Main Camera Rig");
            Container.transform.parent = Vars.MainCamera.transform.parent;

            Container.transform.localPosition = new Vector3(0, -4.3f, 0);
            Container.transform.localRotation = Vars.MainCamera.transform.rotation;

            Container.AddComponent<VRCameraController>();

            Vars.MainCamera.transform.parent = Container.transform;
            Vars.UICamera.transform.parent = Container.transform;

            #region Desktop View
            DesktopWorldCam = new GameObject("Desktop World Camera").AddComponent<Camera>();
            DesktopWorldCam.transform.parent = Vars.MainCamera.transform;
            DesktopWorldCam.transform.localPosition = Vector3.zero;
            DesktopWorldCam.gameObject.AddComponent<DesktopCamera>();
            DesktopWorldCam.gameObject.SetActive(false);

            DesktopUICam = new GameObject("Desktop UI Camera").AddComponent<Camera>();
            DesktopUICam.transform.parent = Vars.MainCamera.transform;
            DesktopUICam.transform.localPosition = Vector3.zero;
            DesktopUICam.gameObject.AddComponent<DesktopUICamera>();
            DesktopUICam.gameObject.SetActive(false);
            #endregion

            #region Third Person Camera
            GameObject TPCContainer = new GameObject("Third Person Camera");
            TPCContainer.transform.localPosition = Vector3.zero;

            GameObject TPCOffset = new GameObject("TPC Limiter");
            TPCOffset.transform.parent = TPCContainer.transform;
            TPCOffset.transform.localPosition = Vector3.zero;

            ThirdPersonCam = new GameObject("TPC Camera").AddComponent<Camera>();
            ThirdPersonCam.transform.parent = TPCContainer.transform;
            ThirdPersonCam.transform.localPosition = Vector3.zero;
            ThirdPersonCam.stereoTargetEye = StereoTargetEyeMask.None;

            ThirdPersonCamera TPC = TPCContainer.AddComponent<ThirdPersonCamera>();
            TPC.TPCam = ThirdPersonCam;
            TPC.FollowTarget = Vars.MainCamera.transform;
            TPC.Offset = TPCOffset.transform;

            // Pushback 2.0
            Rigidbody TPCRB = ThirdPersonCam.gameObject.AddComponent<Rigidbody>();
            TPCRB.mass = 0; TPCRB.drag = 0; TPCRB.angularDrag = 0; TPCRB.useGravity = false;
            TPCRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            TPCRB.constraints = RigidbodyConstraints.FreezeRotation;
            SphereCollider TTPC = ThirdPersonCam.gameObject.AddComponent<SphereCollider>(); TTPC.radius = .01f;
            TPC.RB = TPCRB;

            TPCContainer.SetActive(false);
            #endregion
        }
        [HarmonyPostfix] [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))] static void ScaleObjects(NewMovement __instance)
        {
            // this should've been bigger, but i've changed my mind a thousand years ago and it works
            // this mod is officially my opus magnum spaghetti code and dumpster fire
            Container.transform.localScale = new Vector3(2, 2, 2);
            __instance.gameObject.AddComponent<VRKeybindsController>();
        }

        [HarmonyPrefix] [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))] static void ConvertCameras(CameraController __instance)
        {
            while (__instance.cam == null && __instance.hudCamera == null) {}

            __instance.cam.nearClipPlane = .01f;
            __instance.cam.stereoTargetEye = StereoTargetEyeMask.Both;
            // some binary magic (that i don't understand) to enable another layer
            __instance.cam.cullingMask |= 1 << (int)Layers.AlwaysOnTop;
            __instance.cam.depth++;
            
            __instance.hudCamera.stereoTargetEye = StereoTargetEyeMask.Both;
            __instance.hudCamera.depth++;

            XRSettings.gameViewRenderMode = GameViewRenderMode.RightEye;

            // for some particular reason destroying it is a bad idea.
            GameObject.Find("Virtual Camera").SetActive(false);
        }
        [HarmonyPostfix] [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))] static void AddSVRCam(CameraController __instance)
        { __instance.gameObject.AddComponent<SteamVR_Camera>(); }
    }
}
