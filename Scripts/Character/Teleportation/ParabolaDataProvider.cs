using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaDataProvider : PredictionLineDataProvider
{
    protected float farthestDistance = 5f;
    protected float velo;
    public ParabolaDataProvider(float farthestDistance, LayerMask hittingTargetLayer, LayerMask hittingIgnoreLayer)
    {
        this.farthestDistance = farthestDistance;
        this.hittingTargetLayer = hittingTargetLayer;
        this.hittingIgnoreLayer = hittingIgnoreLayer;
        velo = Mathf.Sqrt(farthestDistance);
    }

    public override LineData GeneratingLineData(Vector3 origin, Vector3 direction)
    {
        float timePrecision = 0.1f;
        int maxIteration = 200;
        List<Vector3> lineData = new List<Vector3>();
        lineData.Add(origin);
        Vector3 p = origin;
        Vector3 q = origin;
        Vector3 v = direction.normalized * velo;

        HittingStatus hittingStatus = HittingStatus.NoHitting;
        Collider hitCollider = null;
        for (int i = 0; i < maxIteration; i++)
        {
            q = p + v * timePrecision;


            RaycastHit hitInfo = new RaycastHit();
            //Hit something
            if (Physics.Raycast(p, q - p, out hitInfo, (q - p).magnitude, ~hittingIgnoreLayer))
            {
                if (((1 << hitInfo.collider.gameObject.layer) & hittingTargetLayer.value) != 0)
                {
                    var up = Vector3.Dot(hitInfo.normal, Vector3.up);
                    if (up < Mathf.Cos(maxSlopAngle * 3.14159f / 180f))//slop is too steep
                    {
                        hittingStatus = HittingStatus.HitNonTeleportingArea;
                    }
                    else
                        hittingStatus = HittingStatus.HitTeleportingArea;
                }
                else
                {
                    hittingStatus = HittingStatus.HitNonTeleportingArea;
                }
                lineData.Add(hitInfo.point);
                hitCollider = hitInfo.collider;
                break;
            }
            lineData.Add(q);
            p = q;
            v += Vector3.down * timePrecision;
        }

        //foreach (var u in lineData)
        //{
        //    Debug.Log(u);
        //}

        return new LineData(hittingStatus, hitCollider, lineData.ToArray());
    }

}
