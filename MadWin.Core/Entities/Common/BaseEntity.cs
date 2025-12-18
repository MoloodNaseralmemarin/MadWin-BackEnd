
using MadWin.Core.Common;
using System.ComponentModel.DataAnnotations;
namespace MadWin.Core.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = ErrorMessage.MaxLength)]
        public string Description { get; set; } = default!;

        public bool IsDelete { get; set; }

        public DateTime CreateDate { get; set; } 

        public DateTime LastUpdateDate { get; set; }
    }
}
