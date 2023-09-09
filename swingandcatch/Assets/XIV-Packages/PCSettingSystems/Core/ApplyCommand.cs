namespace XIV_Packages.PCSettingSystems.Core
{
    public abstract class ApplyCommand
    {
        public abstract void Apply(object value);
    }

    public abstract class ApplyCommand<T> : ApplyCommand
    {
        public abstract void Apply(T value);

        public override void Apply(object value)
        {
            Apply((T)value);
        }
    }
}