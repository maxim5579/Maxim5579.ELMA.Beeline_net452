using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxim5579.ELMA.Beeline_net452
{
    class FileLogsWriter
    {
        public const string PathLogFile = @"C:\ELMA3-Express\MaximLogs\";
        private const string OnePartFileName = "PhoneEventLog_";
        private const string fileExtension = ".txt";

        //private string nowFileName;
        public FileLogsWriter()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(PathLogFile);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public async void addText(string str)
        {
            string fileName = PathLogFile + OnePartFileName + DateTime.Now.ToString("dd_MM_yyyy") + fileExtension;
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                await writer.WriteAsync(string.Format("{0}\r\n\r\n\r\n", str));
            }
        }

    }
}
