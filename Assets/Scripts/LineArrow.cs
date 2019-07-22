using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Camera viewCamera;
    public int maxCursorDistance;
    public int speedOfTrail;
    public float sphereRadius;     

    private string terrainTag;
    private List<Vector3> positions;
    private List<GameObject> arrows;    
    private LineRenderer line;
    private int layerMask;    
    private Vector3 previousObject = new Vector3(0, 0, 0);
    private Vector3 currentObject;
    private GameObject terrain;
    private int terrainLayerIndex;
    private int ignoreRaycastIndex = 2;    
    private Material mat;

    private void Awake()
    {        
        terrain = GameObject.Find("AQUASWater");
        terrain.tag = "terrain";
        terrainLayerIndex = LayerMask.NameToLayer("Terrain"); 
    }

    void Start()
    {
        layerMask = (1 << terrainLayerIndex) | (1 << ignoreRaycastIndex);
        layerMask = ~layerMask;
        SetupLine();
        SetupArrow();
        terrain.layer = terrainLayerIndex;
        positions = new List<Vector3>();
        arrows = new List<GameObject>();
    }

    private void OnDisable()
    {
        foreach (GameObject obj in arrows)
        {
            Destroy(obj);
        }        
    }

    void Update()
    {
        Cursor();
        DrawLine();
    }

    private void Cursor()
    {
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out RaycastHit hit, maxCursorDistance))
        {            
            this.transform.position = Vector3.MoveTowards(this.transform.position, hit.point, speedOfTrail * Time.deltaTime);
            if (hit.collider.bounds.center != previousObject && !hit.collider.CompareTag("terrain"))
            {
                if (positions.Count > 10)
                {
                    positions.RemoveAt(0);
                    Destroy(arrows[0]);
                    arrows.RemoveAt(0);
                }
                positions.Add(hit.collider.bounds.center);
                DrawArrow(previousObject, hit.collider.bounds.center);
                previousObject = hit.collider.bounds.center;
                UpdateArrows();
            }
        }
        else
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, ray.origin + ray.direction.normalized * maxCursorDistance, speedOfTrail * Time.deltaTime);
        }
    }

    private void DrawLine()
    {
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
    }

    private void SetupLine()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.SetInt("_ZWrite", 0);
        line.startColor = new Color32(60, 180, 75, 50);
        line.endColor = new Color32(60, 180, 75, 255);
        line.startWidth = 0.1f;
        line.endWidth = 0.2f;
    }

    private void SetupArrow()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetFloat("_Mode", 0);
        mat.color = new Color32(230, 25, 75, 255);
        mat.SetInt("_ZWrite", 1);
        mat.renderQueue = 3000;
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        arrowPrefab.GetComponent<Renderer>().material = mat;
    }

    private void DrawArrow(Vector3 start, Vector3 end)
    {
        if (positions.Count >= 2)
        {
            Vector3 position = Vector3.Lerp(start, end, .5f);
            GameObject arrow = Instantiate(arrowPrefab, position, Quaternion.identity);
            arrow.transform.LookAt(end, transform.up);
            arrow.transform.Rotate(0, 0, -90);
            arrows.Add(arrow);
        }
    }

    private void UpdateArrows()
    {
        byte start = 255;
        for (int i = arrows.Count - 1; i >= 0; i--)
        {
            Color32 color = new Color32(230, 25, 75, start);
            arrows[i].GetComponent<Renderer>().material.color = color;
            start -= 25;
        }
    }
}