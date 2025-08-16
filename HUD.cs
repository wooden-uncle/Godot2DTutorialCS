using Godot;
using System;

namespace DodgeTheCreepsCS
{

/// <summary>
/// 用户界面类，负责显示游戏信息、消息和按钮
/// </summary>
public partial class HUD : CanvasLayer
{
    /// <summary>
    /// 当开始按钮被点击时发出的信号
    /// </summary>
    [Signal]
    public delegate void StartGameEventHandler();

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
    /// 用于存储临时事件处理程序的字段，以便后续可以移除
    /// </summary>
    private System.Action _messageTimerHandler;
    
    /// <summary>
    /// 显示游戏结束界面
    /// </summary>
    public void ShowGameOver()
    {
        GD.Print("HUD.ShowGameOver: 显示游戏结束界面");
        ShowMessage("Game Over");

        // 移除之前可能存在的事件处理程序
        var messageTimer = GetNode<Timer>("MessageTimer");
        if (_messageTimerHandler != null)
        {
            GD.Print("HUD.ShowGameOver: 移除之前的MessageTimer事件处理程序");
            messageTimer.Timeout -= _messageTimerHandler;
            _messageTimerHandler = null;
        }
        
        // 创建新的事件处理程序并连接
        _messageTimerHandler = () =>
        {
            GD.Print("HUD.ShowGameOver: MessageTimer超时，显示游戏标题");
            var message = GetNode<Label>("Message");
            message.Text = "Dodge the\nCreeps!";
            message.Show();

            // 使用一次性计时器
            GD.Print("HUD.ShowGameOver: 创建一次性计时器，1秒后显示开始按钮");
            var oneShot = GetTree().CreateTimer(1.0);
            oneShot.Timeout += () =>
            {
                GD.Print("HUD.ShowGameOver: 一次性计时器超时，显示开始按钮");
                GetNode<Button>("StartButton").Show();
                // 一次性计时器不需要断开连接，因为它会自动销毁
            };
            
            // 完成后移除事件处理程序
            GD.Print("HUD.ShowGameOver: 移除MessageTimer事件处理程序");
            messageTimer.Timeout -= _messageTimerHandler;
            _messageTimerHandler = null;
        };
        
        GD.Print("HUD.ShowGameOver: 连接新的MessageTimer事件处理程序");
        messageTimer.Timeout += _messageTimerHandler;
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

    private void _on_start_button_pressed()
    {
        GD.Print("HUD._on_start_button_pressed: 开始按钮被点击");
        GetNode<Button>("StartButton").Hide();
        GD.Print("HUD._on_start_button_pressed: 隐藏开始按钮");
        GD.Print("HUD._on_start_button_pressed: 发出StartGame信号");
        EmitSignal(SignalName.StartGame);
    }

    /// <summary>
    /// 当MessageTimer超时时隐藏消息
    /// </summary>
    private void _on_message_timer_timeout()
    {
        GD.Print("HUD._on_message_timer_timeout: 隐藏消息");
        GetNode<Label>("Message").Hide();
    }
}
}