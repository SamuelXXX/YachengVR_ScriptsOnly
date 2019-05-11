using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HittingStatus
{
    NoHitting = 0,
    HitTeleportingArea,
    HitNonTeleportingArea,
    HitModifiedTarget
}

public class LineData
{
    public HittingStatus hittingStatus;
    public Collider hitCollider;
    public Vector3[] lineData;

    public LineData(HittingStatus hittingStatus, Collider hitCollider, Vector3[] lineData)
    {
        this.hittingStatus = hittingStatus;
        this.hitCollider = hitCollider;
        this.lineData = lineData;
    }
}


public class PredictionLineDataProvider
{
    public LayerMask hittingIgnoreLayer = 0;
    public LayerMask hittingTargetLayer = -1;
    public float maxSlopAngle = 10f;
    public virtual LineData GeneratingLineData(Vector3 origin, Vector3 direction)
    {
        return null;
    }
}
