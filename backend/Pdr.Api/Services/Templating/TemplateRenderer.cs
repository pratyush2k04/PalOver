namespace Pdr.Api.Services.Templating;

public class TemplateRenderer
{
    public string Render(string template, IDictionary<string, object?> values)
    {
        foreach (var (key, value) in values)
        {
            template = template.Replace($"{{{key}}}", value?.ToString() ?? string.Empty);
        }

        return template;
    }
}
