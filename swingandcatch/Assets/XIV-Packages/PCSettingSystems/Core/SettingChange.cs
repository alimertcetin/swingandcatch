using System;

namespace XIV_Packages.PCSettingSystems.Core
{
    public struct SettingChange
    {
        public int index;
        public Type settingType;
        public ISetting from;
        public ISetting to;

        public SettingChange reversed => new SettingChange(index, settingType, to, from);

        public SettingChange(int index, ISetting from, ISetting to) : this(index, from.GetType(), from, to)
        {

        }

        public SettingChange(int index, Type settingType, ISetting from, ISetting to)
        {
            this.index = index;
            this.settingType = settingType;
            this.from = from;
            this.to = to;
        }

        public void Reverse()
        {
            var temp = to;
            to = from;
            from = temp;
        }

        public override string ToString()
        {
            return $"{nameof(from)} : {from} - {nameof(to)} : {to}";
        }
    }
}