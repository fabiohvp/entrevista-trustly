using System.Collections.Generic;
using System.Threading.Tasks;
using Trustly.Models;

namespace Trustly.Managers
{
	public interface IGithubManager
	{
		void Configure(string username, string repository);
		Task<IEnumerable<GitFile>> GetFiles(IEnumerable<Link> links);
		Task<IEnumerable<Link>> GetLinks(string folderPath = default);
		Task<GitFile> ReadFile(string blobPath);
		Task<IEnumerable<GitFile>> ReadFolderRecursively(Link link);
	}
}