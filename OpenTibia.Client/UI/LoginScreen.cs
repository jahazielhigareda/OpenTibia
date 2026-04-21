using Raylib_cs;
using System;

namespace OpenTibia.Client.UI
{
    public class LoginScreen
    {
        // Public properties reading from backing fields
        public string Host     => _host;
        public string Port     => _port;
        public string Account  => _account;
        public string Password => _password;

        private int _activeField = 0; // 0=Host, 1=Port, 2=Account, 3=Password
        private readonly string[] _labels = { "Server", "Port", "Account", "Password" };

        // Backing fields needed for ref return
        private string _host     = "127.0.0.1";
        private string _port     = "7171";
        private string _account  = "";
        private string _password = "";

        // Event fired when user clicks Connect
        public event Action<string, int, string, string> OnConnect;

        public void Update()
        {
            // Cycle fields with Tab
            if (Raylib.IsKeyPressed(KeyboardKey.Tab))
            {
                _activeField = (_activeField + 1) % 4;
                Console.WriteLine($"[UI] Tab pressed. Active field: {_labels[_activeField]}");
            }

            // Click on field to focus
            for (int i = 0; i < 4; i++)
            {
                var rect = GetFieldRect(i);
                if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                    Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), rect))
                {
                    _activeField = i;
                    Console.WriteLine($"[UI] Clicked on field: {_labels[i]}");
                }
            }

            // Text input
            int ch;
            while ((ch = Raylib.GetCharPressed()) > 0)
            {
                if (ch >= 32)
                {
                    ref string field = ref GetFieldRef(_activeField);
                    if (field.Length < 64)
                    {
                        char c = (char)ch;
                        field += c;
                        string logVal = _activeField == 3 ? "*" : c.ToString();
                        Console.WriteLine($"[UI] Text entered in {_labels[_activeField]}: {logVal}");
                    }
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                ref string field = ref GetFieldRef(_activeField);
                if (field.Length > 0)
                {
                    field = field[..^1];
                    Console.WriteLine($"[UI] Backspace pressed in {_labels[_activeField]}");
                }
            }

            // Enter key or Connect button
            bool connectPressed = Raylib.IsKeyPressed(KeyboardKey.Enter);
            var btnRect = new Rectangle(300, 410, 200, 40);
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) &&
                Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), btnRect))
            {
                connectPressed = true;
                Console.WriteLine("[UI] Connect button clicked");
            }

            if (connectPressed && !string.IsNullOrEmpty(Host) &&
                int.TryParse(Port, out int port))
            {
                Console.WriteLine($"[UI] TRIGGER CONNECT: Host={Host}, Port={port}, Account={Account}");
                OnConnect?.Invoke(Host, port, Account, Password);
            }
        }

        public void Render()
        {
            // Background panel
            Raylib.DrawRectangle(200, 150, 400, 320, new Color(30, 30, 30, 230));
            Raylib.DrawRectangleLines(200, 150, 400, 320, Color.Gold);
            Raylib.DrawText("OpenTibia Client — Login", 215, 160, 18, Color.Gold);

            for (int i = 0; i < 4; i++)
            {
                var rect = GetFieldRect(i);
                bool active = (_activeField == i);

                Raylib.DrawText(_labels[i], (int)rect.X, (int)rect.Y - 18, 14, Color.LightGray);
                Raylib.DrawRectangleRec(rect, active ? new Color(50, 50, 70, 255) : new Color(40, 40, 40, 255));
                Raylib.DrawRectangleLinesEx(rect, 1, active ? Color.Gold : Color.DarkGray);

                string display = i == 3 ? new string('*', Password.Length) : GetField(i);

                Raylib.DrawText(display + (active ? "|" : ""), (int)rect.X + 6, (int)rect.Y + 7, 16, Color.White);
            }

            // Connect Button
            var btnRect = new Rectangle(300, 410, 200, 40);
            bool hover = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), btnRect);
            Raylib.DrawRectangleRec(btnRect, hover ? Color.DarkGreen : Color.Green);
            Raylib.DrawText("CONNECT", 360, 422, 18, Color.White);
        }

        private Rectangle GetFieldRect(int i) => new Rectangle(220, 210 + i * 50, 360, 32);

        private string GetField(int i) => i switch
        {
            0 => Host,
            1 => Port,
            2 => Account,
            _ => Password
        };

        private ref string GetFieldRef(int i)
        {
            switch (i)
            {
                case 0: return ref _host;
                case 1: return ref _port;
                case 2: return ref _account;
                default: return ref _password;
            }
        }
    }
}
