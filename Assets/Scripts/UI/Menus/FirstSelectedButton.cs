using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstSelectedButton : MonoBehaviour
{
    [SerializeField] private Button firstSelectedButton;

    public void FocusMenu()
    {
        if (firstSelectedButton == null) return;

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        
        firstSelectedButton.Select();
    }

    public void FocusCreditsMenu(Button thisSelectedButton)
    {
        if (thisSelectedButton == null) return;

        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(thisSelectedButton.gameObject);
        
        thisSelectedButton.Select();
    }
}
