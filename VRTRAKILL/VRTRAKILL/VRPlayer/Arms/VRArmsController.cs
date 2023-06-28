﻿using Plugin.Helpers;
using UnityEngine;

namespace Plugin.VRTRAKILL.VRPlayer.Arms
{
    internal class VRArmsController : MonoBehaviour
    {
        public VRIK.Armature Arm; public bool UseVRIK;
        public Vector3 OffsetPosition = new Vector3(.145f, .09f, .04f); // hack to fix whiplash
        public Quaternion OffsetRotation = Quaternion.Euler(-90, 180, 0);

        public Vector3 LastPosition, Velocity;

        public bool IsSandboxer = false, IsRevolver = false;

        public void Start()
        {
            if (OffsetPosition == null || OffsetPosition == new Vector3(.145f, .09f, .04f))
            {
                switch (Arm.Type)
                {
                    case VRIK.ArmType.Feedbacker:
                        OffsetPosition = new Vector3(0, -.25f, -.5f); break;
                    case VRIK.ArmType.Knuckleblaster:
                        OffsetPosition = new Vector3(0, -.01f, -.035f); break;
                    case VRIK.ArmType.Whiplash:
                        OffsetPosition = new Vector3(.145f, .09f, .04f); break;

                    case VRIK.ArmType.Spear:
                    default: Destroy(GetComponent<VRArmsController>()); break;
                }
            }

            LastPosition = transform.position;
        }

        public void FixedUpdate()
        {
            if (LastPosition != transform.position)
            {
                Velocity = (transform.position - LastPosition).normalized;
                LastPosition = transform.position;
            }
        }
        public void LateUpdate()
        {
            try
            {
                // Update positions & rotations of the main gameobject + hand rotation (because animator stuff)
                if (IsSandboxer)
                {
                    Arm.GameObjectT.position = Vars.DominantHand.transform.position;
                    Arm.GameObjectT.rotation = Vars.DominantHand.transform.rotation;
                    Arm.Root.localPosition = OffsetPosition;
                    Arm.Hand.rotation = Vars.DominantHand.transform.rotation * OffsetRotation;
                }
                else if (IsRevolver)
                {
                    if (GetComponent<Revolver>().anim.GetBool("Spinning"))
                        Arm.Hand.rotation = Vars.DominantHand.transform.rotation * OffsetRotation;
                }
                else
                {
                    Arm.GameObjectT.position = Vars.NonDominantHand.transform.position;
                    Arm.GameObjectT.rotation = Vars.NonDominantHand.transform.rotation;
                    Arm.Root.localPosition = OffsetPosition;
                    Arm.Hand.rotation = Vars.NonDominantHand.transform.rotation * OffsetRotation;
                }

                // Whiplash specific stuff
                if (gameObject.HasComponent<HookArm>())
                    Arm.Wrist.GetChild(1).rotation = Vars.NonDominantHand.transform.rotation * OffsetRotation;

                // Thingamajig to disable other arms while grapplehooking
                if (HookArm.Instance.model.activeSelf && !gameObject.HasComponent<HookArm>()
                    && !IsSandboxer && !gameObject.HasComponent<Revolver>())
                    Arm.GameObjectT.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                else Arm.GameObjectT.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            } catch {} // do nothing because i know that it 100% works :) (it gives out too many errors which zipbomb your storage)
        }
        private void MoveHands()
        {

        }
    }
}
