using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Notebook.ViewModels;

namespace Notebook;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel settings)
    {
        InitializeComponent();
        DataContext = settings;
    }

    public SettingsWindow() { }
}