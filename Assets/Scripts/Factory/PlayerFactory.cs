using Controller;
using Model;
using UnityEngine;
using View;

namespace Factory
{
    public interface IPlayerModelFactory
    {
        IPlayerModel Model { get; }
    }

    public class PlayerModelFactory : IPlayerModelFactory
    {
        public IPlayerModel Model { get; }

        public PlayerModelFactory()
        {
            Model = new PlayerModel();
        }
    }

    public interface IPlayerViewFactory
    {
        IPlayerView View { get; }
    }

    public class PlayerViewFactory : IPlayerViewFactory
    {
        public IPlayerView View { get; }

        public PlayerViewFactory()
        {
            var prefab = Resources.Load<GameObject>("Player");
            var instance = Object.Instantiate(prefab);
            View = instance.GetComponent<IPlayerView>();
        }
    }

    public interface IPlayerControllerFactory
    {
        IPlayerController Controller { get; }
    }

    public class PlayerControllerFactory : IPlayerControllerFactory
    {
        public IPlayerController Controller { get; private set; }

        public PlayerControllerFactory(IPlayerModel model, IPlayerView view)
        {
            Controller = new PlayerController(model, view);
        }

        public PlayerControllerFactory() : this(new PlayerModel(), new PlayerView())
        {
        }
    }
}
