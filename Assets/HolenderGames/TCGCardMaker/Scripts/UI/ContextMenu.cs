using UnityEngine;
using UnityEngine.EventSystems;

namespace TCG_CardMaker
{
    public class ContextMenu : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject contextMenuView;

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowMenu();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideMenu();
        }

        public void SetContextMenu(GameObject contextMenu)
        {
            this.contextMenuView = contextMenu;
        }

        protected void HideMenu()
        {
            if (contextMenuView != null)
            {
                contextMenuView.SetActive(false);
            }
        }

        protected void ShowMenu()
        {
            if (contextMenuView != null)
            {
                contextMenuView.SetActive(true);
            }
        }

    }
}
