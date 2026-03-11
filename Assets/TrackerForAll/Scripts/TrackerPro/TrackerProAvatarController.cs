using System.Collections;
using System.Collections.Generic;
using TrackerPro.Unity;
using TrackerPro.Unity.PoseTracking;
using UnityEngine;
using UnityEngine.Events;
namespace TrackerPro
{

  public class TrackerProAvatarController : MonoBehaviour
  {
    public static TrackerProAvatarController Instance;
    [SerializeField]
    public PoseTrackingSolution poseTrackingSolution;
    [HideInInspector]
    public Animator model;
   
    [SerializeField]
    private Transform hip;
    [SerializeField]
    private float scaleFactor = 1;
    [SerializeField] private Transform mediaNeck;
    private float width = 1;
    [SerializeField]
    private BoneConstraint[] _boneConstraints = new BoneConstraint[10];
    private Vector3[] previousPos = new Vector3[10];
    private GameObject neck;
    private GameObject nose;
    private bool boneMapped;
    private Vector3 currentPos;
    private Vector3 currentScale;
    [HideInInspector]
    public PoseLandmarkListAnnotation poseLandmarkListAnnotation;
    [HideInInspector]
    public PoseLandmarkListAnnotation poseWorldLandmarkListAnnotation;
    [HideInInspector]
    public MultiHandLandmarkListAnnotation handLandmarksAnnotation;
    [HideInInspector]
    public NormalizedRectAnnotationController roiFromLandmarksAnnotationController;
    [HideInInspector]
    public UnityEvent OnAvatarUpdate;
    [SerializeField]
    private float LerpTime = 1;

    private float previousShouldWidth = 0;
    [HideInInspector]
    public Transform currentSelected;
    public bool isUserDetected = false;
    private bool tracking = false;
    private bool showDressModel = false;
    private Transform modelNeck;
    float zAngle = 0;

    float shoulderDistance = 0;
    private float hipYRotation;

    private void Awake()
    {
      Instance = this;
      poseLandmarkListAnnotation = poseTrackingSolution.poseLandmarksAnnotationController.annotation;
      poseWorldLandmarkListAnnotation = poseTrackingSolution.poseWorldLandmarksAnnotationController.annotation;
      handLandmarksAnnotation = poseTrackingSolution.multiHandLandmarkListAnnotationController.annotation;
      roiFromLandmarksAnnotationController = poseTrackingSolution.roiFromLandmarksAnnotationController;
      for (int i = 0; i < _boneConstraints.Length; i++)
      {
        _boneConstraints[i] = new BoneConstraint();
      }
      neck = new GameObject("neck");
      nose = new GameObject("nose");
      model = hip.GetComponentInParent<Animator>();
      tracking = true;
      poseTrackingSolution.OnPoseUpdate.AddListener(OnPoseUpdate);

    }

    private void Start()
    {
    }

