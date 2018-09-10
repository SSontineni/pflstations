﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PipelineFeatureList.Models
{
    public class SpecRating
    {
        [DisplayName("ID")]
        public int SpecRatingID { get; set; }
        [DisplayName("Spec Rating")]
        public string SpecRatingItem { get; set; }
    }

}