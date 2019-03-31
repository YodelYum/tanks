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

    int mouseLock = 1;

    int selfRotation = 0;
    int selfRotationY = 0;

    Transform myCam;

    Vector3 lookAtPoint;

    RaycastHit hit;
    float lastRotation;

    public Transform thisCollider;


    Transform MagicTank;
    PhysicsTank _tank;

    public int updateRate = 10;
    public float currentRate = 0;

    Vector3 newPosition;
    Quaternion newRotation;

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
            mouseLock = 1;
            thisCollider.parent = MagicTank;
            thisCollider.localPosition = thisCollider.localEulerAngles = Vector3.zero;
            MagicTank.GetComponent<Rigidbody>().isKinematic = false;
            myCam.GetComponent<MouseOrbitImproved>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine) { 
            lookAtPointCalc();
            //turretControls();
            turretControl();
            mouseLocking();
            //camControls();
            posRotUpdater();
        }
        transform.position = Vector3.Slerp(transform.position, newPosition, Time.deltaTime * 5);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 5);
    }

    void posRotUpdater()
    {
        if (currentRate == 0)
        {
            currentRate = 1 / updateRate;
            photonView.RPC("updatePosRot", PhotonTargets.AllViaServer, MagicTank.position, MagicTank.rotation);
        }
        else if (currentRate > 0)
        {
            currentRate -= Time.deltaTime;
        }
        else if (currentRate < 0)
        {
            currentRate = 0;
        }

        

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
            if(mouseLock == 2)
            {
                mouseLock = 1;
                myCam.GetComponent<MouseOrbitImproved>().enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if(mouseLock == 1)
            {
                mouseLock = 2;
                myCam.GetComponent<MouseOrbitImproved>().enabled = true;
            }
            else if (mouseLock == 3)
            {
                mouseLock = 1;
                myCam.GetComponent<MouseOrbitImproved>().enabled = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            mouseLock = 3;
            Cursor.lockState = CursorLockMode.None;
        }


        }

    public void turretControl()
    {
        /*myCam.position = cameraLockSpot.position;
        myCam.rotation = cameraLockSpot.rotation;
        myCam.eulerAngles = new Vector3(myCam.eulerAngles.x, myCam.eulerAngles.y,
            0);*/
        
        if(mouseLock == 1)
        {
            if(Input.GetAxis("Mouse X") != 0)
            {
                selfRotation = 0;
                selfRotationY = 0;
                turret.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * turretRotSpeed);
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                cannon.Rotate(Vector3.right * -Input.GetAxis("Mouse Y"));
            }
            myCam.position = cameraLockSpot.position;
            myCam.rotation = cameraLockSpot.rotation;
        }else if (mouseLock == 2)
        {
            orbitPoint.position = transform.position + Vector3.up * 4;
        }else if(mouseLock == 3)
        {
            myCam.position = cameraLockSpot.position;
            myCam.rotation = cameraLockSpot.rotation;
            if (Input.mousePosition.x < Screen.width / 2)
            {
                selfRotation = -1;
            }
            else if (Input.mousePosition.x > Screen.width / 2)
            {
                selfRotation = 1;
            }
            Debug.Log(Input.mousePosition.y);
            if (Input.mousePosition.y< Screen.height / 2)
            {
                selfRotationY = -1;
            }
            else if (Input.mousePosition.y> Screen.height / 2)
            {
                selfRotationY = 1;
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

        if (selfRotationY == 1)
        {
            cannon.Rotate(Vector3.right * Time.deltaTime * -1 * turretRotSpeed);
        }
        else if (selfRotationY == -1)
        {
            cannon.Rotate(Vector3.right * Time.deltaTime * 1 * turretRotSpeed);
        }
    }

    [PunRPC]
    public void updatePosRot(Vector3 pos, Quaternion rot)
    {
        newPosition = pos;
        newRotation = rot;
    }



    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(turret.eulerAngles.x);
            stream.SendNext(cannon.eulerAngles.y);
        }
        else
        {
            turret.eulerAngles = new Vector3((float)stream.ReceiveNext(), turret.eulerAngles.y, turret.eulerAngles.z);
            cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, (float)stream.ReceiveNext(), cannon.eulerAngles.z);
        }
    }
}