    public void ShowDress()
    {
      model.gameObject.SetActive(true);
      showDressModel = true;
      Debug.Log("Dress Chn");
    }
    public void ChangeModelVisibility(bool visibility)
    {
      model.gameObject.SetActive(visibility);
      showDressModel = visibility;
    }
    public void ChangeColor(UnityEngine.Color color)
    {
      currentSelected.GetComponent<Renderer>().material.color = color;
    }
    private void OnPoseUpdate()
    {
      OnAvatarUpdate?.Invoke();
      if (poseTrackingSolution.roiFromLandmarksAnnotationController._currentTarget != null)
      {
        width = poseTrackingSolution.roiFromLandmarksAnnotationController._currentTarget.Width;
      }
      if (!tracking)
      {
        currentScale = scaleFactor * width * Vector3.one;
        currentPos = new Vector3((poseLandmarkListAnnotation[23].transform.position.x + poseLandmarkListAnnotation[24].transform.position.x) / 2, (poseLandmarkListAnnotation[23].transform.position.y + poseLandmarkListAnnotation[24].transform.position.y) / 2, 0);
        return;
      }
      if (!isUserDetected)
      {
        model.gameObject.SetActive(false);
        return;
      }


      poseLandmarkListAnnotation = poseTrackingSolution.poseLandmarksAnnotationController.annotation;

      //handLandmarksAnnotation = poseTrackingSolution.multiHandLandmarkListAnnotationController.annotation;
      poseWorldLandmarkListAnnotation = poseTrackingSolution.poseWorldLandmarksAnnotationController.annotation;
      roiFromLandmarksAnnotationController = poseTrackingSolution.roiFromLandmarksAnnotationController;
      if (poseLandmarkListAnnotation._landmarkListAnnotation.count > 0 && !boneMapped)
      {
        model.gameObject.SetActive(true);
        MapBones();
      }
      AdjustTransform();



      for (int i = 0; i < _boneConstraints.Length; i++)
      {
        if (_boneConstraints[i].parent != null)
        {
          RotateToward(_boneConstraints[i]);
          previousPos[i] = _boneConstraints[i].child.position;
        }
      }

    }
    private void AdjustTransform()
    {

      float Xoffset = -.012f;
      hipYRotation = (poseLandmarkListAnnotation[12].transform.position.z - poseLandmarkListAnnotation[11].transform.position.z);
      float tempHipYRotation = hipYRotation;
      //Xoffset += (hipYRotation*.05f);
      if (poseTrackingSolution.roiFromLandmarksAnnotationController._currentTarget != null)
      {
        width = poseTrackingSolution.roiFromLandmarksAnnotationController.annotation.width;
        shoulderDistance = (poseLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position.x - poseLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position.x);
        currentScale = scaleFactor * width * Vector3.one + new Vector3(0, .03f, 0);
        model.transform.localScale = currentScale;
        previousShouldWidth = shoulderDistance;
      }
      float neckYAdjustmentFactor = 0;
      if (poseLandmarkListAnnotation[(int)Body.LEFT_ELBOW].transform.position.y > poseLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position.y)
      {
        neckYAdjustmentFactor = poseLandmarkListAnnotation[(int)Body.LEFT_ELBOW].transform.position.y - poseLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position.y;
      }
      else if (poseLandmarkListAnnotation[(int)Body.RIGHT_ELBOW].transform.position.y > poseLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position.y)
      {
        neckYAdjustmentFactor = poseLandmarkListAnnotation[(int)Body.RIGHT_ELBOW].transform.position.y - poseLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position.y;
      }

      hipYRotation = Mathf.Abs(hipYRotation) > 1.5f ? hipYRotation * 11 : 0;

      Xoffset -= tempHipYRotation * .012f;
      model.transform.localScale -= Vector3.one * neckYAdjustmentFactor * .45f;
      hip.position = Vector3.Lerp(hip.position, new Vector3(((poseLandmarkListAnnotation[23].transform.position.x + poseLandmarkListAnnotation[24].transform.position.x) / 2), (poseLandmarkListAnnotation[11].transform.position.y + poseLandmarkListAnnotation[12].transform.position.y) / 2, 0) + new Vector3(Xoffset, (neck.transform.position.y - modelNeck.position.y) - .09f, 0), Time.deltaTime * LerpTime * 2);


      if (Mathf.Abs(hipYRotation) > 50)
      {
        model.gameObject.SetActive(false);
      }
      else if (showDressModel)
      {
        model.gameObject.SetActive(true);
      }

      hip.eulerAngles = new Vector3(hip.eulerAngles.x, Mathf.LerpAngle(hip.eulerAngles.y, hipYRotation, Time.deltaTime * LerpTime), hip.eulerAngles.z);

      neck.transform.position = new Vector3((poseLandmarkListAnnotation[12].transform.position.x + poseLandmarkListAnnotation[11].transform.position.x) / 2, (poseLandmarkListAnnotation[12].transform.position.y + poseLandmarkListAnnotation[11].transform.position.y) / 2, poseLandmarkListAnnotation[11].transform.position.z) + new Vector3(Xoffset, 0, 0);
      nose.transform.position = Vector3.Lerp(neck.transform.position, poseLandmarkListAnnotation[0].transform.position, .3f) + new Vector3(Xoffset, 0, 0);
      mediaNeck.position = neck.transform.position;

    }
    private void MapBones()
    {
      boneMapped = true;
      Debug.Log($"Bone Maps:{model.name}");

      _boneConstraints[0].parent = model.GetBoneTransform(HumanBodyBones.RightUpperArm);
      _boneConstraints[0].child = poseLandmarkListAnnotation[(int)Body.LEFT_ELBOW].transform;
      _boneConstraints[0].boneType = BoneType.LEFT_HAND;
      _boneConstraints[0].body = Body.LEFT_SHOULDER;

      _boneConstraints[1].parent = model.GetBoneTransform(HumanBodyBones.RightLowerArm);
      _boneConstraints[1].child = poseLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform;
      _boneConstraints[1].boneType = BoneType.LEFT_HAND;
      _boneConstraints[1].body = Body.LEFT_ELBOW;

      _boneConstraints[2].parent = model.GetBoneTransform(HumanBodyBones.LeftUpperArm);
      _boneConstraints[2].child = poseLandmarkListAnnotation[(int)Body.RIGHT_ELBOW].transform;
      _boneConstraints[2].boneType = BoneType.RIGHT_HAND;
      _boneConstraints[2].body = Body.RIGHT_SHOULDER;

      _boneConstraints[3].parent = model.GetBoneTransform(HumanBodyBones.LeftLowerArm);
      _boneConstraints[3].child = poseLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform;
      _boneConstraints[3].boneType = BoneType.RIGHT_HAND;
      _boneConstraints[3].body = Body.RIGHT_ELBOW;

      //_boneConstraints[4].parent = model.GetBoneTransform(HumanBodyBones.RightShoulder);
      //_boneConstraints[4].child = poseLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform;
      //_boneConstraints[4].boneType = BoneType.LEFT;
      //_boneConstraints[4].body = Body.LEFT_WRIST;

      //_boneConstraints[5].parent = model.GetBoneTransform(HumanBodyBones.LeftShoulder);
      //_boneConstraints[5].child = poseLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform;
      //_boneConstraints[5].boneType = BoneType.RIGHT;
      //_boneConstraints[5].body = Body.RIGHT_WRIST;

      //_boneConstraints[4].parent = model.GetBoneTransform(HumanBodyBones.RightUpperLeg);
      //_boneConstraints[4].child = poseLandmarkListAnnotation[25].transform;

      //_boneConstraints[5].parent = model.GetBoneTransform(HumanBodyBones.RightLowerLeg);
      //_boneConstraints[5].child = poseLandmarkListAnnotation[27].transform;

      //_boneConstraints[6].parent = model.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
      //_boneConstraints[6].child = poseLandmarkListAnnotation[26].transform;

      //_boneConstraints[7].parent = model.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
      //_boneConstraints[7].child = poseLandmarkListAnnotation[28].transform;

      _boneConstraints[8].parent = model.GetBoneTransform(HumanBodyBones.Chest);
      _boneConstraints[8].child = nose.transform;
      _boneConstraints[8].boneType = BoneType.MIDDLE;

      _boneConstraints[9].parent = model.GetBoneTransform(HumanBodyBones.Spine);
      _boneConstraints[9].child = neck.transform;
      _boneConstraints[9].boneType = BoneType.MIDDLE;

      modelNeck = model.GetBoneTransform(HumanBodyBones.UpperChest);
      model.gameObject.SetActive(false);

    }


