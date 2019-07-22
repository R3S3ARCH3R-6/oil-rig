using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Controller : MonoBehaviour
{
    [Header("All Methods")]
    public int maxCursorDistance = 25;
    public int speedOfTrail = 100;
    public float spherecastRadius = .05f;
    
    [Header("Live Trail")]
    public GameObject liveTrailPrefab;

    [Header("Ring")]
    public GameObject ringPrefab;
    public float lerp = .8f;

    [Header("Head")]
    public GameObject headPrefab;

    [Header("Object Arrow")]
    public GameObject objectArrowPrefab;    

    [Header("Line/Arrow")]
    public GameObject cursor;

    [Header("Adil")]
    public GameObject adil;
    
    private Camera viewCamera;
    private Ray ray;
    private string currentTechnique;
    private List<Material> materials;
    private List<bool> matAvailable;
    private List<Vector3> origins;
    private List<Vector3> locations;
    private List<GameObject> activeObjects;



    private void Awake()
    {
        viewCamera = Camera.main;
    }

    void Start()
    {
        activeObjects = new List<GameObject>();
        SetupMaterials();
        SetupOrigins();
        SetupLookLocation();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (GameObject obj in activeObjects)
            {
                PrepDestroy(obj);
            }
            activeObjects = new List<GameObject>();
        }
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !Input.GetKey(KeyCode.LeftShift))
        {
            CreateLiveReading();            
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftShift))
        {
            CreateAdilLive();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !Input.GetKey(KeyCode.LeftShift))
        {
            CreateData();            
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftShift))
        {
            CreateData();
            CreateAdilData();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CreateRing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CreateHead();
            CreateStaticRing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CreateLine();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SetupObject(obj, "cube");
            activeObjects.Add(obj);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            CreateHead();
            CreateObjectArrow();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateSpheres();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            CreateSpheres();
            SetSphereRotations();
            CreateObjectArrow();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateCircles();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CreatePointerCircles();
        }
    }

    private void SetupMaterials()
    {
        materials = new List<Material>();
        matAvailable = new List<bool>();
        
        List<Color> colors = new List<Color>
        {
            new Color(1,0,0,1),
            new Color(0,1,0,1),
            new Color(0,0,1,1),
            new Color(1,.92f,.016f,1),
            new Color(0,1,1,1)
        };

        foreach(Color col in colors)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 2.0f);
            mat.color = col;
            materials.Add(mat);
            matAvailable.Add(true);
        }
    }

    private void SetupOrigins()
    {
        //original origin points
        /*origins = new List<Vector3>
        {
            new Vector3(-2.7f, 0.15f, -2.75f), 
            new Vector3(-2.7f, 1.15f, -3.75f), 
            new Vector3(-3.7f, 0.15f, -2.75f), 
            new Vector3(-2.1f, 0.15f, -2.75f), 
            new Vector3(-0.72f, 0.17f, -2.75f),
        };*/

        //new origin points (these will appear behind the camera (adjust as necessary)
        origins = new List<Vector3>
        {
            new Vector3(2.45f, 1.71f, -12.85f),
            new Vector3(2.805f, 1.811f, -11.76f),
            new Vector3(2.34f, 1.308f, -10.735f),
            new Vector3(-1.45f, 1.93f, -9.96f),
            new Vector3(1.61f, 1.024f, -12.808f),
        };
    }

    private void SetupLookLocation()
    {
        locations = new List<Vector3>
        {
            new Vector3(-4f, 1.15f, -3f),
            new Vector3(-0.73f, 0.32f, -4.88f),
            new Vector3(0.59f, 0.62f, -3.61f),
            new Vector3(-2f, 0.76f, -5.38f),
            new Vector3(-2.59f, 0.62f, -3.61f)
        };
    }

    private void CreateLiveReading()
    {
        GameObject obj = Instantiate(liveTrailPrefab, GetPosition(), Quaternion.identity);        
        SetupObject(obj, "live");        
        activeObjects.Add(obj);        
    }

    private void CreateData()
    {        
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(liveTrailPrefab);
            List<Quest> quests = LoadData(i);
            SetupObject(obj, "data");            
            obj.AddComponent<TrailFromData>();
            obj.GetComponent<TrailFromData>().SetQuests(quests);
            obj.GetComponent<TrailFromData>().SetSpeedofTrail(speedOfTrail - 25);
            obj.transform.position = new Vector3(quests[0].x, quests[0].y, quests[0].z);
            activeObjects.Add(obj);
        }
    }

    private List<Quest> LoadData(int i)
    {
        List<Quest> questData = new List<Quest>();
        string filePath = "Assets/Data/gazeData" + i + ".json";
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
            foreach (Quest q in loadedData.quests)
            {
                questData.Add(q);
            }
        }
        return questData;
    }

    private void CreateRing()
    {
        GameObject obj = Instantiate(ringPrefab, GetPosition(), Quaternion.identity);
        SetupObject(obj, "live");
        SetupLerp(obj, true);
        activeObjects.Add(obj);
    }

    private void CreateStaticRing()
    {
        GameObject sRing1 = Instantiate(ringPrefab, new Vector3(-4f, 1.15f, -3f), Quaternion.identity);
        SetupObject(sRing1, "static");
        sRing1.transform.LookAt(GameObject.Find("headRed").transform);
        activeObjects.Add(sRing1);

        GameObject sRing2 = Instantiate(ringPrefab, new Vector3(-0.73f, 0.32f, -4.88f), Quaternion.identity);
        SetupObject(sRing2, "static");
        sRing2.transform.LookAt(GameObject.Find("headGreen").transform);
        activeObjects.Add(sRing2);

        GameObject sRing3 = Instantiate(ringPrefab, new Vector3(0.59f, 0.62f, -3.61f), Quaternion.identity);
        SetupObject(sRing3, "static");
        sRing3.transform.LookAt(GameObject.Find("headBlue").transform);
        activeObjects.Add(sRing3);

        GameObject sRing4 = Instantiate(ringPrefab, new Vector3(-2f, 0.76f, -5.38f), Quaternion.identity);
        SetupObject(sRing4, "static");
        sRing4.transform.LookAt(GameObject.Find("headYellow").transform);
        activeObjects.Add(sRing4);

        GameObject sRing5 = Instantiate(ringPrefab, new Vector3(-2.59f, 0.62f, -3.61f), Quaternion.identity);
        SetupObject(sRing5, "static");
        sRing5.transform.LookAt(GameObject.Find("headCyan").transform);
        activeObjects.Add(sRing5);

        //hardcoded for now
        for (int i = 0; i < 5; i++)
        {
            activeObjects[i].transform.LookAt(activeObjects[i + 5].transform);
        }
    }

    //commented-out code can be deleted (the comments are the original code)
    private void CreateHead()
    {
        //GameObject headRed = Instantiate(headPrefab, new Vector3(-2.6783f, 0.7615f, -2.7361f), Quaternion.identity);
        GameObject headRed = Instantiate(headPrefab, new Vector3(2.45f, 1.71f, -12.85f), Quaternion.identity);    
        headRed.name = "headRed";
        headRed.tag = "source";
        //headRed.transform.Rotate(0f, 250f, 0f);
        headRed.transform.Rotate(0f, 110f, 0f); //rotate -110 deg around y-axis
        activeObjects.Add(headRed);

        //GameObject headGreen = Instantiate(headPrefab, new Vector3(-2.539f, 1.548f, -3.049f), Quaternion.identity);
        GameObject headGreen = Instantiate(headPrefab, new Vector3(2.805f, 1.811f, -11.76f), Quaternion.identity);   
        headGreen.name = "headGreen";
        headGreen.tag = "source";
        //headGreen.transform.Rotate(10f, 90f, 30f);
        headGreen.transform.Rotate(10f, -90f, 30f); //change quaternion to 10 deg on x-axis, -90 deg. in y-axis, 30 deg on z-axis
        activeObjects.Add(headGreen);

        //GameObject headBlue = Instantiate(headPrefab, new Vector3(-3.721f, 0.763f, -2.742f), Quaternion.identity);
        GameObject headBlue = Instantiate(headPrefab, new Vector3(2.34f, 1.308f, -10.735f), Quaternion.identity);  
        headBlue.name = "headBlue";
        headBlue.tag = "source";
        headBlue.transform.Rotate(0f, 120f, 0f); //change to 120 deg around y-axis
        activeObjects.Add(headBlue);

        //GameObject headYellow = Instantiate(headPrefab, new Vector3(-2.0999f, 0.7661f, -2.7351f), Quaternion.identity);
        GameObject headYellow = Instantiate(headPrefab, new Vector3(1.45f, 1.93f, -9.96f), Quaternion.identity);     
        headYellow.name = "headYellow";
        headYellow.tag = "source";
        headYellow.transform.Rotate(0f, 180, 0f); //change to 180 deg around y-axis
        activeObjects.Add(headYellow);

        //GameObject headCyan = Instantiate(headPrefab, new Vector3(-0.7087f, 0.787f, -2.7543f), Quaternion.identity);
        GameObject headCyan = Instantiate(headPrefab, new Vector3(1.61f, 1.024f, -12.808f), Quaternion.identity);  
        headCyan.name = "headCyan";
        headCyan.tag = "source";
        //headCyan.transform.Rotate(0f, -120f, 0f); //rotate 0 on every axis
        headCyan.transform.Rotate(0f, 0f, 0f);
        activeObjects.Add(headCyan);
    }

    private void CreateObjectArrow()
    {
        var objects = GameObject.FindGameObjectsWithTag("source");
        foreach (var obj in objects)
        {
            GameObject arrow = Instantiate(objectArrowPrefab);
            arrow.transform.position = obj.transform.position;
            Quaternion rotate = obj.transform.rotation;
            rotate *= Quaternion.Euler(90, 90, 0);
            arrow.transform.rotation = rotate;
            SetupObject(arrow, "static");
            activeObjects.Add(arrow);
        }
    }

    private void CreateLine()
    {        
        GameObject obj = Instantiate(cursor);
        obj.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
        obj.GetComponent<Renderer>().material.color = new Color32(230, 25, 75, 255);
        obj.GetComponent<LineArrow>().viewCamera = viewCamera;
        obj.GetComponent<LineArrow>().maxCursorDistance = maxCursorDistance;
        obj.GetComponent<LineArrow>().sphereRadius = spherecastRadius;
        obj.GetComponent<LineArrow>().speedOfTrail = speedOfTrail;
        activeObjects.Add(obj);        
    }

    private void CreateSpheres()
    {
        foreach(Vector3 origin in origins)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.tag = "source";
            sphere.transform.position = origin;
            sphere.transform.localScale -= new Vector3(0.7f, 0.7f, 0.7f);
            SetupObject(sphere, "static");
            activeObjects.Add(sphere);
        }
    }

    private void SetSphereRotations()
    {
        activeObjects[0].transform.rotation = Quaternion.Euler(-90f, 0f, -90f);
        activeObjects[1].transform.rotation = Quaternion.Euler(90f, 0f, 90f);
        activeObjects[2].transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
        activeObjects[3].transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
        activeObjects[4].transform.rotation = Quaternion.Euler(-20f, -90.445f, 0f);
    }

    private void CreateCircles()
    {
        foreach (Vector3 origin in origins)
        {            
            GameObject circle = new GameObject();            
            circle.AddComponent<LineRenderer>();
            SetupObject(circle, "static");            
            LineRenderer outline = circle.GetComponent<LineRenderer>();

            Vector3 start = origin;
            int pointCount = 20;
            float radius = .05f;
            float lineThickness = .01f;
            outline.widthMultiplier = lineThickness;
            outline.positionCount = pointCount;
            float angle = 0f;
            float changeAngle = (2f * Mathf.PI) / pointCount;   //forgot "pointCount" at first
            outline.SetPosition(0, start);

            for (int j = 1; j < pointCount; j++)
            {
                //Vector3 position = new Vector3(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle), 0f);   //circle visible on the x-axis and y-axis
                Vector3 position = new Vector3(0f, radius * Mathf.Sin(angle), radius * Mathf.Cos(angle));   //circle visible on the y-axis and z-axis
                outline.SetPosition(j, start + position);
                start += position;
                angle += changeAngle;

            }
            outline.loop = true;
            activeObjects.Add(circle);
        }
    }

    private void CreatePointerCircles()
    {
        foreach(Vector3 location in locations)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.position = location;
            SetupObject(obj, "static");
            Color color = obj.GetComponent<Renderer>().material.color;
            color -= new Color(0.0f, 0.0f, 0.0f, .7f);
            obj.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Default"));
            obj.GetComponent<Renderer>().material.color = color;            
            obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.5f);
            activeObjects.Add(obj);
        }
    }

    private void CreateAdilLive()
    {
        GameObject obj = Instantiate(adil, GetPosition(), Quaternion.identity);
        SetupObject(obj, "live");
        activeObjects.Add(obj);        
    }

    private void CreateAdilData()
    {
        GameObject obj = Instantiate(adil);
        List<Quest> quests = new List<Quest>();
        string filePath = "Assets/Data/gazeData" + 4 + ".json";
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
            foreach (Quest q in loadedData.quests)
            {
                float x = q.x + 1;
                float y = q.y + 1;
                float z = q.z - 1;
                float t = q.t;                
                quests.Add(new Quest(x,y,z,t));
            }
        }
        SetupObject(obj, "data");
        obj.AddComponent<TrailFromData>();
        obj.GetComponent<TrailFromData>().SetQuests(quests);
        obj.GetComponent<TrailFromData>().SetSpeedofTrail(speedOfTrail - 25);
        obj.transform.position = new Vector3(quests[0].x, quests[0].y, quests[0].z);
        activeObjects.Add(obj);
    }

    private Vector3 GetPosition()
    {
        Vector3 position;
        if (Physics.Raycast(ray, out RaycastHit hit, maxCursorDistance))
        {
            position = hit.point;
        }
        else
        {
            position = ray.origin + ray.direction.normalized * maxCursorDistance;
        }
        return position;
    }

    private Material GetMaterial(GameObject obj)
    {       
        Material m;
        for(int i=0; i<matAvailable.Count; i++)
        {
            if(matAvailable[i] == true)
            {
                m = materials[i];
                matAvailable[i] = false;
                obj.GetComponent<ObjectInfo>().SetID(i);
                return m;
            }
        }
        m = new Material(Shader.Find("Standard"));
        m.SetFloat("_Mode", 2.0f);
        return m;
    }

    private void PrepDestroy(GameObject obj)
    {
        try
        {
            int id = obj.GetComponent<ObjectInfo>().GetID();
            matAvailable[id] = true;
        }
        catch(NullReferenceException e)
        {

        }
        Destroy(obj);
    }

    private void SetupObject(GameObject obj, string s)
    {
        obj.AddComponent<ObjectInfo>();
        obj.GetComponent<ObjectInfo>().SetCamera(viewCamera);
        obj.GetComponent<ObjectInfo>().SetMaxCursorDistance(maxCursorDistance);
        obj.GetComponent<ObjectInfo>().SetSpeedOfTrail(speedOfTrail);
        obj.GetComponent<ObjectInfo>().SetSphereRadius(spherecastRadius);
        obj.GetComponent<ObjectInfo>().SetLerpDistance(lerp);
        obj.GetComponent<ObjectInfo>().SetMethod(s);
        obj.GetComponent<Renderer>().material = GetMaterial(obj);
    }

    private void SetupLerp(GameObject obj, bool check)
    {
        obj.GetComponent<ObjectInfo>().SetLerp(check);
    }
}
