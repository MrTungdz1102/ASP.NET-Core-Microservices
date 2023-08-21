using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Services.ShoppingCartAPI.Models.DTOs;

namespace Services.ShoppingCartAPI.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        public int CartHeaderId { get; set; }
        [ForeignKey("CartHeaderId")]
        public CartHeader CartHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
    }
}