    private void Update()
    {
      //previousPos = new Vector3(((poseLandmarkListAnnotation[23].transform.position.x + poseLandmarkListAnnotation[24].transform.position.x) / 2) - temp * .04f, (poseLandmarkListAnnotation[23].transform.position.y + poseLandmarkListAnnotation[24].transform.position.y) / 2, 0) + new Vector3(Xoffset, width * .88f, 0);
    }
    private void RotateToward(BoneConstraint bone)
    {
      if ((Mathf.Abs(hipYRotation) > 2 && (bone.body == Body.LEFT_SHOULDER || bone.body == Body.RIGHT_SHOULDER || bone.body == Body.LEFT_WRIST || bone.body == Body.RIGHT_WRIST))) return;
      if (bone.body == Body.LEFT_SHOULDER)
      {
        if (poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position.z <= -2f)
        {
          zAngle = 30;
          if (Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position) - Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position) < 4)
          {
            zAngle = 0;
          }
        }
        else
        {
          zAngle = 0;
        }
      }
      else if (bone.body == Body.RIGHT_SHOULDER)
      {
        if (poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position.z <= -2f)
        {
          zAngle = -30;
          if (Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position) - Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position) < 4)
          {
            zAngle = 0;
          }
        }
        else
        {
          zAngle = 0;
        }
      }
      else
      {
        zAngle = 0;
      }
      zAngle = 0;
      float yRot = bone.parent.localEulerAngles.y;
      Vector2 targetDirection = bone.child.position - bone.parent.position;
      if ((int)bone.body >= 11 && (int)bone.body <= 16 && !shouldLerp())
      {
        bone.parent.up = new Vector2(targetDirection.x, targetDirection.y);
      }
      else
      {
        bone.parent.up = Vector3.Lerp(bone.parent.up, new Vector2(targetDirection.x, targetDirection.y), Time.deltaTime * LerpTime);
      }
      bone.parent.localEulerAngles = bone.boneType == BoneType.MIDDLE ? new Vector3(0, 0, bone.parent.localEulerAngles.z) : new Vector3(bone.boneType == BoneType.LEFT_HAND ? -bone.parent.localEulerAngles.z : bone.parent.localEulerAngles.z, bone.body == Body.LEFT_WRIST ? 90 : bone.body == Body.RIGHT_WRIST ? -90 : 0, zAngle);
      //bone.parent.localEulerAngles = bone.boneType == BoneType.MIDDLE ? new Vector3(0, 0, Mathf.LerpAngle(bone.parent.localEulerAngles.z,bone.parent.localEulerAngles.z,Time.deltaTime * LerpTime)) : new Vector3(Mathf.LerpAngle(bone.parent.localEulerAngles.x, bone.boneType == BoneType.LEFT ? -bone.parent.localEulerAngles.z : bone.parent.localEulerAngles.z, Time.deltaTime * LerpTime), 0, zAngle);
    }
    public bool shouldLerp()
    {
      float zAngle = 0;
      if (poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position.z <= -2f)
      {
        zAngle = 30;
        if (Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position) - Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position) < 4)
        {
          zAngle = 0;
        }
      }
      else if (poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position.z <= -2f)
      {
        zAngle = -30;
        if (Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_SHOULDER].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_SHOULDER].transform.position) - Vector3.Distance(poseWorldLandmarkListAnnotation[(int)Body.LEFT_WRIST].transform.position, poseWorldLandmarkListAnnotation[(int)Body.RIGHT_WRIST].transform.position) < 4)
        {
          zAngle = 0;
        }
      }
      return Mathf.Abs(zAngle) <= 0;
    }

  }
}
