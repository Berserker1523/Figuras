using UnityEngine;

public class ObjectSpawnerSpawner1 : MonoBehaviour
{
    [SerializeField] private int instances;
    [SerializeField] private int threads;
    [SerializeField] private GameObject objectSpawner;
    private int bestBoxesSolution = int.MaxValue;
    private int finishedThreads;
    private int currentThread = 0;
    
    [SerializeField] private GameObject box;
    private float boxHeight;
    
    void Awake()
    {
        SetBoxDimsenions();
        InstantiateThreads();
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
        ObjectSpawner6 objectSpawnerInstance = Instantiate(objectSpawner, new Vector3(0, boxHeight * -currentThread, 0), Quaternion.identity).GetComponent<ObjectSpawner6>();
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

    private void OnFinish(GameObject gameobject, int boxCount)
    {
        finishedThreads++;

        if (boxCount <= bestBoxesSolution)
        {
            bestBoxesSolution = boxCount;
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
            Debug.LogWarning($"Best Solution: {bestBoxesSolution} Boxes");
        }
    }
}
