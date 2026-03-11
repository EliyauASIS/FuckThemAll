using System.Collections;
using System.Collections.Generic;

using TrackerPro.Unity.CoordinateSystem;
using TrackerPro.Unity.PoseTracking;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
namespace TrackerPro.Unity
{
    public class Skeleton : TrackingEventHandler
    {
        public GameObject tPoseInstruction;
        [SerializeField]
        private Transform _bonePrefab;
        [SerializeField]
        public Animator avatar;
        [SerializeField]
        public bool visualizeZ = false;
        private bool isInTPose = false;
        [SerializeField]
        private float avatarScaleFactor = 1;
        [SerializeField]
        public RectTransform skeletonParent;
        [SerializeField]
        private int _numberOfBones = 33;
        [HideInInspector]
        public List<Transform> bones = new List<Transform>();
        public List<Vector3> bonesWorld = new List<Vector3>();
        private CorePoseTrackingSolution _corePoseTrackingSolution;
        [HideInInspector]
        public Transform virtualHip;
        [HideInInspector]
        public Transform virtualNeck;
        private List<BoneMapper> boneMappers = new List<BoneMapper>();
        private int[] tPoseBodyPoint = new int[] { (int)BodyLandmark.LEFT_WRIST, (int)BodyLandmark.LEFT_ELBOW, (int)BodyLandmark.LEFT_SHOULDER, (int)BodyLandmark.RIGHT_SHOULDER, (int)BodyLandmark.RIGHT_ELBOW, (int)BodyLandmark.RIGHT_WRIST };
        public UnityEvent OnTPose;
        [HideInInspector]
        public float hipLength = 0;
        [HideInInspector]
        public float shoulderLength = 0;
        [HideInInspector]
        public float verticalHeight = 0;
        [HideInInspector]
        public float verticalRotation = 0;
        public float initialWidth = 1;
        public float y = 0;
        public float z = 0;
        private void Awake()
        {
            for (int i = 0; i < _numberOfBones; i++)
            {
                Transform trans = Instantiate(_bonePrefab, skeletonParent);
                trans.name = ((Body)i).ToString();
                bones.Add(trans);
            }
            virtualNeck = new GameObject().transform;
            virtualNeck.parent = skeletonParent;
            virtualNeck.localEulerAngles = Vector3.zero;
            virtualNeck.name = "NECK";
            virtualHip = new GameObject().transform;
            virtualHip.parent = skeletonParent;
            virtualHip.name = "HIP";
            virtualHip.localEulerAngles = Vector3.zero;
            Debug.Log("Virtaul Bone Created");
        }

