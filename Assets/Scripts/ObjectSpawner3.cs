using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner3 : MonoBehaviour
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
    private List<Rigidbody2D> currentBoxFiguresRigidBodies = new List<Rigidbody2D>();
    private int boxCounter = 0;
    private int currentAttemptsToCreateNewBox = 0;
    private const int attemptsToCreateNewBox = 50;

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
    private const float SpawnDelay = 0.01f;

    //private float spawnBoxDelayTimer = 2f;
    //private const float spawnBoxDelay = 0f;

    private float globalTimer = 0f;
    #endregion

    private const float deleteTolerance = 0.1f;

    private Rect debugRect = new Rect();


    // Awake is called before the first frame
    private void Awake()
    { 
        LoadGroups();
        CreateBox();
    }

    private void CreateBox()
    {
        foreach (Rigidbody2D rb2d in currentBoxFiguresRigidBodies)
        {
            rb2d.simulated = false;
        }

        currentBoxFiguresRigidBodies.Clear();

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
        Debug.Log(boxCounter);

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
            CheckCurrentGroupCompleted();
            InvokeGroup();
            spawnDelayTimer = 0;
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

    private Vector2 GetRandomPosition(float polygonExtentsX, float polygonExtentsY, float? fixedX = null, float? fixedY = null)
    {
        float spawnPositionX = 0;

        if (fixedX != null)
        {
            spawnPositionX = fixedX.Value;
        }
        else
        {
            spawnPositionX = UnityEngine.Random.Range(walls[0].position.x + walls[0].colliderExtents.x + polygonExtentsX * 2 + deleteTolerance,
                 walls[1].position.x - walls[1].colliderExtents.x - deleteTolerance);
        }

        float spawnPositionY = 0;

        if (fixedY != null)
        {
            spawnPositionY = fixedY.Value;
        }
        else if (currentAttemptsToCreateNewBox >= attemptsToCreateNewBox / 2)
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

        GameObject figure2Spawn = GetFigure2Spawn();

        GameObject instantiatedObject = Instantiate(figure2Spawn, transform);

        PolygonCollider2D polygonCollider = instantiatedObject.GetComponent<PolygonCollider2D>();
        float polygonExtentsX = polygonCollider.bounds.extents.x;
        float polygonExtentsY = polygonCollider.bounds.extents.y;

        Vector2 spawnPosition = GetRandomPosition(polygonExtentsX, polygonExtentsY);

        //Elige una posicion aleatoria dentro de la caja. Intenta introducir el objeto en cualquiera de esas posiciones sin que haga overlap (intenta 360 veces)
        float tests = 0;
        while (Physics2D.OverlapArea(new Vector2(spawnPosition.x - polygonExtentsX * 2, spawnPosition.y + polygonExtentsY), new Vector2(spawnPosition.x, spawnPosition.y - polygonExtentsY)) != null &&
            tests < 100)
        {
            tests++;
            spawnPosition = GetRandomPosition(polygonExtentsX, polygonExtentsY);
        }

        if (tests == 100)
        {
            Destroy(instantiatedObject);
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
            /*debugRect = new Rect(spawnPosition.x - polygonExtentsX * 2, 
                spawnPosition.y + polygonExtentsY, 
                spawnPosition.x - (spawnPosition.x - polygonExtentsX * 2), 
                spawnPosition.y - polygonExtentsY - (spawnPosition.y + polygonExtentsY));*/
            instantiatedObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            currentGroup[objects2Spawn.IndexOf(figure2Spawn)]--;
            instantiatedObject.transform.SetParent(currentBox.transform, true);
            currentBoxFiguresRigidBodies.Add(instantiatedObject.GetComponent<Rigidbody2D>());
        }
    }

    private void CheckCurrentGroupCompleted()
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
                Debug.Log($"timer: {globalTimer}");
                enabled = false;
            }
        }
    }

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

    /*
     * Debug
     */
    /*void OnDrawGizmos()
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
