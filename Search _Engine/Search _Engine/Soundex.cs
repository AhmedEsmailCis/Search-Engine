using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Search__Engine
{
    class Soundex
    {
        public static string get_soundex(string word)
        {
            const int MaxSoundexCodeLength = 4;

            var soundexWord = new StringBuilder();
            var previousWasHOrW = false;

            word = Regex.Replace(
                word == null ? string.Empty : word.ToUpper(),
                    @"[^\w\s]",
                        string.Empty);

            if (string.IsNullOrEmpty(word))
                return string.Empty.PadRight(MaxSoundexCodeLength, '0');

            soundexWord.Append(word.First());

            for (var i = 1; i < word.Length; i++)
            {
                var numberCharForCurrentLetter =
                    GetCharNumberForLetter(word[i]);

                if (i == 1 &&
                        numberCharForCurrentLetter ==
                            GetCharNumberForLetter(soundexWord[0]))
                    continue;

                if (soundexWord.Length > 2 && previousWasHOrW &&
                        numberCharForCurrentLetter ==
                            soundexWord[soundexWord.Length - 2])
                    continue;

                if (soundexWord.Length > 0 &&
                        numberCharForCurrentLetter ==
                            soundexWord[soundexWord.Length - 1])
                    continue;

                soundexWord.Append(numberCharForCurrentLetter);

                previousWasHOrW = "HW".Contains(word[i]);
            }

            return soundexWord
                    .Replace("0", string.Empty)
                        .ToString()
                            .PadRight(MaxSoundexCodeLength, '0')
                                .Substring(0, MaxSoundexCodeLength);
        }

        private static char GetCharNumberForLetter(char letter)
        {
            if ("AIOUHWY".Contains(letter)) return '0';
            if ("BFPV".Contains(letter)) return '1';
            if ("CGJKQSXZ".Contains(letter)) return '2';
            if ("DT".Contains(letter)) return '3';
            if ('L' == letter) return '4';
            if ("MN".Contains(letter)) return '5';
            if ('R' == letter) return '6';
            else
                return '0';
        }
    }
}