        private void Start()
        {
            if (avatar != null) MapBones();
        }
        public void MapBones()
        {
            virtualHip = avatar.GetBoneTransform(HumanBodyBones.Hips);
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.LeftUpperArm), bones[(int)Body.RIGHT_ELBOW], BoneType.RIGHT_HAND, Body.RIGHT_SHOULDER));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.RightUpperArm), bones[(int)Body.LEFT_ELBOW], BoneType.LEFT_HAND, Body.LEFT_SHOULDER));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.LeftLowerArm), bones[(int)Body.RIGHT_WRIST], BoneType.RIGHT_HAND, Body.RIGHT_WRIST));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.RightLowerArm), bones[(int)Body.LEFT_WRIST], BoneType.LEFT_HAND, Body.LEFT_WRIST));

            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.LeftUpperLeg), bones[(int)Body.RIGHT_KNEE], BoneType.RIGHT_LEG, Body.RIGHT_KNEE));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.RightUpperLeg), bones[(int)Body.LEFT_KNEE], BoneType.LEFT_LEG, Body.LEFT_KNEE));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.LeftLowerLeg), bones[(int)Body.RIGHT_ANKLE], BoneType.RIGHT_LEG, Body.RIGHT_ANKLE));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.RightLowerLeg), bones[(int)Body.LEFT_ANKLE], BoneType.LEFT_LEG, Body.LEFT_ANKLE));

            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.Spine), virtualNeck, BoneType.MIDDLE, Body.LEFT_SHOULDER));
            boneMappers.Add(new BoneMapper(avatar.GetBoneTransform(HumanBodyBones.Neck), bones[(int)Body.NOSE], BoneType.MIDDLE, Body.RIGHT_ANKLE));

        }

        public override void OnPoseUpdate(Solution solution)
        {
            _corePoseTrackingSolution = (CorePoseTrackingSolution)solution;
            if (_corePoseTrackingSolution.poseLandmarks == null)
            {
                foreach (BoneMapper boneMapper in boneMappers)
                {
                    boneMapper.parent.rotation = boneMapper.initialRotation;
                }
                isInTPose = false;
                tPoseInstruction.SetActive(true);
                return;
            }

            for (int i = 0; i < _numberOfBones; i++)
            {
                var position = skeletonParent.rect.GetPoint(_corePoseTrackingSolution.poseLandmarks.Landmark[i], 0, true);
                var world_position = skeletonParent.rect.GetPoint(_corePoseTrackingSolution.poseWorldLandmarks.Landmark[i], Vector3.one * _corePoseTrackingSolution.roiFromLandmarks.Width * 510, 0, true);
                if (visualizeZ)
                {
                    position.z = world_position.z;
                }
                else
                {
                    position.z = world_position.z;
                }
                //position.z *=.5f;
                //position = new Vector3(_corePoseTrackingSolution.poseWorldLandmarks.Landmark[i].X, _corePoseTrackingSolution.poseWorldLandmarks.Landmark[i].Y, _corePoseTrackingSolution.poseWorldLandmarks.Landmark[i].Z)*100;

                bones[i].position = skeletonParent.TransformPoint(position) + new Vector3(_corePoseTrackingSolution.roiFromLandmarks.Width * -6, 0, 0);
            }


            if (!isInTPose && CheckTPose())
            {
                hipLength = Vector2.Distance(bones[(int)Body.LEFT_HIP].transform.position, bones[(int)Body.RIGHT_HIP].transform.position) - 1.5f;
                y = Mathf.Abs(bones[(int)Body.LEFT_HIP].transform.position.y - bones[(int)Body.RIGHT_HIP].transform.position.y);
                z = Mathf.Abs(bones[(int)Body.LEFT_HIP].transform.position.z - bones[(int)Body.RIGHT_HIP].transform.position.z);

                verticalRotation = (bones[(int)Body.LEFT_HIP].transform.position.x - bones[(int)Body.RIGHT_HIP].transform.position.x) + 1;
                initialWidth = _corePoseTrackingSolution.roiFromLandmarks.Width - .02f;
                verticalHeight = Mathf.Abs(((bones[(int)Body.LEFT_SHOULDER].position + bones[(int)Body.RIGHT_SHOULDER].position) / 2).y - ((bones[(int)Body.LEFT_HIP].position + bones[(int)Body.RIGHT_HIP].position) / 2).y);

                OnTPose?.Invoke();
                tPoseInstruction.SetActive(false);
            }
            else
            {
                // log.text = $"isInTPose:{isInTPose}";
            }
            if (avatar != null) UpdateAvatar();
            //CheckPauseGesture();
        }
        public void UpdateAvatar()
        {
            AdjustPosition();
            AdjustIndependentRotation();
            AdjustScale();
        }
        public void AdjustPosition()
        {
            virtualNeck.localPosition = (bones[(int)Body.LEFT_SHOULDER].localPosition + bones[(int)Body.RIGHT_SHOULDER].localPosition) / 2;
            virtualHip.position = ((bones[(int)Body.LEFT_HIP].position + bones[(int)Body.RIGHT_HIP].position) / 2);
            virtualHip.position = new Vector3(virtualHip.position.x, virtualHip.position.y, skeletonParent.position.z - 30);
            virtualNeck.localPosition = new Vector3(virtualNeck.localPosition.x, virtualNeck.localPosition.y, bones[(int)Body.RIGHT_SHOULDER].localPosition.z);
        }
        public void AdjustIndependentRotation()
        {
            foreach (BoneMapper boneMapper in boneMappers)
            {
                Vector3 direction = (boneMapper.child.position - boneMapper.parent.position).normalized;
                Vector3 flatDirection = new Vector3(direction.x, direction.y, 0).normalized;
                Quaternion toRotation = Quaternion.FromToRotation((Vector2)boneMapper.parent.up, (Vector2)flatDirection) * boneMapper.parent.rotation;
                boneMapper.parent.eulerAngles = new Vector3(toRotation.eulerAngles.x, toRotation.eulerAngles.y, toRotation.eulerAngles.z);
            }
        }
        public void AdjustDependentRotation()
        {

            foreach (BoneMapper boneMapper in boneMappers)
            {
                float hipToWristZDistance = (boneMapper.child.position.z - bones[(int)Body.RIGHT_HIP].position.z) * .8f;
                Vector2 direction = boneMapper.child.position - boneMapper.parent.position;
                float angle = Vector2.SignedAngle(direction, Vector2.up);
                float zAngle = boneMapper.boneType.ToString().Contains("WRIST") ? 0 : Mathf.Clamp(hipToWristZDistance, -30, 30);
                if (boneMapper.boneType.ToString().Contains("HAND"))
                {
                    int negation = boneMapper.boneType.ToString().Contains("LEFT") ? -1 : 1;
                    boneMapper.parent.eulerAngles = new Vector3(angle * negation, negation * 90, negation * zAngle);
                }
                else
                {
                    boneMapper.parent.eulerAngles = new Vector3(0, 180, angle);
                }

            }
        }
        public void AdjustScale()
        {
            virtualHip.localScale = Vector3.one * _corePoseTrackingSolution.roiFromLandmarks.Height * avatarScaleFactor;
        }
        public void CheckPauseGesture()
        {
            if ((bones[(int)Body.RIGHT_WRIST].position.y - bones[(int)Body.NOSE].position.y) > 1.2)
            {
#if UNITY_EDITOR
            EditorApplication.isPaused = !EditorApplication.isPaused;
#endif
            }
        }
        public bool CheckTPose()
        {
            float calculatedTotalLength = 0;
            float Ydifference = 0;
            for (int i = 0; i < tPoseBodyPoint.Length - 1; i++)
            {
                calculatedTotalLength += Vector2.Distance(bones[tPoseBodyPoint[i]].transform.position, bones[tPoseBodyPoint[i + 1]].transform.position);
                Ydifference += Mathf.Abs(bones[tPoseBodyPoint[i]].transform.position.y - bones[tPoseBodyPoint[i + 1]].transform.position.y);
            }
            float expectedTotalLength = Vector2.Distance(bones[tPoseBodyPoint[0]].transform.position, bones[tPoseBodyPoint[tPoseBodyPoint.Length - 1]].transform.position);
            isInTPose = false;
            if (Mathf.Abs(expectedTotalLength - calculatedTotalLength) <= 15 && calculatedTotalLength > 10 && Ydifference < 50)
            {
                isInTPose = true;

            }
            return isInTPose;

        }


    }
    public class BoneMapper
    {
        public Transform parent;
        public Transform child;
        public Transform refParent;
        public Transform refChild;
        public BoneType boneType;
        public Body boneName;
        public Quaternion initialRotation;
        public int hip;
        public BoneMapper(Transform parent, Transform child, BoneType boneType, Body boneName)
        {
            this.parent = parent;
            this.child = child;
            this.boneType = boneType;
            this.boneName = boneName;
            initialRotation = parent.rotation;
            hip = (int)(this.boneType.ToString().Contains("LEFT") ? Body.LEFT_HIP : Body.RIGHT_HIP);

        }

    }
    public enum BoneType
    {
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_HAND,
        RIGHT_HAND,
        LEFT_LEG,
        RIGHT_LEG,
        MIDDLE
    }
}