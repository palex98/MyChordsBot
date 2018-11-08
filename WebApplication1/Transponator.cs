using System;
using System.Collections.Generic;

namespace WebApplication1
{
    class Tranponator
    {
        string text { set; get; }
        Dictionary<string, string> UpDictionary = new Dictionary<string, string>();
        Dictionary<string, string> DownDictionary = new Dictionary<string, string>();

        public Tranponator(string text)
        {
            this.text = text;

            //------------------------
            UpDictionary.Add("Ab", "Qab");
            UpDictionary.Add("Bb", "Qbb");
            UpDictionary.Add("Db", "Qdb");
            UpDictionary.Add("Eb", "Qeb");
            UpDictionary.Add("Gb", "Qgb");
            UpDictionary.Add("A&", "Qa");
            UpDictionary.Add("B&", "Qb");
            UpDictionary.Add("C&", "Qc");
            UpDictionary.Add("D&", "Qd");
            UpDictionary.Add("E&", "Qe");
            UpDictionary.Add("F&", "Qf");
            UpDictionary.Add("G&", "Qg");
            //--------
            UpDictionary.Add("A#", "Ra#");
            UpDictionary.Add("C#", "Rc#");
            UpDictionary.Add("D#", "Rd#");
            UpDictionary.Add("F#", "Rf#");
            UpDictionary.Add("G#", "Rg#");
            UpDictionary.Add("A", "Ra");
            UpDictionary.Add("B", "Rb");
            UpDictionary.Add("C", "Rc");
            UpDictionary.Add("D", "Rd");
            UpDictionary.Add("E", "Re");
            UpDictionary.Add("F", "Rf");
            UpDictionary.Add("G", "Rg");
            //------------------------Transponation
            UpDictionary.Add("Qab", "^a");
            UpDictionary.Add("Qbb", "^b");
            UpDictionary.Add("Qdb", "^d");
            UpDictionary.Add("Qeb", "^e");
            UpDictionary.Add("Qgb", "^g");
            UpDictionary.Add("Qe", "^f");
            UpDictionary.Add("Qb", "^c");
            UpDictionary.Add("Qc", "^db");
            UpDictionary.Add("Qd", "^eb");
            UpDictionary.Add("Qf", "^gb");
            UpDictionary.Add("Qa", "^bb");
            UpDictionary.Add("Qg", "^ab");
            //-------------
            UpDictionary.Add("Ra#", "^Б");
            UpDictionary.Add("Ra", "^А#");
            UpDictionary.Add("Rb", "^Ц");
            UpDictionary.Add("Rc#", "^Д");
            UpDictionary.Add("Rc", "^Ц#");
            UpDictionary.Add("Rd#", "^Е");
            UpDictionary.Add("Rd", "^Д#");
            UpDictionary.Add("Re", "^Ф");
            UpDictionary.Add("Rf#", "^Ж");
            UpDictionary.Add("Rf", "^Ф#");
            UpDictionary.Add("Rg#", "^А");
            UpDictionary.Add("Rg", "^Ж#");
            //----------------------Normalization
            UpDictionary.Add("^ab", "*Ab*");
            UpDictionary.Add("^bb", "*Bb*");
            UpDictionary.Add("^gb", "*Gb*");
            UpDictionary.Add("^db", "*Db*");
            UpDictionary.Add("^eb", "*Eb*");
            UpDictionary.Add("^f", "*F*");
            UpDictionary.Add("^a", "*A&*");
            UpDictionary.Add("^b", "*B&*");
            UpDictionary.Add("^d", "*D&*");
            UpDictionary.Add("^e", "*E&*");
            UpDictionary.Add("^g", "*G&*");
            UpDictionary.Add("^c", "*C&*");
            UpDictionary.Add("^Б", "*B*");
            UpDictionary.Add("^А#", "*A#*");
            UpDictionary.Add("^А", "*A*");
            UpDictionary.Add("^Ц#", "*C#*");
            UpDictionary.Add("^Ц", "*C*");
            UpDictionary.Add("^Д#", "*D#*");
            UpDictionary.Add("^Д", "*D*");
            UpDictionary.Add("^Е", "*E*");
            UpDictionary.Add("^Ф#", "*F#*");
            UpDictionary.Add("^Ф", "*F*");
            UpDictionary.Add("^Ж#", "*G#*");
            UpDictionary.Add("^Ж", "*G*");
            //&******************DOWN
            DownDictionary.Add("Ab", "Qab");
            DownDictionary.Add("Bb", "Qbb");
            DownDictionary.Add("Db", "Qdb");
            DownDictionary.Add("Eb", "Qeb");
            DownDictionary.Add("Gb", "Qgb");
            DownDictionary.Add("A&", "Qa");
            DownDictionary.Add("B&", "Qb");
            DownDictionary.Add("C&", "Qc");
            DownDictionary.Add("D&", "Qd");
            DownDictionary.Add("E&", "Qe");
            DownDictionary.Add("F&", "Qf");
            DownDictionary.Add("G&", "Qg");
            //--------
            DownDictionary.Add("A#", "Ra#");
            DownDictionary.Add("C#", "Rc#");
            DownDictionary.Add("D#", "Rd#");
            DownDictionary.Add("F#", "Rf#");
            DownDictionary.Add("G#", "Rg#");
            DownDictionary.Add("A", "Ra");
            DownDictionary.Add("B", "Rb");
            DownDictionary.Add("C", "Rc");
            DownDictionary.Add("D", "Rd");
            DownDictionary.Add("E", "Re");
            DownDictionary.Add("F", "Rf");
            DownDictionary.Add("G", "Rg");
            //---------------Transponation
            DownDictionary.Add("Qab", "^g&");
            DownDictionary.Add("Qgb", "^f&");
            DownDictionary.Add("Qeb", "^d&");
            DownDictionary.Add("Qdb", "^c&");
            DownDictionary.Add("Qbb", "^a&");
            DownDictionary.Add("Rg#", "^G");
            DownDictionary.Add("Rf#", "^F");
            DownDictionary.Add("Rd#", "^D");
            DownDictionary.Add("Ra#", "^A");
            DownDictionary.Add("Rc#", "^C");
            DownDictionary.Add("Rg", "^F#");
            DownDictionary.Add("Qg", "^gb");
            DownDictionary.Add("Qf", "^e&");
            DownDictionary.Add("Qe", "^eb");
            DownDictionary.Add("Qd", "^db");
            DownDictionary.Add("Qc", "^b&");
            DownDictionary.Add("Qb", "^bb");
            DownDictionary.Add("Qa", "^ab");
            DownDictionary.Add("Rf", "^E");
            DownDictionary.Add("Re", "^D#");
            DownDictionary.Add("Rd", "^C#");
            DownDictionary.Add("Rc", "^B");
            DownDictionary.Add("Rb", "^A#");
            DownDictionary.Add("Ra", "^G#");
            //-----------------------Normalization
            DownDictionary.Add("^g&", "*G&*");
            DownDictionary.Add("^f&", "*F&*");
            DownDictionary.Add("^d&", "*D&*");
            DownDictionary.Add("^c&", "*C&*");
            DownDictionary.Add("^a&", "*A&*");
            DownDictionary.Add("^gb", "*Db*");
            DownDictionary.Add("^e&", "*E&*");
            DownDictionary.Add("^eb", "*Eb*");
            DownDictionary.Add("^db", "*Db*");
            DownDictionary.Add("^b&", "*B&*");
            DownDictionary.Add("^bb", "*Bb*");
            DownDictionary.Add("^ab", "*Ab*");
            DownDictionary.Add("^G", "*G*");
            DownDictionary.Add("^F#", "*F#*");
            DownDictionary.Add("^F", "*F*");
            DownDictionary.Add("^E", "*E*");
            DownDictionary.Add("^D#", "*D#*");
            DownDictionary.Add("^D", "*D*");
            DownDictionary.Add("^C#", "*C#*");
            DownDictionary.Add("^C", "*C*");
            DownDictionary.Add("^B", "*B*");
            DownDictionary.Add("^A#", "*A#*");
            DownDictionary.Add("^A", "*A*");
            DownDictionary.Add("^G#", "*G#*");
        }

