using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeerInformation.Areas.finance.Models;
using Ext.Net.MVC;

namespace DeerInformation.Areas.finance.Controllers
{
    public class InvoiceController : Controller
    {
		public ActionResult InvoiceLst(string referenceId)
		{
			InvoiceModle.ReferenceId = referenceId;
			InvoiceModle.CurId = 0;
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public RestResult Create(InvoiceModle person)
		{
			try
			{
				InvoiceModle.AddPerson(person);

				return new RestResult
				{
					Success = true,
					Message = "新的发票信息已添加",
					Data = person
				};
			}
			catch (Exception e)
			{
				return new RestResult
				{
					Success = false,
					Message = e.Message
				};
			}
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public RestResult Read()
		{
			try
			{
				return new RestResult
				{
					Success = true,
					Data = InvoiceModle.TestData
				};
			}
			catch (Exception e)
			{
				return new RestResult
				{
					Success = false,
					Message = e.Message
				};
			}
		}

		[AcceptVerbs(HttpVerbs.Put)]
		public RestResult Update(InvoiceModle person)
		{
			try
			{
				InvoiceModle.UpdatePerson(person);

				return new RestResult
				{
					Success = true,
					Message = "发票信息已更新"
				};
			}
			catch (Exception e)
			{
				return new RestResult
				{
					Success = false,
					Message = e.Message
				};
			}
		}

		[AcceptVerbs(HttpVerbs.Delete)]
		public RestResult Destroy(int id)
		{
			try
			{
				InvoiceModle.DeletePerson(id);

				return new RestResult
				{
					Success = true,
					Message = "发票信息已删除"
				};
			}
			catch (Exception e)
			{
				return new RestResult
				{
					Success = false,
					Message = e.Message
				};
			}
		}
    }
}
