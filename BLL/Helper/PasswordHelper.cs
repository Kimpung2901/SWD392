using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Helper
{
    public class PasswordHelper
    {
        private static readonly Regex BCryptRegex =
       new(@"^\$(2[aby]|2x)\$\d{2}\$[./A-Za-z0-9]{53}$", RegexOptions.Compiled);

        public static bool LooksLikeBCrypt(string? value)
            => !string.IsNullOrEmpty(value) && BCryptRegex.IsMatch(value!);
    }
}
