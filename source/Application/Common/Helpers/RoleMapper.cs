using FSP.Api.Domain.Constants;

namespace FSP.Api.Application.Common.Helpers
{
    public static class RoleMapper
    {
        public static string ToFrontendRole(string role) => role switch
        {
            ProfileRoles.Admin => "admin",
            ProfileRoles.Author => "author",
            ProfileRoles.Reader => "reader",
            _ => "reader"
        };

        public static string ToDbRole(string frontendRole) => frontendRole switch
        {
            "admin" => ProfileRoles.Admin,
            "author" => ProfileRoles.Author,
            "reader" => ProfileRoles.Reader,
            _ => ProfileRoles.Reader
        };
    }
}
