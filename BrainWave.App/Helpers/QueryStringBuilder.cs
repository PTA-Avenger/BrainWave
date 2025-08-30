using System.Web;
namespace BrainWave.APP.Helpers;
public static class QueryStringBuilder
{
    public static string WithQuery(this string path, IDictionary<string, string?> query)
    {
        var nvc = HttpUtility.ParseQueryString(string.Empty);
        foreach (var kv in query)
            if (!string.IsNullOrWhiteSpace(kv.Value)) nvc[kv.Key] = kv.Value;
        var qs = nvc.ToString();
        return string.IsNullOrEmpty(qs) ? path : $"{path}?{qs}";
    }
}