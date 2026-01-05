using Avalonia.Controls;
using Avalonia.Media;


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

            string text = current.Text;
            current.Text = text[..^1];

            var next = container.FindNameScope()?.Find<TextBox>($"_{index + 1}");
            if (next == null)
                return;

            next.Text = text[^1] + next.Text;
            next.CaretIndex = next.Text.Length;
            next.Focus();
        }

        public static void HandleUnderflow(LineEditorControl container, TextBox current, int index)
        {
            if (current.CaretIndex != 0)
                return;

            var prev = container.FindNameScope()?.Find<TextBox>($"_{index - 1}");
            if (string.IsNullOrEmpty(prev?.Text))
            {
                prev?.CaretIndex = 0;
                prev?.Focus();
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
