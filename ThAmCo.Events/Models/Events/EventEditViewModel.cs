﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class EventEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public TimeSpan? Duration { get; set; }

    }
}