using Microsoft.AspNetCore.Components;

namespace RocketEngineDocs.Components;

public class MembersBuilder
{
    public enum MemberKind { Field, Method, Class, Struct, Enum }

    public record DataField(string Type, string Name, string Summary);

    public record MemberInfo(
        MemberKind Kind,
        string? Type,
        string Name,
        string Summary,
        string? Url = null,
        List<DataField>? StructFields = null,
        Method? MethodDetails = null
    );

    public List<MemberInfo> Members { get; } = new();

    // ============================
    // FIELD
    // ============================
    public MembersBuilder Field(string type, string name, string summary)
    {
        Members.Add(new MemberInfo(MemberKind.Field, type, name, summary));
        return this;
    }

    // ============================
    // METHOD (with details)
    // ============================
    public MembersBuilder Method(string returnType, string nameWithParams, string summary, Action<Method> build)
    {
        var m = new Method();
        build(m);

        Members.Add(new MemberInfo(
            MemberKind.Method,
            returnType,
            nameWithParams,
            summary,
            null,
            null,
            m
        ));

        return this;
    }

    // ============================
    // METHOD (summary only)
    // ============================
    public MembersBuilder Method(string returnType, string nameWithParams, string summary)
    {
        Members.Add(new MemberInfo(
            MemberKind.Method,
            returnType,
            nameWithParams,
            summary
        ));

        return this;
    }

    // ============================
    // CLASS
    // ============================
    public MembersBuilder Class(string name, string summary, string url)
    {
        Members.Add(new MemberInfo(MemberKind.Class, null, name, summary, url));
        return this;
    }

    // ============================
    // STRUCT
    // ============================
    public MembersBuilder Struct(string name, string summary, Action<List<DataField>> buildFields)
    {
        var fields = new List<DataField>();
        buildFields(fields);

        Members.Add(new MemberInfo(
            MemberKind.Struct,
            null,
            name,
            summary,
            null,
            fields
        ));

        return this;
    }
    
    // ============================
    // STRUCT
    // ============================
    public MembersBuilder Enum(string name, string summary, Action<List<DataField>> buildFields)
    {
        var fields = new List<DataField>();
        buildFields(fields);

        Members.Add(new MemberInfo(
            MemberKind.Enum,
            null,
            name,
            summary,
            null,
            fields
        ));

        return this;
    }

    // ============================
    // TABLE
    // ============================
    public static RenderFragment BuildMembersTable(List<MemberInfo> members, string pagePath) => b =>
    {
        var seq = 0;

        b.OpenElement(seq++, "div");
        b.AddAttribute(seq++, "class", "doc-table-wrapper");

        b.OpenElement(seq++, "table");
        b.AddAttribute(seq++, "class", "doc-table");
        
        // === HEADER ROW === I hate this.............................AHHHHHH
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

        foreach (var m in members)
        {
            b.OpenElement(seq++, "tr");

            // Type
            b.OpenElement(seq++, "td");
            b.AddContent(seq++, m.Type ?? m.Kind.ToString());
            b.CloseElement();

            // Name with anchor
            b.OpenElement(seq++, "td");
            b.OpenElement(seq++, "a");
            b.AddAttribute(seq++, "href", $"{pagePath}#{SafeId(m.Name)}"); // \/ WWIUAHIKDHSUIghdokJAhdiouahodkj
            b.AddAttribute(seq++, "onclick", $"event.preventDefault(); location.hash='{SafeId(m.Name)}';");
            b.AddContent(seq++, m.Name);
            b.CloseElement();
            b.CloseElement();

            // Summary
            b.OpenElement(seq++, "td");
            b.AddContent(seq++, m.Summary);
            b.CloseElement();

            b.CloseElement(); // tr
        }

        b.CloseElement(); // table
        b.CloseElement(); // wrapper
    };

    // ============================
    // MEMBER SECTION RENDERING
    // ============================
    public static RenderFragment BuildMemberSection(MemberInfo m) => b =>
    {
        var seq = 0;

        switch (m.Kind)
        {
            // ============================
            // METHOD
            // ============================
            case MemberKind.Method:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "method-box");
                b.AddAttribute(seq++, "id", SafeId(m.Name));

                // Signature
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "member-signature");

                b.OpenElement(seq++, "pre");
                b.OpenElement(seq++, "code");
                b.AddAttribute(seq++, "class", "language-csharp");
                b.AddContent(seq++, $"{m.Type} {m.Name}");
                b.CloseElement();
                b.CloseElement();

                b.CloseElement(); // signature

                // Summary
                b.OpenElement(seq++, "p");
                b.AddContent(seq++, m.Summary);
                b.CloseElement();

