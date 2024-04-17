using Avalonia;
using System;
using System.Collections.Generic;
using Avalonia.Markup.Xaml.Styling;

namespace AvaloniaFlyoutPerfMinimalRepro;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var avaloniaApp = BuildAvaloniaApp();
        //InitializeStyles();
        avaloniaApp.StartWithClassicDesktopLifetime(args);
    }

    static void InitializeStyles()
    {
        foreach (StyleInclude styleInclude in GetStyles())
        {
            Application.Current.Styles.Add(styleInclude);
        }
    }

    static List<StyleInclude> GetStyles()
    {
        return new List<StyleInclude>()
        {
            LoadAppFrameworkStyle("ScrollLessFlyoutPresenter.axaml")
        };
    }

    static StyleInclude LoadAppFrameworkStyle(string relativePath)
    {
        return LoadStyle("avares://AvaloniaFlyoutPerfMinimalRepro", relativePath);
    }

    static StyleInclude LoadStyle(string baseUri, string relativeUri)
    {
        return new StyleInclude(new Uri(baseUri))
        {
            Source = new Uri(relativeUri, UriKind.Relative)
        };
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}