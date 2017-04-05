using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Web;
using System.Globalization;
using Ext.Net;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DeerInformation.Models;

namespace DeerInformation.Areas.user.Models
{
    public class Plant
    {
        public Plant(string id,string name, string site, string no)
        {
            this.ID = id;
            this.Name = name;
            this.Site = site;
            this.No = no;
        }

        public Plant()
        {
        }

        public string ID { get; set; }
        public string No { get; set; }

        public string Site { get; set; }

        public string Name { get; set; }

        public static Paging<Plant> PlantsPaging(int start, int limit, string sort, string dir, string filter)
        {
            List<Plant> plants = Plant.TestData;

            if (!string.IsNullOrEmpty(filter) && filter != "*")
            {
                plants.RemoveAll(plant => !plant.No.ToLower().StartsWith(filter.ToLower()));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                plants.Sort(delegate(Plant x, Plant y)
                {
                    object a;
                    object b;

                    int direction = dir == "DESC" ? -1 : 1;

                    a = x.GetType().GetProperty(sort).GetValue(x, null);
                    b = y.GetType().GetProperty(sort).GetValue(y, null);

                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }

            if ((start + limit) > plants.Count)
            {
                limit = plants.Count - start;
            }

            List<Plant> rangePlants = (start < 0 || limit < 0) ? plants : plants.GetRange(start, limit);

            return new Paging<Plant>(rangePlants, plants.Count);
        }

        public static List<Plant> TestData
        {
            get
            {
                Entities entities = new Entities();
                var pro = (from o in entities.T_GW_FieldInfoManagement
                           select new { o.ID, o.ProjectNo,o.ProjectName,o.FieldName }).ToList();
                List<Plant> data = new List<Plant>();

                foreach (var item in pro)
                {
                    Plant plant = new Plant();

                    plant.ID = item.ID.ToString();
                    plant.No = item.ProjectNo;
                    plant.Name = item.ProjectName;
                    plant.Site = item.FieldName;

                    data.Add(plant);
                }

                return data;
            }
        }
    }
}