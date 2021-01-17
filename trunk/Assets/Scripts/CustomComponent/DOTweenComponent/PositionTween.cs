using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;

namespace Creator
{
    public class PositionTween : TweenBase
    {
        [Tooltip("移动类型")]
        public MoveType type;
        [Tooltip("偏移量")]
        public Vector3 offset;
        [Tooltip("目标位置")]
        public Vector3 target;

        public override Tweener CreateHandler(float duration, Ease curve)
        {
            if (Handler != null) return Handler;
            var rect = GetComponent<RectTransform>();
            switch (type)
            {
                case MoveType.FromPositionToTarget:
                    DOMove(rect, target, duration);
                    break;
                case MoveType.FromTargetToPosition:
                    DOMove(rect, target, duration);
                    Handler.From();                    
                    break;
                case MoveType.OffsetToPosition:
                    target = transform.position;
                    transform.position += offset;
                    DOMove(rect, target, duration);
                    break;
                case MoveType.PositionToOffset:
                    target = transform.position + offset;
                    DOMove(rect, target, duration);
                    break;
                default: break;
            }
            return Handler;
        }

        private void DOMove(RectTransform rect, Vector3 target, float duration)
        {
            if (rect == null)
                Handler = transform.DOMove(target, duration);
            else
                Handler = rect.DOAnchorPos(target, duration);
        }

        [SerializeField]
        public enum MoveType
        {
            [Tooltip("从当前位置移动到目标位置")]
            FromPositionToTarget,   //从当前位置移动到目标位置
            [Tooltip("从目标位置移动到当前位置")]
            FromTargetToPosition,   //从目标位置移动到当前位置
            [Tooltip("从偏移位置移动到当前位置")]
            OffsetToPosition,       //从偏移位置移动到当前位置
            [Tooltip("从当前位置移动到偏移位置")]
            PositionToOffset,       //从当前位置移动到偏移位置
        }
    }
}