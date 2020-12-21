using System.Threading.Tasks;

namespace Trustly.Managers
{
	public interface IRequestManager
	{
		Task<string> DownloadStringAsync(string url);
	}
}