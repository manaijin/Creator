using Framework;
using Framework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    class StartView : ViewBase
    {
        public UI_GameStart mainUI;

        public override IEnumerator OnEnter()
        {
            if (mainUI == null)
            {
                yield return CreatureUI<UI_GameStart>((ui) => { mainUI = ui; });
                mainUI.start.onClick.AddListener(()=> {
                    Destroy();
                    PoolMgr.Instance.ClearAll();
                });
                mainUI.hide.onClick.AddListener(Hide);
            }
            yield return true;
        }
    }
}
