
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using PillowcaseScrapper.Extensions;
using PillowcaseScrapper.Models;
using PillowcaseScrapper.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace PillowcaseScrapper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	private string _searchText;
	private bool _isBusy;
	private ObservableCollection<Song> _songs;
	private SongService _songService;
	private readonly Window? _mainWindow;

	public MainWindowViewModel()
	{
		_songService = new SongService();
		_songs = [];
		_searchText = string.Empty;

		_ = LoadSongs();
	}

	public MainWindowViewModel(Window window) : this()
	{
		_mainWindow = window;
	}

	public bool IsBusy 
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value);
	}
	
	public string SearchText
	{
		get => _searchText;
		set
		{
			SetProperty(ref _searchText, value);
			
			SearchSongs(value);
		}
	}

	public ObservableCollection<Song> Songs 
	{
		get => _songs;
		set => SetProperty(ref _songs, value);
	}

	[RelayCommand]
	private async Task LoadSongs()
	{
		IsBusy = true;

		Songs.Clear();

		await _songService.LoadSongs();

		if(string.IsNullOrWhiteSpace(SearchText))
		{
			Songs.Clear();
			Songs.Populate(_songService.Songs);
		}
		else
		{
			SearchSongs(SearchText);
		}

		IsBusy = false;
	}

	[RelayCommand]
	private async Task Download(Song song)
	{
		string path = string.Empty;
		
		// Get download path
		if(_mainWindow is null)
			path = Path.GetTempPath();
		else
		{
			IReadOnlyList<IStorageFolder> folderPicker = await _mainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
			{
				Title = "Download location",
				SuggestedStartLocation = await _mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Music)
			});

			if(folderPicker.Count == 0)
				return;

			path = folderPicker[0].Path.LocalPath;
		}


		song.Downloading = true;
		song.DownloadPath = Path.Combine(path, song.FileName);

		OnPropertyChanged(nameof(Songs));

		_ = Task.Run(async () => await _songService.DownloadSong(song, song.DownloadPath));

		OnPropertyChanged(nameof(Songs));
	}

	[RelayCommand]
	private void OpenFile(Song song)
	{
		if(File.Exists(song.DownloadPath))
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = song.DownloadPath,
				UseShellExecute = true
			});
		}
	}

	[RelayCommand]
	private void OpenLink(Song song)
	{
		Process.Start(new ProcessStartInfo()
		{
			FileName = string.Format("https://pillowcase.su/f/{0}", song.Key),
			UseShellExecute = true
		});
	}

	[RelayCommand]
	private async Task CopyLink(Song song)
	{
		if(_mainWindow is not null && _mainWindow.Clipboard is not null)
			await _mainWindow.Clipboard.SetTextAsync(string.Format("https://pillowcase.su/f/{0}", song.Key));
	}

	private void SearchSongs(string query)
	{
		Songs.Clear();
		Songs.Populate(_songService.SearchSongs(query));
	}
}
