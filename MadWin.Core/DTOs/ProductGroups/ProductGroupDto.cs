namespace MadWin.Core.DTOs.ProductGroups
{
    #region ProductGroupForAdminDto

    public class ProductGroupItemForAdminDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }

    }
    public class ProductGroupForAdminDto
    {
        public List<ProductGroupItemForAdminDto> ProductGroups { get; set; }
        public int CurrentPage { get; set; }
        public int CountPage { get; set; }
    }
    #endregion
}
