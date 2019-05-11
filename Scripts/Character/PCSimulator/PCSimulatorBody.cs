using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCSimulatorBody : MonoBehaviour
{
    #region Settings
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("PC View Settings")]
    public float headRotatingSpeedX = 8f;
    public float headRotatingSpeedY = 8f;

    public float minimumY = -70;
    public float maximumY = 70;

    [Header("PC Move Settings")]
    public float moveSpeed = 1f;
    #endregion

    #region Run-time data
    bool viewFreezed = false;
    bool moveFreezed = false;
    #endregion

    #region LifeCircle

    private void Awake()
    {
        GlobalEventManager.RegisterHandler("VRPlayer.FreezeView", FreezeView);
        GlobalEventManager.RegisterHandler("VRPlayer.UnfreezeView", UnfreezeView);
    }

    private void OnDestroy()
    {
        GlobalEventManager.UnregisterHandler("VRPlayer.FreezeView", FreezeView);
        GlobalEventManager.UnregisterHandler("VRPlayer.UnfreezeView", UnfreezeView);
    }

    private void OnEnable()
    {        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    // Update is called once per frame
    void Update()
    {
        if (head == null)
            return;

        UpdateHeadRot();
        UpdateHeadPos();

        //Special Process for Teleporting
        Teleporter teleporter = GetComponentInChildren<Teleporter>();
        VRInteractor interactor = GetComponentInChildren<VRInteractor>();
        if (Input.GetMouseButton(1))
        {
            FreezeView();
            if (teleporter != null)
            {
                Vector3 localEuler = teleporter.transform.localEulerAngles;

                float rotationX = localEuler.y + Input.GetAxis("Mouse X") * headRotatingSpeedX / 10f;
                float rotationY = -localEuler.x;
                rotationY += Input.GetAxis("Mouse Y") * headRotatingSpeedY / 10f;
                if (rotationY > 180)
                {
                    rotationY -= 360f;
                }
                if (rotationY < -180f)
                {
                    rotationY += 360f;
                }
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                localEuler = new Vector3(-rotationY, rotationX, 0);
                teleporter.transform.localRotation = Quaternion.Euler(localEuler);
            }
        }
        else
        {
            if (teleporter != null)
            {
                teleporter.transform.localRotation = Quaternion.identity;
            }
            UnfreezeView();
        }
    }
    #endregion

    #region Body Posture Control
    void UpdateHeadRot()
    {
        if (viewFreezed)
            return;

        Vector3 localBodyEuler = transform.localEulerAngles;
        Vector3 localHeadEuler = head.localEulerAngles;

        float rotationX = localBodyEuler.y + Input.GetAxis("Mouse X") * headRotatingSpeedX / 10f;
        float rotationY = -localHeadEuler.x;
        rotationY += Input.GetAxis("Mouse Y") * headRotatingSpeedY / 10f;
        if (rotationY > 180)
        {
            rotationY -= 360f;
        }
        if (rotationY < -180f)
        {
            rotationY += 360f;
        }
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        localBodyEuler = new Vector3(0, rotationX, 0);
        localHeadEuler = new Vector3(-rotationY, 0, 0);


        if (!viewFreezed)
        {
            transform.localEulerAngles = localBodyEuler;
            head.localEulerAngles = localHeadEuler;
        }
    }

    void UpdateHeadPos()
    {
        if (moveFreezed)
            return;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        transform.GetComponent<Rigidbody>().velocity = transform.forward * vertical * moveSpeed + transform.right * horizontal * moveSpeed;
    }

    HashSet<Behaviour> freezingRequesters = new HashSet<Behaviour>();


    public void FreezeView()
    {
        viewFreezed = true;
    }

    public void UnfreezeView()
    {
        viewFreezed = false;
    }

    public void FreezeMovement()
    {
        moveFreezed = true;
    }

    public void UnfreezeMovement()
    {
        moveFreezed = false;
    }

    #endregion
}
