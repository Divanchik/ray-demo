using System.Numerics;

public struct Wall {
    public Vector2 a;
    public Vector2 b;

    public Wall(float x1, float y1, float x2, float y2)
    {
        a = new Vector2(x1, y1);
        b = new Vector2(x2, y2);
    }
}