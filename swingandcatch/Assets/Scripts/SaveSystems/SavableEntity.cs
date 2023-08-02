using System;
using System.Collections.Generic;
using UnityEngine;
using XIV.Core;

namespace TheGame.SaveSystems
{
    [DisallowMultipleComponent]
    public class SavableEntity : MonoBehaviour
    {
        [DisplayWithoutEdit] public string guid;
        
        ISavable[] savables;
        readonly Dictionary<string, object> saveDatas = new Dictionary<string, object>();

        void Awake()
        {
            savables = GetComponentsInChildren<ISavable>();
            SaveSystem.AddSavableEntity(gameObject.scene.name, this);
        }

        void OnDestroy()
        {
            SaveSystem.RemoveSavableEntity(gameObject.scene.name, this);
        }

        public object GetSaveData()
        {
            saveDatas.Clear();
            int length = savables.Length;
            for (int i = 0; i < length; i++)
            {
                var savable = savables[i];
                saveDatas.Add(savable.GetType().ToString(), savable.GetSaveData());
            }
            return saveDatas;
        }

        public void LoadSaveData(object data)
        {
            var saveDatas = (Dictionary<string, object>)data;

            for (int i = 0; i < savables.Length; i++)
            {
                ISavable savable = savables[i];
                string typeName = savable.GetType().ToString();
                if (saveDatas.TryGetValue(typeName, out object value))
                {
                    savable.LoadSaveData(value);
                }
            }
        }

#if UNITY_EDITOR
        
        void Reset() => GenerateGuid();
        
        [Button]
        void GenerateGuid()
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, nameof(GenerateGuid));
            guid = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
        
    }
    
}