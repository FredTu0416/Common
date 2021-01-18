using CommonService.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoConsole.Models.OjbectCreate
{
    public class OrderEntity
    {
        [ExcelEntity(Field = "mobile", IsRequired = false)]
        public string mobile { get; set; }

        [ExcelEntity(Field = "vin")]
        public string vin { get; set; }

        [ExcelEntity(Field = "enterpriseName")]
        public string enterpriseName { get; set; }

        [ExcelEntity(Field = "enterpriseCode")]
        public string enterpriseCode { get; set; }

        [ExcelEntity(Field = "owner.address.address")]
        public string owner_address_address { get; set; }

        [ExcelEntity(Field = "owner.address.provinceName")]
        public string owner_address_provinceName { get; set; }

        [ExcelEntity(Field = "owner.address.province")]
        public string owner_address_province { get; set; }

        [ExcelEntity(Field = "owner.address.cityName")]
        public string owner_address_cityName { get; set; }

        [ExcelEntity(Field = "owner.address.city")]
        public string owner_address_city { get; set; }

        [ExcelEntity(Field = "owner.address.districtName")]
        public string owner_address_districtName { get; set; }

        [ExcelEntity(Field = "owner.address.district")]
        public string owner_address_district { get; set; }

        [ExcelEntity(Field = "personageName")]
        public string personageName { get; set; }

        [ExcelEntity(Field = "personageMobile")]
        public string personageMobile { get; set; }

        [ExcelEntity(Field = "personageEmail")]
        public string personageEmail { get; set; }

        [ExcelEntity(Field = "personageIdentityNo")]
        public string personageIdentityNo { get; set; }

        [ExcelEntity(Field = "consumer.address.address")]
        public string consumer_address_address { get; set; }

        [ExcelEntity(Field = "consumer.address.provinceName")]
        public string consumer_address_provinceName { get; set; }

        [ExcelEntity(Field = "consumer.address.province")]
        public string consumer_address_province { get; set; }

        [ExcelEntity(Field = "consumer.address.cityName")]
        public string consumer_address_cityName { get; set; }

        [ExcelEntity(Field = "consumer.address.city")]
        public string consumer_address_city { get; set; }

        [ExcelEntity(Field = "consumer.address.districtName")]
        public string consumer_address_districtName { get; set; }

        [ExcelEntity(Field = "consumer.address.district")]
        public string consumer_address_district { get; set; }

        [ExcelEntity(Field = "dealer.name")]
        public string dealer_name { get; set; }

        [ExcelEntity(Field = "dealer.code")]
        public string dealer_code { get; set; }

        [ExcelEntity(Field = "vehicleLicencePlate.provinceName")]
        public string vehicleLicencePlate_provinceName { get; set; }

        [ExcelEntity(Field = "vehicleLicencePlate.province")]
        public string vehicleLicencePlate_province { get; set; }

        [ExcelEntity(Field = "vehicleLicencePlate.cityName")]
        public string vehicleLicencePlate_cityName { get; set; }

        [ExcelEntity(Field = "vehicleLicencePlate.city")]
        public string vehicleLicencePlate_city { get; set; }
    }
}
