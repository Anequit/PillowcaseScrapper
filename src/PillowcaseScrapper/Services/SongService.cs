using PillowcaseScrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PillowcaseScrapper.Services;

internal class SongService
{
	private readonly HttpClient _client;
	private readonly string _api = "https://api.pillowcase.su/api/get/{0}";

	public SongService()
	{
		_client = new HttpClient(new SocketsHttpHandler()
		{
			PooledConnectionLifetime = TimeSpan.FromMinutes(15)
		});

		Songs = [];
	}

	public List<Song> Songs { get; set; }

	public IEnumerable<Song> SearchSongs(string query) => Songs.Where(song => song.FileName.Contains(query, StringComparison.OrdinalIgnoreCase));

	public async Task LoadSongs()
	{
		Songs.Clear();

		string[] indexResult = (await _client.GetStringAsync("https://pillowcase.su/onlyfiles.txt")).Split('\n');

		for(int i = 0; i < indexResult.Length; i++)
		{
			// Split by [
			string[] firstHalf = indexResult[i].Split('[');

			// Parse filename
			string filename = firstHalf.First().Trim();

			// Parse filesize and type
			string[] secondHalf = firstHalf.Last().Split(']');

			// Parse url
			string url = secondHalf[1].Split(' ').Last();

			Songs.Add(new Song()
			{
				FileName = filename,
				Size = secondHalf[0],
				Key = url.Split('/').Last()
			});
		}
	}

	public async Task DownloadSong(Song song, string path)
	{
		using(FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using(Stream downloadStream = await _client.GetStreamAsync(string.Format("https://api.pillowcase.su/api/get/{0}", song.Key)))
			{
				await downloadStream.CopyToAsync(fileStream);
			}
		}

		song.Downloaded = true;
	}
}
