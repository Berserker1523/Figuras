using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFigure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
        print(collider.bounds.extents);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
