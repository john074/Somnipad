using Avalonia.Controls;
using Avalonia.Input;
using Notebook.Data.Models;
using Notebook.Services;
using System;
using System.Linq;
using static Notebook.Services.TextLayoutService;

namespace Notebook;

public partial class LineEditorControl : UserControl
{
    bool SELECTING = false;
    (int line, int caret) selStart = (0, 0);
    (int line, int caret) selEnd = (0, 0);
    public LineEditorControl()
    {
        InitializeComponent();
        AddHandler(PointerPressedEvent, _OnPointerClick, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        for (int i = 0; i <= 14; i++)
        {
            var tb = FindTextBox(i);
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
            var tb = FindTextBox(i);
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
            var tb = FindTextBox(i);
            tb.Text = lines[i];
        }
    }

    public void SelectAll()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = FindTextBox(i);
            vb.SelectAll();
        }
    }

    public void Select(TextBox target)
    {
        if (target == null)
            return;

        var carretIndex = GetFocusedTextBox().CaretIndex;
        var txtLen = (target.Text ?? string.Empty).Length;
        if (!SELECTING)
        {
            SELECTING = true;
            selStart = (GetTextBoxIndex(GetFocusedTextBox()), carretIndex);
            selEnd = (GetTextBoxIndex(GetFocusedTextBox()), carretIndex);
        }

        selEnd = (GetTextBoxIndex(target), Math.Min(txtLen, carretIndex));

        target.Focus();
        target.CaretIndex = Math.Min(txtLen, carretIndex);

        (int line, int caret) logicalStart;
        (int line, int caret) logicalEnd;

        if (selStart.line < selEnd.line || (selStart.line == selEnd.line && selStart.caret <= selEnd.caret))
        {
            logicalStart = selStart;
            logicalEnd = selEnd;
        }
        else
        {
            logicalStart = selEnd;
            logicalEnd = selStart;
        }

        for (int i = logicalStart.line; i <= logicalEnd.line; i++)
        {
            var vb = FindTextBox(i);
            var len = (vb.Text ?? string.Empty).Length;

            if (logicalStart.line == logicalEnd.line)
            {
                vb.SelectionStart = logicalStart.caret;
                vb.SelectionEnd = logicalEnd.caret;
            }
            else if (i == logicalStart.line)
            {
                vb.SelectionStart = logicalStart.caret;
                vb.SelectionEnd = len;
            }
            else if (i == logicalEnd.line)
            {
                vb.SelectionStart = 0;
                vb.SelectionEnd = logicalEnd.caret;
            }
            else
            {
                vb.SelectionStart = 0;
                vb.SelectionEnd = len;
            }
        }
    }

    public void UnselectAll()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = FindTextBox(i);
            vb.ClearSelection();
        }

        SELECTING = false;
        selStart = (0, 0);
        selEnd = (0, 0);
    }

    public void DeleteSelection()
    {
        for (int i = 0; i <= 14; i++)
        {
            var vb = FindTextBox(i);

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
            TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(txt);
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

        var focused = GetFocusedTextBox();

        if (focused == null)
            return;

        DeleteSelection();

        int startLine = GetTextBoxIndex(focused);
        int caret = focused.CaretIndex;

        var currentLines = GetPage(-1).Content.Split('\n').ToList();
        var newLines = TextLayoutService.Paste(currentLines, startLine, caret, text, 15);

        LoadPage(new Page() { Content = string.Join('\n', newLines) });
    }

    private void _OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        var index = GetTextBoxIndex(textBox);
        var caretIndex = textBox.CaretIndex;
        var hasShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        var hasCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);

        switch (e.Key)
        {
            case Key.Back:
                DeleteSelection();
                HandleUnderflow(this, textBox, index);
                break;

            case Key.Down:
                HandleArrowUpDown(index, caretIndex, hasShift, false);
                e.Handled = true;
                break;

            case Key.Up:
                HandleArrowUpDown(index, caretIndex, hasShift, true);
                e.Handled = true;
                break;

            case Key.Left when hasShift:
                MarkSelection(index, caretIndex);
                break;

            case Key.Right when hasShift:
                MarkSelection(index, caretIndex);
                break;

            case Key.A when hasCtrl:
                SelectAll();
                e.Handled = true;
                break;

            case Key.C when hasCtrl:
                CopySelected();
                e.Handled = true;
                break;

            case Key.V when hasCtrl:
                Paste();
                e.Handled = true;
                break;

            case Key.Escape:
                UnselectAll();
                SELECTING = false;
                e.Handled = true;
                break;

            case Key.Enter:
                NewLine(this);
                e.Handled = true;
                break;
        }
    }

    private int GetTextBoxIndex(TextBox textBox) => Convert.ToInt32(textBox.Name[1..]);

    private TextBox? FindTextBox(int index) => this.FindNameScope()?.Find<TextBox>($"_{index}");

    private TextBox? GetFocusedTextBox() => TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as TextBox;

    private LineEditorControl? FindOtherPage()
    {
        var otherPageName = Name == "PgL" ? "PgR" : "PgL";
        return Parent?.FindNameScope()?.Find<LineEditorControl>(otherPageName);
    }

    private void FocusTextBox(TextBox? textBox, int caretIndex)
    {
        textBox?.Focus();
        textBox?.CaretIndex = caretIndex;
    }

    private void MarkSelection(int index, int caretIndex)
    {
        if (!SELECTING)
        {
            SELECTING = true;
            selStart = (index, caretIndex);
        }
        else
        {
            selEnd = (index, caretIndex);
        }
    }

    private void HandleArrowUpDown(int index, int caretIndex, bool hasShift, bool isUp)
    {
        var nextIndex = isUp ? index - 1 : index + 1;
        var nextPageStart = isUp ? 14 : 0;

        var nextTextBox = FindTextBox(nextIndex);

        if (hasShift)
        {
            if (nextTextBox != null)
                Select(nextTextBox);
            return;
        }

        if (nextTextBox == null)
        {   
            var otherPage = FindOtherPage();
            nextTextBox = otherPage?.FindTextBox(nextPageStart);
            UnselectAll();
        }

        FocusTextBox(nextTextBox, caretIndex);
    }

    private void _OnPointerClick(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            UnselectAll();
            SELECTING = false;
        }
    }
}