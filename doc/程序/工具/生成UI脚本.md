# UI代码自动化生成

## 一、需求

&emsp;&emsp;游戏中存在大量UI需求逻辑，使用UI的prefab就需要对其节点进行定义。大批量并且频繁得修改prefab对代码的修改会比较麻烦，如果能够将UI的定义类通过自动导出代码的方式生成，将极大加快开发效率。

## 二、设计思路

&emsp;&emsp;首先需要对UI进行标记，确定哪些节点需要输出。通过节点名称前后缀标记的方式，来标记UI节点。
&emsp;&emsp;然后需要确定脚本的输出路径。根据UI Prefab的相对路径，输出到脚本根节点的相对路径。
&emsp;&emsp;最后开始生成脚本内容。脚本内容由两部分组成：模板和节点数据。不同脚本的有着基本相同的模板，因此考虑配置模板文件，然后将标记的节点数据写入模板中。根据前后缀的标记，可以确定节点组件类型。

## 三、操作

1. 设置参数：点击Tools > UI > UI Code Generator
2. 生成代码：右击Prefba， 弹出生成UI代码按钮
3. 修改映射：param.type

``` csharp
static void InitData()
{
    param = new UICodeParam();
    param.types = new List<Type>()
    {
        typeof(Text),
        typeof(Button),
        typeof(Image),
        typeof(Dropdown),
        typeof(InputField),
        typeof(Transform), // 新增
    };
    ...
}
```
