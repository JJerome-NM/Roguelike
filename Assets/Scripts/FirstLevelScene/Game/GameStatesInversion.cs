using System.Collections.Generic;
using DefaultNamespace;

namespace FirstLevelScene.Game
{
    public static class GameStatesInversion
    {
        private static readonly Dictionary<GameEndState, GameStartState> OppositeOfEndStates = new()
        {
            { GameEndState.Pause, GameStartState.Resume },
            { GameEndState.AllRunesWasFound, GameStartState.Start },
            { GameEndState.PlayerDied, GameStartState.Restart }
        };
        
                
        private static readonly Dictionary<GameStartState, GameEndState> OppositeOfStartStates = new()
        {
            { GameStartState.Resume, GameEndState.Pause },
            { GameStartState.Restart, GameEndState.PlayerDied },
            { GameStartState.Start, GameEndState.AllRunesWasFound }
        };
        
        public static GameStartState GetEndStateInversion(this GameEndState state)
        {
            return OppositeOfEndStates[state];
        }
        
        public static GameEndState GetStartStateInversion(this GameStartState state)
        {
            return OppositeOfStartStates[state];
        }
    }
}