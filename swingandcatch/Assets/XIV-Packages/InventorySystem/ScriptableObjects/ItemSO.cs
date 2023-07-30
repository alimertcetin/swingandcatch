using UnityEngine;

namespace XIV.InventorySystem.ScriptableObjects
{
    public abstract class ItemSO : ScriptableObject
    {
#if UNITY_EDITOR
        [ContextMenu(nameof(GenerateID))]
        void GenerateID()
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Generate ID");
            GetItem().GenerateID();
        }
#endif

        public abstract ItemBase GetItem();
    }
    
    public class ItemSO<T> : ItemSO where T : ItemBase, new()
    {
        [SerializeField] T item;
     
        public override ItemBase GetItem()
        {
            return new T();
        }
    }
}