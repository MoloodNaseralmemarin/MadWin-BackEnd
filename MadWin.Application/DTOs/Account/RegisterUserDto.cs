namespace MadWin.Application.DTOs.Account
{
    public record RegisterUserDto(
     string FirstName,
     string LastName,
     string CellPhone,
     string TelPhone,
     string Username,
     string Password,
     string ConfirmPassword,
     string Address
 )
    {
        public string FullName => $"{FirstName} {LastName}";
    }

}
