using System.ComponentModel.DataAnnotations;


namespace BaggageStorage.Data.Models
{
    public class Permission
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        [Required]
        public string OperationId { get; set; }

        [Required]
        public bool Permitted { get; set; }


        public virtual ApplicationRole Role { get; set; }
        public virtual Operation Operation { get; set; }
    }
}