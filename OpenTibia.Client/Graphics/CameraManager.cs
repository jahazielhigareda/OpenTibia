using System.Numerics;
using Raylib_cs;

namespace OpenTibia.Client.Graphics
{
    public class CameraManager
    {
        public Camera2D Camera2D;

        public CameraManager()
        {
            Camera2D = new Camera2D
            {
                Target = new Vector2(0, 0),
                Offset = new Vector2(Raylib.GetScreenWidth() / 2.0f, Raylib.GetScreenHeight() / 2.0f),
                Rotation = 0.0f,
                Zoom = 1.0f
            };
        }

        public void Update()
        {
            // Basic panning for testing
            float speed = 200.0f * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.Left)) Camera2D.Target.X -= speed;
            if (Raylib.IsKeyDown(KeyboardKey.Right)) Camera2D.Target.X += speed;
            if (Raylib.IsKeyDown(KeyboardKey.Up)) Camera2D.Target.Y -= speed;
            if (Raylib.IsKeyDown(KeyboardKey.Down)) Camera2D.Target.Y += speed;

            // Basic zooming for testing
            float wheel = Raylib.GetMouseWheelMove();
            if (wheel != 0)
            {
                // Get the world point that is under the mouse
                Vector2 mouseWorldPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), Camera2D);

                // Set the offset to where the mouse is
                Camera2D.Offset = Raylib.GetMousePosition();

                // Set the target to match, so that the camera maps the target to the offset
                Camera2D.Target = mouseWorldPos;

                // Zoom control
                float scaleFactor = 1.0f + (0.25f * Math.Abs(wheel));
                if (wheel < 0) scaleFactor = 1.0f / scaleFactor;
                Camera2D.Zoom = Math.Clamp(Camera2D.Zoom * scaleFactor, 0.125f, 8.0f);
            }

            // Ensure offset stays centered if not zooming
            if (wheel == 0)
            {
                Camera2D.Offset = new Vector2(Raylib.GetScreenWidth() / 2.0f, Raylib.GetScreenHeight() / 2.0f);
            }
        }

        public Vector2 GetWorldToScreen(int tileX, int tileY)
        {
            return new Vector2(tileX * 32, tileY * 32);
        }
    }
}
