using DG.Tweening;
using Framework;
using UnityEngine;

namespace Creator
{
    public class TweenManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("缓动类型")]
        private TweenType[] type;
        [Tooltip("持续时间")]
        public float duration = 1;
        [Tooltip("运动曲线")]
        public Ease ease = Ease.Linear;
        [Tooltip("循环次数,-1为无限循环")]
        public int loopNum = 1;
        [Tooltip("循环模式")]
        public LoopType loopType = LoopType.Yoyo;
        [Tooltip("播放结束后的处理")]
        public PlayEndDo endAction = PlayEndDo.None;
        [Tooltip("对象池名称")]
        public string poolName;

        private Sequence sequence;

        public UnityPool Pool
        {
            get
            {
                if (m_Pool == null)
                    PoolMgr.Instance.TryGetPool(poolName, out m_Pool);
                return m_Pool;
            }
        }
        private UnityPool m_Pool;

        private void OnEnable()
        {
            if(sequence == null)
            {
                sequence = DOTween.Sequence();
                var tweens = GetComponentsInChildren<TweenBase>();
                foreach (var tween in tweens)
                {
                    var handler = tween.CreateHandler(duration, ease);
                    sequence.Insert(0, handler);
                }
                sequence.SetLoops(loopNum, loopType);
                sequence.onKill += OnTweenEnd;
                sequence.SetAutoKill(false);
            }
            else
            {
                sequence.Restart();
            }

        }

        void OnDisable()
        {
            //if (sequence != null)
            //{
            //    sequence.Kill(true);
            //    sequence = null;
            //}
        }

        void OnDestroy()
        {
            if (sequence != null)
            {
                sequence.Kill(true);
                sequence = null;
            }
        }

        void OnTweenEnd()
        {
            switch (endAction)
            {
                case PlayEndDo.Destroy:
                    Destroy(gameObject);
                    break;
                case PlayEndDo.Cache:
                    Pool.Put(gameObject);
                    break;
                default: break;
            }
        }

        public enum TweenType
        {
            PositionTween,
            RotationTween,
            ScaleTween,
            ColorTween,
        }

        [SerializeField]
        public enum PlayEndDo
        {
            [Tooltip("缓动播放完毕，销毁对象")]
            Destroy,
            [Tooltip("缓动播放完毕，缓存对象")]
            Cache,
            [Tooltip("不处理")]
            None
        }
    }
}