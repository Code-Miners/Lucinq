using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Lucinq.SitecoreIntegration.UnitTests.IntegrationTests
{
    [TestFixture]
    [Ignore("This fixture is solely for the purposes of creating a test setup on a blank sitecore db with the base setup package.")]
    public class Creation
    {
        private readonly TemplateID makePageTemplateId = new TemplateID(new ID("{A78D0869-DE46-40C7-AB7D-FED5718BEF3C}"));
        private readonly TemplateID modelPageTemplateId = new TemplateID(new ID("{C867C440-7590-4DE8-987D-4E28CDEC9764}"));
        private readonly TemplateID usedTemplateId = new TemplateID(new ID("{EF967FFC-CD37-4B04-93AD-82F288BF0468}"));
        private readonly TemplateID leaseTemplateId = new TemplateID(new ID("{E66DF66A-AA6A-4BB0-95EB-D83E61BA3E37}"));
        private Database masterDatabase;
        private Item homepageItem;
        private readonly Dictionary<int, ID> modelIds = new Dictionary<int, ID>();

        private Item leaseCarsItem;
        private Item usedCarsItem;
        private Item makesPageItem;


        protected Database MasterDatabase
        {
            get
            {
                return masterDatabase ?? (masterDatabase = Factory.GetDatabase("master"));
            }
        }

        protected Item HomepageItem
        {
            get
            {
                return homepageItem ?? ( homepageItem = MasterDatabase.GetItem(SitecoreIds.HomepageItemId));
            }
        }

        protected Item LeaseCarsItem
        {
            get
            {
                return leaseCarsItem ?? (leaseCarsItem = MasterDatabase.GetItem(SitecoreIds.LeaseCarsId));
            }
        }

        protected Item UsedCarsItem
        {
            get
            {
                return usedCarsItem ?? (usedCarsItem = MasterDatabase.GetItem(SitecoreIds.UsedCarsId));
            }
        }

        protected Item MakesPageItem
        {
            get
            {
                return makesPageItem ?? (makesPageItem = MasterDatabase.GetItem(SitecoreIds.MakesPageId));
            }
        }

        [Test]
        public void CreateMakesAndModels()
        {
            using (new SecurityDisabler())
            {
                using (new BulkUpdateContext())
                {
                    using (
                        SqlConnection connection =
                            new SqlConnection(ConfigurationManager.ConnectionStrings["cardata"].ConnectionString))
                    {
                        connection.Open();
                        using (SqlCommand makesCommand = new SqlCommand("up_getallmakes", connection))
                        {
                            SqlDataReader reader = makesCommand.ExecuteReader();
                            while (reader.Read())
                            {
                                string makeName = reader["Name"].ToString();
                                string validName = ItemUtil.ProposeValidItemName(makeName);
                                Item makeItem = MakesPageItem.Add(validName, makePageTemplateId);
                                using (new EditContext(makeItem))
                                {
                                    bool displayInUsedCars = (bool) reader["DisplayInUsedDropDown"];
                                    bool displayInNewCars = (bool) reader["DisplayInNewCars"];
                                    makeItem["Display In Used Cars"] = displayInUsedCars ? "1" : "0";
                                    makeItem["Display In New Cars"] = displayInNewCars ? "1" : "0";
                                }
                                int makeId = (int) reader["MakeId"];
                                AddModels(makeId, makeItem);
                            }
                        }
                    }
                    AddAdverts();
                }
            }
        }

        private void AddAdverts()
        {
            var usedCarDataItems = GetUsedCarDataItems();
            foreach (var carDataItem in usedCarDataItems)
            {
                AddUsedAdvert(carDataItem);
            }

            var carItems = GetLeaseCarDataItems();
            foreach (var carDataItem in carItems)
            {
                AddLeaseAdvert(carDataItem);
            }
        }

        private void AddUsedAdvert(UsedCarDataItem usedCarDataItem)
        {
            if (!modelIds.ContainsKey(usedCarDataItem.ModelId))
            {
                return;
            }
            string validName =
                ItemUtil.ProposeValidItemName(usedCarDataItem.Code + "-" + usedCarDataItem.Make + " " + usedCarDataItem.Model);
            Item advertItem = UsedCarsItem.Add(validName, usedTemplateId);
            using (new EditContext(advertItem))
            {
                PopulateAdvertBaseData(usedCarDataItem, advertItem);
                advertItem["Town"] = usedCarDataItem.Town;
                advertItem["County"] = usedCarDataItem.County;
                advertItem["Postcode"] = usedCarDataItem.Postcode;
                advertItem["Variant"] = usedCarDataItem.Variant;
                advertItem["Options"] = usedCarDataItem.Options;
                advertItem["Mileage"] = usedCarDataItem.Options;
            }
        }

        private void AddLeaseAdvert(LeaseCarDataItem leaseCarDataItem)
        {
            if (!modelIds.ContainsKey(leaseCarDataItem.ModelId))
            {
                return;
            }
            string validName =
                ItemUtil.ProposeValidItemName(leaseCarDataItem.Code + "-" + leaseCarDataItem.Make + " " + leaseCarDataItem.Model);
            Item advertItem = LeaseCarsItem.Add(validName, leaseTemplateId);
            using (new EditContext(advertItem))
            {
                PopulateAdvertBaseData(leaseCarDataItem, advertItem);
                advertItem["Payment Profile"] = leaseCarDataItem.PaymentProfile;
                advertItem["Term"] = leaseCarDataItem.Term;
                advertItem["Description Summary"] = leaseCarDataItem.DescriptionSummary;
            }
        }

        private void PopulateAdvertBaseData(CarDataItem usedCarDataItem, Item advertItem)
        {
            advertItem["Model"] = modelIds[usedCarDataItem.ModelId].ToString();
            advertItem["Created Date"] = DateUtil.ToIsoDate(usedCarDataItem.Created);
            advertItem["Code"] = usedCarDataItem.Code;
            advertItem["Is Live"] = usedCarDataItem.AdvertStatusId > 0 ? "1" : "0";
            advertItem["Price"] = usedCarDataItem.Price.ToString();
            advertItem["Description"] = usedCarDataItem.Description.ToString();
        }

        private void AddModels(int makeId, Item makeItem)
        {
            using (
                SqlConnection modelsConnection =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["cardata"].ConnectionString))
            {
                modelsConnection.Open();
                using (SqlCommand modelsCommand = new SqlCommand("up_getallmodels", modelsConnection))
                {
                    modelsCommand.CommandType = CommandType.StoredProcedure;
                    modelsCommand.Parameters.Add(new SqlParameter("@MakeId", makeId));
                    var modelsReader = modelsCommand.ExecuteReader();
                    while (modelsReader.Read())
                    {
                        string modelName = modelsReader["Name"].ToString();
                        if (string.IsNullOrEmpty(modelName))
                        {
                            continue;
                        }
                        bool popularUsedCar = (bool) modelsReader["PopularUsedCar"];
                        bool popularNewCar = (bool) modelsReader["PopularNewCar"];
                        bool popularLeaseCar = (bool) modelsReader["PopularLeaseCar"];
                        string validModelName = ItemUtil.ProposeValidItemName(modelName);
                        Item modelItem = makeItem.Add(validModelName, modelPageTemplateId);
                        using (new EditContext(modelItem))
                        {
                            modelItem["Popular Used Car"] = popularUsedCar ? "1" : "0";
                            modelItem["Popular New Car"] = popularNewCar ? "1" : "0";
                            modelItem["Popular Lease Car"] = popularLeaseCar ? "1" : "0";
                        }
                        int modelId = (int) modelsReader["ModelId"];
                        modelIds.Add(modelId, modelItem.ID);
                    }
                }
            }
        }

        private static IEnumerable<UsedCarDataItem> GetUsedCarDataItems()
        {
            List<UsedCarDataItem> carDataItems = new List<UsedCarDataItem>();

            using (
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CarData"].ConnectionString)
                )
            {
                using (SqlCommand command = new SqlCommand("usp_LucinqUsedData", connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // ignore data with no price
                        if (reader["Price"] == DBNull.Value)
                        {
                            continue;
                        }
                        UsedCarDataItem item = new UsedCarDataItem
                        {

                            Town = reader.GetValueOrDefault<string>("Town"),
                            County = reader.GetValueOrDefault<string>("County"),
                            Postcode = reader.GetValueOrDefault<string>("Postcode"),
                            Mileage = reader.GetValueOrDefault<decimal?>("Mileage"),
                            Variant = reader.GetValueOrDefault<string>("Variant"),
                            Options = reader.GetValueOrDefault<string>("Options"),
                            FuelType = reader.GetValueOrDefault<string>("FuelType")
                        };
                        PopulateUsedCarItem(reader, item);
                        carDataItems.Add(item);
                    }
                }
            }
            return carDataItems;
        }

        private static IEnumerable<LeaseCarDataItem> GetLeaseCarDataItems()
        {
            List<LeaseCarDataItem> carDataItems = new List<LeaseCarDataItem>();

            using (
                SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CarData"].ConnectionString)
                )
            {
                using (SqlCommand command = new SqlCommand("usp_LucinqLeaseData", connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // ignore data with no price
                        if (reader["Price"] == DBNull.Value)
                        {
                            continue;
                        }
                        LeaseCarDataItem item = new LeaseCarDataItem
                        {
                            DescriptionSummary = reader.GetValueOrDefault<string>("DescriptionSummary"),
                            Term = reader.GetValueOrDefault<string>("Term"),
                            PaymentProfile = reader.GetValueOrDefault<string>("PaymentProfile")
                        };
                        PopulateUsedCarItem(reader, item);
                        carDataItems.Add(item);
                    }
                }
            }
            return carDataItems;
        }

        private static void PopulateUsedCarItem(SqlDataReader reader, CarDataItem carDataItem)
        {
            carDataItem.AdvertId = (Guid) reader["AdvertId"];
            carDataItem.Created = (DateTime) reader["DateCreated"];
            carDataItem.MakeId = (int) reader["MakeId"];
            carDataItem.ModelId = (int) reader["ModelId"];
            carDataItem.Code = reader["Code"].ToString();
            carDataItem.Make = reader["Make"].ToString();
            carDataItem.Model = reader["Model"].ToString();
            carDataItem.Description = reader.GetValueOrDefault<string>("Description");
            carDataItem.AdvertStatusId = (int) reader["AdvertStatusId"];
            carDataItem.AdvertStatus = reader["AdvertStatus"].ToString();
            carDataItem.Price = reader.GetValueOrDefault<decimal>("Price");
        }
    }

    public class UsedCarDataItem : CarDataItem
    {
        public decimal? Mileage { get; set; }
        public string FuelType { get; set; }
        public string Postcode { get; set; }
        public string Variant { get; set; }
        public string Options { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
    }

    public class LeaseCarDataItem : CarDataItem
    {
        public string DescriptionSummary { get; set; }
        public string PaymentProfile { get; set; }
        public string Term { get; set; }
    }

    public class CarDataItem
    {
        public Guid AdvertId { get; set; }
        public DateTime Created { get; set; }
        public int MakeId { get; set; }
        public int ModelId { get; set; }
        public string Code { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int AdvertStatusId { get; set; }
        public string AdvertStatus { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } 
    }


    public static class ReaderExtensions
    {
        public static T GetValueOrDefault<T>(this SqlDataReader reader, string fieldName)
        {
            if (reader[fieldName] == DBNull.Value)
            {
                return default(T);
            }
            return (T)reader[fieldName];
        }
    }
}
