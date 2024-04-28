using System.Collections.Generic;

namespace DefaultNamespace
{
    public enum GameStartState
    {
        Start,
        Restart,
        Resume
    }
    
    public static class GameStartStateText
    {
        private static readonly string RestartText = "Try again";
        private static readonly string ResumeText = "Resume";
        private static readonly string StartText = "Start";

        private static readonly Dictionary<GameStartState, string> ButtonText = new()
        {
            { GameStartState.Restart, RestartText },
            { GameStartState.Resume, ResumeText },
            { GameStartState.Start, StartText }
        };

        public static string ToButtonText(this GameStartState state)
        {
            return ButtonText[state];
        }
    }
}