namespace DCS.Alternative.Launcher
{
    public class ExportFile
    {
        public ExportFile(string fileName, params string[] exportNames)
        {
            FileName = fileName;
            ExportNames = exportNames;
        }

        public string[] ExportNames
        {
            get;
        }

        public string FileName
        {
            get;
        }
    }
}