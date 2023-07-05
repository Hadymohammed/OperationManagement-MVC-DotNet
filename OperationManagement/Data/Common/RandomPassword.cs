using System.Text;

namespace OperationManagement.Data.Common
{
    public static class RandomPassword
    {
        public static string GenerateRandomPassword(int length)
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numericChars = "0123456789";
            const string specialChars = "@";
            var random = new Random();
            var password = new StringBuilder();
            bool hasUppercase = false;
            bool hasNumeric = false;

            // Add at least one uppercase letter
            password.Append(uppercaseChars[random.Next(uppercaseChars.Length)]);
            hasUppercase = true;

            // Add at least one numeric character
            password.Append(numericChars[random.Next(numericChars.Length)]);
            hasNumeric = true;

            // Add remaining characters
            for (int i = 2; i < length; i++)
            {
                var validChars = lowercaseChars + uppercaseChars + numericChars + specialChars;
                password.Append(validChars[random.Next(validChars.Length)]);

                // Check if password contains at least one uppercase letter and one numeric character
                if (uppercaseChars.IndexOf(password[i]) != -1)
                {
                    hasUppercase = true;
                }
                else if (numericChars.IndexOf(password[i]) != -1)
                {
                    hasNumeric = true;
                }
            }

            // If the password does not contain at least one uppercase letter or one numeric character,
            // regenerate the password
            if (!hasUppercase || !hasNumeric)
            {
                return GenerateRandomPassword(length);
            }

            return password.ToString();
        }
    }
}