        public string WorkUp()
        {
            foreach (var chord in UpDictionary)
            {
                text = text.Replace(chord.Key, chord.Value);
            }
            string stone = text[text.Length - 3].ToString() + text[text.Length - 2].ToString() + text[text.Length - 1].ToString();
            int itone;
            Int32.TryParse(stone, out itone);
            if (text.Length > 4056)
            {
                text = text.Substring(0, 4056);
                text = text + "\n\n" + "🔵 Тональность: " + itone.ToString();
            }
            if (itone != 0)
            {
                if (itone >= 10)
                {
                    text = text.Substring(0, text.Length - 3);
                }
                else
                {
                    text = text.Substring(0, text.Length - 2);
                }
            }
            else if (itone == 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            itone++;
            if (itone > 11) itone = 0;
            if (itone > 0)
            {
                return text.Replace("&", "") + "*+" + itone.ToString() + "*";
            }
            else if (itone <= 0)
            {
                return text.Replace("&", "") + "*" + itone.ToString() + "*";
            }
            return text.Replace("&", "");
        }

        public string WorkDown()
        {
            foreach (var chord in DownDictionary)
            {
                text = text.Replace(chord.Key, chord.Value);
            }
            string stone = text[text.Length - 3].ToString() + text[text.Length - 2].ToString() + text[text.Length - 1].ToString();
            int itone;
            Int32.TryParse(stone, out itone);
            if (text.Length > 4056)
            {
                text = text.Substring(0, 4056);
                text = text + "\n\n" + "🔵 Тональность: " + itone.ToString();
            }
            if (itone != 0)
            {
                if (itone <= -10)
                {
                    text = text.Substring(0, text.Length - 3);
                }
                else
                {
                    text = text.Substring(0, text.Length - 2);
                }
            }
            else if (itone == 0)
            {
                text = text.Substring(0, text.Length - 1);
            }
            itone--;
            if (itone < -11) itone = 0;
            if (itone > 0)
            {
                return text.Replace("&", "") + "*+" + itone.ToString() + "*";
            }
            else if (itone <= 0)
            {
                return text.Replace("&", "") + "*" + itone.ToString() + "*";
            }
            return text.Replace("&", "");
        }
    }
}
