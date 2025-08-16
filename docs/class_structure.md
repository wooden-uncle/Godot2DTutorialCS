# 类结构文档

## 概述

本文档详细说明了"Dodge the Creeps"游戏C#版本中的各个类的结构、功能和实现细节。

## Player 类

`Player`类表示玩家控制的角色，继承自`Area2D`。

### 属性

| 属性名 | 类型 | 描述 |
|--------|------|------|
| Speed | int | 玩家移动速度（像素/秒），可在编辑器中导出 |
| _screenSize | Vector2 | 游戏窗口大小，用于限制玩家移动范围 |

### 信号

| 信号名 | 参数 | 描述 |
|--------|------|------|
| Hit | 无 | 当玩家被敌人击中时触发 |

### 方法

#### Start(Vector2 pos)

初始化玩家位置并显示玩家。

```csharp
public void Start(Vector2 pos)
{
    Position = pos;
    Show();
    GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
}
```

#### _Ready()

当节点进入场景树时调用，初始化屏幕大小并隐藏玩家。

```csharp
public override void _Ready()
{
    _screenSize = GetViewportRect().Size;
    Hide();
}
```

#### _Process(double delta)

每帧调用，处理玩家输入和移动。

```csharp
public override void _Process(double delta)
{
    Vector2 velocity = Vector2.Zero; // 玩家移动向量
    
    // 处理输入
    if (Input.IsActionPressed("move_right"))
        velocity.X += 1;
    if (Input.IsActionPressed("move_left"))
        velocity.X -= 1;
    if (Input.IsActionPressed("move_down"))
        velocity.Y += 1;
    if (Input.IsActionPressed("move_up"))
        velocity.Y -= 1;

    // 处理动画和移动
    if (velocity.Length() > 0)
    {
        velocity = velocity.Normalized() * Speed;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
    }
    else
    {
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
    }

    // 更新位置并限制在屏幕范围内
    Position += velocity * (float)delta;
    Position = Position.Clamp(Vector2.Zero, _screenSize);

    // 更新动画
    if (velocity.X != 0)
    {
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "walk";
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = false;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = velocity.X < 0;
    }
    else if (velocity.Y != 0)
    {
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "up";
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = velocity.Y > 0;
    }
}
```

#### _on_body_entered(Node2D body)

当敌人进入玩家碰撞区域时调用，处理碰撞逻辑。

```csharp
private void _on_body_entered(Node2D body)
{
    Hide(); // 玩家被击中后消失
    EmitSignal(SignalName.Hit);
    // 必须延迟设置，因为不能在物理回调中更改物理属性
    GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
}
```

## Mob 类

`Mob`类表示游戏中的敌人，继承自`RigidBody2D`。

### 方法

#### _Ready()

当节点进入场景树时调用，随机选择并播放动画。

```csharp
public override void _Ready()
{
    var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    string[] mobTypes = animatedSprite.SpriteFrames.GetAnimationNames();
    animatedSprite.Play(mobTypes[new Random().Next(0, mobTypes.Length)]);
}
```

#### _on_visible_on_screen_notifier_2d_screen_exited()

当敌人离开屏幕时调用，销毁敌人实例。

```csharp
private void _on_visible_on_screen_notifier_2d_screen_exited()
{
    QueueFree();
}
```

## Main 类

`Main`类是主游戏场景，控制游戏流程，继承自`Node`。

### 属性

| 属性名 | 类型 | 描述 |
|--------|------|------|
| MobScene | PackedScene | 敌人场景，可在编辑器中导出 |
| _score | int | 当前游戏分数 |
| _gameVersion | string | 游戏版本号 |

### 方法

#### _Ready()

当节点进入场景树时调用，显示游戏版本号。

```csharp
public override void _Ready()
{
    GetNode<DodgeTheCreepsCS.HUD>("HUD").ShowVersion(_gameVersion);
}
```

#### GameOver()

处理游戏结束逻辑。

```csharp
public void GameOver()
{
    GetNode<Timer>("MobTimer").Stop();
    GetNode<Timer>("ScoreTimer").Stop();
    GetNode<DodgeTheCreepsCS.HUD>("HUD").ShowGameOver();
    GetNode<AudioStreamPlayer>("Music").Stop();
    GetNode<AudioStreamPlayer>("DeathSound").Play();
}
```

