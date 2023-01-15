using fourtynine.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;

namespace fourtynine.Pages.Account;

[Authorize]
public class Account : PageModel
{
    public async Task OnGet()
    {
        var accessToken = User.GetAccessToken();
        if (accessToken is null)
            return;

        var header = new ProductHeaderValue("fourtynine");
        var client = new GitHubClient(header);
        client.Credentials = new Credentials(accessToken);
        try
        {
            GitHubUser = await client.User.Get(User.Identity?.Name);
        }
        catch (ApiException e)
        {
            Console.WriteLine("The key is other than github?");
            Console.WriteLine(e.Message);
        }
    }
    
    // view model
    public record struct Row(string Type, string Value, bool IsImage);
    public record struct Table(string Name, IEnumerable<Row> Rows);

    private User? GitHubUser { get; set; }
    
    private IEnumerable<Row> ClaimsInfos => User.Claims
        .Where(c => !c.IsAccessToken())
        .Select(c => new Row(StripUri(c.Type), c.Value, IsImage: false));
    
    private Row[] GithubUserInfos => GitHubUser is null
        ? Array.Empty<Row>()
        : new[]
        {
            new Row(nameof(GitHubUser.Bio), GitHubUser.Bio, IsImage: false),
            new Row(nameof(GitHubUser.Location), GitHubUser.Location, IsImage: false),
            new Row("Avatar", GitHubUser.AvatarUrl, IsImage: true),
        };
    
    public IEnumerable<Table> Tables
    {
        get
        {
            yield return new("User Claims", ClaimsInfos);
            if (GitHubUser is not null)
                yield return new("GitHub Octokit Response", GithubUserInfos);
        }
    }
    
    const string Uri = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/";

    private string StripUri(string source)
    {
        return source.StartsWith(Uri)
            ? source[Uri.Length ..]
            : source;
    }
}
