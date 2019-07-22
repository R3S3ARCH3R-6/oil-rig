using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    private int materialID;
    private string method;
    private Ray ray;
    private Camera viewCamera;
    private int maxCursorDistance;
    private int speedOfTrail;
    private float sphereRadius;
    private float lerpDistance;
    private bool lerp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (method.Equals("live")){
            Cursor();
            transform.LookAt(viewCamera.transform, transform.up);
        } 
    }

    public int GetID()
    {
        return materialID;
    }

    public string GetMethod()
    {
        return method;
    }

    public void SetID(int id)
    {
        materialID = id;
    }

    public void SetMethod(string s)
    {
        method = s;
    }

    public void SetCamera(Camera cam)
    {
        viewCamera = cam;
    }

    public void SetMaxCursorDistance(int max)
    {
        maxCursorDistance = max;
    }

    public void SetSpeedOfTrail(int speed)
    {
        speedOfTrail = speed;
    }
    
    public void SetSphereRadius(float radius)
    {
        sphereRadius = radius;
    }

    public void SetLerpDistance(float f)
    {
        lerpDistance = f;
    }

    public void SetLerp(bool b)
    {
        lerp = b;
    }

    private void Cursor()
    {
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out RaycastHit hit, maxCursorDistance))
        {
            if(lerp == false)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, hit.point, speedOfTrail * Time.deltaTime);
            }
            else
            {
                Vector3 position = Vector3.Lerp(viewCamera.transform.position, hit.point, lerpDistance);
                this.transform.position = Vector3.MoveTowards(this.transform.position, position, speedOfTrail * Time.deltaTime);
            }

        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, ray.origin + ray.direction.normalized * maxCursorDistance, speedOfTrail * Time.deltaTime);
        }
    }
}
