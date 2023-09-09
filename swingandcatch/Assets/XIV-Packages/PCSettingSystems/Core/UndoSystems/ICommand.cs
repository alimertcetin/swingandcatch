namespace XIV_Packages.PCSettingSystems.Core.UndoSystems
{
    public interface ICommand
    {
        void Execute();

        void Unexecute();
    }
}