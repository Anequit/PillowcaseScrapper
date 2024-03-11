using FluentAvalonia.UI.Windowing;
using PillowcaseScrapper.ViewModels;

namespace PillowcaseScrapper.Views;

public partial class MainWindow : AppWindow
{
	public MainWindow()
	{
		InitializeComponent();

		DataContext = new MainWindowViewModel(this);

		TitleBar.ButtonHoverForegroundColor = new Avalonia.Media.Color(255, 255, 255, 255);
		TitleBar.ButtonHoverBackgroundColor = new Avalonia.Media.Color(255, 61, 61, 61);

		TitleBar.ButtonPressedForegroundColor = new Avalonia.Media.Color(255, 167, 167, 167);
		TitleBar.ButtonPressedBackgroundColor = new Avalonia.Media.Color(255, 56, 56, 56);

		TitleBar.ButtonInactiveForegroundColor = new Avalonia.Media.Color(255, 167, 167, 167);
	}
}