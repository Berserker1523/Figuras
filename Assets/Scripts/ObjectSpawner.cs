using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    #region SpawnObjects
    [SerializeField] protected List<GameObject> objects2Spawn;
    protected List<GameObject> spawnedObjects = new List<GameObject>();
    protected bool hasDeletedAll = false;
    #endregion


    #region walls
    [SerializeField] protected GameObject[] wallsGameObjects = new GameObject[4];

    /// Clase que permite identificar las cordenadas X y Y de los 4 lados de la caja, al igual que la longitud de las parades.
    protected class Wall
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

    protected Wall[] walls = new Wall[4];
    #endregion


    #region timers
    protected float spawnDelayTimer = 0f;
    protected const float SpawnDelay = 0.001f;

    protected float runTimer = 0f;
    protected const float RunSeconds = 20f;
    #endregion

    protected const float deleteTolerance = 0.1f;


    // Start is called before the first frame update
    protected void Start()
    {
        SetWallsDimensions();
    }

    protected void SetWallsDimensions()
    {
        for (int i = 0; i < wallsGameObjects.Length; i++)
        {
            GameObject wallGameObject = wallsGameObjects[i];
            Wall wall = new Wall(wallGameObject.transform.position, wallGameObject.GetComponent<BoxCollider2D>());
            walls[i] = wall;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        spawnDelayTimer += Time.deltaTime;
        runTimer += Time.deltaTime;

        if (spawnDelayTimer >= SpawnDelay && runTimer <= RunSeconds)
        {
            InvokeFigure();
            spawnDelayTimer = 0;
        }

        /*if (!hasDeletedAll && runTimer >= RunSeconds)
        {
            DeleteOverlappedFigures();
        }*/

        //Debug.Log(runTimer);
    }

    virtual protected GameObject GetFigure2Spawn()
    {
        return objects2Spawn[Random.Range(0, objects2Spawn.Count)];
    }

    protected Vector2 GetRandomPosition(float polygonExtentsX, float polygonExtentsY)
    {
        float spawnPositionX = Random.Range(walls[0].position.x + walls[0].colliderExtents.x + polygonExtentsX * 2 + deleteTolerance,
           walls[1].position.x - walls[1].colliderExtents.x - deleteTolerance);

        float spawnPositionY = Random.Range(walls[2].position.y + walls[2].colliderExtents.y + deleteTolerance,
            walls[3].position.y - walls[3].colliderExtents.y - polygonExtentsY * 2 - deleteTolerance);

        return new Vector2(spawnPositionX, spawnPositionY);
    }

    virtual protected bool InvokeFigure()
    {
        GameObject figure2Spawn = GetFigure2Spawn();

        if (figure2Spawn == null)
        {
            return false;
        }

        GameObject instantiatedObject = Instantiate(figure2Spawn, Vector3.zero, Quaternion.identity);

        PolygonCollider2D polygonCollider = instantiatedObject.GetComponent<PolygonCollider2D>();
        float polygonExtentsX = polygonCollider.bounds.extents.x;
        float polygonExtentsY = polygonCollider.bounds.extents.y;


        Vector2 spawnPosition = GetRandomPosition(polygonExtentsX, polygonExtentsY);

        //Elige una posicion aleatoria dentro de la caja. Intenta introducir el objeto en cualquiera de esas posiciones sin que haga overlap (intenta 360 veces)
        float tests = 0;
        while (Physics2D.OverlapArea(new Vector2(spawnPosition.x - polygonExtentsX * 2, spawnPosition.y + polygonExtentsY * 2), spawnPosition) != null &&
            tests < 360)
        {
            tests++;
            spawnPosition = GetRandomPosition(polygonExtentsX, polygonExtentsY);
        }

        if (tests == 360)
        {
            //si hace overlap la elimina y trata de introducir la pieza otra vez durante el proximo milisegundo
            Destroy(instantiatedObject);
            return false;
        }
        else
        {
            //si no hace overlap la introduce
            instantiatedObject.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
            spawnedObjects.Add(instantiatedObject);
            return true;
        }
    }


    //hace la verificación de que no haya ninguna figura sobre las paredes. Pero el method ya no se usa.
    protected void DeleteOverlappedFigures()
    {
        foreach (GameObject polygon in spawnedObjects)
        {
            Collider2D polygonCollider = polygon.GetComponent<Collider2D>();
            float polygonExtentsX = polygonCollider.bounds.extents.x;
            float polygonExtentsY = polygonCollider.bounds.extents.y;

            if (polygon.transform.position.x - polygonExtentsX * 2 < walls[0].position.x + walls[0].colliderExtents.x - deleteTolerance ||
                polygon.transform.position.x > walls[1].position.x - walls[1].colliderExtents.x + deleteTolerance ||
                polygon.transform.position.y < walls[2].position.y + walls[2].colliderExtents.y - deleteTolerance ||
                polygon.transform.position.y + polygonExtentsY * 2 > walls[3].position.y - walls[3].colliderExtents.y + deleteTolerance)
            {
                Destroy(polygon);
                continue;
            }

            Rigidbody2D rb = polygon.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
        }
        hasDeletedAll = true;
    }
}