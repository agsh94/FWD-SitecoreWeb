/*9fbef606107a605d69c0edbcd8029e5d*/
namespace FWD.Foundation.SitecoreExtensions.RequestValidation
{
    public static class CheckForDangerousString
    {
        private static readonly char[] invalidCharacters = new char[] { '<', '>', '\\', '%', '*' };

        internal static bool IsDangerousString(string str)
        {
            int index = str.IndexOfAny(invalidCharacters);
            if (index >= 0)
            {
                return true;
            }
            return false;
        }
    }
}