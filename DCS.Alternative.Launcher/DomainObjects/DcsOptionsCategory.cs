namespace DCS.Alternative.Launcher.DomainObjects
{
    public class DcsOptionsCategory
    {
        public string Id
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public Option[] Options
        {
            get;
            set;
        }

        public int DisplayOrder
        {
            get;
            set;
        }
    }
}