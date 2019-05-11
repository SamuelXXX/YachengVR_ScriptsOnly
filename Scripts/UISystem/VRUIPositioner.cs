using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to keep UI in front your 
/// </summary>
public class VRUIPositioner : MonoBehaviour
{
    public enum ViewType
    {
        Middle = 0,
        Top,
        Bottom
    }

    public enum BaseType
    {
        Head = 0,
        Body
    }
    public float lerpSpeed = 8f;
    public float distance = 2f;
    public ViewType viewType = ViewType.Middle;
    public BaseType baseType = BaseType.Head;
    public bool applyToPosition = true;
    public bool applyToRotation = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = GeneratePosition();
        if (applyToPosition)
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSpeed);
        if (applyToRotation)
            transform.LookAt(2 * transform.position - VRPlayer.Instance.Head.transform.position);
    }

    private void OnEnable()
    {
        GlobalEventManager.RegisterHandler("internal_VRPlayer.Repositioned", Reposition);

    }

    private void OnDisable()
    {
        GlobalEventManager.UnregisterHandler("internal_VRPlayer.Repositioned", Reposition);
    }

    public void SetPositionImmediately()
    {
        if (VRPlayer.Instance == null)
            return;

        Vector3 position = GeneratePosition();
        transform.position = position;
        transform.LookAt(2 * transform.position - VRPlayer.Instance.Head.transform.position);
    }


    /// <summary>
    /// Global event call back for transporting
    /// </summary>
    void Reposition()
    {
        transform.position = GeneratePosition();
        transform.LookAt(2 * transform.position - VRPlayer.Instance.Head.transform.position);
    }

    Vector3 GeneratePosition()
    {
        Vector3 direction = Vector3.zero;
        switch (baseType)
        {
            case BaseType.Head:
                direction = VRPlayer.Instance.Head.transform.forward;
                break;
            case BaseType.Body:
                direction = VRPlayer.Instance.Body.transform.forward;
                break;
            default: break;
        }

        direction.y = 0f;
        direction = direction.normalized;

        switch (viewType)
        {
            case ViewType.Top:
                direction += Vector3.up;
                direction = direction.normalized;
                break;
            case ViewType.Middle:

                break;
            case ViewType.Bottom:
                direction -= Vector3.up;
                direction = direction.normalized;
                break;
            default: break;
        }

        Vector3 position = direction * distance + VRPlayer.Instance.Head.transform.position;
        return position;
    }
}
