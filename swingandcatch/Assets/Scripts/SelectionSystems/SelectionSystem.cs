using TheGame.ScriptableObjects.Channels;
using UnityEngine;
using XIV.Core.Collections;
using XIV.Core.Utils;

namespace TheGame.SelectionSystems
{
    public class SelectionSystem : MonoBehaviour
    {
        struct SelectionData
        {
            public Timer timer;
            public ISelectable selectable;
            public Transform selectableTransform;
        }
        
        [SerializeField] TransformChannelSO selectableSelectChannel;
        [SerializeField] TransformChannelSO selectableDeselectChannel;
        DynamicArray<SelectionData> selectionDatas = new DynamicArray<SelectionData>();

        void OnEnable()
        {
            selectableSelectChannel.Register(HandleSelection);
            selectableDeselectChannel.Register(HandleDeselection);
        }

        void OnDisable()
        {
            selectableSelectChannel.Unregister(HandleSelection);
            selectableDeselectChannel.Unregister(HandleDeselection);
        }

        void Update()
        {
            for (int i = selectionDatas.Count - 1; i >= 0; i--)
            {
                ref var selectionData = ref selectionDatas[i];
                if (selectionData.timer.Update(Time.deltaTime))
                {
                    if (selectionData.selectableTransform) selectionData.selectable.OnDeselect();
                    selectionDatas.RemoveAt(i);
                }
            }
        }

        void HandleSelection(Transform selectableTransform)
        {
            if (selectableTransform.TryGetComponent(out ISelectable selectable) == false) return;
            int index = selectionDatas.Exists((selectionData) => selectionData.selectable == selectable);
            SelectionSettings settings = selectable.GetSelectionSettings();
            if (index != -1)
            {
                selectionDatas[index].timer.Restart(settings.duration);
                return;
            }

            selectable.OnSelect();
            selectionDatas.Add() = new SelectionData
            {
                timer = new Timer(settings.duration),
                selectable = selectable,
                selectableTransform = selectableTransform,
            };
        }

        void HandleDeselection(Transform selectableTransform)
        {
            if (selectableTransform.TryGetComponent(out ISelectable selectable) == false) return;
            int index = selectionDatas.Exists((selectionData) => selectionData.selectable == selectable);
            if (index != -1)
            {
                selectable.OnDeselect();
                selectionDatas.RemoveAt(index);
                return;
            }
        }
    }
}