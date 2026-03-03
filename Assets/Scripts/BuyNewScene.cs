using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyNewScene : MonoBehaviour 
{
    public SlotUpgradeScript _slotUpgradeScript;
    public CanvasGroup _canvasGroup;




    public void Active()
    {
        if (_slotUpgradeScript.level > 0)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }
        else
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    public void Disable()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
}
