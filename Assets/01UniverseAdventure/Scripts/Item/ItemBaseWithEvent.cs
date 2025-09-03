using UnityEngine;
using UnityEngine.Events;

public class ItemBaseWithEvent<TOther> : ItemBase<TOther> where TOther : class
{
    UnityEvent touchEvent;

    public void SetTouchEvent(UnityEvent setEvent)
    {
        touchEvent = setEvent;
        DebugLog.Log($"‚«‚¿‚ñ‚Æ“o˜^‚µ‚Ü‚µ‚½ : {gameObject.name}");
    }
    protected override void GetItem(TOther getter)
    {
        touchEvent.Invoke();
    }

    
}
