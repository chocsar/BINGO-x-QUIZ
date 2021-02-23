namespace Utility
{
    public static class UtilityPass
    {
        private const string PASSWORD_CHARS =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GeneratePassword(int length = 15)
        {
            var sb = new System.Text.StringBuilder(length);
            var r = new System.Random();

            for (int i = 0; i < length; i++)
            {
                int pos = r.Next(PASSWORD_CHARS.Length);
                char c = PASSWORD_CHARS[pos];
                sb.Append(c);
            }

            return sb.ToString();
        }
    }

    public static class UtilityFirebase
    {
        public static string StringParse(string _data)
        {
            return _data.Trim();
        }
    }
}
