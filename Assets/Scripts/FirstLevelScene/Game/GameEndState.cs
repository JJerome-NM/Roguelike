using System.Collections.Generic;

namespace FirstLevelScene.Game
{
    public enum GameEndState
    {
        Pause,
        PlayerDied,
        AllRunesWasFound
    }

    public static class StateToText
    {
        private static readonly string PlayerDeadText = "Player was died";
        private static readonly string AllRunesWasFoundText = "All runes have been found";
        private static readonly string PauseText = "Game on pause";

        private static readonly Dictionary<GameEndState, string> StateText = new()
        {
            { GameEndState.Pause, PauseText },
            { GameEndState.PlayerDied, PlayerDeadText },
            { GameEndState.AllRunesWasFound, AllRunesWasFoundText }
        };

        public static string ToText(this GameEndState state)
        {
            return StateText[state];
        }
    }

    public static class StateToStartButtonText
    {
        private static readonly string NextLevelText = "Next level";
        private static readonly string RestartText = "Try again";
        private static readonly string ResumeText = "Resume";

        private static readonly Dictionary<GameEndState, string> ButtonText = new()
        {
            { GameEndState.AllRunesWasFound, NextLevelText },
            { GameEndState.PlayerDied, RestartText },
            { GameEndState.Pause, ResumeText }
        };

        public static string ToStartButtonText(this GameEndState state)
        {
            return ButtonText[state];
        }
    }
}