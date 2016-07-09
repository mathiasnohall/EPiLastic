using System.Configuration;

namespace EpiLastic.Helpers
{
    public static class IndexAlias
    {

        public static string GetAlias(string language)
        {
            var alias = ConfigurationManager.AppSettings["IndexAliasName"];
            return alias + "_" + language;
        }
    }
}
