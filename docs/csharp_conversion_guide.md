# GDScript到C#转换指南

## 概述

本文档记录了将Godot项目从GDScript转换为C#的经验和最佳实践，基于"Dodge the Creeps"游戏的转换过程。

## 基本语法差异

### 类声明

**GDScript:**
```gdscript
extends Area2D
```

**C#:**
```csharp
public partial class Player : Area2D
```

### 变量声明

**GDScript:**
```gdscript
@export var speed = 400
var screen_size
```

**C#:**
```csharp
[Export]
public int Speed { get; set; } = 400;
private Vector2 _screenSize;
```

### 信号(事件)声明

**GDScript:**
```gdscript
signal hit
```

**C#:**
```csharp
[Signal]
public delegate void HitEventHandler();
```

### 函数声明

**GDScript:**
```gdscript
func _ready():
    screen_size = get_viewport_rect().size
    hide()
```

**C#:**
```csharp
public override void _Ready()
{
    _screenSize = GetViewportRect().Size;
    Hide();
}
```

## 节点访问

### 获取节点

**GDScript:**
```gdscript
$AnimatedSprite2D.play()
```

**C#:**
```csharp
GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
```

### 信号发射

**GDScript:**
```gdscript
emit_signal("hit")
```

**C#:**
```csharp
EmitSignal(SignalName.Hit);
```

### 延迟设置属性

**GDScript:**
```gdscript
$CollisionShape2D.set_deferred("disabled", true)
```

**C#:**
```csharp
GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
```

## 异步操作

### 等待信号

**GDScript:**
```gdscript
await $MessageTimer.timeout
```

**C#:**
```csharp
// 使用事件订阅方式
GetNode<Timer>("MessageTimer").Timeout += () =>
{
    // 在这里处理超时后的逻辑
};
```

### 创建一次性定时器

**GDScript:**
```gdscript
await get_tree().create_timer(1.0).timeout
```

**C#:**
```csharp
var oneShot = GetTree().CreateTimer(1.0);
oneShot.Timeout += () =>
{
    // 在这里处理超时后的逻辑
};
```

## 随机数生成

**GDScript:**
```gdscript
randf()
randi() % n
randf_range(min, max)
```

**C#:**
```csharp
GD.Randf()
new Random().Next(0, n)
(float)GD.RandRange(min, max)
```

## 命名约定

### GDScript
- 变量和函数使用snake_case: `var player_speed`, `func game_over()`
- 常量使用大写加下划线: `const MAX_SPEED = 100`
- 类名使用PascalCase: `class_name PlayerController`

### C#
- 私有变量使用_camelCase: `private Vector2 _screenSize`
- 公共属性使用PascalCase: `public int Speed { get; set; }`
- 方法使用PascalCase: `public void GameOver()`
- 常量使用PascalCase: `private const string GameVersion = "1.0.0"`

## 项目结构差异

### GDScript项目
- 不需要显式的项目文件
- 脚本直接与场景关联

### C#项目
- 需要.csproj项目文件
- 需要正确设置命名空间
- 需要确保程序集名称与项目配置一致

## 常见问题和解决方案

### 命名空间问题

确保在C#脚本中使用正确的命名空间，并且命名空间与项目的程序集名称一致。例如：

```csharp
namespace DodgeTheCreepsCS
{
    public partial class Player : Area2D
    {
        // 类实现
    }
}
```

### 程序集名称问题

在project.godot文件中，确保`dotnet/project/assembly_name`设置与.csproj文件中的`<AssemblyName>`一致，并且不包含空格或特殊字符。

### 目标框架问题

确保.csproj文件中的目标框架与Godot版本兼容。例如，Godot 4.x需要.NET 6.0或更高版本：

```xml
<TargetFramework>net8.0</TargetFramework>
```

## 性能考虑

C#在Godot中的性能通常与GDScript相当或更好，但有一些注意事项：

1. C#代码需要编译，这可能导致更长的启动时间
2. C#的垃圾收集可能导致偶尔的性能波动
3. 对于性能关键的部分，考虑使用结构体而不是类来减少垃圾收集压力

## 结论

将Godot项目从GDScript转换为C#需要理解两种语言之间的语法和结构差异，但总体过程是直接的。C#提供了更强的类型安全和更好的IDE支持，这对于大型项目特别有价值。