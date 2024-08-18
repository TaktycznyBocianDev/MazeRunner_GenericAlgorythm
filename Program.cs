using Raylib_cs;
using System.Numerics;

class Program
{
    static void Main()
    {
        // Initialize the Raylib window
        Raylib.InitWindow(800, 600, "Raylib Grid Example");

        // Set the camera mode to be free (you can move the camera around)
        Camera3D camera = new Camera3D();
        camera.Position = new Vector3(10.0f, 10.0f, 10.0f); // Camera position
        camera.Target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.Up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera up vector (rotation towards target)
        camera.FovY = 45.0f;                                // Camera field-of-view Y
        //camera.type = CameraType.CAMERA_PERSPECTIVE;        // Camera mode type

        //Raylib.SetCameraMode(camera, CameraMode.CAMERA_FREE); // Set a free camera mode

        // Main game loop
        while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
        {
            // Update the camera
            Raylib.UpdateCamera(ref camera, CameraMode.Free);

            // Start drawing
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            // Begin 3D mode drawing
            Raylib.BeginMode3D(camera);

            // Draw the grid - parameters are slices and spacing
            Raylib.DrawGrid(20, 1.0f);

            // End 3D mode drawing
            Raylib.EndMode3D();

            // Display some text
            Raylib.DrawText("Move the camera with arrow keys or WASD", 10, 10, 20, Color.DarkGray);

            // End drawing
            Raylib.EndDrawing();
        }

        // De-initialize the Raylib window
        Raylib.CloseWindow();
    }
}