#### NewGame()

开始新游戏。

```csharp
public void NewGame()
{
    _score = 0;

    var player = GetNode<DodgeTheCreepsCS.Player>("Player");
    var startPosition = GetNode<Marker2D>("StartPosition");
    player.Start(startPosition.Position);

    GetNode<Timer>("StartTimer").Start();

    var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
    hud.UpdateScore(_score);
    hud.ShowMessage("Get Ready!");

    GetNode<AudioStreamPlayer>("Music").Play();
}
```

#### _on_mob_timer_timeout()

当敌人生成定时器超时时调用，生成新敌人。

```csharp
private void _on_mob_timer_timeout()
{
    // 创建敌人实例
    var mob = MobScene.Instantiate<DodgeTheCreepsCS.Mob>();

    // 选择路径上的随机位置
    var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
    mobSpawnLocation.ProgressRatio = GD.Randf();

    // 设置敌人方向垂直于路径方向
    float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

    // 设置敌人位置
    mob.Position = mobSpawnLocation.Position;

    // 添加随机方向变化
    direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
    mob.Rotation = direction;

    // 设置敌人速度
    var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
    mob.LinearVelocity = velocity.Rotated(direction);

    // 将敌人添加到场景中
    AddChild(mob);
}
```

#### _on_score_timer_timeout()

当分数定时器超时时调用，增加分数。

```csharp
private void _on_score_timer_timeout()
{
    _score++;
    GetNode<DodgeTheCreepsCS.HUD>("HUD").UpdateScore(_score);
}
```

#### _on_start_timer_timeout()

当开始定时器超时时调用，开始游戏。

```csharp
private void _on_start_timer_timeout()
{
    GetNode<Timer>("MobTimer").Start();
    GetNode<Timer>("ScoreTimer").Start();
}
```

## HUD 类

`HUD`类表示游戏的用户界面，继承自`CanvasLayer`。

### 信号

| 信号名 | 参数 | 描述 |
|--------|------|------|
| StartGame | 无 | 当玩家点击开始按钮时触发 |

### 方法

#### ShowMessage(string text)

显示消息。

```csharp
public void ShowMessage(string text)
{
    var message = GetNode<Label>("Message");
    message.Text = text;
    message.Show();

    GetNode<Timer>("MessageTimer").Start();
}
```

#### ShowGameOver()

显示游戏结束界面。

```csharp
// 用于存储临时事件处理程序的字段，以便后续可以移除
private System.Action _messageTimerHandler;

public void ShowGameOver()
{
    ShowMessage("Game Over");

    // 移除之前可能存在的事件处理程序
    var messageTimer = GetNode<Timer>("MessageTimer");
    if (_messageTimerHandler != null)
    {
        messageTimer.Timeout -= _messageTimerHandler;
        _messageTimerHandler = null;
    }
    
    // 创建新的事件处理程序并连接
    _messageTimerHandler = () =>
    {
        var message = GetNode<Label>("Message");
        message.Text = "Dodge the\nCreeps!";
        message.Show();

        // 使用一次性计时器
        var oneShot = GetTree().CreateTimer(1.0);
        oneShot.Timeout += () =>
        {
            GetNode<Button>("StartButton").Show();
            // 一次性计时器不需要断开连接，因为它会自动销毁
        };
        
        // 完成后移除事件处理程序
        messageTimer.Timeout -= _messageTimerHandler;
        _messageTimerHandler = null;
    };
    
    messageTimer.Timeout += _messageTimerHandler;
}
```

#### UpdateScore(int score)

更新分数显示。

```csharp
public void UpdateScore(int score)
{
    GetNode<Label>("ScoreLabel").Text = score.ToString();
}
```

#### ShowVersion(string version)

显示游戏版本号。

```csharp
public void ShowVersion(string version)
{
    GetNode<Label>("GameVersion").Text = $"v{version}";
}
```

#### _on_start_button_pressed()

当开始按钮被点击时调用。

```csharp
private void _on_start_button_pressed()
{
    GetNode<Button>("StartButton").Hide();
    EmitSignal(SignalName.StartGame);
}
```

#### _on_message_timer_timeout()

当消息定时器超时时调用，隐藏消息。

```csharp
private void _on_message_timer_timeout()
{
    GetNode<Label>("Message").Hide();
}
```