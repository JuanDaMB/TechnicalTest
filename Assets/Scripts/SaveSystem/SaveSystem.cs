using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Values;

namespace SaveSystem
{
    public class SaveSystem 
    {
        public static SaveData ReadState()
        {
            string saveDataPath = Path.Combine(Application.persistentDataPath, "playerData.json");
            if (File.Exists(saveDataPath))
            {
                string dataString = File.ReadAllText(saveDataPath);
                SaveData data = JsonUtility.FromJson<SaveData>(dataString);
                return data;
            }

            return null;
        }
        public static void SaveState(SaveScriptableData oldData)
        {
            SaveData data = new SaveData
            {
                player = oldData.player,
                battle = oldData.battle,
                load = true,
                equipmentValues = new List<EquipmentValues>(),
                inventoryValues = new List<EquipmentValues>()
            };
            foreach (EquipmentValues values in oldData.equipmentValues)
            {
                data.equipmentValues.Add(values);
            }
            foreach (EquipmentValues values in oldData.inventoryValues)
            {
                data.inventoryValues.Add(values);
            }

            string savedata = JsonUtility.ToJson(data, true);
        
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;

            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(savedata), 0, Encoding.UTF8.GetByteCount(savedata));

            foreach (byte b in crypto)
            {
                hash += b.ToString("x2");
            }

            data.hasOfObjects = hash;

            string saveDataPath = Path.Combine(Application.persistentDataPath, "playerData.json");
            File.WriteAllText(saveDataPath, JsonUtility.ToJson(data,true));
        }
    }
}
