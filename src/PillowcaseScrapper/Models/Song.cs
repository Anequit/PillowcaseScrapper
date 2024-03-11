using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace PillowcaseScrapper.Models;

public class Song : ObservableObject
{
	private bool _downloaded;
	private string _downloadPath;
	private bool _downloading;

	public Song()
	{
		_downloadPath = string.Empty;
	}

	public string FileName { get; init; } = string.Empty;
	public string Size { get; init; } = string.Empty;
	public string Key { get; init; }

	public bool Downloaded
	{
		get => _downloaded;
		set => SetProperty(ref _downloaded, value);
	}

	public bool Downloading
	{
		get => _downloading;
		set => SetProperty(ref _downloading, value);
	}

	public string DownloadPath
	{
		get => _downloadPath;
		set => SetProperty(ref _downloadPath, value);
	}
}
