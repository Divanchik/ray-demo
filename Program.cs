using System.Numerics;
using Raylib_cs;

class Program
{
    private const int WIDTH = 640;
    private const int HEIGHT = 480;
    private const float SPEED = 2f;
    private const float SENSITIVITY = 0.01f;
    private static bool mouseCaptured = false;
    private static readonly Vector2 sT = new Vector2(WIDTH /2, HEIGHT/2);
    private static readonly Vector2 sRS = new Vector2(20, -20);
    private static readonly float rayLen = 20f;
    private static readonly int rayCount = 64;
    private static readonly float fovRad = (float)Math.PI/2;
    private static Player player = new();
    private static Wall[] walls;

    public static void Main()
    {
        walls = new Wall[]{
            new Wall(-5, 2, -2, 5),
            new Wall(-2, 5, 2, 5),
            new Wall(2, 5, 3, 2)
        };
        Raylib.InitWindow(WIDTH, HEIGHT, "DOOM-like graphics");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            var delta = Raylib.GetFrameTime();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawRectangle(0, HEIGHT/2, WIDTH, HEIGHT/2, Color.DarkGray);
            // actual render
            RenderWalls();
            // visualization
            foreach (var wall in walls)
            {
                Raylib.DrawLineEx(Scr(wall.a), Scr(wall.b), 2, Color.Orange);
            }
            DrawPlayer();
            // input managing

            // move player
            Vector2 input = Raymath.Vector2Normalize(
                new Vector2(
                    Raylib.IsKeyDown(KeyboardKey.D) - Raylib.IsKeyDown(KeyboardKey.A),
                    Raylib.IsKeyDown(KeyboardKey.W) - Raylib.IsKeyDown(KeyboardKey.S)
                )
            );
            Vector2 move = Raymath.Vector2Normalize(player.forward() * input.Y + player.right() * input.X);
            player.pos += move * SPEED * delta;
            Raylib.DrawLineEx(new Vector2(30, 30), new Vector2(30, 30)+input*sRS, 2, Color.Magenta);

            // rotate player
            if (mouseCaptured)
            {
                var mouseMotion = Raylib.GetMouseDelta();
                var mousePos = Raylib.GetMousePosition();
                Raylib.SetMousePosition((int)(mousePos.X-mouseMotion.X), (int)(mousePos.Y-mouseMotion.Y));
                player.rotate(-mouseMotion.X * SENSITIVITY);
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Tab)) {
                mouseCaptured = !mouseCaptured;
                if (mouseCaptured)
                    Raylib.HideCursor();
                else
                    Raylib.ShowCursor();
            }
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }

    private static void RenderWalls()
    {
        float radDelta = fovRad / rayCount;
        var ray = Raymath.Vector2Rotate(player.lookDir*rayLen, (fovRad-radDelta)/2);
        for (int i=0;i<rayCount;i++) {
            float dist = float.PositiveInfinity;
            foreach (var wall in walls) {
                var cast = IntersectionPoint(player.pos, player.pos + ray, wall.a, wall.b);
                if (cast != null) {
                    var range = Raymath.Vector2Distance(player.pos, (Vector2)cast);
                    if (range < dist) dist = range;
                }
            }
            if (dist < rayLen) {
                var val = 1/(dist+1);
                var clr = new Color(val, val, val);
                int rectH = (int)(HEIGHT * val);
                int rectY = (HEIGHT - rectH) / 2;
                int w = WIDTH/rayCount;
                Raylib.DrawRectangle(i*w, rectY, w, rectH, clr);
            }
            ray = Raymath.Vector2Rotate(ray, -radDelta);
        }
    }

    private static Vector2? IntersectionPoint(Vector2 v1, Vector2 v2,Vector2 v3, Vector2 v4) {
        var dotProd = Raymath.Vector2DotProduct(
            Raymath.Vector2Normalize(Raymath.Vector2Subtract(v2, v1)), 
            Raymath.Vector2Normalize(Raymath.Vector2Subtract(v4, v3))
        );
        if (dotProd == 1) return null;
        float x1 = v1.X;
        float y1 = v1.Y;
        float x2 = v2.X;
        float y2 = v2.Y;
        float x3 = v3.X;
        float y3 = v3.Y;
        float x4 = v4.X;
        float y4 = v4.Y;

        float a = (x1-x3)*(y3-y4)-(y1-y3)*(x3-x4);
        float b = (x1-x2)*(y1-y3)-(y1-y2)*(x1-x3);
        float d = (x1-x2)*(y3-y4)-(y1-y2)*(x3-x4);
        var t = a/d;
        var u = -b/d;
        
        if (0 <= t && t <= 1 && 0 <= u && u <= 1 ) {
            return new Vector2(v1.X + t*(v2.X-v1.X), v1.Y + t*(v2.Y-v1.Y));
        }
        return null;
    }

    private static void DrawPlayer() {
        Raylib.DrawCircleV(Scr(player.pos), 5, Color.Red);
    }

    private static Vector2 Scr(Vector2 v)
    {
        return v * sRS + sT;
    }
}