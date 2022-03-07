using System;
using System.Collections.Generic;
using UnityEngine;
using Values;

namespace SaveSystem
{
    [Serializable][CreateAssetMenu(menuName = "Dev/SaveData")]
    public class SaveScriptableData : ScriptableObject
    {
        public List<EquipmentValues> equipmentValues;
        public List<EquipmentValues> inventoryValues;
        public PlayerValues player;
        public int battle;
        public bool load;
    }
}
