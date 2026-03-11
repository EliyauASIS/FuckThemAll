using System.Collections;
using System.Collections.Generic;
using TrackerPro.Unity.PoseTracking;
using UnityEngine;
namespace TrackerPro
{
    public class BoneConstraint
    {
        public Transform parent;
        public Transform child;
        public BoneType boneType;
        public Body body;

    }
    public enum BoneType
    {
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_HAND,
        RIGHT_HAND,
        LEFT_LEG,
        LEFT_ANKLE,
        RIGHT_ANKLE,
        RIGHT_LEG,
        MIDDLE
    }
}
