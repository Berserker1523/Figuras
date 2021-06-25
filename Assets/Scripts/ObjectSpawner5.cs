using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner5 : MonoBehaviour
{
    private const string ConfigurationDataFileName = "data.csv";

    #region SpawnObjects
    [SerializeField] private List<GameObject> objects2Spawn;
    private List<List<int>> groupObjects2Spawn = new List<List<int>>();
    private List<int> currentGroup;
    private int currentGroupIndex = 0;
    #endregion


    #region walls
    [SerializeField] private GameObject box;
    private GameObject currentBox;
    private List<Figure> currentBoxFigures = new List<Figure>();
    private int boxCounter = 0;
    private int currentAttemptsToCreateNewBox = 0;
    private const int attemptsToCreateNewBox = 100;
    private const int maxTriesToSpawn = 100;
    
    // Clase que permite identificar las cordenadas X y Y de los 4 lados de la caja, al igual que la longitud de las parades.
    private class Wall
    {
        public Vector2 position;
        public Vector2 colliderExtents;

        public Wall(Vector3 position, BoxCollider2D collider)
        {
            //punto en X y Y
            this.position = new Vector2(position.x, position.y);
            //longitud
            this.colliderExtents = new Vector2(collider.bounds.extents.x, collider.bounds.extents.y);
        }
    }

    private Wall[] walls = new Wall[4];
    #endregion


    #region timers
    private float spawnDelayTimer = 0f;
    private const float SpawnDelay = 0.05f;

    //private float spawnBoxDelayTimer = 2f;
    //private const float spawnBoxDelay = 0f;

    private float globalTimer = 0f;
    #endregion

    private const float deleteTolerance = 0.1f;

    // Awake is called before the first frame
    private void Awake()
    { 
        LoadGroups();
        InstantiateFigures();
        CreateBox();
    }

    private void CreateBox()
    {
        foreach (Figure figure in currentBoxFigures)
        {
            figure.rb2d.simulated = false;
            figure.collider2D.enabled = false;
            //Destroy(figure.rb2d);
            //Destroy(figure.collider2D);
        }

        currentBoxFigures.Clear();

        if (currentBox == null)
        {
            currentBox = Instantiate(box, transform);
        }
        else
        {
            currentBox = Instantiate(box, new Vector3(2 * walls[1].position.x - walls[0].position.x, walls[0].position.y, 0), Quaternion.identity);
            currentBox.transform.parent = transform;
        }

        boxCounter++;
        //Debug.Log(boxCounter);

        int i = 0;
        foreach (Transform wallGameObject in currentBox.transform)
        {
            Wall wall = new Wall(wallGameObject.transform.position, wallGameObject.GetComponent<BoxCollider2D>());
            walls[i] = wall;
            i++;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        globalTimer += Time.deltaTime;
        spawnDelayTimer += Time.deltaTime;

        /*if (currentAttemptsToCreateNewBox >= attemptsToCreateNewBox / 2)
        {
            spawnBoxDelayTimer += Time.deltaTime;
        }*/

        if (spawnDelayTimer >= SpawnDelay)//&& spawnBoxDelayTimer >= spawnBoxDelay)
        {
            spawnDelayTimer = 0;
            ChangeToNextGroup();
            InvokeGroup();
        }
    }

    private GameObject GetFigure2Spawn()
    {
        for(int i = 0; i < objects2Spawn.Count; i++)
        {
            if(currentGroup[i] > 0)
            {
                return objects2Spawn[i];
            }
        }

        return null;
    }

    /*private int GetFigure2SpawnIndex()
    {
        for (int i = 0; i < objects2Spawn.Count; i++)
        {
            if (currentGroup[i] > 0)
            {
                return i;
            }
        }

        return -1;
    }*/

    private int GetFigure2SpawnIndex()
    {
        List<int> availableFigureKindIndexes = new List<int>();

        for (int i = 0; i < objects2Spawn.Count; i++)
        {
            if (currentGroup[i] > 0)
            {
                availableFigureKindIndexes.Add(i);
            }
        }

        if (availableFigureKindIndexes.Count == 0)
        {
            return -1;
        }

        if (availableFigureKindIndexes.Count == 1)
        {
            return availableFigureKindIndexes[0];
        }

        for (int i = 0; i < availableFigureKindIndexes.Count; i++)
        {
            int temp = availableFigureKindIndexes[i];
            int randomIndex = UnityEngine.Random.Range(i, availableFigureKindIndexes.Count);
            availableFigureKindIndexes[i] = availableFigureKindIndexes[randomIndex];
            availableFigureKindIndexes[randomIndex] = temp;
        }

        return availableFigureKindIndexes[0];
    }

    private Vector2 GetRandomPosition(float polygonExtentsX, float polygonExtentsY)
    {

        float spawnPositionX = UnityEngine.Random.Range(walls[0].position.x + walls[0].colliderExtents.x + polygonExtentsX + deleteTolerance,
                 walls[1].position.x - walls[1].colliderExtents.x - polygonExtentsX - deleteTolerance);

        float spawnPositionY = 0;


        if (currentAttemptsToCreateNewBox >= attemptsToCreateNewBox / 2)
        {
            spawnPositionY = UnityEngine.Random.Range(walls[2].position.y + 2 * (walls[3].position.y - walls[2].position.y) / 3 + polygonExtentsY + deleteTolerance,
                walls[3].position.y - walls[3].colliderExtents.y - polygonExtentsY - deleteTolerance);
        }
        else
        {
            spawnPositionY = UnityEngine.Random.Range(walls[2].position.y + walls[2].colliderExtents.y + polygonExtentsY + deleteTolerance,
                walls[3].position.y - walls[3].colliderExtents.y - polygonExtentsY - deleteTolerance);
        }

        return new Vector2(spawnPositionX, spawnPositionY);
    }

    private void InvokeGroup()
    {
        if (currentGroup == null)
        {
            return;
        }

        //GameObject figure2Spawn = GetFigure2Spawn();
        int figure2SpawnIndex = GetFigure2SpawnIndex();

        //GameObject instantiatedObject = Instantiate(figure2Spawn, transform);
        List<Figure> instantiatedObjectList = intantiatedObjects[currentGroupIndex][figure2SpawnIndex];
        Figure instantiatedFigure = instantiatedObjectList[0];
        GameObject instantiatedObject = instantiatedFigure.gameObject;
        instantiatedObject.SetActive(true);

        PolygonCollider2D polygonCollider = instantiatedObject.GetComponent<PolygonCollider2D>();
        float polygonExtentsX = polygonCollider.bounds.extents.x;
        float polygonExtentsY = polygonCollider.bounds.extents.y;


        Vector2 spawnPosition;
        float tests = 0;
        Collider2D d;
        
        do
        {
            /*contactFilter = new ContactFilter2D();
            contactFilter2 = new ContactFilter2D();
            allOverlappingColliders = new Collider2D[16];
            allOverlappingColliders2 = new Collider2D[16];
            allOverlappingColliders3 = new Collider2D[16];*/
            spawnPosition = GetRandomPosition(polygonExtentsX, polygonExtentsY);
            instantiatedObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            tests++;
            /*b = instantiatedObject.GetComponent<Collider2D>().OverlapCollider(contactFilter, allOverlappingColliders);
            Debug.Log($"b: {b}");
            a = Physics2D.OverlapAreaNonAlloc(
                new Vector2(spawnPosition.x - polygonExtentsX - deleteTolerance, spawnPosition.y - polygonExtentsY - deleteTolerance),
                new Vector2(spawnPosition.x + polygonExtentsY + deleteTolerance, spawnPosition.y + polygonExtentsY + deleteTolerance), allOverlappingColliders2);
            Debug.Log($"a: {a}");
            c = Physics2D.OverlapCollider(instantiatedFigure.collider2D, contactFilter2, allOverlappingColliders3);
            Debug.Log($"c: {c}");
            d = Physics2D.OverlapArea(
                new Vector2(spawnPosition.x - polygonExtentsX - deleteTolerance, spawnPosition.y - polygonExtentsY - deleteTolerance),
                new Vector2(spawnPosition.x + polygonExtentsY + deleteTolerance, spawnPosition.y + polygonExtentsY + deleteTolerance),
                LayerMask.NameToLayer("Default"), 
                float.MinValue, float.MaxValue);
            Debug.Log($"d: {d}");*/
            d = Physics2D.OverlapArea(
            new Vector2(spawnPosition.x - polygonExtentsX, spawnPosition.y + polygonExtentsY),
            new Vector2(spawnPosition.x + polygonExtentsX, spawnPosition.y - polygonExtentsY));
            //Debug.Log($"d: {d}");
        }
        while (d != null && tests < maxTriesToSpawn);

        if (tests == maxTriesToSpawn)
        {
            instantiatedObject.SetActive(false);
            currentAttemptsToCreateNewBox++;
            /*if(currentAttemptsToCreateNewBox == attemptsToCreateNewBox / 2)
            {
                spawnBoxDelayTimer = 0;
            }*/
            if(currentAttemptsToCreateNewBox == attemptsToCreateNewBox)
            {
                currentAttemptsToCreateNewBox = 0;
                CreateBox();
            }
        }
        else
        {
            //Debug.Log(spawnPosition);
            currentAttemptsToCreateNewBox = 0;
            /*debugRect = new Rect(
                spawnPosition.x - polygonExtentsX - deleteTolerance, 
                spawnPosition.y - polygonExtentsY - deleteTolerance, 
                2 * polygonExtentsX + deleteTolerance, 
                2 * polygonExtentsY + deleteTolerance);*/

            instantiatedObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            currentGroup[figure2SpawnIndex]--;
            instantiatedObjectList.Remove(instantiatedFigure);
            instantiatedObject.transform.SetParent(currentBox.transform, true);
            currentBoxFigures.Add(instantiatedFigure);
        }
    }

    private bool CheckCurrentGroupCompleted()
    {
        int availableFigures = 0;
        foreach (int figureSpawnCount in currentGroup)
        {
            availableFigures += figureSpawnCount;
        }

        return availableFigures == 0;
    }

    private void ChangeToNextGroup()
    {
        if (currentGroup == null)
        {
            return;
        }

        if (CheckCurrentGroupCompleted())
        {
            groupObjects2Spawn.Remove(currentGroup);
            if (groupObjects2Spawn.Count > 0)
            {
                currentGroup = groupObjects2Spawn[0];
                currentGroupIndex++;
            }
            else
            {
                currentGroup = null;
                Debug.Log($"boxes: {boxCounter}, timer: {globalTimer}");
                enabled = false;
            }
        }
    }

    /*private void CheckCurrentGroupCompleted()
    {
        if (currentGroup == null)
        {
            return;
        }

        int figuresCompleted = 0;
        foreach (int figureSpawnCount in currentGroup)
        {
            if (figureSpawnCount == 0)
            {
                figuresCompleted++;
            }
        }

        if(figuresCompleted == currentGroup.Count)
        {
            groupObjects2Spawn.Remove(currentGroup);
            if(groupObjects2Spawn.Count > 0)
            {
                currentGroup = groupObjects2Spawn[0];
                currentGroupIndex++;
            }
            else 
            {
                currentGroup = null;
                Debug.Log($"boxes: {boxCounter}, timer: {globalTimer}");
                enabled = false;
            }
        }
    }*/

    private void LoadGroups()
    {
        StreamReader file = null;
        try
        {
            file = File.OpenText(Path.Combine(Application.streamingAssetsPath, ConfigurationDataFileName));
            string currentLine = file.ReadLine();
            while (currentLine != null)
            {
                int[] group = Array.ConvertAll(currentLine.Split(','), int.Parse);
                groupObjects2Spawn.Add(new List<int>(group));
                currentLine = file.ReadLine();
            }

            currentGroup = groupObjects2Spawn[0];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    private struct Figure
    {
        public GameObject gameObject;
        public Rigidbody2D rb2d;
        public Collider2D collider2D;
    }

    private List<List<List<Figure>>> intantiatedObjects = new List<List<List<Figure>>>();

    private void InstantiateFigures()
    {
        int i = 0;
        foreach (List<int> group in groupObjects2Spawn)
        {
            intantiatedObjects.Add(new List<List<Figure>>());
            int j = 0;
            foreach(int numberObjects in group)
            {
                intantiatedObjects[i].Add(new List<Figure>());
                for (int k = 0; k < numberObjects; k++)
                {
                    Figure figure = new Figure();
                    figure.gameObject = Instantiate(objects2Spawn[j], transform);
                    figure.rb2d = figure.gameObject.GetComponent<Rigidbody2D>();
                    figure.collider2D = figure.gameObject.GetComponent<Collider2D>();
                    figure.gameObject.SetActive(false);
                    intantiatedObjects[i][j].Add(figure);
                }
                j++;
            }
            i++;
        }
    }

    /*
     * Debug
     */
    
    /*private Rect debugRect = new Rect(); 
    void OnDrawGizmos()
    {
        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        DrawRect(debugRect);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }*/
}
