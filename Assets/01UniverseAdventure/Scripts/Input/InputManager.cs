using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputMapName inputMapName;
    ReactiveProperty<bool> _isJump = new ReactiveProperty<bool>();
    ReactiveProperty<bool> _isHipDrop = new ReactiveProperty<bool>();
    ReactiveProperty<Vector3> _directionInput = new ReactiveProperty<Vector3>();
    ReactiveProperty<bool> _isMenuOpenClose = new ReactiveProperty<bool>();
    ReactiveProperty<bool> _isDecided = new ReactiveProperty<bool>();

    
    //------------------プロパティ----------------------
    public IReadOnlyReactiveProperty<Vector3> DirectionInput => _directionInput;
    public IReadOnlyReactiveProperty<bool> JumpInput => _isJump;
    public IReadOnlyReactiveProperty<bool> IsHipDrop => _isHipDrop;
    public IReadOnlyReactiveProperty<bool> IsMenuOpenClose => _isMenuOpenClose;

    public IReadOnlyReactiveProperty<bool> IsDecided => _isDecided;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _isJump.Value = playerInput.currentActionMap[inputMapName.JumpInputMap].WasPressedThisFrame();
        _isHipDrop.Value = playerInput.currentActionMap[inputMapName.HipDropInputMapName].WasPressedThisFrame();

        _directionInput.Value = playerInput.currentActionMap[inputMapName.DirectionInputMap].ReadValue<Vector2>();
        _isMenuOpenClose.Value = playerInput.currentActionMap[inputMapName.PauseInputMapName].WasPressedThisFrame();
        _isDecided.Value = playerInput.currentActionMap[inputMapName.DecisionInputMapName].WasPressedThisFrame();
        
    }
}
