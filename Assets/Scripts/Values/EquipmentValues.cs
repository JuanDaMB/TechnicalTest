using System;
using UnityEngine;

namespace Values
{
    [CreateAssetMenu(menuName = "Dev/Equipment")][Serializable]
    public class EquipmentValues : ScriptableObject
    {
        public Sprite sprite;
        public int effectPotency;
        public int basePrice;
        public int condition;
        public string effect;
        public bool effectUsed;
        [Range(0f,1f)]public float discount;
        public ItemActivationCondition activationCondition;
        public Ubication itemUbication;
        public ItemEffect itemEffect;
    }
}
