using System.Text.Json;
using Pdr.Api.Constants;
using Pdr.Api.Services.Templating;
using Pdr.Api.Models.Audit;

namespace Pdr.Api.Services.Audit.Mappers;

public class UserLoggedInMapper : IAuditTemplateMapper
{
    public string TemplateKey => nameof(AuditActionEnum.UserLoggedIn);

    private record UserLoggedInData(int UserId);
    public void CollectIds(string data, BatchLoadContext context)
    {
        var payload = JsonSerializer.Deserialize<UserLoggedInData>(data);
        if (payload is null) return;

        context.UserIds.Add(payload.UserId);
    }


    public IDictionary<string, object?> Map(string data, BatchLoadContext context, int? actorUserId = null)
    {
        var payload = JsonSerializer.Deserialize<UserLoggedInData>(data)
            ?? throw new InvalidOperationException(AuditConstants.Messages.ParsePayloadFailed(TemplateKey));

        var user = context.Users.GetValueOrDefault(payload.UserId);

        return new Dictionary<string, object?>
        {
            ["userName"] = user?.FullName
        };
    }
}
