using DefaultNamespace;
using UnityEngine.Events;

namespace UIController
{
    public static class UiMainPanelEventManager
    {
        public static readonly UnityEvent<GameStartState> OnMainMenuHidden = new ();
        public static readonly UnityEvent<GameEndState> OnMainMenuShown = new ();

        public static void HideMainMenu(GameStartState state)
        {
            OnMainMenuHidden.Invoke(state);
        }
        
        public static void ShowMainMenu(GameEndState state)
        {
            OnMainMenuShown.Invoke(state);
        }
    }
}