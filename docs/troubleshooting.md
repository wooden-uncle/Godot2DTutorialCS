# 故障排除指南

## 概述

本文档提供了在使用Godot C#项目时可能遇到的常见问题及其解决方案，特别是针对"Dodge the Creeps"游戏的C#版本。

## 常见问题

### 1. C#脚本无法实例化

**症状**：
- 控制台显示错误："Cannot instance script..."或"Cannot instance C# script..."
- 场景无法正确加载
- 游戏对象不显示或不响应

**可能原因**：
- 命名空间问题
- 程序集名称不匹配
- 目标框架不兼容
- 类名与文件名不匹配

**解决方案**：

1. **检查命名空间**：
   - 确保所有C#脚本使用相同的命名空间
   - 确保命名空间语法正确（包括大括号）
   ```csharp
   namespace DodgeTheCreepsCS
   {
       public partial class Player : Area2D
       {
           // 类实现
       }
   }
   ```

2. **检查程序集名称**：
   - 在project.godot文件中，确保`dotnet/project/assembly_name`设置正确
   - 在.csproj文件中，确保`<AssemblyName>`与project.godot中的设置匹配
   - 避免在程序集名称中使用空格或特殊字符

3. **检查目标框架**：
   - 确保.csproj文件中的目标框架与Godot版本兼容
   - Godot 4.x需要.NET 6.0或更高版本
   ```xml
   <TargetFramework>net8.0</TargetFramework>
   ```

4. **检查类名与文件名**：
   - 确保C#类名与文件名完全匹配（区分大小写）
   - 例如，Player.cs文件应包含名为Player的类

5. **手动编译项目**：
   - 使用命令行手动编译项目，查看详细错误信息
   ```
   dotnet build
   ```

### 2. 信号连接问题

**症状**：
- 信号（事件）不触发
- 控制台显示错误："Cannot connect signal..."

**可能原因**：
- 信号名称不匹配
- 方法名称不匹配
- 命名空间问题

**解决方案**：

1. **检查信号名称**：
   - 在C#中，信号名称应使用SignalName枚举
   ```csharp
   EmitSignal(SignalName.Hit);
   ```

2. **检查方法名称**：
   - 确保回调方法名称与场景编辑器中连接的方法名称完全匹配
   - 方法名称区分大小写

3. **检查命名空间**：
   - 确保信号发送者和接收者在同一命名空间中，或使用完全限定名称

### 3. 信号重复连接问题

**症状**：
- 控制台显示错误："ERROR: Signal 'timeout' is already connected to given callable 'Delegate::Invoke' in that object."
- 游戏在某些情况下可能表现异常，如游戏结束后界面不正确显示

**可能原因**：
- 重复连接同一信号到同一方法
- 未正确断开之前的信号连接
- 在对象生命周期中多次调用连接代码

**解决方案**：

1. **使用临时变量存储事件处理程序**：
   ```csharp
   // 声明一个字段来存储事件处理程序
   private System.Action _messageTimerHandler;
   
   public void SomeMethod()
   {
       // 移除之前可能存在的事件处理程序
       var timer = GetNode<Timer>("Timer");
       if (_messageTimerHandler != null)
       {
           timer.Timeout -= _messageTimerHandler;
           _messageTimerHandler = null;
       }
       
       // 创建新的事件处理程序并连接
       _messageTimerHandler = () =>
       {
           // 处理逻辑
           
           // 完成后移除事件处理程序
           timer.Timeout -= _messageTimerHandler;
           _messageTimerHandler = null;
       };
       
       timer.Timeout += _messageTimerHandler;
   }
   ```

2. **使用ConnectOneShot方法（仅适用于GDScript）**：
   - 注意：此方法在C#中不可用，C#需使用上述方法

3. **检查连接逻辑**：
   - 确保在对象初始化时只连接一次信号
   - 在需要重复连接的场景中，先断开现有连接

### 4. 资源加载问题

**症状**：
- 控制台显示错误："Cannot load resource..."
- 游戏资源（图像、音频等）不显示

**可能原因**：
- 资源路径错误
- 资源导入问题
- 资源类型不匹配

**解决方案**：

1. **检查资源路径**：
   - 确保资源路径正确，使用相对路径或绝对路径
   - 在C#中，资源路径应使用正斜杠（/）

2. **重新导入资源**：
   - 在Godot编辑器中，选择资源并使用"重新导入"选项

3. **检查资源类型**：
   - 确保加载资源的类型与实际资源类型匹配
   ```csharp
   var texture = ResourceLoader.Load<Texture2D>("res://art/player.png");
   ```

### 4. 性能问题

**症状**：
- 游戏帧率低
- 游戏卡顿
- 内存使用高

**可能原因**：
- 垃圾收集问题
- 资源泄漏
- 代码效率低

**解决方案**：

1. **优化垃圾收集**：
   - 避免在每帧创建新对象
   - 使用对象池模式
   - 考虑使用结构体而不是类

2. **检查资源泄漏**：
   - 确保不再使用的资源被正确释放
   - 使用`QueueFree()`而不是`Free()`来销毁节点

3. **优化代码**：
   - 避免在`_Process`方法中执行昂贵的操作
   - 使用`_PhysicsProcess`处理物理相关的操作
   - 考虑使用信号而不是每帧检查

### 5. 编译错误

**症状**：
- 控制台显示编译错误
- 游戏无法启动

**可能原因**：
- 语法错误
- 引用错误
- 版本不兼容

**解决方案**：

1. **检查语法错误**：
   - 确保C#代码语法正确
   - 检查括号、分号等

