namespace Web.Models.Staff
{
    public class ListDriversViewModel
    {
        public string StationName { get; set; } = string.Empty;
        public List<DriverViewModel> Drivers { get; set; } = new List<DriverViewModel>();
    }
}
