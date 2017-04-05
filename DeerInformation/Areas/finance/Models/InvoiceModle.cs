using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Ext.Net.MVC;
using DeerInformation.Models;

namespace DeerInformation.Areas.finance.Models
{
	[Model(Name = "Person")]
	public class InvoiceModle
	{
		[ModelField(IDProperty = true, UseNull = true)]
		[Field(Ignore = true)]
		[Column(Text = "序号 ")]
		public int? Id
		{
			get;
			set;
		}

		[PresenceValidator]
		[Column(Text = "发票编号")]
		public string InvoiceNo
		{
			get;
			set;
		}

		[PresenceValidator]
		[Column(Text = "不含税金额")]
		public decimal? AmountNoTax
		{
			get;
			set;
		}

		[PresenceValidator]
		[Column(Text = "开票日期")]
		public DateTime? InvoiceDate
		{
			get;
			set;
		}

		public int? UId { get; set; }
		public static string ReferenceId;
		public static int CurId;
		private static readonly object LockObj = new object();

		private static int NewId
		{
			get
			{
				return System.Threading.Interlocked.Increment(ref CurId);
			}
		}

		public static List<InvoiceModle> TestData
		{
			get
			{
				using (Entities db = new Entities())
				{
					Storage = new List<InvoiceModle>();
					var dbdata = db.T_FD_InvoiceList.Where(l => l.ReferenceId == ReferenceId).ToList();
					foreach (var item in dbdata)
					{
						Storage.Add(new InvoiceModle()
						{
							Id = NewId,
							UId = item.Id,
							InvoiceNo = item.InvoiceNo,
							AmountNoTax = item.AmountNoTax,
							InvoiceDate = item.InvoiceDate
						});
					}


					return Storage;
				}
			}
		}



		public static List<InvoiceModle> Storage { get; set; }

		public static void Clear()
		{
			InvoiceModle.Storage = null;
		}

		public static int? AddPerson(InvoiceModle person)
		{
			lock (LockObj)
			{
				var persons = InvoiceModle.Storage;
				person.Id = InvoiceModle.NewId;
				try
				{
					UpdateToDatabase(person);
				}
				catch (Exception e)
				{
					throw;
				}
				persons.Add(person);
				InvoiceModle.Storage = persons;

				return person.Id;
			}
		}

		public static void DeletePerson(int id)
		{
			lock (LockObj)
			{
				var persons = InvoiceModle.Storage;
				InvoiceModle person = persons.FirstOrDefault(p => p.Id == id);

				if (person == null)
				{
					throw new Exception("InvoiceModle not found");
				}
				try
				{
					DeleteTFromDatabase(person);
				}
				catch (Exception e)
				{
					throw;
				}
				persons.Remove(person);

				InvoiceModle.Storage = persons;
			}
		}

		public static void UpdatePerson(InvoiceModle person)
		{
			lock (LockObj)
			{
				var persons = InvoiceModle.Storage;
				InvoiceModle updatingPerson = persons.FirstOrDefault(p => p.Id == person.Id);

				if (updatingPerson == null)
				{
					throw new Exception("InvoiceModle not found");
				}
				updatingPerson.UId = person.UId;
				updatingPerson.InvoiceNo = person.InvoiceNo;
				updatingPerson.AmountNoTax = person.AmountNoTax;
				updatingPerson.InvoiceDate = person.InvoiceDate;
				try
				{
					UpdateToDatabase(updatingPerson);
				}
				catch (Exception e)
				{
					throw;
				}
				InvoiceModle.Storage = persons;
			}
		}

		private static void UpdateToDatabase(InvoiceModle updatingPerson)
		{
			using (Entities db = new Entities())
			{
				if (updatingPerson.UId == null || db.T_FD_InvoiceList.Find(updatingPerson.UId) == null)
				{
					db.T_FD_InvoiceList.Add(new T_FD_InvoiceList()
					{
						AmountNoTax = updatingPerson.AmountNoTax,
						InvoiceDate = updatingPerson.InvoiceDate,
						InvoiceNo = updatingPerson.InvoiceNo,
						ReferenceId = ReferenceId
					});
				}
				else
				{
					var dbItem = db.T_FD_InvoiceList.Find(updatingPerson.UId);
					dbItem.AmountNoTax = updatingPerson.AmountNoTax;
					dbItem.InvoiceDate = updatingPerson.InvoiceDate;
					dbItem.InvoiceNo = updatingPerson.InvoiceNo;
					dbItem.ReferenceId = ReferenceId;
				}
				db.SaveChanges();
			}
		}

		private static void DeleteTFromDatabase(InvoiceModle updatingPerson)
		{
			using (Entities db = new Entities())
			{
				var dbItem = db.T_FD_InvoiceList.Find(updatingPerson.UId);
				if (dbItem == null) return;
				db.T_FD_InvoiceList.Remove(dbItem);
				db.SaveChanges();
			}

		}
	}
}