using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner2 : ObjectSpawner
{
    #region SpawnObjects
    [SerializeField] protected List<int> nObjects2Spawn;
    protected int currentSpawnedObjectIndex;
    #endregion


    override protected GameObject GetFigure2Spawn()
    {
        int attempts = 0;
        GameObject randomObject = null;
        int randomIndex = -1;

        while (randomObject == null)
        {
            //escojo un numero aleatorio
            randomIndex = Random.Range(0, objects2Spawn.Count);
            randomObject = objects2Spawn[randomIndex];

            //con el numero aleatorio tomo una figura aleatoria de la lista siempre y cuando la cantidad de objetos restantes por ubicar de cada tipo sea diferente de 0
            if (nObjects2Spawn[randomIndex] <= 0)
            {
                randomObject = null;
                //actualizo el numero de intentos para que no haga un loop infinito
                attempts++;
                if (attempts >= nObjects2Spawn.Count)
                {
                    return null;
                }
            }
        }
        currentSpawnedObjectIndex = randomIndex;
        return randomObject;
    }

    override protected bool InvokeFigure()
    {
        bool created = base.InvokeFigure();

        if (created)
        {
            nObjects2Spawn[currentSpawnedObjectIndex]--;
            currentSpawnedObjectIndex = -1;
        }

        return created;
    }
}
