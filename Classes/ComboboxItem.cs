namespace BookingApp.Views
{
    public class ComboboxItem
    {
        public string Aufsteigend { get; set; }
        public string Absteigend { get; set; }

        public ComboboxItem(string ASC, string DSC)
        {
            Aufsteigend = ASC;
            Absteigend = DSC;
        }
    }
}