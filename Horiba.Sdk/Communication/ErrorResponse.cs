namespace Horiba.Sdk.Communication;

// TODO is it possible to have an error and results at the same time?
public record ErrorResponse(int Id, string CommandName, Dictionary<string, object> Results, List<string> Errors) : Response(Id, CommandName, Results)
{
    public List<string> Errors { get; set; } = Errors;
}