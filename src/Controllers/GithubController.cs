using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trustly.Models;
using Trustly.Managers;

namespace Trustly.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class GithubController : ControllerBase
	{
		private readonly ILogger<GithubController> _logger;
		private readonly IGithubManager _githubManager;

		public GithubController(ILogger<GithubController> logger, IGithubManager githubManager)
		{
			_logger = logger;
			_githubManager = githubManager;
		}

		[HttpGet]
		public async Task<IEnumerable<GitFile>> Get(string username = "fabiohvp", string repository = "svelte-table")
		{
			_githubManager.Configure(username, repository);
			var links = await _githubManager.GetLinks();
			return await _githubManager.GetFiles(links);
		}
	}
}
