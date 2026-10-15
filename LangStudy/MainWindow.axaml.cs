using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.Threading;

namespace LangStudy;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        //ProcessHandling.listProcesses();
        ProcessHandling pH = new ProcessHandling();
        Thread.Sleep(5000);
        foreach (var p in pH.BlacklistProcesses)
            Debug.WriteLine(p);

        // TODO: launch function that kills all processes and presists until .... until what
    }

    private void Config_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO: make new window pop up
    }
}