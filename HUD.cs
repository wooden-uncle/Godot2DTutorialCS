using Godot;
using System;

namespace DodgeTheCreepsCS
{
/// <summary>
/// 用户界面类，负责显示游戏信息、消息和按钮
/// </summary>
public partial class HUD : CanvasLayer
{
    #region Signals
    /// <summary>
    /// 当开始按钮被点击时发出的信号
    /// </summary>
    [Signal]
    public delegate void StartGameEventHandler();
    #endregion

    #region Public Methods
    /// <summary>
    /// 显示游戏消息
    /// </summary>
    /// <param name="text">要显示的消息文本</param>
    public void ShowMessage(string text)
    {
        GD.Print($"HUD.ShowMessage: 显示消息 '{text}'");
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        GetNode<Timer>("MessageTimer").Start();
        GD.Print("HUD.ShowMessage: 启动MessageTimer");
    }

    /// <summary>
    /// 显示游戏结束界面
    /// </summary>
    public void ShowGameOver()
    {
        GD.Print("HUD.ShowGameOver: 显示游戏结束界面");
        ShowMessage("Game Over");

        // 使用一次性连接处理MessageTimer超时事件
        var messageTimer = GetNode<Timer>("MessageTimer");
        messageTimer.Timeout += OnGameOverMessageTimerTimeout;
        GD.Print("HUD.ShowGameOver: 连接MessageTimer一次性事件处理程序");
    }

    /// <summary>
    /// 更新分数显示
    /// </summary>
    /// <param name="score">当前分数</param>
    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
        GD.Print($"HUD.UpdateScore: 更新分数显示为 {score}");
    }

    /// <summary>
    /// 显示游戏版本号
    /// </summary>
    /// <param name="version">版本号</param>
    public void ShowVersion(string version)
    {
        GetNode<Label>("GameVersion").Text = $"v{version}";
        GD.Print($"HUD.ShowVersion: 显示游戏版本 {version}");
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// 游戏结束时MessageTimer超时的处理方法
    /// </summary>
    private void OnGameOverMessageTimerTimeout()
    {
        GD.Print("HUD.OnGameOverMessageTimerTimeout: MessageTimer超时，显示游戏标题");
        var message = GetNode<Label>("Message");
        message.Text = "Dodge the\nCreeps!";
        message.Show();

        // 使用一次性计时器
        GD.Print("HUD.OnGameOverMessageTimerTimeout: 创建一次性计时器，1秒后显示开始按钮");
        var oneShot = GetTree().CreateTimer(1.0);
        oneShot.Timeout += OnGameOverDelayTimeout;
        
        // 移除事件处理程序，确保只执行一次
        var messageTimer = GetNode<Timer>("MessageTimer");
        messageTimer.Timeout -= OnGameOverMessageTimerTimeout;
    }
    
    /// <summary>
    /// 游戏结束延迟计时器超时的处理方法
    /// </summary>
    private void OnGameOverDelayTimeout()
    {
        GD.Print("HUD.OnGameOverDelayTimeout: 延迟计时器超时，显示开始按钮");
        GetNode<Button>("StartButton").Show();
        // 一次性计时器不需要断开连接，因为它会自动销毁
    }

    /// <summary>
    /// 当开始按钮被点击时的处理方法
    /// </summary>
    private void OnStartButtonPressed()
    {
        GD.Print("HUD.OnStartButtonPressed: 开始按钮被点击");
        GetNode<Button>("StartButton").Hide();
        GD.Print("HUD.OnStartButtonPressed: 隐藏开始按钮");
        GD.Print("HUD.OnStartButtonPressed: 发出StartGame信号");
        EmitSignal(SignalName.StartGame);
    }

    /// <summary>
    /// 当MessageTimer超时时隐藏消息
    /// </summary>
    private void OnMessageTimerTimeout()
    {
        GD.Print("HUD.OnMessageTimerTimeout: 隐藏消息");
        GetNode<Label>("Message").Hide();
    }
    #endregion
}
}