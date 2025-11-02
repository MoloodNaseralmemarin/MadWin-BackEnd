namespace Shop2City.WebHost.ViewModels.DeliveryMethods
{
    public class DeliveryMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DeliveryMethodForAdminViewModel
    {
        public int Id { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
    }
}
