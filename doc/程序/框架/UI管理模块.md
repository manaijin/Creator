# UI管理模块

## 一、总体设计

&emsp;&emsp;UI模块用于管理所有UI对象以及相关渲染。UI模块主要分为层级(UILayer)、视图（View）以及UIBase。
&emsp;&emsp;层级用于存储视图，不同层级可能会有不同特性，比如视图独占、显示在最前方、屏蔽前层的相关设置等。
&emsp;&emsp;视图是UI的集合，并处理UI的具体逻辑。当视图的内容过多，需要将视图细分为多个子视图
&emsp;&emsp;UIBase处理UI Prefab，用于代码解析UI。此部分代码可以通过工具，自动生成。

![avatar](/程序/图片/UI管理模块.png)

## 二、UIBase

### 2.1 需求与设计

&emsp;&emsp;UIBase处理具体的UI Prefab，其中涉及UI的解析、脚本自动生成、自定义组件、回调绑定等问题。

* UI解析
&emsp;&emsp;需要生成UI的Prefab,定义UI中需要使用的节点变量。

* 自定义组件
&emsp;&emsp;所有的节点都通过前后缀标记，自定义组件同样可以通过UI的前后缀进行映射。当检测到特定标记节点后，调用已有的UIBase进行解析。</br>

* UI回调绑定
&emsp;&emsp;UI与用户交互有很多回调响应，这部分回调的使用比较麻烦，考虑将各组件统一封装。

### 2.2 对外接口

``` csharp
/// <summary>
/// 创建UI
/// <summary>
/// <param name="cb">回调</param>
/// <param name="parent">父节点</param>
public void CreatUI(Action<GameObject> cb = null, Transform parent = null)
```

## 三、视图

### 3.1 需求与设计

&emsp;&emsp;视图中具体实现UI的交互逻辑。视图有自身的生命周期方法，在特定情况下触发。视图应该有相应的UI创建、销毁、显示隐藏视图等功能。

### 3.2 对外接口

``` csharp
// 进入Layer时
public abstract IEnumerable OnEnter()

// 离开Layer
public virtual void OnExit(){}

// 暂停UI功能
public virtual void OnPause(){}

// 恢复UI功能
public virtual void OnResume(){}

/// <summary>
/// 创建UI
/// <summary>
/// <param name="cb">回调</param>
/// <param name="parent">UI父节点,默认为View</param>
public virtual IEnumerator CreatureUI<T>(Action<T> cb, RectTransform parent = null) where T : UIBase, new(){}

// 销毁
public virtual void Destroy(){}

// 显示
public virtual void Show(){}

// 隐藏
public virtual void Hide(){}
```

## 四、层级

### 4.1 需求与设计

&emsp;&emsp;层级后续可能会拓展更很多参数，为了方便使用，将所有参数封装为LayerParam。在创建层级时，可以只设置一部分参数，避免过多的构造函数编写。
&emsp;&emsp;层级可以添加不同的视图，层级之间有不同的渲染顺序。相应的，层级还应该可以移除视图。

### 4.2 对外接口

``` csharp
public struct LayerParam
{
    public string Name;
    public int Order;
    public RectTransform Layer;
}

/// <summary>
/// 新增视图
/// <summary>
/// <param name="ui">视图</param>
public IEnumerable PushView(ViewBase ui){}

/// <summary>
/// 移除视图
/// <summary>
/// <param name="ui">视图</param>
public void PopView(ViewBase ui){}
```
