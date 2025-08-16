using Godot;
using System;

namespace DodgeTheCreepsCS
{
    /// <summary>
    /// 主游戏类，负责游戏流程控制、敌人生成和分数管理
    /// </summary>
    public partial class Main : Node
    {
        #region Fields and Properties

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

        #endregion

        #region Lifecycle Methods

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
                try
                {
                    MobScene = GD.Load<PackedScene>("res://mob.tscn");
                    GD.Print("Main._Ready: 已加载Mob场景");
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Main._Ready: 加载Mob场景失败: {ex.Message}");
                    GetNode<DodgeTheCreepsCS.HUD>("HUD").ShowMessage("Error: Failed to load resources!");
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 游戏结束处理
        /// </summary>
        public void GameOver()
        {
            try
            {
                GD.Print("Main.GameOver: 游戏结束");

                var mobTimer = GetNode<Timer>("MobTimer");
                var scoreTimer = GetNode<Timer>("ScoreTimer");
                if (mobTimer != null && scoreTimer != null)
                {
                    mobTimer.Stop();
                    scoreTimer.Stop();
                    GD.Print("Main.GameOver: 停止MobTimer和ScoreTimer");
                }
                else
                {
                    GD.PrintErr("Main.GameOver: 无法找到计时器节点");
                }

                var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
                if (hud != null)
                {
                    hud.ShowGameOver();
                    GD.Print("Main.GameOver: 显示游戏结束界面");
                }
                else
                {
                    GD.PrintErr("Main.GameOver: 无法找到HUD节点");
                }

                var music = GetNode<AudioStreamPlayer>("Music");
                var deathSound = GetNode<AudioStreamPlayer>("DeathSound");
                if (music != null)
                {
                    music.Stop();
                    GD.Print("Main.GameOver: 停止背景音乐");
                }
                else
                {
                    GD.PrintErr("Main.GameOver: 无法找到Music节点");
                }

                if (deathSound != null)
                {
                    deathSound.Play();
                    GD.Print("Main.GameOver: 播放死亡音效");
                }
                else
                {
                    GD.PrintErr("Main.GameOver: 无法找到DeathSound节点");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Main.GameOver: 游戏结束处理时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 开始新游戏
        /// </summary>
        public void NewGame()
        {
            try
            {
                GD.Print("Main.NewGame: 开始新游戏");
                _score = 0;

                var player = GetNode<DodgeTheCreepsCS.Player>("Player");
                if (player == null)
                {
                    GD.PrintErr("Main.NewGame: 无法找到Player节点");
                    return;
                }

                var startPosition = GetNode<Marker2D>("StartPosition");
                if (startPosition == null)
                {
                    GD.PrintErr("Main.NewGame: 无法找到StartPosition节点");
                    return;
                }

                GD.Print($"Main.NewGame: 设置玩家起始位置 {startPosition.Position}");
                try
                {
                    player.Start(startPosition.Position);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Main.NewGame: 设置玩家起始位置时出错: {ex.Message}");
                    return;
                }

                var startTimer = GetNode<Timer>("StartTimer");
                if (startTimer == null)
                {
                    GD.PrintErr("Main.NewGame: 无法找到StartTimer节点");
                    return;
                }

                GD.Print("Main.NewGame: 启动StartTimer");
                startTimer.Start();

                var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
                if (hud == null)
                {
                    GD.PrintErr("Main.NewGame: 无法找到HUD节点");
                    return;
                }

                hud.UpdateScore(_score);
                hud.ShowMessage("Get Ready!");
                GD.Print("Main.NewGame: 显示'Get Ready!'消息");

                try
                {
                    // 清理所有敌人
                    GetTree().CallGroup("mobs", "queue_free");
                    GD.Print("Main.NewGame: 清理所有敌人");
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Main.NewGame: 清理敌人时出错: {ex.Message}");
                    // 继续执行，因为这不是关键错误
                }

                var music = GetNode<AudioStreamPlayer>("Music");
                if (music != null)
                {
                    music.Play();
                    GD.Print("Main.NewGame: 播放背景音乐");
                }
                else
                {
                    GD.PrintErr("Main.NewGame: 无法找到Music节点");
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Main.NewGame: 开始新游戏时出错: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 当MobTimer超时时生成新的敌人
        /// </summary>
        private void OnMobTimerTimeout()
        {
            GD.Print("Main.OnMobTimerTimeout: 生成新敌人");

            // 检查MobScene是否有效
            if (MobScene == null)
            {
                GD.PrintErr("Main.OnMobTimerTimeout: MobScene为空，无法生成敌人");
                return;
            }

            try
            {
                // Create a new instance of the Mob scene.
                var mob = MobScene.Instantiate<DodgeTheCreepsCS.Mob>();
                GD.Print("Main.OnMobTimerTimeout: 实例化Mob场景");

                try
                {
                    // Choose a random location on Path2D.
                    var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
                    if (mobSpawnLocation == null)
                    {
                        GD.PrintErr("Main.OnMobTimerTimeout: 无法找到MobPath/MobSpawnLocation节点");
                        return;
                    }

                    mobSpawnLocation.ProgressRatio = GD.Randf();
                    GD.Print($"Main.OnMobTimerTimeout: 设置敌人生成位置 {mobSpawnLocation.Position}");

                    // Set the mob's direction perpendicular to the path direction.
                    float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

                    // Set the mob's position to a random location.
                    mob.Position = mobSpawnLocation.Position;

                    // Add some randomness to the direction.
                    direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
                    mob.Rotation = direction;
                    GD.Print($"Main.OnMobTimerTimeout: 设置敌人旋转角度 {direction}");

                    // Choose the velocity for the mob.
                    var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
                    mob.LinearVelocity = velocity.Rotated(direction);
                    GD.Print($"Main.OnMobTimerTimeout: 设置敌人速度 {mob.LinearVelocity}");

                    // 确保敌人被添加到mobs组中
                    mob.AddToGroup("mobs");
                    GD.Print("Main.OnMobTimerTimeout: 将敌人添加到mobs组");

                    // Spawn the mob by adding it to the Main scene.
                    AddChild(mob);
                    GD.Print("Main.OnMobTimerTimeout: 将敌人添加到场景");
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Main.OnMobTimerTimeout: 设置敌人属性时出错: {ex.Message}");
                    // 确保清理未成功添加的敌人实例
                    if (mob != null && !mob.IsQueuedForDeletion())
                    {
                        mob.QueueFree();
                    }
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Main.OnMobTimerTimeout: 实例化敌人时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 当ScoreTimer超时时更新分数
        /// </summary>
        private void OnScoreTimerTimeout()
        {
            try
            {
                _score++;
                var hud = GetNode<DodgeTheCreepsCS.HUD>("HUD");
                if (hud == null)
                {
                    GD.PrintErr("Main.OnScoreTimerTimeout: 无法找到HUD节点");
                    return;
                }
                hud.UpdateScore(_score);
                GD.Print($"Main.OnScoreTimerTimeout: 更新分数 {_score}");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Main.OnScoreTimerTimeout: 更新分数时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 当StartTimer超时时启动游戏计时器
        /// </summary>
        private void OnStartTimerTimeout()
        {
            try
            {
                GD.Print("Main.OnStartTimerTimeout: StartTimer超时");

                var mobTimer = GetNode<Timer>("MobTimer");
                if (mobTimer == null)
                {
                    GD.PrintErr("Main.OnStartTimerTimeout: 无法找到MobTimer节点");
                    return;
                }

                var scoreTimer = GetNode<Timer>("ScoreTimer");
                if (scoreTimer == null)
                {
                    GD.PrintErr("Main.OnStartTimerTimeout: 无法找到ScoreTimer节点");
                    return;
                }

                mobTimer.Start();
                scoreTimer.Start();
                GD.Print("Main.OnStartTimerTimeout: 启动MobTimer和ScoreTimer");
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Main.OnStartTimerTimeout: 启动计时器时出错: {ex.Message}");
            }
        }

        #endregion
    }
}