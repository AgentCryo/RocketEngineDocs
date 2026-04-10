using Microsoft.AspNetCore.Components;

namespace RocketEngineDocs.Components;

public class TableBuilder
{
    readonly List<(string Type, string Member, string Description)> _rows = new();

    public TableBuilder Row(string type, string member, string description)
    {
        _rows.Add((type, member, description));
        return this;
    }

    public RenderFragment Build() => b =>
    {
        var seq = 0;

        b.OpenElement(seq++, "div");
        b.AddAttribute(seq++, "class", "doc-table-wrapper");

        // Table
        b.OpenElement(seq++, "table");
        b.AddAttribute(seq++, "class", "doc-table");

        // === HEADER ROW ===
        b.OpenElement(seq++, "tr");

        b.OpenElement(seq++, "th");
        b.AddContent(seq++, "Type");
        b.CloseElement();

        b.OpenElement(seq++, "th");
        b.AddContent(seq++, "Name");
        b.CloseElement();

        b.OpenElement(seq++, "th");
        b.AddContent(seq++, "Description");
        b.CloseElement();

        b.CloseElement(); // tr

        // === DATA ROWS ===
        foreach (var (type, member, desc) in _rows)
        {
            b.OpenElement(seq++, "tr");

            b.OpenElement(seq++, "td");
            b.AddContent(seq++, type);
            b.CloseElement();

            b.OpenElement(seq++, "td");
            b.AddContent(seq++, member);
            b.CloseElement();

            b.OpenElement(seq++, "td");
            b.AddContent(seq++, desc);
            b.CloseElement();

            b.CloseElement(); // tr
        }

        b.CloseElement(); // table
        b.CloseElement(); // wrapper
    };
}