                // Parameters
                if (m.MethodDetails?.Parameters?.Count > 0)
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "doc-block");

                    b.OpenElement(seq++, "h4");
                    b.AddContent(seq++, "Parameters:");
                    b.CloseElement();

                    foreach (var p in m.MethodDetails.Parameters)
                    {
                        b.OpenElement(seq++, "div");
                        b.AddAttribute(seq++, "class", "doc-line");

                        b.OpenElement(seq++, "span");
                        b.AddAttribute(seq++, "class", "doc-label");
                        b.AddContent(seq++, p.signature);
                        b.CloseElement();

                        b.OpenElement(seq++, "span");
                        b.AddAttribute(seq++, "class", "doc-text");
                        b.AddContent(seq++, p.text);
                        b.CloseElement();

                        b.CloseElement();
                    }

                    b.CloseElement();
                }

                // Returns
                if (!string.IsNullOrWhiteSpace(m.MethodDetails?.ReturnsText))
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "doc-block");

                    b.OpenElement(seq++, "h4");
                    b.AddContent(seq++, "Returns:");
                    b.CloseElement();

                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "doc-line");
                    b.AddContent(seq++, m.MethodDetails.ReturnsText);
                    b.CloseElement();

                    b.CloseElement();
                }

                // Exceptions
                if (m.MethodDetails?.Exceptions?.Count > 0)
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "doc-block");

                    b.OpenElement(seq++, "h4");
                    b.AddContent(seq++, "Exceptions:");
                    b.CloseElement();

                    foreach (var ex in m.MethodDetails.Exceptions)
                    {
                        b.OpenElement(seq++, "div");
                        b.AddAttribute(seq++, "class", "doc-line");
                        b.AddContent(seq++, ex);
                        b.CloseElement();
                    }

                    b.CloseElement();
                }

                b.CloseElement(); // method-box
                return;

            // ============================
            // FIELD
            // ============================
            case MemberKind.Field:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "method-box");
                b.AddAttribute(seq++, "id", SafeId(m.Name));

                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "member-signature");

                b.OpenElement(seq++, "pre");
                b.OpenElement(seq++, "code");
                b.AddAttribute(seq++, "class", "language-csharp");
                b.AddContent(seq++, $"{m.Type} {m.Name}");
                b.CloseElement();
                b.CloseElement();

                b.CloseElement();

                b.OpenElement(seq++, "p");
                b.AddContent(seq++, m.Summary);
                b.CloseElement();

                b.CloseElement();
                return;

            // ============================
            // CLASS
            // ============================
            case MemberKind.Class:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "method-box");
                b.AddAttribute(seq++, "id", SafeId(m.Name));

                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "member-signature");

                b.OpenElement(seq++, "pre");
                b.OpenElement(seq++, "code");
                b.AddAttribute(seq++, "class", "language-csharp");
                b.AddContent(seq++, $"class {m.Name}");
                b.CloseElement();
                b.CloseElement();

                b.CloseElement();

                b.OpenElement(seq++, "p");
                b.AddContent(seq++, m.Summary);
                b.CloseElement();

                if (m.Url is not null)
                {
                    b.OpenElement(seq++, "a");
                    b.AddAttribute(seq++, "href", m.Url);
                    b.AddContent(seq++, "View class documentation");
                    b.CloseElement();
                }

                b.CloseElement();
                return;

            // ============================
            // STRUCT
            // ============================
            case MemberKind.Struct:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "method-box");
                b.AddAttribute(seq++, "id", SafeId(m.Name));

                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "member-signature");

                b.OpenElement(seq++, "pre");
                b.OpenElement(seq++, "code");
                b.AddAttribute(seq++, "class", "language-csharp");
                b.AddContent(seq++, $"struct {m.Name}");
                b.CloseElement();
                b.CloseElement();

                b.CloseElement();

                b.OpenElement(seq++, "p");
                b.AddContent(seq++, m.Summary);
                b.CloseElement();

                if (m.StructFields is { Count: > 0 })
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "struct-table-wrapper");

                    b.OpenElement(seq++, "table");
                    b.AddAttribute(seq++, "class", "doc-table");

                    b.OpenElement(seq++, "tr");
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Type"); b.CloseElement();
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Name"); b.CloseElement();
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Description"); b.CloseElement();
                    b.CloseElement();

                    foreach (var f in m.StructFields)
                    {
                        b.OpenElement(seq++, "tr");
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Type); b.CloseElement();
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Name); b.CloseElement();
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Summary); b.CloseElement();
                        b.CloseElement();
                    }

                    b.CloseElement();
                    b.CloseElement();
                }

                b.CloseElement();
                return;
            
            // ============================
            // STRUCT
            // ============================
            case MemberKind.Enum:
                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "method-box");
                b.AddAttribute(seq++, "id", SafeId(m.Name));

                b.OpenElement(seq++, "div");
                b.AddAttribute(seq++, "class", "member-signature");

                b.OpenElement(seq++, "pre");
                b.OpenElement(seq++, "code");
                b.AddAttribute(seq++, "class", "language-csharp");
                b.AddContent(seq++, $"enum {m.Name}");
                b.CloseElement();
                b.CloseElement();

                b.CloseElement();

                b.OpenElement(seq++, "p");
                b.AddContent(seq++, m.Summary);
                b.CloseElement();

                if (m.StructFields is { Count: > 0 })
                {
                    b.OpenElement(seq++, "div");
                    b.AddAttribute(seq++, "class", "struct-table-wrapper");

                    b.OpenElement(seq++, "table");
                    b.AddAttribute(seq++, "class", "doc-table");

                    b.OpenElement(seq++, "tr");
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Type"); b.CloseElement();
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Name"); b.CloseElement();
                    b.OpenElement(seq++, "th"); b.AddContent(seq++, "Description"); b.CloseElement();
                    b.CloseElement();

                    foreach (var f in m.StructFields)
                    {
                        b.OpenElement(seq++, "tr");
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Type); b.CloseElement();
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Name); b.CloseElement();
                        b.OpenElement(seq++, "td"); b.AddContent(seq++, f.Summary); b.CloseElement();
                        b.CloseElement();
                    }

                    b.CloseElement();
                    b.CloseElement();
                }

                b.CloseElement();
                return;
        }
    };

    static string SafeId(string name)
    {
        var invalid = new[] { " ", "(", ")", ",", "<", ">", "[", "]" };
        foreach (var c in invalid)
            name = name.Replace(c, "-");
        return name;
    }
}
