
using System.ComponentModel.DataAnnotations;
namespace MadWin.Core.Entities.Common
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CreateDate = DateTime.Now;
            LastUpdateDate = DateTime.Now;
            IsDelete = false;
        }
        [Key]
        public int Id { get; set; }

        public string? Description { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreateDate { get; set; } 

        public DateTime LastUpdateDate { get; set; }
    }
}
