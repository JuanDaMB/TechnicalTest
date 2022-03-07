using System;

namespace Model
{
    public interface IInventoryModel
    {
        event EventHandler OnGoldChanged;
        int Gold { get; set; }
    }
    public class InventoryModel: IInventoryModel
    {
        private int _gold;

        public event EventHandler OnGoldChanged;

        public int Gold
        {
            get => _gold;
            set
            {
                if (_gold == value) return;
                _gold = value;
                OnGoldChanged?.Invoke(this,EventArgs.Empty);
            }
        }
    }
}
