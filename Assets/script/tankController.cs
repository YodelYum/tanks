using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tankController : Photon.MonoBehaviour
{

    public Transform turret;
    public Transform cannon;
    public Transform cameraLockSpot;

    public Transform orbitPoint;

    public float turretRotSpeed = 5;

    bool mouseLock = true;

    int selfRotation = 0;

    Transform myCam;

    Vector3 lookAtPoint;

    RaycastHit hit;
    float lastRotation;

    public Transform thisCollider;


    Transform MagicTank;
    PhysicsTank _tank;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        if (photonView.isMine)
        {
            MagicTank = GameObject.Find("MagicSteeringTank").transform;
            MagicTank.position = transform.position;
            _tank = GameObject.Find("MagicSteeringTank").GetComponent<PhysicsTank>();
            Cursor.lockState = CursorLockMode.Locked;
            myCam = Camera.main.transform;
            myCam.GetComponent<MouseOrbitImproved>().target = orbitPoint;
            lastRotation = turret.localEulerAngles.y;

            thisCollider.parent = MagicTank;
            thisCollider.localPosition = thisCollider.localEulerAngles = Vector3.zero;
            MagicTank.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        lookAtPointCalc();
        turretControls();

        //turretControl();
        //mouseLocking();
        camControls();
    }

    void lookAtPointCalc()
    {
        Ray ray = myCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, 500))
        {
            lookAtPoint = hit.point;
        }
        else
        {
            lookAtPoint = GameObject.Find("farawaypoint").transform.position;
        }
    }

    void camControls()
    {
        orbitPoint.position = transform.position + Vector3.up * 5;
    }

    void turretControls()
    {
        lastRotation = turret.localEulerAngles.y;
        turret.LookAt(lookAtPoint);
        turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y, 0);

        cannon.LookAt(lookAtPoint);
        cannon.localEulerAngles = new Vector3(cannon.localEulerAngles.x, 0, 0);
    }

    void mouseLocking()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(!mouseLock)
            {
                mouseLock = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                mouseLock = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void turretControl()
    {
        /*myCam.position = cameraLockSpot.position;
        myCam.rotation = cameraLockSpot.rotation;
        myCam.eulerAngles = new Vector3(myCam.eulerAngles.x, myCam.eulerAngles.y,
            0);*/
        if (!mouseLock)
        {
            if (Input.mousePosition.x < Screen.width/2)
            {
                turret.Rotate(Vector3.up * Time.deltaTime * -turretRotSpeed);
            }else if (Input.mousePosition.x >= Screen.width / 2)
            {
                turret.Rotate(Vector3.up * Time.deltaTime * turretRotSpeed);
            }
        }
        else
        {
            if(Input.GetAxis("Mouse X") != 0)
            {
                selfRotation = 0;
                turret.Rotate(Vector3.up * Time.deltaTime * Input.GetAxis("Mouse X") * turretRotSpeed);
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                cannon.Rotate(Vector3.right * Time.deltaTime * -Input.GetAxis("Mouse Y") * turretRotSpeed);
            }
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            selfRotation = -1;
        }else if (Input.GetKeyDown(KeyCode.E))
        {
            selfRotation = 1;
        }
        if (selfRotation == 1)
        {
            turret.Rotate(Vector3.up * Time.deltaTime * 1 * turretRotSpeed);
        } else if (selfRotation == -1)
        {
            turret.Rotate(Vector3.up * Time.deltaTime * -1 * turretRotSpeed);
        }
    }
}
