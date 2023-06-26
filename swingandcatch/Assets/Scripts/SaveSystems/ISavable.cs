namespace TheGame.SaveSystems
{
    public interface ISavable
    {
        object GetSaveData();
        void LoadSaveData(object data);
    }
}