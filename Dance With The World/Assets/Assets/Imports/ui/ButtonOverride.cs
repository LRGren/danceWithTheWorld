using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//这个脚本用于自定义按钮行为，按钮被鼠标点击后按钮不会保持select的状态，而是会恢复到高亮状态
public class ButtonOverride : Button
{
    protected override void Awake() {
        base.Awake();
        // 点击后，如果指针仍在按钮上，则强制设置为高亮状态
        onClick.AddListener(() => {
            if (interactable) {
                DoStateTransition(SelectionState.Highlighted, false);
            }
        });
    }
    
    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if (interactable) {
            // 指针移入时设置为高亮状态
            DoStateTransition(SelectionState.Highlighted, true);
        }
    }
    
    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        if (interactable) {
            // 指针移出时设置为正常状态
            DoStateTransition(SelectionState.Normal, true);
        }
    }
}
