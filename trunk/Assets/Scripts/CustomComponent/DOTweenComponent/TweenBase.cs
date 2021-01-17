using DG.Tweening;
using UnityEngine;

namespace Creator
{
    public abstract class TweenBase : MonoBehaviour
    {
        public Tweener Handler
        {
            get => m_handler;
            set => m_handler = value;
        }

        private Tweener m_handler;

        public abstract Tweener CreateHandler(float duration, Ease curve);
    }
}