using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public abstract class XIVColoredDropdown<T> : XIVDropdown<T>
    {
        // Since TMP_Dropdown has its own custom inspector we cant see these properties directly
        // But we can edit them by going to debug mode in inspector which it is a bit tricky
        [SerializeField] Color normalColor;
        [SerializeField] Color selectedColor;
        [SerializeField] Color itemColor1;
        [SerializeField] Color itemColor2;
        int itemCount;

        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            var item = base.CreateItem(itemTemplate);
            var img = item.GetComponent<Image>();
            if (img == false) return item;

            img.color = itemCount % 2 == 0 ? itemColor1 : itemColor2;
            itemCount++;
            return item;
        }

        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            itemCount = 0;
            base.DestroyDropdownList(dropdownList);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (targetGraphic == false) targetGraphic = GetComponent<Image>();
            targetGraphic.color = selectedColor;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            targetGraphic.color = normalColor;
        }
    }
}