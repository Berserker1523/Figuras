using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysicsOverlap : MonoBehaviour
{
    private Rect debugRect = new Rect();
    void OnDrawGizmos()
    {
        // Green
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        DrawRect(debugRect);
    }

    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x1 = -14.2f;
        float y1 = 0f;

        float x2 = -5.2f;
        float y2 = 3f;

        Collider2D a = Physics2D.OverlapArea(
            new Vector2(x1, y1),
            new Vector2(x2, y2));

        Debug.Log(a);

        debugRect = new Rect(
                x1,
                y1,
                x2 - x1,
                y2 - y1);
    }
}
