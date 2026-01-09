using Avalonia.Controls;
using Avalonia.Input;
using Notebook.ViewModels;
using System;

namespace Notebook.Views;

public partial class MainWindow : Window
{
    private int _leftPageNumber = 1;
    private SettingsViewModel _settings;
    public MainWindow()
    {
        InitializeComponent();
        _settings = new SettingsViewModel();
        LoadPages();
        AddHandler(KeyDownEvent, OnKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
    }

    private void OnLeftPageClick(object? sender, PointerPressedEventArgs e)
    {
        SavePages();
        _leftPageNumber = Math.Max(1, _leftPageNumber - 2);
        LoadPages();
    }

    private void OnRightPageClick(object? sender, PointerPressedEventArgs e)
    {
        SavePages();
        _leftPageNumber += 2;
        LoadPages();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            SavePages();
            e.Handled = true;
        }
        else if (e.Key == Key.O && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            SettingsWindow settingsWindow = new SettingsWindow(_settings);
            settingsWindow.ShowDialog(this);
        }
    }

    private void SavePages()
    {
        var leftPage = PgL.GetPage(_leftPageNumber);
        var rightPage = PgR.GetPage(_leftPageNumber + 1);

        App.Repository.SavePage(leftPage);
        App.Repository.SavePage(rightPage);
    }

    private void LoadPages()
    {
        var left = App.Repository.LoadPage(_leftPageNumber);
        var right = App.Repository.LoadPage(_leftPageNumber + 1);

        PgL.LoadPage(left);
        PgR.LoadPage(right);

        PgNumL.Content = _leftPageNumber;
        PgNumR.Content = _leftPageNumber + 1;
    }
}
