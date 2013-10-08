using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Lucinq.Sitecore.UnitTests.IntegrationTests
{
    [TestFixture]
    [Ignore("This fixture is solely for the purposes of creating a test setup on a blank sitecore db with the base setup package.")]
    public class Creation
    {
        private ID homepageItemId = new ID("{82386E72-2F86-4495-9C93-43D68C98132C}");
        private TemplateID makePageTemplateId = new TemplateID(new ID("{A78D0869-DE46-40C7-AB7D-FED5718BEF3C}"));
        private Database masterDatabase;
        private Item homepageItem;

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
                return homepageItem ?? ( homepageItem = MasterDatabase.GetItem(homepageItemId));
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
                                Item makeItem = HomepageItem.Add(validName, makePageTemplateId);
                                using (new EditContext(makeItem))
                                {
                                    bool displayInUsedCars = (bool) reader["DisplayInUsedDropDown"];
                                    bool displayInNewCars = (bool) reader["DisplayInNewCars"];
                                    makeItem["Display In Used Cars"] = displayInUsedCars ? "1" : "0";
                                    makeItem["Display In New Cars"] = displayInNewCars ? "1" : "0";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
