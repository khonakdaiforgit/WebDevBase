

namespace MyApp.Application.Helper
{
    public static class UniqueCodeGenerator
    {
        private static readonly Random Random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GenerateUniqueCode(int length = 10)
        {
            char[] code = new char[length];
            for (int i = 0; i < length; i++)
            {
                code[i] = Chars[Random.Next(Chars.Length)];
            }
            return new string(code);
        }
    }
}
