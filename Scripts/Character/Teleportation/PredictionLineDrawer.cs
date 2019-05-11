using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PredictionLineDrawer : MonoBehaviour
{
    #region Settings
    public float farthestDistance;
    public LayerMask hittingTargetLayer;
    public LayerMask hittingIgnoreLayer;
    public GameObject hittingTargetPrefab;
    public GameObject hittingObstaclePrefab;
    public Gradient normalColor;
    public Gradient hitObstacleColor;
    public float maxSlopAngle = 10f;


    public Transform emittingPoint;
    #endregion

    #region Runtime data
    public bool drawLine = false;

    protected PredictionLineDataProvider lineDataProvider = null;
    protected LineRenderer lineRenderer;
    protected GameObject hittingTargetGameObject;
    protected GameObject hittingObstacleGameObject;
    #endregion

    public Vector3? modifiedTargetPoint = null;

    #region Exposed Data Every Frame
    protected Vector3 anticipateHitPoint = Vector3.zero;
    protected Vector3 hitPoint = Vector3.zero;
    protected Collider hitCollider = null;
    protected HittingStatus status = HittingStatus.NoHitting;

    public Vector3 HitPoint
    {
        get
        {
            return hitPoint;
        }
    }

    public Vector3 AnticipateHitPoint
    {
        get
        {
            return anticipateHitPoint;
        }
    }

    public Collider HitCollider
    {
        get
        {
            return hitCollider;
        }
    }

    public HittingStatus Status
    {
        get
        {
            return status;
        }
    }
    #endregion

    #region Life Circle
    // Use this for initialization
    void Start()
    {
        lineDataProvider = new ParabolaDataProvider(farthestDistance, hittingTargetLayer, hittingIgnoreLayer);
        lineDataProvider.maxSlopAngle = maxSlopAngle;
        if (emittingPoint == null)
            emittingPoint = transform;

        lineRenderer = GetComponent<LineRenderer>();
        if (hittingTargetPrefab)
        {
            hittingTargetGameObject = Instantiate(hittingTargetPrefab);
            GameObject.DontDestroyOnLoad(hittingTargetGameObject);
            hittingTargetGameObject.SetActive(false);
        }

        if (hittingObstaclePrefab)
        {
            hittingObstacleGameObject = Instantiate(hittingObstaclePrefab);
            GameObject.DontDestroyOnLoad(hittingObstacleGameObject);
            hittingObstacleGameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (drawLine)
        {
            LineData data = lineDataProvider.GeneratingLineData(emittingPoint.position, emittingPoint.forward);
            lineRenderer.positionCount = data.lineData.Length;
            Vector3[] lineData = null;
            if (modifiedTargetPoint == null)
            {
                lineData = data.lineData;
            }
            else
            {
                lineData = new Vector3[data.lineData.Length];
                Vector3 beginPoint = data.lineData[0];
                Vector3 endPoint = modifiedTargetPoint.Value;

                for (int i = 0; i < data.lineData.Length; i++)
                {
                    Vector3 interplote = ((float)i * endPoint + (float)(data.lineData.Length - 1 - i) * beginPoint) / ((float)data.lineData.Length - 1);
                    lineData[i] = ((float)i * interplote + (float)(data.lineData.Length - 1 - i) * data.lineData[i]) / ((float)data.lineData.Length - 1);
                }
            }


            anticipateHitPoint = data.lineData[data.lineData.Length - 1];
            hitPoint = lineData[lineData.Length - 1];

            lineRenderer.SetPositions(lineData);


            if (modifiedTargetPoint != null)
            {
                hitCollider = null;//No hit collider when hit the modified target
                status = HittingStatus.HitModifiedTarget;
                lineRenderer.colorGradient = normalColor;
                if (hittingTargetGameObject)
                {
                    hittingTargetGameObject.SetActive(false);
                }
                if (hittingObstacleGameObject)
                {
                    hittingObstacleGameObject.SetActive(false);
                }
            }
            else
            {
                hitCollider = data.hitCollider;
                status = data.hittingStatus;
                switch (data.hittingStatus)
                {
                    case HittingStatus.NoHitting:
                        if (hittingTargetGameObject)
                        {
                            hittingTargetGameObject.SetActive(false);
                        }
                        if (hittingObstacleGameObject)
                        {
                            hittingObstacleGameObject.SetActive(false);
                        }
                        lineRenderer.colorGradient = hitObstacleColor;

                        break;
                    case HittingStatus.HitTeleportingArea:
                        lineRenderer.colorGradient = normalColor;
                        if (hittingObstacleGameObject)
                        {
                            hittingObstacleGameObject.SetActive(false);
                        }
                        if (hittingTargetGameObject)
                        {
                            hittingTargetGameObject.SetActive(true);
                            hittingTargetGameObject.transform.position = data.lineData[data.lineData.Length - 1];
                        }
                        break;
                    case HittingStatus.HitNonTeleportingArea:
                        lineRenderer.colorGradient = hitObstacleColor;
                        if (hittingTargetGameObject)
                        {
                            hittingTargetGameObject.SetActive(false);
                        }
                        if (hittingObstacleGameObject)
                        {
                            hittingObstacleGameObject.SetActive(true);
                            hittingObstacleGameObject.transform.position = data.lineData[data.lineData.Length - 1];
                        }
                        break;
                    default: break;
                }
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
            if (hittingTargetGameObject)
            {
                hittingTargetGameObject.SetActive(false);
            }
            if (hittingObstacleGameObject)
            {
                hittingObstacleGameObject.SetActive(false);
            }
        }
    }
    #endregion
}
