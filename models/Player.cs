
using System.Numerics;
using Raylib_cs;

public class Player {
    public Vector2 pos = new();
    public Vector2 lookDir = new(0, 1);

    public Vector2 forward() {
        return lookDir;
    }
    public Vector2 right() {
        return Raymath.Vector2Rotate(lookDir, (float)(-Math.PI/2));
    }

    public void rotate(float rad) {
        lookDir = Raymath.Vector2Rotate(lookDir, rad);
    }

    public float getRad() {
        return (float)Math.Asin(lookDir.Y);
    }
}