using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Models
{
    using static Common.EntityValidationConstants.FishingTechnique;
    public class FishingTechnique
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<FishCatch> FishCatches { get; set; } = new List<FishCatch>();
    }

}
