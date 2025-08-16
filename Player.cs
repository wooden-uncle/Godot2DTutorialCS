using Godot;
using System;

namespace DodgeTheCreepsCS
{

/// <summary>
/// 玩家类，负责处理玩家输入、移动和碰撞
/// </summary>
public partial class Player : Area2D
{
    /// <summary>
    /// 当玩家被敌人击中时发出的信号
    /// </summary>
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
    
    private Vector2 _screenSize; // Size of the game window.

    /// <summary>
    /// 初始化玩家位置并显示玩家
    /// </summary>
    /// <param name="pos">玩家的初始位置</param>
    public void Start(Vector2 pos)
    {
        GD.Print("Player.Start: 初始化玩家位置");
        Position = pos;
        GD.Print("Player.Start: 显示玩家");
        Show();
        GD.Print("Player.Start: 启用碰撞检测");
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    /// <summary>
    /// 初始化玩家，获取屏幕大小并隐藏玩家
    /// </summary>
    public override void _Ready()
    {
        GD.Print("Player._Ready: 初始化玩家");
        _screenSize = GetViewportRect().Size;
        GD.Print($"Player._Ready: 获取屏幕大小 {_screenSize}");
        Hide();
        GD.Print("Player._Ready: 隐藏玩家，等待游戏开始");
    }

    /// <summary>
    /// 每帧处理玩家输入和移动
    /// </summary>
    /// <param name="delta">帧间隔时间</param>
    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero; // The player's movement vector.
        
        if (Input.IsActionPressed("move_right"))
            velocity.X += 1;
        if (Input.IsActionPressed("move_left"))
            velocity.X -= 1;
        if (Input.IsActionPressed("move_down"))
            velocity.Y += 1;
        if (Input.IsActionPressed("move_up"))
            velocity.Y -= 1;

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
        }
        else
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
        }

        Position += velocity * (float)delta;
        Position = Position.Clamp(Vector2.Zero, _screenSize);

        if (velocity.X != 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "walk";
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = false;
            // See the note below about boolean assignment.
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "up";
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = velocity.Y > 0;
        }
    }

    /// <summary>
    /// 当玩家与敌人碰撞时调用
    /// </summary>
    /// <param name="body">碰撞的物体（敌人）</param>
    private void _on_body_entered(Node2D body)
    {
        GD.Print("Player._on_body_entered: 玩家被敌人击中");
        Hide(); // Player disappears after being hit.
        GD.Print("Player._on_body_entered: 隐藏玩家");
        
        EmitSignal(SignalName.Hit);
        GD.Print("Player._on_body_entered: 发出Hit信号");
        
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GD.Print("Player._on_body_entered: 禁用碰撞检测");
    }
}
}