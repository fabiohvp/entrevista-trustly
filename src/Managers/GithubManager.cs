using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trustly.Models;

namespace Trustly.Managers
{
	public class GithubManager : IGithubManager
	{
		private const string GITHUB = "https://github.com";
		private const string GITHUB_FILE = "https://raw.githubusercontent.com";
		private const string HREF = "href=\"";

		private IRequestManager _requestManager;
		private string _userRepository;

		public GithubManager(IRequestManager requestManager)
		{
			_requestManager = requestManager;
		}

		public void Configure(string username = "fabiohvp", string repository = "livraria")
		{
			_userRepository = $"/{username}/{repository}";
		}

		public async Task<IEnumerable<GitFile>> GetFiles(IEnumerable<Link> links)
		{
			var gitFiles = new ConcurrentBag<GitFile>();
			var folders = links.Where(o => o.FileType == FileType.Folder)
				.ToList();

			var tasksFolders = folders.Select(async link =>
				{
					var _gitFiles = await ReadFolderRecursively(link);
					foreach (var gitFile in _gitFiles)
					{
						gitFiles.Add(gitFile);
					}
				});

			var files = links.Where(o => o.FileType == FileType.File)
				.ToList();

			var tasksFiles = files.Select(async link =>
				{
					var _gitFile = await ReadFile(link.Url);
					gitFiles.Add(_gitFile);
				});

			await Task.WhenAll(tasksFolders.Concat(tasksFiles));
			return gitFiles;
		}

		public async Task<IEnumerable<Link>> GetLinks(string folderPath = default)
		{
			if (folderPath == default)
			{
				folderPath = _userRepository;
			}

			folderPath = $"{GITHUB}{folderPath}";
			var html = await _requestManager.DownloadStringAsync(folderPath);

			//split by this css that is unique to files/folder links
			var links = html.Split("js-navigation-open link-gray-dark")
				.Skip(1) //ignore first item because it doesn't have links we are interested in
				.Select(o => //get the file/folder path
				{
					var partialHref = o.Substring(o.IndexOf(HREF) + HREF.Length);
					var href = partialHref.Substring(0, partialHref.IndexOf("\""));

					return new Link
					{
						FileType = href.StartsWith($"{_userRepository}/tree", StringComparison.InvariantCultureIgnoreCase) ? FileType.Folder : FileType.File,
						Url = href.Replace($"{_userRepository}/blob/", $"{_userRepository}/", StringComparison.InvariantCultureIgnoreCase)
					};
				})
				.ToList();
			return links;
		}

		public async Task<GitFile> ReadFile(string blobPath)
		{
			blobPath = $"{GITHUB_FILE}{blobPath}";
			var fileContents = await _requestManager.DownloadStringAsync(blobPath);

			return new GitFile
			{
				Filename = Path.GetFileName(blobPath),
				Size = ASCIIEncoding.UTF8.GetByteCount(fileContents),
				LinesCount = fileContents.Select(o => o == '\n').Count()
			};
		}

		public async Task<IEnumerable<GitFile>> ReadFolderRecursively(Link link)
		{
			var links = await GetLinks(link.Url);
			return await GetFiles(links);
		}

		// private static async Task<GitFile> ReadFile(string url)
		// {
		// 	url = $"https://raw.githubusercontent.com{url}";
		// 	var request = (HttpWebRequest)WebRequest.Create(url);
		// 	request.Method = "HEAD";

		// 	using (var response = (HttpWebResponse)await request.GetResponseAsync())
		// 	{
		// 		return new GitFile
		// 		{
		// 			Filename = Path.GetFileName(url),
		// 			Size = response.ContentLength
		// 		};
		// 	}
		// }
	}
}