namespace OpenTibia.Client
{
    public enum ClientState
    {
        Login,        // Showing login screen
        Connecting,   // TCP connect in progress
        LoggingIn,    // Waiting for server response (0x0A/0x17)
        EnteringWorld,// Received 0x17 (pending), waiting for 0x0F
        InGame        // Game active
    }
}
