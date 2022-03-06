using Controller;
using Model;
using UnityEngine;
using View;

namespace Factory
{
    public interface ICardModelFactory
    {
        ICardModel Model { get; }
    }

    public class CardModelFactory : ICardModelFactory
    {
        public ICardModel Model { get; }

        public CardModelFactory()
        {
            Model = new CardModel();
        }
    }

    public interface ICardViewFactory
    {
        ICardView View { get; }
    }

    public class CardViewFactory : ICardViewFactory
    {
        public ICardView View { get; }

        public CardViewFactory(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("Card");
            var instance = Object.Instantiate(prefab, parent);
            View = instance.GetComponent<ICardView>();
        }
    }

    public interface ICardControllerFactory
    {
        ICardController Controller { get; }
    }

    public class CardControllerFactory : ICardControllerFactory
    {
        public ICardController Controller { get; private set; }

        public CardControllerFactory(ICardModel model, ICardView view)
        {
            Controller = new CardController(model, view);
        }

        public CardControllerFactory() : this(new CardModel(), new CardView())
        {
        }
    }
}
