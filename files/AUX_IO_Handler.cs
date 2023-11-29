using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class AUX_IO_Handler
    {
        public string data_path { get; set; }
        public string xml_folder_path { get; set; }

        public AUX_IO_Handler()
        {
            data_path = @".\data\data.xml";

            xml_folder_path = @".\data\";

            check_paths();
        }

        private void check_paths()
        {
            if (!Directory.Exists(xml_folder_path))
            {
                Directory.CreateDirectory(xml_folder_path);
            }
            if (true)
            {

            }
        }

        public void delete_files(string folderpath)
        {
            string[] paths;

            paths = Directory.GetFiles(folderpath);

            foreach (string path in paths)
            {
                File.Delete(path);
            }
        }


        public void delete_files(string folderpath, string exception)
        {
            string[] paths;

            paths = Directory.GetFiles(folderpath);

            foreach (string path in paths)
            {
                if (path != exception)
                {
                    File.Delete(path);
                }                
            }
        }
    }
}