﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PipelineFeatureList.Models
{
    public class SeamType
    {
        [DisplayName("ID")]
        public int SeamTypeID { get; set; }
        [DisplayName("Seam Type")]
        public string SeamTypeItem { get; set; }
        [DisplayName("Joint Factor")]
        public decimal? JointFactor { get; set; }
    }
}