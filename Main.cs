using Godot;
using System;

namespace DodgeTheCreepsCS
{
/// <summary>
/// 主游戏类，负责游戏流程控制、敌人生成和分数管理
/// </summary>
public partial class Main : Node
{
    /// <summary>
    /// 敌人场景，用于实例化敌人
    /// </summary>
    [Export]
    public PackedScene MobScene { get; set; }

    /// <summary>
    /// 当前游戏分数
    /// </summary>
    private int _score;
    
    /// <summary>
    /// 游戏版本号
    /// </summary>
    private string _gameVersion = "1.0.0";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Main._Ready: 初始化游戏");
        var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
        hud.ShowVersion(_gameVersion);
        
        // 连接HUD的StartGame信号到Main的NewGame方法
        hud.StartGame += NewGame;
        GD.Print("Main._Ready: 已连接HUD.StartGame信号到Main.NewGame方法");
        
        // 连接Player的Hit信号到Main的GameOver方法
        var player = GetNode<DodgeTheCreepsCS.Player>("Player");
        player.Hit += GameOver;
        GD.Print("Main._Ready: 已连接Player.Hit信号到Main.GameOver方法");
        
        // 加载Mob场景
        if (MobScene == null)
        {
            MobScene = GD.Load<PackedScene>("res://mob.tscn");
            GD.Print("Main._Ready: 已加载Mob场景");
        }
    }

    /// <summary>
    /// 游戏结束处理
    /// </summary>
    public void GameOver()
    {
        GD.Print("Main.GameOver: 游戏结束");
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GD.Print("Main.GameOver: 停止MobTimer和ScoreTimer");
        
        GetNode<DodgeTheCreepsCS.HUD>("HUD").ShowGameOver();
        GD.Print("Main.GameOver: 显示游戏结束界面");
        
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
        GD.Print("Main.GameOver: 停止背景音乐，播放死亡音效");
    }

    public void NewGame()
    {
        GD.Print("Main.NewGame: 开始新游戏");
        _score = 0;

        var player = GetNode<DodgeTheCreepsCS.Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        GD.Print($"Main.NewGame: 设置玩家起始位置 {startPosition.Position}");
        player.Start(startPosition.Position);

        GD.Print("Main.NewGame: 启动StartTimer");
        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
        GD.Print("Main.NewGame: 显示'Get Ready!'消息");
        
        // 清理所有敌人
        GetTree().CallGroup("mobs", "queue_free");
        GD.Print("Main.NewGame: 清理所有敌人");

        GetNode<AudioStreamPlayer>("Music").Play();
        GD.Print("Main.NewGame: 播放背景音乐");
    }

    /// <summary>
    /// 当MobTimer超时时生成新的敌人
    /// </summary>
    private void _on_mob_timer_timeout()
    {
        GD.Print("Main._on_mob_timer_timeout: 生成新敌人");
        
        // Create a new instance of the Mob scene.
        var mob = MobScene.Instantiate<DodgeTheCreepsCS.Mob>();
        GD.Print("Main._on_mob_timer_timeout: 实例化Mob场景");

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();
        GD.Print($"Main._on_mob_timer_timeout: 设置敌人生成位置 {mobSpawnLocation.Position}");

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;
        GD.Print($"Main._on_mob_timer_timeout: 设置敌人旋转角度 {direction}");

        // Choose the velocity for the mob.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);
        GD.Print($"Main._on_mob_timer_timeout: 设置敌人速度 {mob.LinearVelocity}");

        // 确保敌人被添加到mobs组中
        mob.AddToGroup("mobs");
        GD.Print("Main._on_mob_timer_timeout: 将敌人添加到mobs组");

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
        GD.Print("Main._on_mob_timer_timeout: 将敌人添加到场景");
    }

    /// <summary>
    /// 当ScoreTimer超时时更新分数
    /// </summary>
    private void _on_score_timer_timeout()
    {
        _score++;
        GetNode<DodgeTheCreepsCS.HUD>("HUD").UpdateScore(_score);
        GD.Print($"Main._on_score_timer_timeout: 更新分数 {_score}");
    }

    private void _on_start_timer_timeout()
    {
        GD.Print("Main._on_start_timer_timeout: StartTimer超时");
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
        GD.Print("Main._on_start_timer_timeout: 启动MobTimer和ScoreTimer");
    }
}
}