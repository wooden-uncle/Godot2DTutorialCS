using Godot;
using System;

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

    public void ShowGameOver()
    {
        ShowMessage("Game Over");

        // Wait until the MessageTimer has counted down.
        GetNode<Timer>("MessageTimer").Timeout += () =>
        {
            var message = GetNode<Label>("Message");
            message.Text = "Dodge the\nCreeps!";
            message.Show();

            // Make a one-shot timer and wait for it to finish.
            var oneShot = GetTree().CreateTimer(1.0);
            oneShot.Timeout += () =>
            {
                GetNode<Button>("StartButton").Show();
            };
        };
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
        GetNode<Button>("StartButton").Hide();
        EmitSignal(SignalName.StartGame);
    }

    private void _on_message_timer_timeout()
    {
        GetNode<Label>("Message").Hide();
    }
}