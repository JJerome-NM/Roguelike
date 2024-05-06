using UnityEngine.Events;

namespace UIController.Inventory
{
    public static class UiInventoryEventManager
    {
        public static readonly UnityEvent OnInventoryHidden = new ();
        public static readonly UnityEvent OnInventoryShown = new ();

        public static void HideInventory()
        {
            OnInventoryHidden.Invoke();
        } 
        
        public static void ShowInventory()
        {
            OnInventoryShown.Invoke();
        } 
    }
}