using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Vector2Int {

    public int x;
    public int y;

    public Vector2Int (int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString() {
        return string.Format("X: {0}  Y: {1}", x, y);
    }

    public static Vector2Int operator + (Vector2Int a, Vector2Int b) {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    
}
