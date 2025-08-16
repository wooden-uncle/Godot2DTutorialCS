# Dodge the Creeps C#

![游戏截图](docs/images/game_screenshot.png)

## 项目简介

"Dodge the Creeps"是一个简单的2D游戏，玩家控制一个角色在屏幕上移动，目标是尽可能长时间地避开随机生成的敌人。本项目是使用Godot 4.x和C#实现的版本，基于官方GDScript版本的教程。

## 功能特点

- 简单直观的控制：使用方向键或WASD控制角色移动
- 随机生成的敌人：敌人从屏幕边缘随机生成，具有不同的外观和移动速度
- 计分系统：根据生存时间计算分数
- 动画效果：角色和敌人都有动画效果，增强游戏体验
- 音效：包含背景音乐和游戏结束音效

## 安装与运行

### 前提条件

- [Godot 4.x](https://godotengine.org/download) (C#版本)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download) 或更高版本

### 运行步骤

1. 克隆或下载本仓库
2. 使用Godot 4.x (C#版本) 打开项目
   - 可以通过Godot编辑器界面打开
   - 或使用命令行启动：
     ```
     路径\到\Godot_v4.x-stable_mono_win64_console.exe --path "项目路径" --verbose
     ```
3. 点击"运行"按钮或按F5启动游戏

### 环境配置

本项目使用以下环境配置进行开发和测试：

- **Godot版本**: 4.4.1-stable (Mono/C#版本)
- **目标框架**: .NET 8.0
- **操作系统**: Windows

详细的环境配置信息请参考[环境信息文档](docs/environment_info.md)。

## 项目结构

```
/
├── art/                  # 游戏美术资源
├── fonts/                # 字体资源
├── docs/                 # 项目文档
├── Player.cs             # 玩家角色脚本
├── Mob.cs                # 敌人脚本
├── Main.cs               # 主游戏场景脚本
├── HUD.cs                # 用户界面脚本
└── *.tscn                # 场景文件
```

## 文档

详细文档请参考`docs`目录下的相关文件。

## 游戏控制

- **方向键** 或 **WASD**: 移动角色
- **开始按钮**: 开始新游戏

## 开发笔记

本项目是将Godot官方的"Dodge the Creeps"教程从GDScript转换为C#的实现。主要目标是展示如何在保持相同游戏功能的前提下，使用C#替代GDScript进行Godot游戏开发。

### 重要注意事项

1. **修改C#代码后需要重新构建**：与GDScript不同，修改C#代码后必须先构建项目再运行，否则更改不会生效。

2. **场景属性初始化**：对于在场景编辑器中未设置的导出属性，建议在代码中添加动态加载逻辑，避免空引用异常。

3. **信号连接**：除了在场景编辑器中连接信号外，也可以在代码中手动连接，特别是当信号连接丢失时。



## 许可证

本项目采用MIT许可证 - 详情请参阅[LICENSE](LICENSE)文件

## 致谢

- Godot Engine团队提供的原始"Dodge the Creeps"教程
- 所有游戏资源来自Godot官方教程