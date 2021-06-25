using UnityEngine;

public class ObjectSpawnerSpawner : MonoBehaviour
{
    [SerializeField] private int instances;
    [SerializeField] private GameObject objectSpawner;
    [SerializeField] private GameObject box;
    float boxHeight;

    void Awake()
    {
        float boxBottomPosition = 0;
        GameObject boxTemp = Instantiate(box);
        int k = 0;
        foreach (Transform wallGameObject in boxTemp.transform)
        {
            if(k == 2)
            {
                boxBottomPosition = wallGameObject.transform.position.y;
            }

            if(k == 3)
            {
                boxHeight = wallGameObject.transform.position.y - boxBottomPosition;
                boxHeight = boxHeight * 1.2f;
            }

            k++;
        }

        Destroy(boxTemp);

        for(int i = 0; i < instances; i++)
        {
            Instantiate(objectSpawner, new Vector3(0, boxHeight * -i, 0), Quaternion.identity);
        }

    }
}
