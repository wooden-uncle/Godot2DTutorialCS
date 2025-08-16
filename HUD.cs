using Godot;
using System;

namespace DodgeTheCreepsCS
{

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    public void ShowMessage(string text)
    {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        GetNode<Timer>("MessageTimer").Start();
    }

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

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

    public void ShowVersion(string version)
    {
        GetNode<Label>("GameVersion").Text = $"v{version}";
    }

    private void _on_start_button_pressed()
    {
        GD.Print("HUD._on_start_button_pressed: 开始按钮被点击");
        GetNode<Button>("StartButton").Hide();
        GD.Print("HUD._on_start_button_pressed: 隐藏开始按钮");
        GD.Print("HUD._on_start_button_pressed: 发出StartGame信号");
        EmitSignal(SignalName.StartGame);
    }

    private void _on_message_timer_timeout()
    {
        GetNode<Label>("Message").Hide();
    }
}
}