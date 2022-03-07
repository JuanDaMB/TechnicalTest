using System;
using Model;
using UnityEngine;
using View;

namespace Controller
{
    public interface IInventoryController
    {
        int GetCurrentGold();
        void SetCurrentGold(int e);
    }
    public class InventoryController : IInventoryController
    {
        private readonly IInventoryModel _model;
        private readonly IInventoryView _view;
        
        
        public InventoryController(IInventoryModel model, IInventoryView view)
        {
            _model = model;
            _view = view;
 
            model.OnGoldChanged+= OnGoldChanged;
        }

        private void OnGoldChanged(object sender, EventArgs e)
        {
            _view.Gold = _model.Gold;
        }

        public int GetCurrentGold()
        {
            return _model.Gold;
        }

        public void SetCurrentGold(int e)
        {
            _model.Gold = e;
        }
    }
}
