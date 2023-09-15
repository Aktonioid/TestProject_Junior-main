using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClinicApp
{
    static public class RegexpHelper
    {
        //checking if a patient name is correct
        public static bool IsNameValid(this string name)
        {
            Regex regex = new Regex(@"([А-ЯЁ][а-яё]+[\-\s]?){3,}");
            return regex.IsMatch(name);
        }

    }
}
