using Avalonia.Controls;
using Notebook.Data.Models;
using System;
using System.Linq;
using static Notebook.Services.TextLayoutService;
using static System.Net.Mime.MediaTypeNames;

namespace Notebook;

public partial class LineEditorControl : UserControl
{
    public LineEditorControl()
    {
        InitializeComponent();
        for (int i = 0; i <= 14; i++)
        {
            var tb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            tb.AddHandler(KeyDownEvent, _OnKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }
    }

    public void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox current)
            return;

        int index = int.Parse(current.Name[1..]);
        HandleOverflow(this, current, index);
    }

    public Page GetPage(int pageNumber)
    {   
        string text = string.Empty;
        for (int i = 0; i <= 14; i++)
        {
            var tb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            if (!string.IsNullOrEmpty(tb.Text))
                text += tb.Text + "\n";
            else 
                text += "\n";
        }

        return new Page() { PageNumber = pageNumber, Content = text }; 
    }

    public void LoadPage(Page page)
    {
        string[] lines;
        if (!(page == null || page.Content == null))
            lines = page.Content.Split('\n');
        else
            lines = Enumerable.Repeat("", 15).ToArray();

        for (int i = 0; i <= 14; i++)
        {
            var tb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            tb.Text = lines[i];
        }
    }

    private void _OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Avalonia.Input.Key.Back:
                HandleUnderflow(this, (TextBox)sender, Convert.ToInt32(((TextBox)sender).Name[1..]));
                break;
            case Avalonia.Input.Key.Down:
                var next = this.FindNameScope()?.Find<TextBox>($"_{Convert.ToInt32(((TextBox)sender).Name[1..]) + 1}");
                next?.Focus();
                break;
            case Avalonia.Input.Key.Up:
                var prev = this.FindNameScope()?.Find<TextBox>($"_{Convert.ToInt32(((TextBox)sender).Name[1..]) - 1}");
                prev?.Focus();
                break;
        }
    }
}