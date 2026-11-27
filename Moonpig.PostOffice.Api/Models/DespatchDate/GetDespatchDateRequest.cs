using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moonpig.PostOffice.Api.Models.DespatchDate
{
    public class GetDespatchDateRequest
    {
        [Required(ErrorMessage = "One or more product Id is required")]
        [MinLength(1, ErrorMessage = "One or more product Id is required")]
        public List<int> ProductIds { get; set; }
        [Required(ErrorMessage = "Order date is required")]
        public DateTime? OrderDate { get; set; }
    }
}
