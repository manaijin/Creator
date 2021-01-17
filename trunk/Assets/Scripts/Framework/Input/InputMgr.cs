using Framework.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static Framework.InputType;

namespace Framework
{
    /**
    √1.不同设备的输入事件注册、销毁
    √2.组合输入事件的注册、销毁
    √3.获取轴偏移量
    √4.双击事件
         */

    /// <summary>
    /// 输入事件管理模块
    /// 创建时间：2020年5月5日12:27:23
    /// </summary>
    public class InputMgr : Singleton<InputMgr>
    {
        public float DoublePressDuration { get => doublePressDuration; set => doublePressDuration = value; }



        private float doublePressDuration = 0.5f;

        private readonly Dictionary<InputCombination<Key>, List<int>> keyboardID;
        private readonly Dictionary<InputCombination<MouseButton>, List<int>> mouseID;
        private readonly Dictionary<int, Action> inputCallBack;
        private readonly Dictionary<ButtonControl, float> lastClickTime;

        public InputMgr()
        {
            keyboardID = new Dictionary<InputCombination<Key>, List<int>>();
            mouseID = new Dictionary<InputCombination<MouseButton>, List<int>>();
            inputCallBack = new Dictionary<int, Action>();
            lastClickTime = new Dictionary<ButtonControl, float>();
        }
        public int RegistKeyBoardCallBack(InputCombination<Key> key, Action ctr)
        {
            var id = GetInptID();
            if (!keyboardID.ContainsKey(key))
            {
                keyboardID.Add(key, new List<int>());
            }
            keyboardID[key].Add(id);
            inputCallBack.Add(id, ctr);
            return id;
        }

        public int RegistMouseCallBack(InputCombination<MouseButton> button, Action ctr)
        {
            var id = GetInptID();
            if (!mouseID.ContainsKey(button))
            {
                mouseID.Add(button, new List<int>());
            }
            mouseID[button].Add(id);
            inputCallBack.Add(id, ctr);
            return id;
        }

        public void UnregistKeyBoardCallBack(int id)
        {
            if (!UnregistCallBack(id)) return;

            InputCombination<Key> key;
            List<int> value;
            foreach (var pair in keyboardID)
            {
                key = pair.Key;
                value = pair.Value;
                if (value.Contains(id))
                {
                    value.Remove(id);
                    if (value.Count == 0)
                        keyboardID.Remove(key);
                    return;
                }
            }
            Util.Debug.LogError(string.Format("the id {0} is not exist in KeyBoardCallBack", id));
        }
        public void Update()
        {
            if (!Application.isFocused) return;
            UpdateKeyBoard();
            UpdateMouse();
        }

        public void UpdateKeyBoard()
        {
            InputCombination<Key> combination;
            List<int> callBackID;
            foreach (var input in keyboardID)
            {
                // 清除无效注册ID
                combination = input.Key;
                callBackID = input.Value;
                if (callBackID == null || callBackID.Count == 0)
                {
                    keyboardID.Remove(combination);
                    Util.Debug.LogError("keyboardID has null value");
                    continue;
                }

                // 检测键盘输入
                var flag = CheckKeyBoardInput(combination, callBackID);
                if (!flag) continue;


                // 执行回调方法
                ExcludeCallBack(callBackID);
            }
        }

        public void UpdateMouse()
        {
            InputCombination<MouseButton> combination;
            List<int> callBackID;
            foreach (var input in mouseID)
            {
                // 清除无效注册ID
                combination = input.Key;
                callBackID = input.Value;
                if (callBackID == null || callBackID.Count == 0)
                {
                    mouseID.Remove(combination);
                    Util.Debug.LogError("mouseID has null value");
                    continue;
                }

                // 检测键盘输入
                var flag = CheckMouseInput(combination, callBackID);
                if (!flag) continue;

                // 执行回调方法
                ExcludeCallBack(callBackID);
            }
        }

        private bool CheckKeyBoardInput(InputCombination<Key> combination, List<int> callBackID)
        {
            var flag = true;
            var length = combination.InputIndexes.Length;
            for (int i = 0; i < length; i++)
            {
                var key = combination.InputIndexes[i];
                if (!CheckButtonState((Keyboard.current[(Key)key]), combination.states[i]))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        private bool CheckMouseInput(InputCombination<MouseButton> combination, List<int> callBackID)
        {
            var flag = true;
            var length = combination.InputIndexes.Length;
            for (int i = 0; i < length; i++)
            {
                var key = combination.InputIndexes[i];
                string button = ((MouseButton)key).ToString();
                if (!CheckButtonState(((ButtonControl)UnityEngine.InputSystem.Mouse.current[button]), combination.states[i]))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        private void ExcludeCallBack(List<int> callBackID)
        {
            var length = callBackID.Count;
            int id;
            for (int i = length - 1; i >= 0; i--)
            {
                id = callBackID[i];
                if (inputCallBack.ContainsKey(id))
                {
                    inputCallBack[id]();
                }
                else
                {
                    callBackID.Remove(id);
                }
            }
        }



        public void UnregistMouseCallBack(int id)
        {
            if (!UnregistCallBack(id)) return;

            InputCombination<MouseButton> key;
            List<int> value;
            foreach (var pair in mouseID)
            {
                key = pair.Key;
                value = pair.Value;
                if (value.Contains(id))
                {
                    value.Remove(id);
                    if (value.Count == 0)
                        mouseID.Remove(key);
                    return;
                }
            }
            Util.Debug.LogError(string.Format("the id {0} is not exist in MouseCallBack", id));
        }

        private bool UnregistCallBack(int id)
        {
            if (!inputCallBack.ContainsKey(id))
            {
                Util.Debug.LogError(string.Format("the id {0} is not exist in inputCallBack", id));
                return false;
            }

            inputCallBack.Remove(id);
            return true;
        }

        /// <summary>
        /// 生成全局输入回调id
        /// </summary>
        /// <returns></returns>
        private int GetInptID()
        {
            var length = inputCallBack.Count;
            for (int i = length + 1; i >= 1; i++)
            {
                if (!inputCallBack.ContainsKey(i))
                {
                    return i;
                }
            }
            Util.Debug.LogError(string.Format("Failed to generate id"));
            return -1;
        }

        private bool CheckButtonState(ButtonControl ctrl, KeyState state)
        {
            switch (state)
            {
                case KeyState.isPressed:
                    return ctrl.isPressed;
                case KeyState.wasPressedThisFrame:
                    return ctrl.wasPressedThisFrame && !CheckDoublePressedButtonState(ctrl);
                case KeyState.wasReleasedThisFrame:
                    return ctrl.wasReleasedThisFrame;
                case KeyState.wasDoublePressedThisFrame:
                    if (!ctrl.wasPressedThisFrame)
                        return false;

                    if (!lastClickTime.ContainsKey(ctrl))
                    {
                        lastClickTime.Add(ctrl, Time.unscaledTime);
                        return false;
                    }

                    var result = CheckDoublePressedButtonState(ctrl);
                    lastClickTime[ctrl] = Time.unscaledTime;
                    return result;
                default: return false;
            }
        }

        private bool CheckDoublePressedButtonState(ButtonControl ctrl)
        {
            if (!lastClickTime.ContainsKey(ctrl))
                return false;
            return Time.unscaledTime - lastClickTime[ctrl] < doublePressDuration;
        }
    }
}
