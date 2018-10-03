﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PipelineFeatureList.Models;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Collections;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using PipelineFeatureList.AppCode;

namespace PipelineFeatureList.Controllers
{
    public class OverviewController : Controller
    {
        private PipelineFeatureListDBContext db = new PipelineFeatureListDBContext();

        //
        // GET: /ValveSectionFeature/

        public ActionResult Index(int ValveSectionID, string OrionStationSeries)
        {
            Session["CurrentValveSection"] = ValveSectionID;
            Session["CurrentOrionStationSeries"] = OrionStationSeries;

            // Valve Section
            var VSmodel = db.ValveSection
                .Include("ValveSectionStatus")
                .Where(v => v.ValveSectionID == ValveSectionID)
                .ToList();
            ViewData.Add("ValveSection", VSmodel);
            //ViewData.Add("IsDirty",(VSmodel.First().IsSegmentationDirty == null ? false : VSmodel.First().IsSegmentationDirty));

            int? vsStatusID = VSmodel.FirstOrDefault().ValveSectionStatusID;

            // Status
            var Smodel = db.ValveSectionStatus
                .Include("DisplayGroup")
                .Where(v => v.ValveSectionStatusID == vsStatusID)
                .ToList();
            ViewData.Add("StatusSection", Smodel);
            ViewData.Add("EditDisabled", Smodel.FirstOrDefault().DisableEdit);
            ViewData.Add("DisplayGroupName", Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName);

            if (Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Engineering" ||
                Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Final Engineering" ||
                Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Annual Review")
            {
                var DSRmodel = db.DynamicSegmentationRecords
                               .Where(d => d.ValveSectionID == ValveSectionID)
                               .OrderBy(d => d.FeatureNumber)
                               .ThenBy(d => d.SegmentNumber)
                               .ToList();
                ViewData.Add("DSRSection", DSRmodel);
            }

            var model = (from v in db.ValveSection
                            join vs in db.ValveSectionFeatures on v.ValveSectionID equals vs.ValveSectionID into vs1
                            from vsd in vs1.DefaultIfEmpty()
                            join ps in db.PipeSystems on v.PipeSystemID equals ps.PipeSystemID into ps1
                            from psd in ps1.DefaultIfEmpty()
                            join p in db.Pipelines on v.PipelineID equals p.PipelineID into p1
                            from pd in p1.DefaultIfEmpty()
                            join a in db.ANSIRatings on vsd.ANSIRatingID equals a.ANSIRatingID into a1
                            from ad in a1.DefaultIfEmpty()
                            join br in db.BendRadiuses on vsd.RadiusID equals br.BendRadiusID into br1
                            from brd in br1.DefaultIfEmpty()
                            join c in db.CoatingTypes on vsd.CoatingTypeID equals c.CoatingTypeID into c1
                            from cd in c1.DefaultIfEmpty()
                            join ct in db.ConstructionTypes on vsd.ConstructionTypeID equals ct.ConstructionTypeID into ct1
                            from ctd in ct1.DefaultIfEmpty()
                            join d in db.RecordIdentifiers on vsd.DrawingID equals d.RecordIdentifierID into d1
                            from dd in d1.DefaultIfEmpty()
                            join f in db.Features on vsd.FeatureID equals f.FeatureID into f1
                            from fd in f1.DefaultIfEmpty()
                            join g in db.Grades on vsd.GradeID equals g.GradeID into g1
                            from gd in g1.DefaultIfEmpty()
                            join m in db.Manufacturers on vsd.ManufacturerID equals m.ManufacturerID into m1
                            from md in m1.DefaultIfEmpty()
                            join mft in db.ManufacturerTypes on vsd.ManufacturerTypeID equals mft.ManufacturerTypeID into mft1
                            from mftd in mft1.DefaultIfEmpty()
                            join mt in db.MaterialTypes on vsd.MaterialTypeID equals mt.MaterialTypeID into mt1
                            from mtd in mt1.DefaultIfEmpty()
                            join o in db.Orientations on vsd.OrientID equals o.OrientationID into o1
                            from od in o1.DefaultIfEmpty()
                            join outd1 in db.OutsideDiameters on vsd.ODID1 equals outd1.OutsideDiameterID into outd11
                            from outd1d in outd11.DefaultIfEmpty()
                            join outd2 in db.OutsideDiameters on vsd.ODID2 equals outd2.OutsideDiameterID into outd12
                            from outd2d in outd12.DefaultIfEmpty()
                            join pt in db.PipeTypes on vsd.TypeID equals pt.PipeTypeID into pt1
                            from ptd in pt1.DefaultIfEmpty()
                            join riOD1 in db.RecordIdentifiers on vsd.ODRecordID1 equals riOD1.RecordIdentifierID into riOD11
                            from riOD1d in riOD11.DefaultIfEmpty()
                            join riOD2 in db.RecordIdentifiers on vsd.ODRecordID2 equals riOD2.RecordIdentifierID into riOD21
                            from riOD2d in riOD21.DefaultIfEmpty()
                            join riWT1 in db.RecordIdentifiers on vsd.WTRecordID1 equals riWT1.RecordIdentifierID into riWT11
                            from riWT1d in riWT11.DefaultIfEmpty()
                            join riWT2 in db.RecordIdentifiers on vsd.WTRecordID2 equals riWT2.RecordIdentifierID into riWT21
                            from riWT2d in riWT21.DefaultIfEmpty()
                            join riST1 in db.RecordIdentifiers on vsd.SeamRecordID1 equals riST1.RecordIdentifierID into riST11
                            from riST1d in riST11.DefaultIfEmpty()
                            join riST2 in db.RecordIdentifiers on vsd.SeamRecordID2 equals riST2.RecordIdentifierID into riST21
                            from riST2d in riST21.DefaultIfEmpty()
                            join riSR1 in db.RecordIdentifiers on vsd.SpecRatingRecordID1 equals riSR1.RecordIdentifierID into riSR11
                            from riSR1d in riSR11.DefaultIfEmpty()
                            join riSR2 in db.RecordIdentifiers on vsd.SpecRatingRecordID2 equals riSR2.RecordIdentifierID into riSR21
                            from riSR2d in riSR21.DefaultIfEmpty()
                            join st in db.SeamTypes on vsd.SeamWeldTypeID equals st.SeamTypeID into st1
                            from std in st1.DefaultIfEmpty()
                            join sr in db.SpecRatings on vsd.SpecRatingID equals sr.SpecRatingID into sr1
                            from srd in sr1.DefaultIfEmpty()
                            where v.ValveSectionID == ValveSectionID
                            orderby vsd.FeatureNumber
                            select new Overview
                            {
                                ValveSectionData = v,
                                ValveSectionFeatureData = vsd,
                                PipeSystemData = psd,
                                PipelineData = pd,
                                ANSIRatingData = ad,
                                BendRadiusData = brd,
                                ConstructionTypeData = ctd,
                                DrawingData = dd,
                                ODRecordID1Data = riOD1d,
                                ODRecordID2Data = riOD2d,
                                WTRecordID1Data = riWT1d,
                                WTRecordID2Data = riWT2d,
                                STRecordID1Data = riST1d,
                                STRecordID2Data = riST2d,
                                SRRecordID1Data = riSR1d,
                                SRRecordID2Data = riSR2d,
                                FeatureData = fd,
                                GradeData = gd,
                                MaterialTypeData = mtd,
                                OrientationData = od,
                                PipeTypeData = ptd,
                                SeamTypeData = std,
                                SpecRatingData = srd,
                                ManufacturerData = md,
                                ManufacturerTypeData = mftd,
                                CoatingTypeData = cd,
                                OutsideDiameterData1 = outd1d,
                                OutsideDiameterData2 = outd2d
                            }).ToList();
            int i = 0;
            foreach (var items in model)
            {
                try
                {
                    i = items.ValveSectionFeatureData.DrawingID.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.DrawingData == null)
                        items.DrawingData = new RecordIdentifier();
                    items.DrawingData.RecordIdentifierID = k.riid;
                    items.DrawingData.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e)
                {
                    int m = 1;
                }
                try
                {
                    i = items.ValveSectionFeatureData.ODRecordID1.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                    {
                                        riid = r.RecordIdentifierID,
                                        riitem = r.RecordIdentifierItem
                                    }).First();
                    if (items.ODRecordID1Data == null)
                        items.ODRecordID1Data = new RecordIdentifier();
                    items.ODRecordID1Data.RecordIdentifierID = k.riid;
                    items.ODRecordID1Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.ODRecordID2.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.ODRecordID2Data == null)
                        items.ODRecordID2Data = new RecordIdentifier();
                    items.ODRecordID2Data.RecordIdentifierID = k.riid;
                    items.ODRecordID2Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.WTRecordID1.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.WTRecordID1Data == null)
                        items.WTRecordID1Data = new RecordIdentifier();
                    items.WTRecordID1Data.RecordIdentifierID = k.riid;
                    items.WTRecordID1Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.WTRecordID2.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.WTRecordID2Data == null)
                        items.WTRecordID2Data = new RecordIdentifier();
                    items.WTRecordID2Data.RecordIdentifierID = k.riid;
                    items.WTRecordID2Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.SeamRecordID1.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.STRecordID1Data == null)
                        items.STRecordID1Data = new RecordIdentifier();
                    items.STRecordID1Data.RecordIdentifierID = k.riid;
                    items.STRecordID1Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.SeamRecordID2.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.STRecordID2Data == null)
                        items.STRecordID2Data = new RecordIdentifier();
                    items.STRecordID2Data.RecordIdentifierID = k.riid;
                    items.STRecordID2Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.SpecRatingRecordID1.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.SRRecordID1Data == null)
                        items.SRRecordID1Data = new RecordIdentifier();
                    items.SRRecordID1Data.RecordIdentifierID = k.riid;
                    items.SRRecordID1Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
                try
                {
                    i = items.ValveSectionFeatureData.SpecRatingRecordID2.Value;
                    var k = (from a in db.DocumentRecords
                                join r in db.RecordIdentifiers on a.DocumentRecordID equals r.RecordIdentifierID
                                where a.DocumentRecordID == i && a.PipelineID == items.ValveSectionFeatureData.ValveSectionID
                                select new
                                {
                                    riid = r.RecordIdentifierID,
                                    riitem = r.RecordIdentifierItem
                                }).First();
                    if (items.SRRecordID2Data == null)
                        items.SRRecordID2Data = new RecordIdentifier();
                    items.SRRecordID2Data.RecordIdentifierID = k.riid;
                    items.SRRecordID2Data.RecordIdentifierItem = k.riitem;
                }
                catch (Exception e) { }
            }

            // Document Records
            //var RDmodel = db.DocumentRecords.Include("RecordIdentifier").Include("DocumentType").Where(d => d.ValveSectionID == ValveSectionID).OrderBy(d => d.DocumentRecordID).ToList();
            //ViewData.Add("RecordData", RDmodel);

            // Issues
            var FImodel = db.FeatureIssues.Where(f => f.ValveSectionID == ValveSectionID).OrderBy(f => f.FeatureNumber).ThenBy(f => f.BuilderCreatedOn).ThenBy(f => f.CheckerCreatedOn).ThenBy(f => f.EngineerCreatedOn).ToList();
            ViewData.Add("IssueData", FImodel);

            // Errors
            var VSEmodel = db.ValveSectionErrors
                .Include("ValveSectionErrorLevel")
                .Include("ValveSectionFeature")
                .Include("FeatureError")
                .Where(d => d.ValveSectionID == ValveSectionID)
                .OrderBy(d => d.ValveSectionFeature.FeatureNumber)
                .ThenBy(d => d.FeatureError.OrderBy)
                .ToList();
            ViewData.Add("ErrorData", VSEmodel);
            
            // Rules
            //var Rmodel = db.WorkflowRules
            //    .Include("WorkflowAction")
            //    .Where(w => w.Old_ValveSectionStatusID == vsStatusID)
            //    .ToList();

            var Rmodel = (from wr in db.WorkflowRules
                           join wa in db.WorkflowActions on wr.WorkflowActionID equals wa.WorkflowActionID
                           join v in db.ValveSectionStatus on wr.New_ValveSectionStatusID equals v.ValveSectionStatusID
                           where wr.Old_ValveSectionStatusID == vsStatusID
                                select new WorkflowRuleList
                                {
                                    WorkflowRule = wr,
                                    WorkflowAction = wa,
                                    DisableDirty = v.DirtyDisable
                                }
                            ).ToList();
            ViewData.Add("WorkflowSection", Rmodel);

            // Current MAOP
            var Cmodel = db.ValveSection
                .Where(v => v.ValveSectionID == ValveSectionID)
                .ToList();
            ViewData.Add("MAOP", Cmodel.FirstOrDefault().CurrentMAOP);

            // Users
            var Bmodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.BuilderID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewBuilder { BuilderData = u }).ToList();
            ViewData.Add("BuilderData", Bmodel);
            var Qmodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.QCID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewQC { QCData = u }).ToList();
            ViewData.Add("QCData", Qmodel);
            var Emodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.EngineerID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewEngineer { EngineerData = u }).ToList();
            ViewData.Add("EngineerData", Emodel);

            return View(model);
        }

        //
        // GET: /ValveSectionFeature/DSRs

        public ActionResult DSRs()
        {
            Int64 ValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            string OrionStationSeries = Session["CurrentOrionStationSeries"].ToString();

            // Valve Section
            var VSmodel = db.ValveSection
                .Include("ValveSectionStatus")
                .Where(v => v.ValveSectionID == ValveSectionID)
                .ToList();
            ViewData.Add("ValveSection", VSmodel);
            //ViewData.Add("IsDirty", VSmodel.First().IsSegmentationDirty);

            int? vsStatusID = VSmodel.FirstOrDefault().ValveSectionStatusID;

            // Status
            var Smodel = db.ValveSectionStatus
                .Include("DisplayGroup")
                .Where(v => v.ValveSectionStatusID == vsStatusID)
                .ToList();
            ViewData.Add("StatusSection", Smodel);
            ViewData.Add("EditDisabled", Smodel.FirstOrDefault().DisableEdit);
            ViewData.Add("DisplayGroupName", Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName);

            //if (VSmodel.First().IsSegmentationDirty == true && Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName != "Certification Approved")
            //{
            //    //Resegment();
                
            //    //PH 2014.05.22 need user id
            //    //PipelineFeatureList.AppCode.AppLibrary.CopyToHistoryGenerateGradesDynamicSegmentation(ValveSectionID, 0, 0, 1);
            //    PipelineFeatureList.AppCode.AppLibrary.CopyToHistoryGenerateGradesDynamicSegmentation(
            //        Convert.ToInt64(Session["UserID"].ToString()),
            //        ValveSectionID,
            //        0,
            //        0,
            //        1); 
            //    //PH 2014.05.22 end edit
            //    Thread.Sleep(10000);
            //}
            
            if (Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Engineering" ||
                Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Final Engineering" ||
                Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Annual Review" ||
                Smodel.FirstOrDefault().DisplayGroup.DisplayGroupName == "Certification Approved")
            {
                var DSRmodel = db.DynamicSegmentationRecords
                               .Where(d => d.ValveSectionID == ValveSectionID)
                               .OrderBy(d => d.FeatureNumber)
                               .ThenBy(d => d.SegmentNumber)
                               .ToList();
                ViewData.Add("DSRSection", DSRmodel);
            }

            var model = (from v in db.ValveSection
                         join ps in db.PipeSystems on v.PipeSystemID equals ps.PipeSystemID into ps1
                         from psd in ps1.DefaultIfEmpty()
                         join p in db.Pipelines on v.PipelineID equals p.PipelineID into p1
                         from pd in p1.DefaultIfEmpty()
                         where v.ValveSectionID == ValveSectionID
                         select new Overview
                         {
                             ValveSectionData = v,
                             PipeSystemData = psd,
                             PipelineData = pd
                         }).ToList();

            // Users
            var Bmodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.BuilderID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewBuilder { BuilderData = u }).ToList();
            ViewData.Add("BuilderData", Bmodel);
            var Qmodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.QCID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewQC { QCData = u }).ToList();
            ViewData.Add("QCData", Qmodel);
            var Emodel = (from u in db.Users
                          join v in db.ValveSection on u.UserID equals v.EngineerID
                          where v.ValveSectionID == ValveSectionID
                          select new OverviewEngineer { EngineerData = u }).ToList();
            ViewData.Add("EngineerData", Emodel);

            return View("DSRs", "_DSRLayout", model);
            //return View(model);
        }
        
        //
        // GET: /ValveSectionFeature/Details/5

        public ActionResult Details(int id = 0)
        {
            ValveSectionFeature valvesectionfeature = db.ValveSectionFeatures.Find(id);
            if (valvesectionfeature == null)
            {
                return HttpNotFound();
            }
            return View(valvesectionfeature);
        }

        public void ActionSetups()
        {
            // for Create and Insert

            ViewBag.FeatureID = new SelectList(db.Features, "FeatureID", "FeatureItem");
            ViewBag.CurrentClassLoc = new SelectList(db.CurrentClassLocations, "CurrentClassLocationID", "CurrentClassLocationItem");
            ViewBag.HCAName = new SelectList(db.HCAs, "HCAID", "HCAItem");
            ViewBag.ConstructionTypeID = new SelectList(db.ConstructionTypes, "ConstructionTypeID", "ConstructionTypeItem");
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var availDocs = from d in db.DocumentRecords
                            join r in db.RecordIdentifiers on d.DocumentRecordID equals r.RecordIdentifierID
                            where d.PipelineID == currSection
                            orderby d.DocumentRecordID
                            select new { d.DocumentRecordID, r.RecordIdentifierItem };
            ViewBag.DrawingID = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.ODRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.ODRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.WTRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.WTRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.SeamRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.SeamRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.SpecRatingRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            ViewBag.SpecRatingRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem");
            List<OutsideDiameter> od1List = db.OutsideDiameters.OrderBy(o => o.OutsideDiameterItem).ToList();
            SelectList OD1List = new SelectList(od1List, "OutsideDiameterID", "OutsideDiameterItem");
            ViewBag.ODID1 = OD1List; 
            //ViewBag.ODID1 = new SelectList(db.OutsideDiameters, "OutsideDiameterID", "OutsideDiameterItem");
            List<OutsideDiameter> od2List = db.OutsideDiameters.OrderBy(o => o.OutsideDiameterItem).ToList();
            SelectList OD2List = new SelectList(od1List, "OutsideDiameterID", "OutsideDiameterItem");
            ViewBag.ODID2 = OD2List; 
            //ViewBag.ODID2 = new SelectList(db.OutsideDiameters, "OutsideDiameterID", "OutsideDiameterItem");
            ViewBag.SeamWeldTypeID = new SelectList(db.SeamTypes, "SeamTypeID", "SeamTypeItem");
            ViewBag.SpecRatingID = new SelectList(db.SpecRatings, "SpecRatingID", "SpecRatingItem");
            ViewBag.GradeID = new SelectList(db.Grades, "GradeID", "GradeItem");
            ViewBag.ANSIRatingID = new SelectList(db.ANSIRatings, "ANSIRatingID", "ANSIRatingItem");
            ViewBag.MaterialTypeID = new SelectList(db.MaterialTypes, "MaterialTypeID", "MaterialTypeItem");
            ViewBag.RadiusID = new SelectList(db.BendRadiuses, "BendRadiusID", "BendRadiusItem");
            ViewBag.OrientID = new SelectList(db.Orientations, "OrientationID", "OrientationItem");
            ViewBag.CoatingTypeID = new SelectList(db.CoatingTypes, "CoatingTypeID", "CoatingTypeItem");
            List<Manufacturer> manuList = db.Manufacturers.OrderBy(m => m.ManufacturerItem).ToList();
            SelectList ManuList = new SelectList(manuList, "ManufacturerID", "ManufacturerItem");
            ViewBag.ManufacturerID = ManuList; // new SelectList(db.Manufacturers, "ManufacturerID", "ManufacturerItem");
            List<ManufacturerType> manutypeList = db.ManufacturerTypes.OrderBy(m => m.ManufacturerTypeItem).ToList();
            SelectList ManuTypeList = new SelectList(manutypeList, "ManufacturerTypeID", "ManufacturerTypeItem");
            ViewBag.ManufacturerTypeID = ManuTypeList; 
            ViewBag.TypeID = new SelectList(db.PipeTypes, "PipeTypeID", "PipeTypeItem");
            ViewBag.Length = 0;
        }

        public void ActionSetups(ValveSectionFeature valvesectionfeature)
        {
            // for Edit

            Session["EditFirstPass"] = "1";
            ViewBag.SelectedFeature = valvesectionfeature.FeatureID;
            ViewBag.SelectedPipeType = valvesectionfeature.TypeID;
            ViewBag.SelectedODRecordID1 = valvesectionfeature.ODRecordID1; 
            ViewBag.SelectedODRecordID2 = valvesectionfeature.ODRecordID2; 
            ViewBag.SelectedWTRecordID1 = valvesectionfeature.WTRecordID1; 
            ViewBag.SelectedWTRecordID2 = valvesectionfeature.WTRecordID2; 
            ViewBag.SelectedSTRecordID1 = valvesectionfeature.SeamRecordID1; 
            ViewBag.SelectedSTRecordID2 = valvesectionfeature.SeamRecordID2; 
            ViewBag.SelectedSRRecordID1 = valvesectionfeature.SpecRatingRecordID1; 
            ViewBag.SelectedSRRecordID2 = valvesectionfeature.SpecRatingRecordID2; 
            ViewBag.SelectedODID1 = valvesectionfeature.ODID1;
            ViewBag.SelectedODID2 = valvesectionfeature.ODID2;
            ViewBag.SelectedSeamWeldTypeID = valvesectionfeature.SeamWeldTypeID;
            ViewBag.SelectedSpecRatingID = valvesectionfeature.SpecRatingID;
            ViewBag.SelectedGradeID = valvesectionfeature.GradeID;
            ViewBag.SelectedANSIRatingID = valvesectionfeature.ANSIRatingID;
            ViewBag.SelectedRadiusID = valvesectionfeature.RadiusID;
            ViewBag.SelectedOrientID = valvesectionfeature.OrientID;

            ViewBag.FeatureID = new SelectList(db.Features, "FeatureID", "FeatureItem", valvesectionfeature.FeatureID);
            ViewBag.CurrentClassLoc = new SelectList(db.CurrentClassLocations, "CurrentClassLocationID", "CurrentClassLocationItem", valvesectionfeature.CurrentClassLoc);
            ViewBag.HCAName = new SelectList(db.HCAs, "HCAID", "HCAItem", valvesectionfeature.HCAName);
            ViewBag.ConstructionTypeID = new SelectList(db.ConstructionTypes, "ConstructionTypeID", "ConstructionTypeItem", valvesectionfeature.ConstructionTypeID);
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var availDocs = from d in db.DocumentRecords
                            join r in db.RecordIdentifiers on d.DocumentRecordID equals r.RecordIdentifierID
                            where d.PipelineID == currSection
                            orderby d.DocumentRecordID
                            select new {d.DocumentRecordID, r.RecordIdentifierItem };
            ViewBag.DrawingID = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.DrawingID);
            ViewBag.ODRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.ODRecordID1);
            ViewBag.ODRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.ODRecordID2);
            ViewBag.WTRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.WTRecordID1);
            ViewBag.WTRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.WTRecordID2);
            ViewBag.SeamRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.SeamRecordID1);
            ViewBag.SeamRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.SeamRecordID2);
            ViewBag.SpecRatingRecordID1 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.SpecRatingRecordID1);
            ViewBag.SpecRatingRecordID2 = new SelectList(availDocs, "DocumentRecordID", "RecordIdentifierItem", valvesectionfeature.SpecRatingRecordID2);
            List<OutsideDiameter> od1List = db.OutsideDiameters.OrderBy(o => o.OutsideDiameterItem).ToList();
            SelectList OD1List = new SelectList(od1List, "OutsideDiameterID", "OutsideDiameterItem", valvesectionfeature.ODID1);
            ViewBag.ODID1 = OD1List;
            //ViewBag.ODID1 = new SelectList(db.OutsideDiameters, "OutsideDiameterID", "OutsideDiameterItem", valvesectionfeature.ODID1);
            List<OutsideDiameter> od2List = db.OutsideDiameters.OrderBy(o => o.OutsideDiameterItem).ToList();
            SelectList OD2List = new SelectList(od1List, "OutsideDiameterID", "OutsideDiameterItem", valvesectionfeature.ODID2);
            ViewBag.ODID2 = OD2List;
            //ViewBag.ODID2 = new SelectList(db.OutsideDiameters, "OutsideDiameterID", "OutsideDiameterItem", valvesectionfeature.ODID2);
            ViewBag.SeamWeldTypeID = new SelectList(db.SeamTypes, "SeamTypeID", "SeamTypeItem", valvesectionfeature.SeamWeldTypeID);
            ViewBag.SpecRatingID = new SelectList(db.SpecRatings, "SpecRatingID", "SpecRatingItem", valvesectionfeature.SpecRatingID);
            ViewBag.GradeID = new SelectList(db.Grades, "GradeID", "GradeItem", valvesectionfeature.GradeID);
            ViewBag.ANSIRatingID = new SelectList(db.ANSIRatings, "ANSIRatingID", "ANSIRatingItem", valvesectionfeature.ANSIRatingID);
            ViewBag.MaterialTypeID = new SelectList(db.MaterialTypes, "MaterialTypeID", "MaterialTypeItem", valvesectionfeature.MaterialTypeID);
            ViewBag.RadiusID = new SelectList(db.BendRadiuses, "BendRadiusID", "BendRadiusItem", valvesectionfeature.RadiusID);
            ViewBag.OrientID = new SelectList(db.Orientations, "OrientationID", "OrientationItem", valvesectionfeature.OrientID);
            ViewBag.CoatingTypeID = new SelectList(db.CoatingTypes, "CoatingTypeID", "CoatingTypeItem", valvesectionfeature.CoatingTypeID);
            List<Manufacturer> manuList = db.Manufacturers.OrderBy(m => m.ManufacturerItem).ToList();
            SelectList ManuList = new SelectList(manuList, "ManufacturerID", "ManufacturerItem", valvesectionfeature.ManufacturerID);
            ViewBag.ManufacturerID = ManuList; // new SelectList(db.Manufacturers, "ManufacturerID", "ManufacturerItem", valvesectionfeature.ManufacturerID);
            List<ManufacturerType> manutypeList = db.ManufacturerTypes.OrderBy(m => m.ManufacturerTypeItem).ToList();
            SelectList ManuTypeList = new SelectList(manutypeList, "ManufacturerTypeID", "ManufacturerTypeItem", valvesectionfeature.ManufacturerTypeID);
            ViewBag.ManufacturerTypeID = ManuTypeList;
            ViewBag.TypeID = new SelectList(db.PipeTypes, "PipeTypeID", "PipeTypeItem", valvesectionfeature.TypeID);
            ViewBag.ODRecordMatrixCheck = "";
            ViewBag.Length = valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart;
        }

        public void ActionUnknowns(ValveSectionFeature valvesectionfeature)
        {
            if (valvesectionfeature.DescriptionUnknown) valvesectionfeature.Description = "";
            if (valvesectionfeature.JobWOPOUnknown) valvesectionfeature.JobWOPO = "";
            if (valvesectionfeature.MillUnknown) valvesectionfeature.Mill = "";
            if (valvesectionfeature.InServiceDateUnknown) valvesectionfeature.InServiceDate = null;
            if (valvesectionfeature.InstallDateUnknown) valvesectionfeature.InstallDate = null;
            if (valvesectionfeature.MFRDateUnknown) valvesectionfeature.MFRDate = null;
        }

        public string CleanData(string field)
        {
            if (field != null)
                field = Regex.Replace(field, @"[^!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~]", " ");
            return field;
        }
        
        //
        // GET: /ValveSectionFeature/Create

        public ActionResult Create()
        {
            Int64 currvalvesection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            decimal featurenumber; 
            try { featurenumber = db.ValveSectionFeatures.Where(v => v.ValveSectionID == currvalvesection).Max(v => v.FeatureNumber); }
            catch { featurenumber = 0; }

            featurenumber++;
            decimal diff = featurenumber % 1;
            featurenumber = featurenumber - diff;

            Session["CurrentFeatureNumber"] = featurenumber;
            ViewBag.FeatureNumber = featurenumber;

            ActionSetups();

            return View();
        }

        //
        // POST: /ValveSectionFeature/Create

        [HttpPost]
        public ActionResult Create(ValveSectionFeature valvesectionfeature)
        {
            if (ModelState.IsValid)
            {
                ActionUnknowns(valvesectionfeature);

                valvesectionfeature.Description = CleanData(valvesectionfeature.Description);
                valvesectionfeature.Notes = CleanData(valvesectionfeature.Notes);

                valvesectionfeature.CreatedBy_UserID = Convert.ToInt64(Session["UserID"].ToString());
                valvesectionfeature.ModifiedBy_UserID = Convert.ToInt64(Session["UserID"].ToString());
                valvesectionfeature.CreatedOn = DateTime.Now;
                valvesectionfeature.ModifiedOn = DateTime.Now;

                valvesectionfeature.Length = valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart;
                valvesectionfeature.LengthDiscrepancy = valvesectionfeature.Length - (valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart);
                valvesectionfeature.ValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
                valvesectionfeature.FeatureNumber = Convert.ToDecimal(Session["CurrentFeatureNumber"].ToString());
                db.ValveSectionFeatures.Add(valvesectionfeature);
                db.SaveChanges();

                UpdateValveSectionHeaderLengths(valvesectionfeature.ValveSectionID);
                GenerateValveSectionErrors(valvesectionfeature);
                GenerateGISBeginEndErrors();

                UpdateValveSectionToDirtyAsNeeded(valvesectionfeature.ValveSectionID);

                return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
            }
             
            ViewBag.FeatureNumber = valvesectionfeature.FeatureNumber;
            ViewBag.GISAlignStart = valvesectionfeature.GISAlignStart;
            ActionSetups();
            
            return View(valvesectionfeature);
        }

        //
        // GET: /ValveSectionFeature/Insert

        public ActionResult Insert(int id = 0)
        {
            ValveSectionFeature valvesectionfeature = db.ValveSectionFeatures.Find(id);
            if (valvesectionfeature == null)
            {
                return HttpNotFound();
            }
            Int64 currvalvesection = Convert.ToInt64(Session["CurrentValveSection"].ToString());

            Int64 currfeaturenumber = Convert.ToInt64(valvesectionfeature.FeatureNumber+1);
            var featurenumberall =
                    from v in db.ValveSectionFeatures
                    where v.ValveSectionID == currvalvesection && v.FeatureNumber < currfeaturenumber
                    orderby v.FeatureNumber descending
                    select new { featurenumber = v.FeatureNumber };
            
            decimal featurenumber = featurenumberall.First().featurenumber + (decimal).01;
            ViewBag.GISAlignStart = valvesectionfeature.GISAlignStart;
            
            Session["CurrentFeatureNumber"] = featurenumber;
            ViewBag.FeatureNumber = featurenumber;

            ActionSetups();

            return View();
        }

        //
        // POST: /ValveSectionFeature/Insert

        [HttpPost]
        public ActionResult Insert(ValveSectionFeature valvesectionfeature)
        {
            if (ModelState.IsValid)
            {
                ActionUnknowns(valvesectionfeature);

                valvesectionfeature.Description = CleanData(valvesectionfeature.Description);
                valvesectionfeature.Notes = CleanData(valvesectionfeature.Notes);

                valvesectionfeature.CreatedBy_UserID = Convert.ToInt64(Session["UserID"].ToString());
                valvesectionfeature.ModifiedBy_UserID = Convert.ToInt64(Session["UserID"].ToString());
                valvesectionfeature.CreatedOn = DateTime.Now;
                valvesectionfeature.ModifiedOn = DateTime.Now;

                valvesectionfeature.Length = valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart;
                valvesectionfeature.LengthDiscrepancy = valvesectionfeature.Length - (valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart);
                valvesectionfeature.ValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
                valvesectionfeature.FeatureNumber = Convert.ToDecimal(Session["CurrentFeatureNumber"].ToString());
                db.ValveSectionFeatures.Add(valvesectionfeature);
                db.SaveChanges();

                UpdateValveSectionHeaderLengths(valvesectionfeature.ValveSectionID);
                GenerateValveSectionErrors(valvesectionfeature);
                GenerateGISBeginEndErrors();

                UpdateValveSectionToDirtyAsNeeded(valvesectionfeature.ValveSectionID);
                
                return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
            }

            ViewBag.FeatureNumber = valvesectionfeature.FeatureNumber;
            ViewBag.GISAlignStart = valvesectionfeature.GISAlignStart;
            ActionSetups();

            return View(valvesectionfeature);
        }

        //
        // GET: /DocumentRecord/Create

        //public ActionResult CreateDoc()
        //{
        //    Int64 currvalvesection = Convert.ToInt64(Session["CurrentValveSection"].ToString());

        //    int entry = 0;
        //    string nextentry = "";
        //    //try { entry = db.DocumentRecords.Where(v => v.PipelineID == currvalvesection).OrderByDescending(v => v.DocumentRecordID).Max(v => v.DocumentRecordID); }
        //    //catch { }

        //    if (entry == 0)
        //    {
        //        Session["CurrentRecordIdentifier"] = "1";
        //        ViewBag.RecordIdentifierName = "A";
        //    }
        //    else
        //    {
        //        entry++;
        //        nextentry = "";
        //        try 
        //        { 
        //            nextentry = db.RecordIdentifiers.OrderByDescending(v => v.RecordIdentifierID).Where(v => v.RecordIdentifierID == entry).Select(v => v.RecordIdentifierItem).First(); 
        //            ViewBag.RecordIdentifierName = nextentry;
        //            Session["CurrentRecordIdentifier"] = entry;
        //        }
        //        catch 
        //        { 
        //            ViewBag.RecordIdentifierName = "A"; 
        //            Session["CurrentRecordIdentifier"] = "1"; 
        //        }
        //    }
            
        //    ViewBag.DocumentTypeID = new SelectList(db.DocumentTypes, "DocumentTypeID", "DocumentTypeItem");
        //    return View();
        //}

        //
        // POST: /DocumentRecord/Create

        //[HttpPost]
        //public ActionResult CreateDoc(DocumentRecord documentrecord)
        //{
        //    var dt = db.DocumentTypes.Where(d => d.DocumentTypeItem == "Completion Report" || d.DocumentTypeItem == "Pipeline Replacement Report").ToList();
        //    if ((documentrecord.DocumentTypeID == dt.First().DocumentTypeID || documentrecord.DocumentTypeID == dt.Last().DocumentTypeID))
        //    {
        //        //return View(documentrecord);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        documentrecord.PipelineID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
        //        documentrecord.DocumentRecordID = Convert.ToInt32(Session["CurrentRecordIdentifier"].ToString());
        //        db.DocumentRecords.Add(documentrecord);
        //        db.SaveChanges();
        //        return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
        //    }

        //    return View(documentrecord);
        //}

        //
        // GET: /FeatureIssue/Create

        public ActionResult CreateIssue()
        {
            Int64 currvalvesection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            //var availFeatures = from v in db.ValveSectionFeatures
            //                where v.ValveSectionID == currvalvesection
            //                orderby v.FeatureNumber
            //                select new { v.ValveSectionFeatureID, v.FeatureNumber };
            //ViewBag.ValveSectionFeatureID = new SelectList(availFeatures, "ValveSectionFeatureID", "FeatureNumber");

            var valveSectionStatus = (from vss in db.ValveSectionStatus
                                join v in db.ValveSection on vss.ValveSectionStatusID equals v.ValveSectionStatusID
                                join d in db.DisplayGroups on vss.DisplayGroupID equals d.DisplayGroupID
                                where v.ValveSectionID == currvalvesection
                                select new { ValveSectionStatusItem = v.ValveSectionStatus.ValveSectionStatusItem,
                                             DisplayGroupName = d.DisplayGroupName}).ToList();
            ViewBag.ValveSectionStatus = valveSectionStatus.FirstOrDefault().ValveSectionStatusItem;
            ViewBag.DisplayGroupName = valveSectionStatus.FirstOrDefault().DisplayGroupName;

            return View();
        }

        //
        // POST: /FeatureIssue/Create

        [HttpPost]
        public ActionResult CreateIssue(FeatureIssue featureissue)
        {
            if (ModelState.IsValid)
            {
                featureissue.BuilderID = Convert.ToInt64(Session["UserID"].ToString());
                featureissue.BuilderCreatedOn = DateTime.Now;
                featureissue.ValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
                db.FeatureIssues.Add(featureissue);
                db.SaveChanges();
                return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
            }

            return View(featureissue);
        }

        //
        // GET: /ValveSectionFeature/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ValveSectionFeature valvesectionfeature = db.ValveSectionFeatures.Find(id);
            if (valvesectionfeature == null)
            {
                return HttpNotFound();
            }

            ActionSetups(valvesectionfeature);

            return View(valvesectionfeature);
        }

        //
        // POST: /ValveSectionFeature/Edit/5

        [HttpPost]
        public ActionResult Edit(ValveSectionFeature valvesectionfeature)
        {
            if (ModelState.IsValid)
            {
                ActionUnknowns(valvesectionfeature);

                valvesectionfeature.Description = CleanData(valvesectionfeature.Description);
                valvesectionfeature.Notes = CleanData(valvesectionfeature.Notes);
                
                valvesectionfeature.ModifiedBy_UserID = Convert.ToInt64(Session["UserID"].ToString());
                valvesectionfeature.ModifiedOn = DateTime.Now;
                valvesectionfeature.Length = valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart;
                valvesectionfeature.LengthDiscrepancy = valvesectionfeature.Length - (valvesectionfeature.GISAlignEnd - valvesectionfeature.GISAlignStart);
                db.Entry(valvesectionfeature).State = EntityState.Modified;
                db.SaveChanges();

                UpdateValveSectionHeaderLengths(valvesectionfeature.ValveSectionID);
                GenerateValveSectionErrors(valvesectionfeature);
                GenerateGISBeginEndErrors();

                UpdateValveSectionToDirtyAsNeeded(valvesectionfeature.ValveSectionID);

                return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
            }

            ActionSetups(valvesectionfeature);

            return View(valvesectionfeature);
        }

        public ActionResult EditIssue(int id = 0)
        {
            FeatureIssue featureissue = db.FeatureIssues.Where(f => f.FeatureIssueID == id).First();
            if (featureissue == null)
            {
                return HttpNotFound();
            }

            Int64 currvalvesection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var valveSectionStatus = (from vss in db.ValveSectionStatus
                                      join v in db.ValveSection on vss.ValveSectionStatusID equals v.ValveSectionStatusID
                                      join d in db.DisplayGroups on vss.DisplayGroupID equals d.DisplayGroupID
                                      where v.ValveSectionID == currvalvesection
                                      select new { ValveSectionStatusItem = v.ValveSectionStatus.ValveSectionStatusItem, DisplayGroupName = d.DisplayGroupName }).ToList();
            ViewBag.ValveSectionStatus = valveSectionStatus.First().ValveSectionStatusItem;
            ViewBag.DisplayGroupName = valveSectionStatus.First().DisplayGroupName;

            return View(featureissue);
        }

        //
        // POST: /FeatureIssue/Edit/5

        [HttpPost]
        public ActionResult EditIssue(FeatureIssue featureissue)
        {
            if (ModelState.IsValid)
            {
                featureissue.ValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
                db.Entry(featureissue).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
            }
            return View(featureissue);
        }

        //
        // GET: /FeatureIssue/Delete/5

        public ActionResult DeleteIssue(int id = 0)
        {
            FeatureIssue featureissue = db.FeatureIssues.Where(f => f.FeatureIssueID == id).FirstOrDefault();
            if (featureissue == null)
            {
                return HttpNotFound();
            }
            return View(featureissue);
        }

        //
        // POST: /FeatureIssue/Delete/5

        [HttpPost, ActionName("DeleteIssue")]
        public ActionResult DeleteIssueConfirmed(int id)
        {
            FeatureIssue featureissue = db.FeatureIssues.Find(id);
            db.FeatureIssues.Remove(featureissue);
            db.SaveChanges();

            return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
        }
        
        //
        // GET: /ValveSectionFeature/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ValveSectionFeature valvesectionfeature = db.ValveSectionFeatures.Find(id);
            if (valvesectionfeature == null)
            {
                return HttpNotFound();
            }
            return View(valvesectionfeature);
        }

        //
        // POST: /ValveSectionFeature/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ValveSectionFeature valvesectionfeature = db.ValveSectionFeatures.Find(id);
            db.ValveSectionFeatures.Remove(valvesectionfeature);
            db.SaveChanges();

            var deleteValveSectionErrors =
                from e in db.ValveSectionErrors
                where e.ValveSectionFeatureID == id
                select e;

            foreach (var item in deleteValveSectionErrors)
            {
                db.ValveSectionErrors.Remove(item);
            }
            db.SaveChanges();

            var deleteFeatureIssues =
                from i in db.FeatureIssues
                where i.ValveSectionFeatureID == id
                select i;

            foreach (var item in deleteFeatureIssues)
            {
                db.FeatureIssues.Remove(item);
            }
            db.SaveChanges();

            UpdateValveSectionHeaderLengths(valvesectionfeature.ValveSectionID);

            UpdateValveSectionToDirtyAsNeeded(valvesectionfeature.ValveSectionID);

            return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
        }

        //
        // GET: /ValveSectionFeature/Errors/5

        public ActionResult Errors()
        {
            return View();
        }

        //
        // POST: /ValveSectionFeature/Errors/5

        [HttpPost, ActionName("Errors")]
        public ActionResult ErrorsConfirmed()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var valvesectionfeature = db.ValveSectionFeatures.Where(v => v.ValveSectionID == currSection).ToList();
            
            foreach (var item in valvesectionfeature)
            {
                GenerateValveSectionErrors(item);
            }
            GenerateGISBeginEndErrors();

            return RedirectToAction("Index", new { ValveSectionID = Session["CurrentValveSection"].ToString(), OrionStationSeries = Session["CurrentOrionStationSeries"].ToString() });
        }

        //
        // GET: /ValveSectionFeature/Resegment/5

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult MatrixCheck(string checktype, int id1, int id2)
        {
            var results = from d in db.DocumentMats
                          join r1 in db.DocumentRecords on d.DocumentTypeID1 equals r1.DocumentTypeID
                          join r2 in db.DocumentRecords on d.DocumentTypeID2 equals r2.DocumentTypeID
                                 where (
                                 (r1.DocumentRecordID == id1 
                                 && r2.DocumentRecordID == id2 
                                 && d.DocumentTypeID1 == r1.DocumentTypeID
                                 && d.DocumentTypeID2 == r2.DocumentTypeID)
                                 || 
                                 (r1.DocumentRecordID == id1  
                                 && r2.DocumentRecordID == id1 
                                 && d.DocumentTypeID1 == r1.DocumentTypeID
                                 && d.DocumentTypeID2 == r1.DocumentTypeID)
                                 ||
                                 (r1.DocumentRecordID == id2
                                 && r2.DocumentRecordID == id2
                                 && d.DocumentTypeID1 == r2.DocumentTypeID
                                 && d.DocumentTypeID2 == r2.DocumentTypeID)
                                 )
                                 && d.ComboType == checktype
                                 select new { Value = d.DocumentMatID, Text = d.DocumentMatID };

            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public bool MatrixCheckforErrors(string checktype, int id1, int id2)
        {
            var results = from d in db.DocumentMats
                          join r1 in db.DocumentRecords on d.DocumentTypeID1 equals r1.DocumentTypeID
                          join r2 in db.DocumentRecords on d.DocumentTypeID2 equals r2.DocumentTypeID
                          where (
                          (r1.DocumentRecordID == id1
                          && r2.DocumentRecordID == id2
                          && d.DocumentTypeID1 == r1.DocumentTypeID
                          && d.DocumentTypeID2 == r2.DocumentTypeID)
                          ||
                          (r1.DocumentRecordID == id1
                          && r2.DocumentRecordID == id1
                          && d.DocumentTypeID1 == r1.DocumentTypeID
                          && d.DocumentTypeID2 == r1.DocumentTypeID)
                          ||
                          (r1.DocumentRecordID == id2
                          && r2.DocumentRecordID == id2
                          && d.DocumentTypeID1 == r2.DocumentTypeID
                          && d.DocumentTypeID2 == r2.DocumentTypeID)
                          )
                          && d.ComboType.Trim() == checktype
                          select new { Value = d.DocumentMatID, Text = d.DocumentMatID };

            if (results.Count() > 0)
                if (results.First().Value > 0)
                    return false;
            return true;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadFeatureNumbers()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var availfeatures = from v in db.ValveSectionFeatures
                                where v.ValveSectionID == currSection
                                orderby v.FeatureNumber
                                select new { Value = v.ValveSectionFeatureID, Text = v.FeatureNumber };

            return Json(availfeatures, JsonRequestBehavior.AllowGet);
        }
        
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadPipeTypes(int featureid)
        {
            var availpipetypes = from p in db.PipeTypes
                                 where p.FeatureID == featureid
                                 orderby p.PipeTypeItem
                                 select new { Value = p.PipeTypeID, Text = p.PipeTypeItem };

            return Json(availpipetypes, JsonRequestBehavior.AllowGet);
        }
        
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadFeatures()
        {
            var availfeatures = from f in db.Features
                                 orderby f.FeatureItem
                                 select new { Value = f.FeatureID, Text = f.FeatureItem };

            return Json(availfeatures, JsonRequestBehavior.AllowGet);
        }
        
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadDocuments()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var availdocuments = from d in db.DocumentRecords
                                 join t in db.DocumentTypes on d.DocumentTypeID equals t.DocumentTypeID
                                 join i in db.RecordIdentifiers on d.DocumentRecordID equals i.RecordIdentifierID
                                 where d.PipelineID == currSection
                                 orderby d.DocumentRecordID
                                 select new { Value = d.DocumentRecordID, Text = i.RecordIdentifierItem };

            return Json(availdocuments, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadODs()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var ODs = from o in db.OutsideDiameters
                                 orderby o.OutsideDiameterItem descending 
                                 select new { Value = o.OutsideDiameterID, Text = o.OutsideDiameterItem };

            return Json(ODs, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSeamWelds()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var SeamWelds = from s in db.SeamTypes
                      orderby s.SeamTypeItem
                      select new { Value = s.SeamTypeID, Text = s.SeamTypeItem };

            return Json(SeamWelds, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSpecRatings()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var SpecRatings = from s in db.SpecRatings
                            orderby s.SpecRatingItem
                            select new { Value = s.SpecRatingID, Text = s.SpecRatingItem };

            return Json(SpecRatings, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadGrades()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var Grades = from g in db.Grades
                              orderby g.GradeItem
                              select new { Value = g.GradeID, Text = g.GradeItem };

            return Json(Grades, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadANSIRatings()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var ANSIRatings = from a in db.ANSIRatings
                         orderby a.ANSIRatingItem
                         select new { Value = a.ANSIRatingID, Text = a.ANSIRatingItem };

            return Json(ANSIRatings, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadRadius()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var Radiuses = from r in db.BendRadiuses
                              orderby r.BendRadiusItem
                              select new { Value = r.BendRadiusID, Text = r.BendRadiusItem };

            return Json(Radiuses, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadOrientations()
        {
            Int64 currSection = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var Orientations = from o in db.Orientations
                           orderby o.OrientationItem
                           select new { Value = o.OrientationID, Text = o.OrientationItem };

            return Json(Orientations, JsonRequestBehavior.AllowGet);
        }

        public void UpdateValveSectionToDirtyAsNeeded(Int64 valvesectionid)
        {
            ValveSection vs = (from vs1 in db.ValveSection
                               where vs1.ValveSectionID == valvesectionid
                               select vs1).SingleOrDefault();
            Int32 status = Convert.ToInt32(vs.ValveSectionStatusID);

            DisplayGroup dg = (from d in db.DisplayGroups
                               join s in db.ValveSectionStatus on d.DisplayGroupID equals s.DisplayGroupID
                               where s.ValveSectionStatusID == status
                               select d).SingleOrDefault();

            //if (dg.DisplayGroupName == "Engineering" || dg.DisplayGroupName == "Final Engineering"
            //    || dg.DisplayGroupName == "Annual Review")
            //{
            //    vs.IsSegmentationDirty = true;
            //    db.SaveChanges();
            //}
        }

        public void UpdateValveSectionHeaderLengths(Int64 valvesectionid)
        {
            // Update Valve Section Header with length totals
            decimal sum = 0, plus = 0, minus = 0;
            try {
                var lengthSum = db.ValveSectionFeatures.Where(v => v.ValveSectionID == valvesectionid).Sum(v => v.Length);
                sum = lengthSum;
            }
            catch { }
            //try {
            //    var discPlusSum = db.ValveSectionFeatures.Where(v => v.ValveSectionID == valvesectionid).Where(v => v.LengthDiscrepancy > 0).Sum(v => v.LengthDiscrepancy);
            //    plus = discPlusSum;
            //}
            //catch { }
            //try
            //{
            //    var discMinusSum = db.ValveSectionFeatures.Where(v => v.ValveSectionID == valvesectionid).Where(v => v.LengthDiscrepancy < 0).Sum(v => v.LengthDiscrepancy);
            //    minus = discMinusSum;
            //}
            //catch { }

            ValveSection vs = (from vs1 in db.ValveSection
                               where vs1.ValveSectionID == valvesectionid
                               select vs1).SingleOrDefault();
            vs.PFLLength = sum;
            //vs.LengthDiscrepancyPlus = (vs.OrionStationEnd - vs.OrionStationBegin) - vs.PFLLength;
            //vs.LengthDiscrepancyMinus = 0;
            db.SaveChanges();
        }

        public void GenerateValveSectionErrors(ValveSectionFeature valvesectionfeature)
        {
            // Get Feature and Pipe Type
            string featurepipetype = "", strFeature = "", strPipeType = "";
            bool boolODMatrixCheckError = false, boolWTMatrixCheckError = false, boolSTMatrixCheckError = false, boolSRMatrixCheckError = false;
            try
            {
                var feature = (from f in db.Features where f.FeatureID == valvesectionfeature.FeatureID select new { f.FeatureItem }).First();
                strFeature = feature.FeatureItem;
                var pipetype = (from p in db.PipeTypes where p.PipeTypeID == valvesectionfeature.TypeID select new { p.PipeTypeItem }).First();
                strPipeType = pipetype.PipeTypeItem;
                if (feature.FeatureItem == "Skip" && pipetype.PipeTypeItem == "No Records")
                {
                    featurepipetype = "SkipNoRecords";
                    /////////////////////////////////////////////////////
                    // Set all matrix checks to "No"
                    valvesectionfeature.ODRecordMatrixCheck = 0;
                    valvesectionfeature.WTRecordMatrixCheck = 0;
                    valvesectionfeature.SeamRecordMatrixCheck = 0;
                    valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
                    /////////////////////////////////////////////////////
                }
                else if (feature.FeatureItem == "Skip")
                {
                    featurepipetype = "SkipFacility";
                    /////////////////////////////////////////////////////
                    // Set all matrix checks to "No"
                    valvesectionfeature.ODRecordMatrixCheck = 0;
                    valvesectionfeature.WTRecordMatrixCheck = 0;
                    valvesectionfeature.SeamRecordMatrixCheck = 0;
                    valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
                    /////////////////////////////////////////////////////
                }
                else if (feature.FeatureItem == "Pipe")
                {
                    featurepipetype = "Pipe";
                    /////////////////////////////////////////////////////
                    // Set all matrix checks to "Yes"
                    valvesectionfeature.ODRecordMatrixCheck = 1;
                    valvesectionfeature.WTRecordMatrixCheck = 1;
                    valvesectionfeature.SeamRecordMatrixCheck = 1;
                    valvesectionfeature.SpecRatingRecordMatrixCheck = 1;
                    /////////////////////////////////////////////////////
                }
                else if (feature.FeatureItem == "Non Rated")
                {
                    featurepipetype = "Non Rated";
                    /////////////////////////////////////////////////////
                    // Set all matrix checks to "Yes"
                    valvesectionfeature.ODRecordMatrixCheck = 1;
                    valvesectionfeature.WTRecordMatrixCheck = 1;
                    valvesectionfeature.SeamRecordMatrixCheck = 1;
                    valvesectionfeature.SpecRatingRecordMatrixCheck = 1;
                    /////////////////////////////////////////////////////
                }
                else
                {
                    featurepipetype = "Invalid";
                    // Set all matrix checks to "No"
                    valvesectionfeature.ODRecordMatrixCheck = 0;
                    valvesectionfeature.WTRecordMatrixCheck = 0;
                    valvesectionfeature.SeamRecordMatrixCheck = 0;
                    valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
                    /////////////////////////////////////////////////////
                }
            }
            catch
            {
                featurepipetype = "Invalid";
                // Set all matrix checks to "No"
                valvesectionfeature.ODRecordMatrixCheck = 0;
                valvesectionfeature.WTRecordMatrixCheck = 0;
                valvesectionfeature.SeamRecordMatrixCheck = 0;
                valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
                /////////////////////////////////////////////////////
            }
            // Get Warning and Error IDs
            var errorLevels = (from l in db.ValveSectionErrorLevels orderby l.ValveSectionErrorLevelItem select new { l.ValveSectionErrorLevelID }).ToList();
            int warningid, errorid;
            string strWarning = "WARNING: ";
            string strError = "ERROR: ";
            //string noValue = " - No Value Provided";
            errorid = errorLevels.First().ValveSectionErrorLevelID;
            warningid = errorLevels.Last().ValveSectionErrorLevelID;

            // clear all existing errors for the feature
            var deleteValvesectionErrors =
                from e in db.ValveSectionErrors
                where e.ValveSectionID == valvesectionfeature.ValveSectionID
                && e.ValveSectionFeatureID == valvesectionfeature.ValveSectionFeatureID
                select e;
            foreach (var err in deleteValvesectionErrors)
            {
                db.ValveSectionErrors.Remove(err);
            }
            try
            {
                db.SaveChanges();
            }
            catch { }

            ValveSectionError vse = new ValveSectionError { };

            /////////////////////////////////////////////////////////////////////
            // Generate Errors
            /////////////////////////////////////////////////////////////////////

            /////////////////////
            // GIS Align 
            /////////////////////
            if (valvesectionfeature.GISAlignStart > valvesectionfeature.GISAlignEnd)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "GISBeginGreaterEnd", false);
            }
            /////////////////////
            // Length
            /////////////////////            
            if (valvesectionfeature.Length < 0)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "Length<0", false);
            }
            /////////////////////
            // Length Discrepancy
            /////////////////////            
            if (valvesectionfeature.LengthDiscrepancy != 0)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "LengthDiscNot0", false);
            }
            /////////////////////
            // Feature Type
            /////////////////////            
            if (valvesectionfeature.FeatureID == 0 || valvesectionfeature.FeatureID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "FeatureNoValue", true);
            }
            /////////////////////
            // Pipe Type
            /////////////////////            
            if (valvesectionfeature.TypeID == 0 || valvesectionfeature.TypeID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "FeatureTypeNoValue", true);
            }
            /////////////////////
            // Job / WO / PO
            /////////////////////
            if (valvesectionfeature.JobWOPO == null && valvesectionfeature.JobWOPOUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "JobWOPONoUnknown", false);
            }
            /////////////////////
            // Install Date
            /////////////////////
            if (valvesectionfeature.InstallDate == null && valvesectionfeature.InstallDateUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "InstallNoUnknown", false);
            }
            /////////////////////
            // In-Service Date
            /////////////////////            
            if (valvesectionfeature.InServiceDate == null && valvesectionfeature.InServiceDateUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "InServiceNoUnknown", false);
            }
            /////////////////////
            // Construction Type
            /////////////////////            
            if (valvesectionfeature.ConstructionTypeID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "ConstructionTypeNoValue", true);
            }
            /////////////////////
            // Drawing Identifier
            /////////////////////
            if (valvesectionfeature.DrawingID == 0 || valvesectionfeature.DrawingID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "DrawingIDNoValue", true);
            }
            /////////////////////
            // OD1
            /////////////////////
            if ((valvesectionfeature.ODID1 == 0 || valvesectionfeature.ODID1 == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "OD1NoValue", true);
            }
            if (featurepipetype == "SkipNoRecords")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "ODMatrixCheck", false);
                boolODMatrixCheckError = true;
                valvesectionfeature.ODRecordMatrixCheck = 0;
            }
            else if (MatrixCheckforErrors("OD", (valvesectionfeature.ODRecordID1 == null ? 0 : valvesectionfeature.ODRecordID1.Value),
                (valvesectionfeature.ODRecordID2 == null ? 0 : valvesectionfeature.ODRecordID2.Value))
                && (featurepipetype == "Pipe" || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "ODMatrixCheck", false);
                boolODMatrixCheckError = true;
                valvesectionfeature.ODRecordMatrixCheck = 0;
            } 
            if ((valvesectionfeature.ODRecordID1 == 0 || valvesectionfeature.ODRecordID1 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolODMatrixCheckError ? errorid : warningid), (boolODMatrixCheckError ? strError : strWarning), "ODID1NoValue", true);
            }
            if ((valvesectionfeature.ODRecordID2 == 0 || valvesectionfeature.ODRecordID2 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolODMatrixCheckError ? errorid : warningid), (boolODMatrixCheckError ? strError : strWarning), "ODID2NoValue", true);
            } 
            /////////////////////
            // WT1
            /////////////////////
            if ((valvesectionfeature.WallThickness1 == null) &&
                (featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe"))// || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "WT1NoValue", true);
            }
            if (featurepipetype == "SkipNoRecords")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "WTMatrixCheck", false);
                boolWTMatrixCheckError = true;
                valvesectionfeature.WTRecordMatrixCheck = 0;
            }
            else if (MatrixCheckforErrors("WT", (valvesectionfeature.WTRecordID1 == null ? 0 : valvesectionfeature.WTRecordID1.Value),
                (valvesectionfeature.WTRecordID2 == null ? 0 : valvesectionfeature.WTRecordID2.Value))
                && (featurepipetype == "Pipe" || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "WTMatrixCheck", false);
                boolWTMatrixCheckError = true;
                valvesectionfeature.WTRecordMatrixCheck = 0;
            } 
            if ((valvesectionfeature.WTRecordID1 == 0 || valvesectionfeature.WTRecordID1 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated" && strPipeType != "No Casing")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolWTMatrixCheckError ? errorid : warningid), (boolWTMatrixCheckError ? strError : strWarning), "WTID1NoValue", true);
            }
            if ((valvesectionfeature.WTRecordID2 == 0 || valvesectionfeature.WTRecordID2 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolWTMatrixCheckError ? errorid : warningid), (boolWTMatrixCheckError ? strError : strWarning), "WTID2NoValue", true);
            }
            /////////////////////
            // OD2
            /////////////////////
            if ((valvesectionfeature.ODID2 == 0 || valvesectionfeature.ODID2 == null) && featurepipetype == "SkipNoRecords")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "OD2NoValue", true);
            }
            if ((valvesectionfeature.ODID2 == 0 || valvesectionfeature.ODID2 == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "Pipe")
                && strPipeType != "No Casing")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "OD2NoValue", true);
            }
            /////////////////////
            // WT2
            /////////////////////
            if (valvesectionfeature.WallThickness2 == null
                && (featurepipetype == "SkipNoRecords" || (featurepipetype == "Pipe" && strPipeType == "Casing")))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "WT2NoValue", true);
            }
            /////////////////////
            // Seam Welds
            /////////////////////
            if ((valvesectionfeature.SeamWeldTypeID == 0 || valvesectionfeature.SeamWeldTypeID == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe"))// || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "SeamTypeNoValue", true);
            }
            if (featurepipetype == "SkipNoRecords")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "STMatrixCheck", false);
                boolSTMatrixCheckError = true;
                valvesectionfeature.SeamRecordMatrixCheck = 0;
            }
            else if (MatrixCheckforErrors("ST", (valvesectionfeature.SeamRecordID1 == null ? 0 : valvesectionfeature.SeamRecordID1.Value),
                (valvesectionfeature.SeamRecordID2 == null ? 0 : valvesectionfeature.SeamRecordID2.Value))
                && (featurepipetype == "Pipe" || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "STMatrixCheck", false);
                boolSTMatrixCheckError = true;
                valvesectionfeature.SeamRecordMatrixCheck = 0;
            } 
            if ((valvesectionfeature.SeamRecordID1 == 0 || valvesectionfeature.SeamRecordID1 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolSTMatrixCheckError ? errorid : warningid), (boolSTMatrixCheckError ? strError : strWarning), "STID1NoValue", true);
            }
            if ((valvesectionfeature.SeamRecordID2 == 0 || valvesectionfeature.SeamRecordID2 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolSTMatrixCheckError ? errorid : warningid), (boolSTMatrixCheckError ? strError : strWarning), "STID2NoValue", true);
            }
            /////////////////////////
            // Spec Rating / Grade
            /////////////////////////
            if ((valvesectionfeature.SpecRatingID == 0 || valvesectionfeature.SpecRatingID == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe")) //|| featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "SpecRatingNoValue", true); 
            }
            if ((valvesectionfeature.GradeID == 0 || valvesectionfeature.GradeID == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe"))// || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "GradeNoValue", true); 
            }
            if (featurepipetype == "SkipNoRecords")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "SRMatrixCheck", false); 
                boolSRMatrixCheckError = true;
                valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
            }
            else if (MatrixCheckforErrors("SR", (valvesectionfeature.SpecRatingRecordID1 == null ? 0 : valvesectionfeature.SpecRatingRecordID1.Value),
                (valvesectionfeature.SpecRatingRecordID2 == null ? 0 : valvesectionfeature.SpecRatingRecordID2.Value))
                && (featurepipetype == "Pipe" || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "SRMatrixCheck", false); 
                boolSRMatrixCheckError = true;
                valvesectionfeature.SpecRatingRecordMatrixCheck = 0;
            } 
            if ((valvesectionfeature.SpecRatingRecordID1 == 0 || valvesectionfeature.SpecRatingRecordID1 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolSRMatrixCheckError ? errorid : warningid), (boolSRMatrixCheckError ? strError : strWarning), "SRID1NoValue", true);
            }
            if ((valvesectionfeature.SpecRatingRecordID2 == 0 || valvesectionfeature.SpecRatingRecordID2 == null)
                && featurepipetype != "SkipFacility" && featurepipetype != "Non Rated")
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, (boolSRMatrixCheckError ? errorid : warningid), (boolSRMatrixCheckError ? strError : strWarning), "SRID2NoValue", true);
            }
            /////////////////////
            // ANSI Rating
            /////////////////////
            if ((featurepipetype == "SkipNoRecords")
                && (valvesectionfeature.ANSIRatingID == null || valvesectionfeature.ANSIRatingID == 0))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "ANSIRatingNoValue", true); 
            }
            /////////////////////
            // Material Types
            /////////////////////
            if ((valvesectionfeature.MaterialTypeID == 0 || valvesectionfeature.MaterialTypeID == null) &&
                (featurepipetype == "Invalid" || featurepipetype == "SkipNoRecords" || featurepipetype == "Pipe" || featurepipetype == "Non Rated"))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "MaterialTypeNoValue", true); 
            }
            /////////////////////
            // Angles
            /////////////////////
            if ((featurepipetype == "SkipNoRecords") && valvesectionfeature.AngleID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "AngleNoValue", true); 
            }
            /////////////////////
            // Radius
            /////////////////////
            if ((featurepipetype == "SkipNoRecords")
                && (valvesectionfeature.RadiusID == null || valvesectionfeature.RadiusID == 0))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "RadiusNoValue", true); 
            }
            /////////////////////
            // Orientations
            /////////////////////
            if ((featurepipetype == "SkipNoRecords")
                && (valvesectionfeature.OrientID == null || valvesectionfeature.OrientID == 0))
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, warningid, strWarning, "OrientationNoValue", true); 
            }
            /////////////////////
            // Coating Types
            /////////////////////
            var description = db.CoatingTypes.Where(v => v.CoatingTypeID == valvesectionfeature.CoatingTypeID);
            try
            {
                if ((description.First().CoatingTypeItem == "Unknown" || description.First().CoatingTypeItem == "Other")
                && valvesectionfeature.Description == null
                && valvesectionfeature.DescriptionUnknown == false)
                {
                    InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "CoatingUnknownNoDesc", false); 
                }
            }
            catch { }
            if (valvesectionfeature.Description == null && valvesectionfeature.DescriptionUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "CoatingNoUnknown", false); 
            }
            /////////////////////
            // Mill
            /////////////////////
            if (valvesectionfeature.Mill == null && valvesectionfeature.MillUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "MillNoUnknown", false); 
            }
            /////////////////////
            // Manufacturer Type
            /////////////////////
            if (valvesectionfeature.ManufacturerTypeID == null)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "ManufacturerTypeNoValue", true);
            }
            /////////////////////
            // MFR Date
            /////////////////////
            if (featurepipetype != "SkipFacility" && valvesectionfeature.MFRDate == null && valvesectionfeature.MFRDateUnknown == false)
            {
                InsertError(valvesectionfeature.ValveSectionID, valvesectionfeature.ValveSectionFeatureID, errorid, strError, "MFRDateNoUnknown", false); 
            }
            
            /// Update ValveSectionFeature with Matrix Checks performed
            db.Entry(valvesectionfeature).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void GenerateGISBeginEndErrors()
        {
            Int64 currValveSectionID = Convert.ToInt64(Session["CurrentValveSection"].ToString());
            var valvesectionfeatures = (from v in db.ValveSectionFeatures
                             where v.ValveSectionID == currValveSectionID
                             orderby v.GISAlignStart
                             select new
                             {
                                 ValveSectionFeature = v
                             }).ToList();
            
            decimal lastend = 0, currstart = 0;
            Int64 lastfeatureid = 0, currfeatureid = 0;
            bool first = true;

            try
            {
                var valvesectionerrors = db.ValveSectionErrors.Where(f => f.ValveSectionID == currValveSectionID)
                    .Where(f => f.ErrorDescription == "ERROR: GIS Alignment Sheet Begin Station does not equal GIS Alignment Sheet End Station of previous feature" || f.ErrorDescription == "ERROR: GIS Alignment Sheet End Station does not equal GIS Alignment Sheet Begin Station of next feature")
                    .ToList();
                foreach (var item in valvesectionerrors)
                {
                    db.ValveSectionErrors.Remove(item);
                    db.SaveChanges();
                }
            }
            catch { }

            // Get Error IDs
            var errorLevels = (from l in db.ValveSectionErrorLevels orderby l.ValveSectionErrorLevelItem select new { l.ValveSectionErrorLevelID }).ToList();
            int errorid;
            string strError = "ERROR: ";
            errorid = errorLevels.First().ValveSectionErrorLevelID;

            foreach (var valvesectionfeature in valvesectionfeatures)
            {
                currstart = valvesectionfeature.ValveSectionFeature.GISAlignStart;
                currfeatureid = Convert.ToInt64(valvesectionfeature.ValveSectionFeature.ValveSectionFeatureID);

                if (!first)
                {
                    if (currstart != lastend)
                    {
                        InsertError(currValveSectionID, currfeatureid, errorid, strError, "GISBeginNotEqPrevEnd", false);
                        InsertError(currValveSectionID, lastfeatureid, errorid, strError, "GISEndNotEqNextBegin", false);
                    }
                }
                else
                    first = false;
                
                lastend = valvesectionfeature.ValveSectionFeature.GISAlignEnd;
                lastfeatureid = Convert.ToInt64(valvesectionfeature.ValveSectionFeature.ValveSectionFeatureID);
            }
        }
        public void InsertError(Int64 valveSectionID, Int64 valveSectionFeatureID, int errorLevelID, string errorLevelString, string featureErrorItem, bool NoValue)
        {
            string errorDescription = "";

            var errorText = (from f in db.FeatureErrors
                     where f.FeatureErrorItem == featureErrorItem
                     select new
                     {
                         ErrorText = f.ErrorText,
                         FeatureErrorID = f.FeatureErrorID
                     }).First();
                    
            if (NoValue)
                errorDescription = errorLevelString + errorText.ErrorText + " - No Value Provided";
            else
                errorDescription = errorLevelString + errorText.ErrorText;

            ValveSectionError vse = new ValveSectionError
            {
                ValveSectionID = valveSectionID,
                ValveSectionFeatureID = valveSectionFeatureID,
                ValveSectionErrorLevelID = errorLevelID,
                ErrorDescription = errorDescription,
                FeatureErrorID = errorText.FeatureErrorID
            };
            db.ValveSectionErrors.Add(vse);
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}