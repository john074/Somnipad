using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Notebook.Services
{
    internal static class TextLayoutService
    {
        public static void HandleOverflow(LineEditorControl container, TextBox current, int index)
        {
            if (string.IsNullOrEmpty(current?.Text))
                return;

            var formatted = GetFormattedText(current);
            if (formatted.Width <= current.Bounds.Width - 20)
                return;

            var next = container.FindNameScope()?.Find<TextBox>($"_{index + 1}");
            if (next == null)
            {
                current.Undo();
                return;
            }

            string text = current.Text;
            current.Text = text[..^1];

            next.Text = text[^1] + next.Text;
            if (current.CaretIndex == current.Text.Length)
            {
                next.Focus();
            }
        }

        public static void HandleUnderflow(LineEditorControl container, TextBox current, int index)
        {
            if (current.CaretIndex != 0)
                return;

            var prev = container.FindNameScope()?.Find<TextBox>($"_{index - 1}");
            if (prev == null)
                return;

            if (string.IsNullOrEmpty(prev.Text) || string.IsNullOrEmpty(current.Text))
            {
                var num = Convert.ToInt32(current.Name[1..]);

                if (string.IsNullOrEmpty(current.Text))
                    num++;

                for (int i = num; i <= 14; i++)
                {
                    var lower = container.FindNameScope()?.Find<TextBox>($"_{i}");
                    var upper = container.FindNameScope()?.Find<TextBox>($"_{i - 1}");

                    upper.Text = lower.Text;
                    if (i == 14)
                        lower.Text = string.Empty;
                }

                prev.CaretIndex = 0;
                prev.Focus();
                return;
            }

            prev.CaretIndex = prev.Text.Length;
            prev.Focus();

            for (int i = 0; i < current.Text.Length; i++)
            {
                if (GetFormattedText(prev).Width + prev.FontSize <= prev.Bounds.Width - 20)
                {
                    prev.Text += current.Text[i];
                }
                else
                {
                    current.Text = current.Text[i..];
                    break;
                }

                if (i == current.Text.Length - 1)
                {
                    current.Text = string.Empty;
                }
            }
        }

        public static List<string> Paste(List<string> currentLines, int startLine, int startIndex, string pastedText, int maxLines)
        {
            var pasteLines = pastedText.Replace("\r", "").Trim().Split('\n');

            var firstLine = currentLines[startLine];
            var head = firstLine[..startIndex];
            var tail = firstLine[startIndex..];

            currentLines[startLine] = head + pasteLines[0];

            int lineIndex = startLine + 1;

            for (int i = 1; i < pasteLines.Length && lineIndex < maxLines; i++)
            {
                currentLines.Insert(lineIndex, pasteLines[i]);
                lineIndex++;
            }

            if (lineIndex - 1 < maxLines)
                currentLines[lineIndex - 1] += tail;

            return currentLines.Take(maxLines).ToList();
        }

        public static void NewLine(LineEditorControl container)
        {
            var vb = (TextBox)TopLevel.GetTopLevel(container).FocusManager.GetFocusedElement();
            int num = Convert.ToInt32(vb.Name[1..]);
            if (num == 14)
                return;

            var text = vb.Text;
            var cur = vb.CaretIndex;
            var MoveOccured = false;

            for (int i = 14; i > num; i--)
            {
                var lower = container.FindNameScope()?.Find<TextBox>($"_{i}");
                var upper = container.FindNameScope()?.Find<TextBox>($"_{i - 1}");

                if (string.IsNullOrEmpty(lower.Text))
                {
                    lower.Text = upper.Text;
                    upper.Text = string.Empty;
                    MoveOccured = true;
                }
                else
                    continue;

                if (i == num + 1)
                {
                    lower.Text = text[cur..];
                    lower.Focus();
                }
            }

            if (MoveOccured)
                vb.Text = text[..cur];
        }

        private static FormattedText GetFormattedText(TextBox current)
        {
            return new FormattedText(
                current.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(current.FontFamily),
                current.FontSize,
                Brushes.Black);
        }
    }
}
