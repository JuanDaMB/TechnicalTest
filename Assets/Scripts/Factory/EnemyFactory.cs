using Controller;
using Model;
using UnityEngine;
using View;

namespace Factory
{
    public interface IEnemyModelFactory
    {
        IEnemyModel Model { get; }
    }

    public class EnemyModelFactory : IEnemyModelFactory
    {
        public IEnemyModel Model { get; }

        public EnemyModelFactory()
        {
            Model = new EnemyModel();
        }
    }

    public interface IEnemyViewFactory
    {
        IEnemyView View { get; }
    }

    public class EnemyViewFactory : IEnemyViewFactory
    {
        public IEnemyView View { get; }

        public EnemyViewFactory(string resource)
        {
            var prefab = Resources.Load<GameObject>(resource);
            var instance = Object.Instantiate(prefab);
            View = instance.GetComponent<IEnemyView>();
        }
    }

    public interface IEnemyControllerFactory
    {
        IEnemyController Controller { get; }
    }

    public class EnemyControllerFactory : IEnemyControllerFactory
    {
        public IEnemyController Controller { get; private set; }

        public EnemyControllerFactory(IEnemyModel model, IEnemyView view)
        {
            Controller = new EnemyController(model, view);
        }

        public EnemyControllerFactory() : this(new EnemyModel(), new EnemyView())
        {
        }
    }
}
