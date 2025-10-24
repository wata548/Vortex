/*using System.Collections.Generic;

namespace Wata.Extension.UI {
    
    public partial class InteractableButtonBase {
        
        private static readonly List<InteractableButtonBase> buttons = new();
        private static InteractableButtonBase selectedButton = null;
        private static InteractableButtonTag activeTag = InteractableButtonTag.None;

        public static void SetActiveTag(InteractableButtonTag tag) {

            if (selectedButton != null && selectedButton._tag != tag)
                selectedButton.Exit();
            
            activeTag = tag;
        }
            
        public static void OnChangeScene() {
            buttons.Clear();
            selectedButton = null;
        }
    }
}*/