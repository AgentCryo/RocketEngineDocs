using Microsoft.AspNetCore.Components;

namespace RocketEngineDocs.Components;

public class SectionBuilder
{
    private readonly string _title;
    private readonly List<RenderFragment> _content = new();

    public SectionBuilder(string title)
    {
        _title = title;
    }

    public SectionBuilder Table(Action<TableBuilder> build)
    {
        var table = new TableBuilder();
        build(table);
        _content.Add(table.Build());
        return this;
    }
    
    public SectionBuilder CodeBlock(string code)
    {
        _content.Add(b =>
        {
            b.OpenComponent<CodeBlock>(0);
            b.AddAttribute(1, "ChildContent", (RenderFragment)(builder =>
            {
                builder.AddContent(0, code);
            }));
            b.CloseComponent();
        });

        return this;
    }

    public RenderFragment Build() => b =>
    {
        b.OpenElement(0, "h2");
        b.AddContent(1, _title);
        b.CloseElement();

        foreach (var c in _content)
            b.AddContent(2, c);
    };
}
