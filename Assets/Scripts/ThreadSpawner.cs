using UnityEngine;
using System.Collections.Generic;

public class ThreadSpawner : MonoBehaviour
{
    [SerializeField] private int instances;
    [SerializeField] private int threads;
    [SerializeField] private GameObject objectSpawner;
    private int bestBoxesSolution = int.MaxValue;
    private int finishedThreads;
    private int currentThread;
    private List<GameObject> bestThreads;
    
    [SerializeField] private GameObject box;
    private float boxHeight;

    private float globalTimer = 0f;

    void Awake()
    {
        bestThreads = new List<GameObject>();
        SetBoxDimsenions();
        InstantiateThreads();
    }

    private void Update()
    {
        globalTimer += Time.deltaTime;
    }

    private void SetBoxDimsenions()
    {
        float boxBottomPosition = 0;
        

        GameObject boxTemp = Instantiate(box);
        int k = 0;
        foreach (Transform wallGameObject in boxTemp.transform)
        {
            if (k == 2)
            {
                boxBottomPosition = wallGameObject.transform.position.y;
            }

            if (k == 3)
            {
                boxHeight = wallGameObject.transform.position.y - boxBottomPosition;
                boxHeight = boxHeight * 1.2f;
            }

            k++;
        }

        Destroy(boxTemp);
    }

    private void InstantiateThread()
    {
        ObjectSpawner objectSpawnerInstance = Instantiate(objectSpawner, new Vector3(0, boxHeight * -currentThread, 0), Quaternion.identity).GetComponent<ObjectSpawner>();
        objectSpawnerInstance.onFinishedFunction = OnFinish;
        currentThread++;
    }

    private void InstantiateThreads()
    {
        for (int i = 0; i < threads; i++)
        {

            InstantiateThread();
        }
    }

    private void DeleteThreads()
    {
        foreach(GameObject thread in bestThreads)
        {
            Destroy(thread);
        }

        bestThreads.Clear();
    }

    private void OnFinish(GameObject gameobject, int boxCount)
    {
        finishedThreads++;

        if (boxCount < bestBoxesSolution)
        {
            bestBoxesSolution = boxCount;
            DeleteThreads();
            bestThreads.Add(gameobject);
        }
        else if(boxCount == bestBoxesSolution)
        {
            bestThreads.Add(gameobject);
        }
        else
        {
            Destroy(gameobject);
        }

        if(currentThread < instances)
        {
            InstantiateThread();
        }

        if(finishedThreads == instances)
        {
            Debug.LogWarning($"Best Solution: {bestBoxesSolution} Boxes, Total Time: {globalTimer}");
        }
    }
}
