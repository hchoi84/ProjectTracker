using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProjectTracker.Utilities
{
  public static class StringExtension
  {
    public static String TrimAndTitleCase(this String value)
    {
      TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
      string newString = textInfo.ToTitleCase(value.Trim());

      RegexOptions options = RegexOptions.None;
      Regex regex = new Regex("[ ]{2,}", options);
      return regex.Replace(newString, " ");
    }
  }
}