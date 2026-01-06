using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Notebook.Data.Repositories;
using Notebook.ViewModels;
using Notebook.Views;

namespace Notebook;

public partial class App : Application
{
    public static NotebookRepository Repository { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        Repository = new NotebookRepository("C:\\Users\\mydog\\source\\repos\\Notebook\\Notebook\\Data\\DB\\notebook.db");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
