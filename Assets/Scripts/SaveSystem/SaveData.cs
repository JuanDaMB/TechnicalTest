using System;
using System.Collections.Generic;
using Values;

namespace SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public List<EquipmentValues> equipmentValues;
        public List<EquipmentValues> inventoryValues;
        public PlayerValues player;
        public int battle;
        public bool load;
        public string hasOfObjects;
    }
}
