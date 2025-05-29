namespace BookClub.Api.Models
{
    public record TokenRequestRole(string Role);
    public record TokenRequestPerms(string[] Permissions);
}
