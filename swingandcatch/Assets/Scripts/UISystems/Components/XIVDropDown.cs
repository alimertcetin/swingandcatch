using System;
using System.Collections.Generic;

namespace TheGame.UISystems.Components
{
    [System.Serializable]
    public struct DropdownOptionData<T>
    {
        public string displayText;
        public T value;

        public DropdownOptionData(string displayText, T value)
        {
            this.displayText = displayText;
            this.value = value;
        }
    }

    public abstract class XIVDropdown<T> : TMPro.TMP_Dropdown
    {
        List<DropdownOptionData<T>> dropdownOptionDatas = new List<DropdownOptionData<T>>();

        [System.Obsolete("Use AddOption, RemoveOption methods instead", true)]
        public new List<OptionData> options => throw new NotSupportedException();

        public void SetSelectedIndexForData(T data)
        {
            base.value = IndexOf(data);
        }

        public void SetSelectedIndexForDataWithoutNotify(T data)
        {
            int index = IndexOf(data);
            SetValueWithoutNotify(index);
        }

        public new void ClearOptions()
        {
            base.ClearOptions();
            dropdownOptionDatas.Clear();
        }

        public int IndexOf(T data)
        {
            for (int i = 0; i < dropdownOptionDatas.Count; i++)
            {
                if (dropdownOptionDatas[i].value.Equals(data))
                {
                    return i;
                }
            }

            return -1;
        }

        public DropdownOptionData<T> GetData(int index)
        {
            return dropdownOptionDatas[index];
        }

        public void BindData(int optionDataIndex, string displayText, T data)
        {
            dropdownOptionDatas[optionDataIndex] = new DropdownOptionData<T>(displayText, data);
        }

        public void AddOption(DropdownOptionData<T> data, bool refreshImmediate = false)
        {
            base.options.Add(new OptionData(data.displayText));
            dropdownOptionDatas.Add(data);
            if (refreshImmediate) base.RefreshShownValue();
        }

        public new void AddOptions(List<OptionData> optionDatas)
        {
            throw new NotSupportedException("Use AddOptions(IList<DropdownOptionData<T>)) instead");
        }

        public void AddOptions(IList<DropdownOptionData<T>> optionDatas)
        {
            int count = optionDatas.Count;
            for (int i = 0; i < count; i++)
            {
                base.options.Add(new OptionData(optionDatas[i].displayText));
            }
            dropdownOptionDatas.AddRange(optionDatas);
            base.RefreshShownValue();
        }

        void RemoveOption(T data)
        {
            int index = IndexOf(data);
            base.options.RemoveAt(index);
            dropdownOptionDatas.RemoveAt(index);
            base.RefreshShownValue();
        }
    }
}