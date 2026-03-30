namespace SlopSlurp.Observability;

using System.Diagnostics;

public static class ActivitySources
{
    public static readonly ActivitySource Validation = new("slopslurp.validation");
}
