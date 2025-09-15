using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commmon.Library
{
    public class Validator
    {
        //Validate stringtype.
        public bool IsNullOrEmpty(string value) => string.IsNullOrEmpty(value);
        public bool IsLengthInRange(string value, int min, int max) => !string.IsNullOrEmpty(value) && value.Length >= min && value.Length <= max;

        public bool IsValdEmail(string email) => !string.IsNullOrEmpty(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public bool IsValidPhoneNumber(string phoneNumber) => !string.IsNullOrEmpty(phoneNumber) && Regex.IsMatch(phoneNumber, @"^\+?[0-9]{7,15}$");

        public bool IsValidUrl(string url) => !string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out _);

        public bool ConstrainsOnlyLetters(string value) => !string.IsNullOrEmpty(value) && value.All(char.IsLetter);

        public bool ConstrainsOnlyDigits(string value) => !string.IsNullOrEmpty(value) && value.All(char.IsDigit);

        //validate numbertype
        public bool IsPositive(int number) => number > 0;
        public bool IsNegative(int number) => number < 0;
        public bool IsPercentage(double number) => number >= 0 && number <= 100;

        public bool HasTwoDecimalPlaces(decimal number) => decimal.Round(number, 2) == number;

        //validate date

        public bool IsPastDay(DateTime day) => day < DateTime.Now;

        public bool IsFuterDaay(DateTime day) => day > DateTime.Now;

        // Age compution
        public bool IsValidAge(DateTime birthday, int minAge, int maxAge)
        {
            var age = DateTime.Now.Year - birthday.Year;
            if (birthday > DateTime.Now.AddYears(-age)) age--;
            return age >= minAge && age <= maxAge;
        }

        // check the range of date in range start to end.
        public bool isWithinRange(DateTime date, DateTime start, DateTime end) => date >= start && date <= end;


    }
}
