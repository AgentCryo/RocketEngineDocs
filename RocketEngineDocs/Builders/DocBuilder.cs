using System.ComponentModel;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Components;
using RocketEngineDocs.Builders;

namespace RocketEngineDocs.Components;

public class DocBuilder
{
    public static string RERL(string page) => $"/RERL/{page}";
    public static string RCS(string page) => $"/RCS/{page}";
    
    public string DisplayName = "Unset Title";
    public string WebAddress = "/";
    readonly List<RenderFragment> _parts = [];
    string _namespace;
    string _library;
    readonly List<(string label, string url)> _implements = [];
    readonly List<(string label, string url)> _dependencies = [];
    readonly List<(string label, string url)> _usedBy = [];

    public static DocBuilder Create(string webAddress)
    {
        return new DocBuilder(webAddress);
    }

    DocBuilder(string webAddress) => WebAddress = webAddress;

    public DocBuilder Title(string text)
    {
        DisplayName = text;
        _parts.Add(b =>
        {
            b.OpenElement(0, "h1");
            b.AddContent(1, text);
            b.CloseElement();
        });
        return this;
    }

    public DocBuilder Subtitle(string text)
    {
        _parts.Add(b =>
        {
            b.OpenElement(0, "h4");
            b.AddContent(1, text);
            b.CloseElement();
        });
        return this;
    }

    public DocBuilder Paragraph(string text)
    {
        _parts.Add(b =>
        {
            b.OpenElement(0, "p");
            b.AddContent(1, text);
            b.CloseElement();
        });
        return this;
    }

    public DocBuilder Section(string title, Action<SectionBuilder> build)
    {
        var section = new SectionBuilder(title);
        build(section);
        _parts.Add(section.Build());    
        return this;
    }

    public DocBuilder CodeBlock(string code)
    {
        _parts.Add(b =>
        {
            b.OpenComponent<CodeBlock>(0);
            b.AddAttribute(1, "ChildContent", (RenderFragment)(builder => builder.AddContent(0, code)));
            b.CloseComponent();
        });

        return this;
    }
    
    public DocBuilder Members(Action<MembersBuilder> build)
    {
        var mb = new MembersBuilder();
        build(mb);

        _parts.Add(MembersBuilder.BuildMembersTable(mb.Members, WebAddress));

        foreach (var member in mb.Members)
            _parts.Add(MembersBuilder.BuildMemberSection(member));

        return this;
    }
    
    public DocBuilder Namespace(string ns)
    {
        _namespace = ns;
        return this;
    }

    public DocBuilder Library(string lib)
    {
        _library = lib;
        return this;
    }

    public DocBuilder Implements(params (string label, string url)[] interfaces)
    {
        _implements.AddRange(interfaces);
        return this;
    }

    public DocBuilder DependsOn(params (string label, string url)[] deps)
    {
        _dependencies.AddRange(deps);
        return this;
    }

    public DocBuilder UsedBy(params (string label, string url)[] users)
    {
        _usedBy.AddRange(users);
        return this;
    }

    public RenderFragment Build() => b =>
    {
        // Metadata header
        b.OpenElement(0, "div");
        b.AddAttribute(1, "class", "doc-meta");

        if (!string.IsNullOrEmpty(_namespace))
            b.AddMarkupContent(2, $"<p><strong>Namespace:</strong> {_namespace}</p>");

        if (!string.IsNullOrEmpty(_library))
            b.AddMarkupContent(3, $"<p><strong>Library:</strong> {_library}</p>");

        if (_implements.Count > 0)
        {
            var links = string.Join(
                " | ",
                _implements.Select(i => $"<a class=\"re-link\" href=\"{i.url}\">{i.label}</a>"
                )
            );

            b.AddMarkupContent(4, $"<p><strong>Implements:</strong> {links}</p>");
        }

        if (_dependencies.Count > 0)
        {
            var links = string.Join(
                " | ",
                _dependencies.Select(d =>
                    $"<a class=\"re-link\" href=\"{d.url}\">{d.label}</a>"
                )
            );

            b.AddMarkupContent(7, $"<p><strong>Depends on:</strong> {links}</p>");
        }

        if (_usedBy.Count > 0)
        {
            var links = string.Join(
                " | ",
                _usedBy.Select(u =>
                    $"<a class=\"re-link\" href=\"{u.url}\">{u.label}</a>"
                )
            );

            b.AddMarkupContent(10, $"<p><strong>Used by:</strong> {links}</p>");
        }
        
        b.OpenComponent<HighlightLoader>(10000);
        b.CloseComponent();
        
        b.CloseElement();

        // Actual content
        foreach (var part in _parts)
            b.AddContent(13, part);
    };
}
