using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UISystems.Components
{
    public class XIVDropDown : TMPro.TMP_Dropdown
    {
        // Since TMP_Dropdown has its own custom inspector we cant see these properties directly
        // But we can edit them by going to debug mode in inspector which it is a bit tricky
        [SerializeField] Color primaryColor;
        [SerializeField] Color secondaryColor;
        int itemCount;
        
        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            var item = base.CreateItem(itemTemplate);
            var img = item.GetComponent<Image>();
            if (img == false) return item;
            
            img.color = itemCount % 2 == 0 ? primaryColor : secondaryColor;
            itemCount++;
            return item;
        }

        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            itemCount = 0;
            base.DestroyDropdownList(dropdownList);
        }
    }
}