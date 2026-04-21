using Raylib_cs;
using OpenTibia.Threading;
using System;
using OpenTibia.Client.Graphics;
using OpenTibia.Client.Graphics.Rendering;
using OpenTibia.Client.Input;
using System.Numerics;
using OpenTibia.FileFormats.Dat;
using OpenTibia.FileFormats.Otb;
using System.Linq;
using OpenTibia.Client.Network;
using OpenTibia.Network.Packets.Incoming.World;
using OpenTibia.Client.UI;

namespace OpenTibia.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize Raylib
            Raylib.InitWindow(800, 600, "OpenTibia Raylib Client");
            Raylib.SetTargetFPS(60);

            // OpenTibia Infrastructure
            var backgroundDispatcher = new Dispatcher(true); 
            backgroundDispatcher.Start();
            var mainDispatcher = new MainThreadDispatcher();

            var gameState = new LocalGameState();
            var textureCache = new TextureCache();
            
            string sprPath = Environment.GetEnvironmentVariable("OT_SPRITE_PATH") ?? "../OpenTibia.GameData/data/items/860/Tibia.spr";
            string datPath = Environment.GetEnvironmentVariable("OT_DAT_PATH") ?? "../OpenTibia.GameData/data/items/860/Tibia.dat";
            string otbPath = Environment.GetEnvironmentVariable("OT_OTB_PATH") ?? "../OpenTibia.GameData/data/items/860/items.otb";
            
            Console.WriteLine("Loading game assets...");
            
            var spriteLoader = new SpriteLoader(mainDispatcher, textureCache, sprPath);
            var datFile = DatFile.Load(datPath, 860);
            var otbFile = OtbFile.Load(otbPath);

            var cameraManager = new CameraManager();
            var server = new ClientServer(mainDispatcher, backgroundDispatcher, gameState);
            
            var worldRenderer = new WorldRenderer(gameState, spriteLoader, datFile);
            var inputManager = new InputManager(server, gameState);

            // Login UI
            var loginScreen = new LoginScreen();
            loginScreen.OnConnect += (host, port, account, password) =>
            {
                gameState.StatusMessage = $"Connecting to {host}:{port}...";
                // Fire and forget, connection runs in background tasks
                _ = server.Connect(host, port, account, password);
            };

            // Game loop
            while (!Raylib.WindowShouldClose())
            {
                try 
                {
                    mainDispatcher.ExecuteAll();

                    if (gameState.State == ClientState.Login)
                    {
                        loginScreen.Update();
                    }
                    else if (gameState.State == ClientState.InGame)
                    {
                        cameraManager.Update();
                        inputManager.Update();
                    }

                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Black);

                    if (gameState.State == ClientState.Login ||
                        gameState.State == ClientState.Connecting ||
                        gameState.State == ClientState.LoggingIn ||
                        gameState.State == ClientState.EnteringWorld)
                    {
                        loginScreen.Render();

                        // Status Bar
                        Raylib.DrawRectangle(0, 560, 800, 40, Raylib.Fade(Color.RayWhite, 0.8f));
                        Raylib.DrawText($"Status: {gameState.StatusMessage}", 10, 570, 16, Color.DarkGray);
                    }
                    else // InGame
                    {
                        Raylib.BeginMode2D(cameraManager.Camera2D);
                        worldRenderer.Render(cameraManager.Camera2D);
                        Raylib.EndMode2D();

                        // UI Overlay
                        Raylib.DrawRectangle(0, 0, 800, 40, Raylib.Fade(Color.RayWhite, 0.8f));
                        Raylib.DrawText($"Status: {gameState.StatusMessage}", 10, 10, 20, Color.Blue);
                        Raylib.DrawText("Arrows: Walk | WASD: Pan | Scroll: Zoom", 450, 10, 16, Color.DarkGray);
                    }

                    Raylib.EndDrawing();
                }
                catch (Exception ex)
                {
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.Red);
                    Raylib.DrawText("FATAL ERROR", 10, 10, 30, Color.White);
                    Raylib.DrawText(ex.Message, 10, 50, 20, Color.Yellow);
                    Raylib.EndDrawing();
                    Console.Error.WriteLine($"FATAL: {ex.Message}");
                }
            }

            textureCache.UnloadAll();
            backgroundDispatcher.Stop();
            server.Dispose();
            Raylib.CloseWindow();
        }
    }
}
