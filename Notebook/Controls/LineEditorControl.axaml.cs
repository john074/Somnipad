using Avalonia.Controls;
using Avalonia.Input;
using Notebook.Data.Models;
using Notebook.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Notebook.Services.TextLayoutService;

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
            AddHandler(PointerPressedEvent, _OnPointerClick, Avalonia.Interactivity.RoutingStrategies.Tunnel);
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

    public void SelectAll()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            vb.SelectAll();
        }
    }

    public void Select(TextBox target)
    {
        var selections = new List<(int, int)>();
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            selections.Add((vb.SelectionStart, vb.SelectionEnd));
        }

        target.Focus();
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            vb.SelectionStart = selections[i].Item1;
            vb.SelectionEnd = selections[i].Item2;
        }

        target.SelectionStart = 0;
    }

    public void UnselectAll()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            vb.ClearSelection();
        }
    }

    public void DeleteSelection()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");

            if (vb.SelectionEnd < vb.SelectionStart)
            {
                int tmp = vb.SelectionEnd;
                vb.SelectionEnd = vb.SelectionStart;
                vb.SelectionStart = tmp;
            }

            var toRemove = vb.Text[vb.SelectionStart..vb.SelectionEnd];
            vb.Text = vb.Text.Remove(vb.Text.IndexOf(toRemove), toRemove.Length);
            vb.ClearSelection();
        }
    }

    public void CopySelected()
    {
        string txt = string.Empty;
        for (int i = 0; i <= 14; i++)
        {
            var vb = this.FindNameScope()?.Find<TextBox>($"_{i}");
            if (!string.IsNullOrEmpty(vb.SelectedText))
            {
                txt += vb.SelectedText + "\n";
            }
            TopLevel.GetTopLevel(this).Clipboard.SetTextAsync(txt);
        }
    }

    public void Paste()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null)
            return;

        var text = clipboard.GetTextAsync().Result;
        if (string.IsNullOrEmpty(text))
            return;

        var focused = (TextBox)TopLevel.GetTopLevel(this)?.FocusManager.GetFocusedElement();

        if (focused == null)
            return;

        int startLine = Convert.ToInt32(focused.Name[1..]);
        int caret = focused.CaretIndex;

        var currentLines = GetPage(-1).Content.Split('\n').ToList();
        var newLines = TextLayoutService.Paste(currentLines, startLine, caret, text, 15);

        LoadPage(new Page() { Content = string.Join('\n', newLines) });
    }


    private void _OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Back:
                DeleteSelection();
                HandleUnderflow(this, (TextBox)sender, Convert.ToInt32(((TextBox)sender).Name[1..]));
                break;

            case Key.Down:
                var next = this.FindNameScope()?.Find<TextBox>($"_{Convert.ToInt32(((TextBox)sender).Name[1..]) + 1}");
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    Select(next);
                    e.Handled = true;
                }
                else
                {
                    if (next == null)
                    {
                        var lookFor = Name == "PgL" ? "PgR" : "PgL";
                        var pg = Parent.FindNameScope()?.Find<LineEditorControl>(lookFor);
                        next = pg.FindNameScope()?.Find<TextBox>($"_0");
                    }
                    next?.Focus();
                }
                break;

            case Key.Up:
                var prev = this.FindNameScope()?.Find<TextBox>($"_{Convert.ToInt32(((TextBox)sender).Name[1..]) - 1}");
                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    Select(prev);
                    e.Handled = true;
                }
                else
                {
                    if (prev == null)
                    {
                        var lookFor = Name == "PgL" ? "PgR" : "PgL";
                        var pg = Parent.FindNameScope()?.Find<LineEditorControl>(lookFor);
                        prev = pg.FindNameScope()?.Find<TextBox>($"_14");
                    }
                    prev?.Focus();
                }
                break;

            case Key.A:
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    SelectAll();
                    e.Handled = true;
                }
                break;

            case Key.C:
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    CopySelected();
                    e.Handled = true;
                }
                break;

            case Key.V:
                if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    Paste();
                    e.Handled = true;
                }
                break;

            case Key.Escape:
                UnselectAll();
                e.Handled = true;
                break;

            case Key.Enter:
                NewLine(this);
                e.Handled = true;
                break;
        }
    }

    private void _OnPointerClick(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            UnselectAll();
    }
}