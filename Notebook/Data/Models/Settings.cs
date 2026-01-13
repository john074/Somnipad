namespace Notebook.Data.Models
{
    public class Settings
    {
        public string Theme { get; set; }
        public string BaseTheme { get; set; }
        public int DarawingLines
        {
            get;
            set => field = (value == 0) ? value : 1;
        }
    }
}
