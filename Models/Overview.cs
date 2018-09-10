﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PipelineFeatureList.Models
{
    public class Overview
    {
        public ValveSection ValveSectionData { get; set; }
        public ValveSectionFeature ValveSectionFeatureData { get; set; }
        public ANSIRating ANSIRatingData { get; set; }
        public BendRadius BendRadiusData { get; set; }
        public CoatingType CoatingTypeData { get; set; }
        public ConstructionType ConstructionTypeData { get; set; }
        public CurrentClassLocation CurrentClassLocationData { get; set; }
        public RecordIdentifier DrawingData { get; set; }
        public Feature FeatureData { get; set; }
        public Grade GradeData { get; set; }
        public HCA HCAData { get; set; }
        public Manufacturer ManufacturerData { get; set; }
        public ManufacturerType ManufacturerTypeData { get; set; }
        public MaterialType MaterialTypeData { get; set; }
        public Orientation OrientationData { get; set; }
        public OutsideDiameter OutsideDiameterData1 { get; set; }
        public OutsideDiameter OutsideDiameterData2 { get; set; }
        public PipeType PipeTypeData { get; set; }
        public RecordIdentifier ODRecordID1Data { get; set; }
        public RecordIdentifier ODRecordID2Data { get; set; }
        public RecordIdentifier WTRecordID1Data { get; set; }
        public RecordIdentifier WTRecordID2Data { get; set; }
        public RecordIdentifier STRecordID1Data { get; set; }
        public RecordIdentifier STRecordID2Data { get; set; }
        public RecordIdentifier SRRecordID1Data { get; set; }
        public RecordIdentifier SRRecordID2Data { get; set; }
        public SeamType SeamTypeData { get; set; }
        public SpecRating SpecRatingData { get; set; }
        public StandardEntry StandardEntryData { get; set; }
        public PipeSystem PipeSystemData { get; set; }
        public Pipeline PipelineData { get; set; }
        public DocumentRecord DocumentRecordData { get; set; }
        public ValveSectionError ValveSectionErrorData { get; set; }
        public FeatureIssue FeatureIssueData { get; set; }
        public User BuilderData { get; set; }
        public User QCData { get; set; }
        public User EngineerData { get; set; }

        public DynamicSegmentationRecord DynamicSegmentationRecordData { get; set; }
    }
    public class OverviewBuilder
    {
        public User BuilderData { get; set; }
    }
    public class OverviewQC
    {
        public User QCData { get; set; }
    }
    public class OverviewEngineer
    {
        public User EngineerData { get; set; }
    }
}