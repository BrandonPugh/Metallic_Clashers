using CustomInput;
using CustomInput.Information;

using Library;

using UnityEngine;

public abstract class SubManager<T> : MonoSingleton<T> where T : MonoBehaviour
{
    protected sealed override void OnAwake()
    {
        InputManager.self.onPress.AddListener(OnPress);
        InputManager.self.onRelease.AddListener(OnRelease);
        InputManager.self.onHold.AddListener(OnHold);

        InputManager.self.onBeginDrag.AddListener(OnBeginDrag);
        InputManager.self.onDrag.AddListener(OnDrag);
        InputManager.self.onEndDrag.AddListener(OnEndDrag);

        Init();
    }

    protected abstract void Init();

    protected virtual void OnPress(TouchInformation touchInfo)
    {
        //TODO: This will be the function that is called when a OnPress Event is called
    }
    protected virtual void OnRelease(TouchInformation touchInfo)
    {
        //TODO: This will be the function that is called when a OnRelease Event is called
    }
    protected virtual void OnHold(TouchInformation touchInfo)
    {
        //TODO: This will be the function that is called when a OnHold Event is called
    }

    protected virtual void OnBeginDrag(DragInformation dragInfo)
    {
        //TODO: This will be the function that is called when a OnSlide Event is called
    }
    protected virtual void OnDrag(DragInformation dragInfo)
    {
        //TODO: This will be the function that is called when a OnSlide Event is called
    }
    protected virtual void OnEndDrag(DragInformation dragInfo)
    {
        //TODO: This will be the function that is called when a OnSlide Event is called
    }
}
