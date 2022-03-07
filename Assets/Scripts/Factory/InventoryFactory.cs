using Controller;
using Model;
using UnityEngine;
using View;

namespace Factory
{
    public interface IInventoryModelFactory
    {
        IInventoryModel Model { get; }
    }

    public class InventoryModelFactory : IInventoryModelFactory
    {
        public IInventoryModel Model { get; }

        public InventoryModelFactory()
        {
            Model = new InventoryModel();
        }
    }

    public interface IInventoryViewFactory
    {
        IInventoryView View { get; }
    }

    public class InventoryViewFactory : IInventoryViewFactory
    {
        public IInventoryView View { get; }

        public InventoryViewFactory(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("Inventory");
            var instance = Object.Instantiate(prefab, parent);
            View = instance.GetComponent<IInventoryView>();
        }
    }

    public interface IInventoryControllerFactory
    {
        IInventoryController Controller { get; }
    }

    public class InventoryControllerFactory : IInventoryControllerFactory
    {
        public IInventoryController Controller { get; private set; }

        public InventoryControllerFactory(IInventoryModel model, IInventoryView view)
        {
            Controller = new InventoryController(model, view);
        }

        public InventoryControllerFactory() : this(new InventoryModel(), new InventoryView())
        {
        }
    }
}