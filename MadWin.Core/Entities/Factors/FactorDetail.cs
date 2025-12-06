using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MadWin.Core.Entities.Common;
using MadWin.Core.Entities.Products;

namespace MadWin.Core.Entities.Factors
{
    [Table("FactorDetails", Schema = "Factors")]
    public class FactorDetail:BaseEntity
    {
        public FactorDetail()
        {  
        }

        public int Id { get; set; }
        public int FactorId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool IsNew { get; set; } = false;

        #region Relationship
        public Factor Factor { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
