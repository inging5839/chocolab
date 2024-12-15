using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class DragAndDropManipulator : PointerManipulator
{
    // 드래그 앤 드롭 생성자
    public DragAndDropManipulator(VisualElement target)
    {
        // 타겟 인스턴스 변수 초기화
        this.target = target;
        // 루트 변수 초기화
        root = target.parent;
    }

//
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    // 타겟 시작 위치
    private Vector2 targetStartPosition { get; set; }
    // 포인터 시작 위치
    private Vector3 pointerStartPosition { get; set; }
    // 활성 여부
    private bool enabled { get; set; }

    // 루트 
    private VisualElement root { get; }

    // 포인터 다운 이벤트 핸들러
    private void PointerDownHandler(PointerDownEvent evt)
    {
        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        enabled = true;
    }

    private void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            Vector3 pointerDelta = evt.position - pointerStartPosition;

            target.transform.position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, -root.worldBound.width, root.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, -root.worldBound.height, root.worldBound.height));
        }
    }

    // target.panel.visualTree.worldBound.width
    // 포인터 업 이벤트 핸들러
    private void PointerUpHandler(PointerUpEvent evt)
    {
        // 활성 여부 확인 && 포인터 캡처 확인
        if (enabled && target.HasPointerCapture(evt.pointerId))
        {
            // 포인터 해제
            target.ReleasePointer(evt.pointerId);
        }
    }

    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        if (enabled)
        {
            VisualElement slotsContainer = root.Q<VisualElement>("slots");
            UQueryBuilder<VisualElement> allSlots =
                slotsContainer.Query<VisualElement>(className: "slot");
            UQueryBuilder<VisualElement> overlappingSlots =
                allSlots.Where(OverlapsTarget);
            VisualElement closestOverlappingSlot =
                FindClosestSlot(overlappingSlots);
            Vector3 closestPos = Vector3.zero;
            if (closestOverlappingSlot != null)
            {
                closestPos = RootSpaceOfSlot(closestOverlappingSlot);
                closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5);
            }

            // 가장 가까운 슬롯이 없으면 시작위치로 이동
            target.transform.position =
                closestOverlappingSlot != null ?
                closestPos :
                targetStartPosition;

            enabled = false;
        }
    }

    // 슬롯이 타겟과 겹치는지 확인
    private bool OverlapsTarget(VisualElement slot)
    {
        return target.worldBound.Overlaps(slot.worldBound);
    }

    // 가까운 슬롯 찾기
    private VisualElement FindClosestSlot(UQueryBuilder<VisualElement> slots)
    {
        // 슬롯 리스트 가져오기
        List<VisualElement> slotsList = slots.ToList();
        // 가장 가까운 슬롯 거리 초기화
        float bestDistanceSq = float.MaxValue;
        // 가장 가까운 슬롯 초기화
        VisualElement closest = null;
        // 슬롯 리스트 반복
        foreach (VisualElement slot in slotsList)
        {
            // 슬롯 위치 계산
            Vector3 displacement =
                RootSpaceOfSlot(slot) - target.transform.position;
            // 슬롯 거리 계산
            float distanceSq = displacement.sqrMagnitude;
            // 가장 가까운 슬롯 확인
            if (distanceSq < bestDistanceSq)
            {
                // 가장 가까운 슬롯 거리 업데이트
                bestDistanceSq = distanceSq;
                // 가장 가까운 슬롯 업데이트
                closest = slot;
            }
        }
        return closest;
    }

    // 슬롯 루트 공간 계산
    private Vector3 RootSpaceOfSlot(VisualElement slot)
    {
        // 슬롯 부모 공간 계산
        Vector2 slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
        // 루트 공간 계산
        return root.WorldToLocal(slotWorldSpace);
    }
}