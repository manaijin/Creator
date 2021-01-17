using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework.Util
{
    class UIUtil
    {
        public static void SetRectFullSize(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 0);
        }
    }
}