2. **检查引用**：
   - 确保所有必要的引用都已添加
   - 检查命名空间导入

3. **检查版本兼容性**：
   - 确保使用的C#特性与目标框架兼容
   - 避免使用较新版本的C#特性，如果目标框架较旧

## 调试技巧

### 使用日志

在C#代码中添加日志输出，帮助定位问题：

```csharp
GD.Print("Debug: ", variableToDebug);
```

### 使用断点

在Godot编辑器中，可以设置断点并使用调试模式运行游戏：

1. 在代码编辑器中点击行号设置断点
2. 使用"调试"按钮运行游戏
3. 当执行到断点时，游戏将暂停，可以检查变量值

### 检查节点树

在运行时，使用Godot的远程调试功能检查节点树：

1. 运行游戏
2. 在Godot编辑器中，选择"调试器"面板
3. 选择"远程"选项卡
4. 连接到运行中的游戏
5. 检查节点树和属性

## 环境配置问题

**症状**：
- 项目无法启动
- 编译错误与环境相关
- 找不到Godot引擎或.NET SDK

**可能原因**：
- Godot引擎路径错误
- .NET SDK版本不兼容
- 项目路径包含特殊字符

**解决方案**：

1. **检查Godot引擎路径**：
   - 确保Godot引擎路径正确
   - 使用正确的Godot版本（Mono/C#版本）
   - 参考[环境信息文档](./environment_info.md)中的正确路径

2. **检查.NET SDK版本**：
   - 确保安装了与Godot版本兼容的.NET SDK
   - Godot 4.4.1需要.NET 8.0
   - 使用以下命令检查当前安装的.NET版本：
   ```
   dotnet --list-sdks
   ```

3. **使用正确的启动命令**：
   - 使用完整路径启动Godot
   - 添加`--path`参数指定项目路径
   - 添加`--verbose`参数获取详细日志
   - 参考[环境信息文档](./environment_info.md)中的启动命令

4. **检查项目路径**：
   - 避免在项目路径中使用空格或特殊字符
   - 确保路径不太长（Windows路径长度限制）

### 6. 代码修改后游戏无响应

**症状**：
- 修改C#代码后，游戏行为没有变化
- 点击按钮或触发事件没有响应
- 控制台没有显示预期的日志输出

**可能原因**：
- 修改C#代码后未重新构建项目
- 场景中的属性未正确设置或为空
- 信号连接丢失或未正确设置

**解决方案**：

1. **修改代码后重新构建项目**：
   - 在Godot C#项目中，修改代码后必须先构建项目，然后再运行游戏
   - 使用以下命令手动构建项目：
   ```
   dotnet build
   ```
   - 或者在Godot编辑器中使用构建选项

2. **检查场景属性**：
   - 确保所有导出的属性（使用`[Export]`标记的字段）在场景编辑器中已正确设置
   - 对于未在编辑器中设置的属性，可以在代码中添加动态加载逻辑：
   ```csharp
   // 示例：动态加载MobScene
   if (MobScene == null)
   {
       MobScene = GD.Load<PackedScene>("res://mob.tscn");
       GD.Print("已加载Mob场景");
   }
   ```

3. **添加调试日志**：
   - 在关键方法中添加`GD.Print()`语句，帮助跟踪代码执行流程
   - 特别是在信号处理方法中添加日志，确认信号是否被触发
   ```csharp
   public void _on_start_button_pressed()
   {
       GD.Print("开始按钮被点击");
       // 方法实现
   }
   ```

4. **检查信号连接**：
   - 确保所有必要的信号已正确连接
   - 可以在`_Ready()`方法中手动连接信号：
   ```csharp
   public override void _Ready()
   {
       var hud = GetNode<HUD>("HUD");
       hud.StartGame += NewGame;
       GD.Print("已连接HUD.StartGame信号到Main.NewGame方法");
   }
   ```

### 7. 空引用异常

**症状**：
- 控制台显示`System.NullReferenceException: Object reference not set to an instance of an object`错误
- 游戏在特定操作时崩溃

**可能原因**：
- 尝试访问未初始化的对象
- 场景中的导出属性未设置
- 节点路径错误

**解决方案**：

1. **添加空值检查**：
   - 在访问可能为空的对象前添加空值检查
   ```csharp
   if (someObject != null)
   {
       someObject.DoSomething();
   }
   ```

2. **动态加载资源**：
   - 对于场景中未设置的导出属性，添加动态加载逻辑
   - 例如，对于`MobScene`属性：
   ```csharp
   [Export] public PackedScene MobScene { get; set; }
   
   public override void _Ready()
   {
       // 如果MobScene未在编辑器中设置，则动态加载
       if (MobScene == null)
       {
           MobScene = GD.Load<PackedScene>("res://mob.tscn");
           GD.Print("已加载Mob场景");
       }
   }
   ```

3. **检查节点路径**：
   - 确保使用`GetNode()`获取节点时路径正确
   - 考虑使用`TryGetNode()`或添加路径存在性检查

## 结论

大多数Godot C#项目中的问题都与命名空间、程序集名称、资源路径或环境配置有关。通过仔细检查这些方面，可以解决大部分常见问题。特别注意，在修改C#代码后必须重新构建项目，这是与GDScript项目的主要区别之一。

对于空引用异常，添加适当的空值检查和动态资源加载逻辑可以大大提高代码的健壮性。

如果问题仍然存在，可以尝试创建一个最小可复现的示例，并在Godot论坛或GitHub上寻求帮助。

请参考[环境信息文档](./environment_info.md)获取本项目的具体环境配置信息，这对于故障排除非常有帮助。