using Controller;
using Model;
using UnityEngine;
using View;

namespace Factory
{
    public interface IEquipmentModelFactory
    {
        IEquipmentModel Model { get; }
    }

    public class EquipmentModelFactory : IEquipmentModelFactory
    {
        public IEquipmentModel Model { get; }

        public EquipmentModelFactory()
        {
            Model = new EquipmentModel();
        }
    }

    public interface IEquipmentViewFactory
    {
        IEquipmentView View { get; }
    }

    public class EquipmentViewFactory : IEquipmentViewFactory
    {
        public IEquipmentView View { get; }

        public EquipmentViewFactory(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("Equipment");
            var instance = Object.Instantiate(prefab, parent);
            View = instance.GetComponent<IEquipmentView>();
        }
    }

    public interface IEquipmentControllerFactory
    {
        IEquipmentController Controller { get; }
    }

    public class EquipmentControllerFactory : IEquipmentControllerFactory
    {
        public IEquipmentController Controller { get; private set; }

        public EquipmentControllerFactory(IEquipmentModel model, IEquipmentView view)
        {
            Controller = new EquipmentController(model, view);
        }

        public EquipmentControllerFactory() : this(new EquipmentModel(), new EquipmentView())
        {
        }
    }
}