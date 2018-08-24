using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageTool
{
    class jezyki
    {

        public List<string> select(string selected)
        {
            return takeStrings(selected);
        }

        private List<string> takeStrings(string language)
        {
            string line;
            List<string> lista = new List<string>();
            if (language == "")
            {
                language = "PL";
            }
            // Read the file and display it line by line. 
            System.IO.StreamReader file;
            try
            {
                 file = new System.IO.StreamReader(@"C:\ProgramData\Garage\" + language + ".txt");
            }
            catch (Exception)
            {
                try
                {
                    string path = System.IO.Directory.GetCurrentDirectory();
                    file = new System.IO.StreamReader($"{path}\\{language}.txt");
                }
                catch (Exception)
                {
                    return null;
                }
            }

            while ((line = file.ReadLine()) != null)
            {
                lista.Add(line);
            }

            file.Close();

            return lista;
        }

        public SortedDictionary<string, string> bindJezyki()
        {
            SortedDictionary<string, string> Settings = new SortedDictionary<string, string>
            {
                { "PL", "PL"},
                 { "UK", "UK"}
            };
            return Settings;
        }

    }
}
