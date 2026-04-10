using Microsoft.AspNetCore.Components;
using RocketEngineDocs.Layout;

namespace RocketEngineDocs.Components;

public class DevlogHeader
{
    public string Title { get; set; } = "";
    public string Version { get; set; } = "";
    public string Summary { get; set; } = "";
    public string Url { get; set; } = "";
    public int Order { get; set; }
    public bool IsWip { get; set; }
    public string[] Libraries { get; set; } = Array.Empty<string>();

    public DateTime Date { get; set; }
}

public abstract class DevlogBase : ComponentBase
{
    public abstract DevlogHeader Header { get; }

    // Called by Blazor when the page is visited
    protected override void OnInitialized()
    {
        RegisterDevlog(Header);
    }

    // Called by Program.cs at startup
    public static void RegisterDevlog(DevlogHeader header)
    {
        DevlogRegistry.Register(header);
        NavMenu.NavRegistry.RegisterStatic(header.Version + " " + header.Title, header.Url, header.Order);
    }
}

public static class DevlogRegistry
{
    public static List<DevlogHeader> Devlogs { get; private set; } = new();

    public static void Register(DevlogHeader header)
    {
        Devlogs.Add(header);

        Devlogs = Devlogs
            .OrderByDescending(d => d.IsWip)
            .ThenByDescending(d => d.Date)
            .ToList();
    }
}

