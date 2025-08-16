using Godot;
using System;

namespace DodgeTheCreepsCS
{
/// <summary>
/// 玩家类，负责处理玩家输入、移动和碰撞
/// </summary>
public partial class Player : Area2D
{
    #region Signals
    /// <summary>
    /// 当玩家被敌人击中时发出的信号
    /// </summary>
    [Signal]
    public delegate void HitEventHandler();
    
    /// <summary>
    /// 当玩家生命值变化时发出的信号
    /// </summary>
    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth);
    #endregion

    #region Fields
    /// <summary>
    /// 游戏窗口大小
    /// </summary>
    private Vector2 _screenSize;
    
    /// <summary>
    /// 当前生命值
    /// </summary>
    private int _currentHealth;
    #endregion

    #region Properties
    /// <summary>
    /// 玩家移动速度（像素/秒）
    /// </summary>
    [Export(PropertyHint.Range, "100,800,10")]
    [ExportGroup("Movement Properties")]
    [ExportSubgroup("Speed Settings")]
    [ExportCategory("Player Settings")]
    public int Speed { get; set; } = 400;
    
    /// <summary>
    /// 动画播放速度
    /// </summary>
    [Export(PropertyHint.Range, "0.5,2.0,0.1")]
    [ExportGroup("Animation Properties")]
    public float AnimationSpeed { get; set; } = 1.0f;
    
    /// <summary>
    /// 是否启用碰撞效果
    /// </summary>
    [Export]
    [ExportGroup("Gameplay Properties")]
    public bool EnableCollisionEffects { get; set; } = true;
    
    /// <summary>
    /// 玩家生命值
    /// </summary>
    [Export(PropertyHint.Range, "1,10,1")]
    [ExportGroup("Gameplay Properties")]
    [ExportSubgroup("Health Settings")]
    public int Health { get; set; } = 3;
    
    /// <summary>
    /// 是否显示调试信息
    /// </summary>
    [Export]
    [ExportGroup("Debug Settings")]
    public bool ShowDebugInfo { get; set; } = false;
    #endregion
    
    #region Lifecycle Methods
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
        // 如果启用了调试信息，显示玩家状态
        if (ShowDebugInfo)
        {
            GD.Print($"Player._Process: 位置={Position}, 生命值={_currentHealth}/{Health}");
        }
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
            var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            animatedSprite.SpeedScale = AnimationSpeed;
            animatedSprite.Play();
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
    #endregion

    #region Public Methods
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
        GD.Print("Player.Start: 重置生命值");
        ResetHealth();
    }
    
    /// <summary>
    /// 重置玩家生命值
    /// </summary>
    public void ResetHealth()
    {
        _currentHealth = Health;
        GD.Print($"Player.ResetHealth: 生命值重置为 {_currentHealth}");
        
        // 发出生命值变化信号
        EmitSignal(SignalName.HealthChanged, _currentHealth);
        GD.Print("Player.ResetHealth: 发出HealthChanged信号");
    }
    
    /// <summary>
    /// 获取当前生命值
    /// </summary>
    /// <returns>当前生命值</returns>
    public int GetCurrentHealth()
    {
        return _currentHealth;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 当玩家与敌人碰撞时调用
    /// </summary>
    /// <param name="body">碰撞的物体（敌人）</param>
    private void OnBodyEntered(Node2D body)
    {
        GD.Print("Player.OnBodyEntered: 玩家被敌人击中");
        
        if (EnableCollisionEffects)
        {
            _currentHealth--;
            GD.Print($"Player.OnBodyEntered: 生命值减少到 {_currentHealth}");
            
            // 发出生命值变化信号
            EmitSignal(SignalName.HealthChanged, _currentHealth);
            GD.Print("Player.OnBodyEntered: 发出HealthChanged信号");
            
            if (_currentHealth <= 0)
            {
                Hide(); // Player disappears after being hit.
                GD.Print("Player.OnBodyEntered: 生命值为0，隐藏玩家");
                
                // Must be deferred as we can't change physics properties on a physics callback.
                GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
                GD.Print("Player.OnBodyEntered: 禁用碰撞检测");
                
                EmitSignal(SignalName.Hit);
                GD.Print("Player.OnBodyEntered: 发出Hit信号");
            }
        }
        else
        {
            EmitSignal(SignalName.Hit);
            GD.Print("Player.OnBodyEntered: 发出Hit信号");
        }
    }
    #endregion
}
}