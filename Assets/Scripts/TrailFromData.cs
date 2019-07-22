using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrailFromData : MonoBehaviour
{
    private List<Quest> questData;
    private Quest currentQuest;
    private float startPoint = 0.0f;
    private float nextUpdate = 0.0f;
    private int speedOfTrail = 100;

    private void Awake()
    {
        questData = new List<Quest>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        currentQuest = new Quest(transform.position.x, transform.position.y, transform.position.z, 0);
    }

    private void OnEnable()
    {
        StartCoroutine("UpdateTrail");
    }

    // Update is called once per frame
    void Update()
    {           
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentQuest.x, currentQuest.y, currentQuest.z), Time.deltaTime);
    }

    private IEnumerator UpdateTrail()
    {
        while (true)
        {
            while(questData.Count > 1)
            {
                startPoint = nextUpdate;
                nextUpdate = questData[0].t;
                currentQuest = questData[0];
                questData.RemoveAt(0);
                yield return new WaitForSeconds(nextUpdate - startPoint);
            }
            yield return new WaitForSeconds(0);
        }
    }

    public void SetQuests(List<Quest> q)
    {
        questData = q;
    }

    public void SetSpeedofTrail(int speed)
    {
        speedOfTrail = speed;
    }
}
