using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeerInformation.Models;
using System.Web.Mvc;
using Ext.Net.MVC;
namespace DeerInformation.Areas.system.Models
{
    public class CheckflowManage
    {
        //枚举所有审核功能供用户选择
        public IEnumerable<T_CH_Checkfunc> checkfuncs
        {
            get
            {

                using (Entities db = new Entities())
                {
                    var result = db.T_CH_Checkfunc.Distinct().ToList();

                    return result;
                }

            }
        }

        //审核功能
        private string m_checkfunc;
        public string checkfunc
        {
            set { m_checkfunc = value; }
            get { return m_checkfunc; }
        }

        //返回当前审核功能下的所有审核流程
        public IEnumerable<V_CH_Checkfuncflow> checkflows
        {
            get
            {

                using (Entities db = new Entities())
                {
                    if (string.IsNullOrEmpty(checkfunc))
                    {
                        var result = db.V_CH_Checkfuncflow.ToList();
                        return result;
                    }
                    else
                    {
                        var result = db.V_CH_Checkfuncflow.Where(li => li.CheckfuncID == checkfunc).ToList();
                        return result;
                    }
                }

            }
        }

    }

    public class CheckFlow
    {
        public string CheckFlowID;
        [Field(FieldLabel = "审核流程名称")]
        public string Name { get; set; }
        [Field(FieldLabel = "有效审核时间(h)", AllowBlank = false)]
        public int TimeLimit { get; set; }
        [Field(FieldLabel = "创建人")]
        public string Creator { get; set; }
        [Field(FieldLabel = "创建时间", ReadOnly = true)]
        public DateTime CreatTime { get; set; }
        [Field(FieldLabel = "描述")]
        public string Description { get; set; }

        [Field(FieldLabel = "禁用")]
        public bool Disable { get; set; }

        private List<Checker> _checkers;

        //审核人集合
        public List<Checker> Checkers
        {
            get
            {
                if (_checkers == null)
                {
                    if (CheckFlowID != null)
                    {
                        using (Entities db = new Entities())
                        {
                            _checkers = db.V_CH_CheckProcess.Where(l => l.CheckFlowID == CheckFlowID).Select(l => new Checker()
                            {
                                ID = l.CheckerID,
                                lvl1 = l.lvl1.Value,
                                lvl2 = l.lvl2.Value,
                                Name = l.CheckerName
                            }).ToList();
                        }
                    }
                    else
                    {
                        _checkers = new List<Checker>();
                    }

                }
                return _checkers;
            }
        }
    }

    public class Checker
    {
        //工号
        public string ID { get; set; }
        //姓名
        public string Name { get; set; }
        //审核等级一
        public int lvl1 { get; set; }
        //审核等级二
        public int lvl2 { get; set; }
    }
}