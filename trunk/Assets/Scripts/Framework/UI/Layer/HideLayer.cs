using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.UI
{
    class HideLayer : LayerBase
    {
        public HideLayer(LayerParam param) : base(param)
        {
            if (Node != null)
            {
                Node.gameObject.SetActive(false);
            }
        }
    }
}
