﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeerInformation.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Company> Company { get; set; }
        public DbSet<FDepartMent> FDepartMent { get; set; }
        public DbSet<SDepartMent> SDepartMent { get; set; }
        public DbSet<T_CH_Check_state> T_CH_Check_state { get; set; }
        public DbSet<T_CH_Check_type> T_CH_Check_type { get; set; }
        public DbSet<T_CH_Checker> T_CH_Checker { get; set; }
        public DbSet<T_CH_Checkflow> T_CH_Checkflow { get; set; }
        public DbSet<T_CH_Checkfunc> T_CH_Checkfunc { get; set; }
        public DbSet<T_CH_Checkfunc_Checkflow_relation> T_CH_Checkfunc_Checkflow_relation { get; set; }
        public DbSet<T_CH_Operation_list> T_CH_Operation_list { get; set; }
        public DbSet<T_CH_Task> T_CH_Task { get; set; }
        public DbSet<T_HR_Duty> T_HR_Duty { get; set; }
        public DbSet<T_HR_EntryPosition> T_HR_EntryPosition { get; set; }
        public DbSet<T_HR_Job> T_HR_Job { get; set; }
        public DbSet<T_HR_PositionCategory> T_HR_PositionCategory { get; set; }
        public DbSet<T_HR_Scheduling> T_HR_Scheduling { get; set; }
        public DbSet<T_HR_StaffJob> T_HR_StaffJob { get; set; }
        public DbSet<T_PE_Menu> T_PE_Menu { get; set; }
        public DbSet<T_PE_Permission> T_PE_Permission { get; set; }
        public DbSet<T_PE_PermissionAction> T_PE_PermissionAction { get; set; }
        public DbSet<T_PE_RolePermissions> T_PE_RolePermissions { get; set; }
        public DbSet<T_PE_Roles> T_PE_Roles { get; set; }
        public DbSet<T_PE_UserRoles> T_PE_UserRoles { get; set; }
        public DbSet<T_PE_Users> T_PE_Users { get; set; }
        public DbSet<T_PM_ApplyMaterial> T_PM_ApplyMaterial { get; set; }
        public DbSet<T_PM_ApplyProjectMaterial> T_PM_ApplyProjectMaterial { get; set; }
        public DbSet<T_PM_MaterialInfo> T_PM_MaterialInfo { get; set; }
        public DbSet<T_PM_MaterialStock> T_PM_MaterialStock { get; set; }
        public DbSet<T_PM_OfferInfo> T_PM_OfferInfo { get; set; }
        public DbSet<T_PM_OrderInfo> T_PM_OrderInfo { get; set; }
        public DbSet<T_PM_ProjectMaterialApply> T_PM_ProjectMaterialApply { get; set; }
        public DbSet<T_PM_StorageApply> T_PM_StorageApply { get; set; }
        public DbSet<T_PM_SupplierMaterial> T_PM_SupplierMaterial { get; set; }
        public DbSet<T_PM_SupplierMaterialExtra> T_PM_SupplierMaterialExtra { get; set; }
        public DbSet<T_PM_SupplierOrder> T_PM_SupplierOrder { get; set; }
        public DbSet<T_PM_SupplierOrderExtra> T_PM_SupplierOrderExtra { get; set; }
        public DbSet<T_PM_MeasuringUnitID> T_PM_MeasuringUnitID { get; set; }
        public DbSet<V_CH_CheckerFlow> V_CH_CheckerFlow { get; set; }
        public DbSet<V_CH_Checkfuncflow> V_CH_Checkfuncflow { get; set; }
        public DbSet<V_CH_CheckProcess> V_CH_CheckProcess { get; set; }
        public DbSet<V_CH_OperationChecker> V_CH_OperationChecker { get; set; }
        public DbSet<V_ComJiaGou> V_ComJiaGou { get; set; }
        public DbSet<V_HR_DutyWithPCName> V_HR_DutyWithPCName { get; set; }
        public DbSet<V_HR_Job> V_HR_Job { get; set; }
        public DbSet<V_HR_JobWithDutyName> V_HR_JobWithDutyName { get; set; }
        public DbSet<V_HR_SdepartMent> V_HR_SdepartMent { get; set; }
        public DbSet<V_PE_Permission_Menu_Roles_Users> V_PE_Permission_Menu_Roles_Users { get; set; }
        public DbSet<V_PE_RolePermissionNoMenuDetail> V_PE_RolePermissionNoMenuDetail { get; set; }
        public DbSet<V_PE_UserPermission> V_PE_UserPermission { get; set; }
        public DbSet<T_HR_ExceptionType> T_HR_ExceptionType { get; set; }
        public DbSet<T_HR_SalaryPerHour> T_HR_SalaryPerHour { get; set; }
        public DbSet<T_HR_WorkTime> T_HR_WorkTime { get; set; }
        public DbSet<V_CH_TaskFunc> V_CH_TaskFunc { get; set; }
        public DbSet<T_US_Message> T_US_Message { get; set; }
        public DbSet<V_CH_TaskOperation> V_CH_TaskOperation { get; set; }
        public DbSet<T_FD_InvoiceType> T_FD_InvoiceType { get; set; }
        public DbSet<V_PE_UserRole> V_PE_UserRole { get; set; }
        public DbSet<T_HR_Salary> T_HR_Salary { get; set; }
        public DbSet<T_HR_SalaryItems> T_HR_SalaryItems { get; set; }
        public DbSet<T_US_WorkHours> T_US_WorkHours { get; set; }
        public DbSet<T_GM_ApplyFixedAsset> T_GM_ApplyFixedAsset { get; set; }
        public DbSet<T_GM_ApplyMaterial> T_GM_ApplyMaterial { get; set; }
        public DbSet<T_GM_CustomerInfo> T_GM_CustomerInfo { get; set; }
        public DbSet<T_GM_DM> T_GM_DM { get; set; }
        public DbSet<T_GM_InfoMaterial> T_GM_InfoMaterial { get; set; }
        public DbSet<T_GM_MeasuringUnit> T_GM_MeasuringUnit { get; set; }
        public DbSet<T_GM_NoBody> T_GM_NoBody { get; set; }
        public DbSet<T_GM_PurchaseFixedAsset> T_GM_PurchaseFixedAsset { get; set; }
        public DbSet<T_GM_ResidualM> T_GM_ResidualM { get; set; }
        public DbSet<T_GM_SupplierInfo> T_GM_SupplierInfo { get; set; }
        public DbSet<T_GM_TempDetailMaterial> T_GM_TempDetailMaterial { get; set; }
        public DbSet<T_GM_TypeNo> T_GM_TypeNo { get; set; }
        public DbSet<T_GW_CustomRating> T_GW_CustomRating { get; set; }
        public DbSet<T_GW_FieldDataManagement> T_GW_FieldDataManagement { get; set; }
        public DbSet<T_GW_FieldInfoManagement> T_GW_FieldInfoManagement { get; set; }
        public DbSet<T_GW_MarkInfo> T_GW_MarkInfo { get; set; }
        public DbSet<V_GM_DM> V_GM_DM { get; set; }
        public DbSet<V_GM_FAApply> V_GM_FAApply { get; set; }
        public DbSet<V_GM_FAPurchase> V_GM_FAPurchase { get; set; }
        public DbSet<V_GM_TempMaterial> V_GM_TempMaterial { get; set; }
        public DbSet<T_HR_Attendance> T_HR_Attendance { get; set; }
        public DbSet<T_HR_AttendInsert> T_HR_AttendInsert { get; set; }
        public DbSet<T_HR_AttendanceExceptionHandleRecords> T_HR_AttendanceExceptionHandleRecords { get; set; }
        public DbSet<V_HR_AttendTimeOriginalWithName> V_HR_AttendTimeOriginalWithName { get; set; }
        public DbSet<V_HR_ExceptionHandleRecordsWithDetail> V_HR_ExceptionHandleRecordsWithDetail { get; set; }
        public DbSet<T_GM_StorageFixedAsset> T_GM_StorageFixedAsset { get; set; }
        public DbSet<T_GM_TempBackup> T_GM_TempBackup { get; set; }
        public DbSet<V_HR_StaffSalaryWithTime> V_HR_StaffSalaryWithTime { get; set; }
        public DbSet<T_HR_AttendAMPMState> T_HR_AttendAMPMState { get; set; }
        public DbSet<T_HR_StaffSalary> T_HR_StaffSalary { get; set; }
        public DbSet<T_GM_Budget> T_GM_Budget { get; set; }
        public DbSet<T_GW_ReceivedInvoiceTime> T_GW_ReceivedInvoiceTime { get; set; }
        public DbSet<V_GM_DetailBudget> V_GM_DetailBudget { get; set; }
        public DbSet<T_GM_PurchaseMaterial> T_GM_PurchaseMaterial { get; set; }
        public DbSet<T_GM_ReceiptMaterial> T_GM_ReceiptMaterial { get; set; }
        public DbSet<T_HR_SalaryInput> T_HR_SalaryInput { get; set; }
        public DbSet<V_GM_DetailRecieve> V_GM_DetailRecieve { get; set; }
        public DbSet<V_GM_MApply> V_GM_MApply { get; set; }
        public DbSet<V_GM_MPurchase> V_GM_MPurchase { get; set; }
        public DbSet<V_GM_MResidual> V_GM_MResidual { get; set; }
        public DbSet<V_HR_PuchedException> V_HR_PuchedException { get; set; }
        public DbSet<T_GM_TempGather> T_GM_TempGather { get; set; }
        public DbSet<T_GW_FieldSchedule> T_GW_FieldSchedule { get; set; }
        public DbSet<V_PM_MaterialStorageApply> V_PM_MaterialStorageApply { get; set; }
        public DbSet<checkinout> checkinout { get; set; }
        public DbSet<V_GM_DetailMStock> V_GM_DetailMStock { get; set; }
        public DbSet<T_GM_EXWarehouse> T_GM_EXWarehouse { get; set; }
        public DbSet<T_GM_Warehouse> T_GM_Warehouse { get; set; }
        public DbSet<T_GM_WarehouseType> T_GM_WarehouseType { get; set; }
        public DbSet<T_FD_AccountReceivable> T_FD_AccountReceivable { get; set; }
        public DbSet<T_HR_Fund> T_HR_Fund { get; set; }
        public DbSet<T_HR_Staff> T_HR_Staff { get; set; }
        public DbSet<V_HR_Fund> V_HR_Fund { get; set; }
        public DbSet<V_GM_EXWithStateDsp> V_GM_EXWithStateDsp { get; set; }
        public DbSet<V_GM_IMWithStateDsp> V_GM_IMWithStateDsp { get; set; }
        public DbSet<T_GM_IMWarehouse> T_GM_IMWarehouse { get; set; }
        public DbSet<V_HR_WorkOverTime> V_HR_WorkOverTime { get; set; }
        public DbSet<T_HR_WorkOverTime> T_HR_WorkOverTime { get; set; }
        public DbSet<T_FD_AccountPayable> T_FD_AccountPayable { get; set; }
        public DbSet<V_GM_EXDetail> V_GM_EXDetail { get; set; }
        public DbSet<V_GM_IMDetail> V_GM_IMDetail { get; set; }
        public DbSet<V_PE_RolePermission> V_PE_RolePermission { get; set; }
        public DbSet<T_FD_CollectionType> T_FD_CollectionType { get; set; }
        public DbSet<T_HR_BusinessTrip> T_HR_BusinessTrip { get; set; }
        public DbSet<T_HR_Dimission> T_HR_Dimission { get; set; }
        public DbSet<T_HR_EducationFund> T_HR_EducationFund { get; set; }
        public DbSet<T_PE_Serial> T_PE_Serial { get; set; }
        public DbSet<V_HR_BTWithDepName> V_HR_BTWithDepName { get; set; }
        public DbSet<V_HR_Dimission> V_HR_Dimission { get; set; }
        public DbSet<V_HR_EducationFundWithName> V_HR_EducationFundWithName { get; set; }
        public DbSet<V_HR_StaffJobWithName> V_HR_StaffJobWithName { get; set; }
        public DbSet<V_HR_VacationWithDepName> V_HR_VacationWithDepName { get; set; }
        public DbSet<T_HR_Vacation> T_HR_Vacation { get; set; }
        public DbSet<T_HR_OverWorkApply> T_HR_OverWorkApply { get; set; }
        public DbSet<V_HR_OWApplyWithDepName> V_HR_OWApplyWithDepName { get; set; }
        public DbSet<V_HR_StaffWithDepName> V_HR_StaffWithDepName { get; set; }
        public DbSet<V_GM_Warehouse> V_GM_Warehouse { get; set; }
        public DbSet<T_FD_DetailPlan> T_FD_DetailPlan { get; set; }
        public DbSet<T_FD_CollectionPlan> T_FD_CollectionPlan { get; set; }
        public DbSet<V_GM_FAStorage> V_GM_FAStorage { get; set; }
        public DbSet<T_FD_InvoiceList> T_FD_InvoiceList { get; set; }
        public DbSet<T_GM_MaterialStock> T_GM_MaterialStock { get; set; }
        public DbSet<T_GM_Project> T_GM_Project { get; set; }
        public DbSet<V_GM_TempDM> V_GM_TempDM { get; set; }
        public DbSet<V_FD_DetailCollectionPlan> V_FD_DetailCollectionPlan { get; set; }
        public DbSet<V_FD_PurchasePayable> V_FD_PurchasePayable { get; set; }
        public DbSet<V_GM_DetailProject> V_GM_DetailProject { get; set; }
        public DbSet<T_HR_SchList> T_HR_SchList { get; set; }
        public DbSet<T_HR_SchListWithStaff> T_HR_SchListWithStaff { get; set; }
        public DbSet<V_HR_SchListWithStaff> V_HR_SchListWithStaff { get; set; }
        public DbSet<T_HR_Department1> T_HR_Department1 { get; set; }
        public DbSet<T_HR_Department2> T_HR_Department2 { get; set; }
        public DbSet<T_HR_Department3> T_HR_Department3 { get; set; }
        public DbSet<T_HR_Department4> T_HR_Department4 { get; set; }
        public DbSet<T_HR_Department5> T_HR_Department5 { get; set; }
        public DbSet<V_HR_Dep> V_HR_Dep { get; set; }
        public DbSet<T_HR_TipsPeople> T_HR_TipsPeople { get; set; }
        public DbSet<V_HR_TipsPeople> V_HR_TipsPeople { get; set; }
        public DbSet<T_US_DailyWorkReport> T_US_DailyWorkReport { get; set; }
        public DbSet<V_US_WorkReportWithHours> V_US_WorkReportWithHours { get; set; }
        public DbSet<V_US_WorkReportWithState> V_US_WorkReportWithState { get; set; }
        public DbSet<V_GM_DMStock> V_GM_DMStock { get; set; }
        public DbSet<T_GW_FieldImportantAttach> T_GW_FieldImportantAttach { get; set; }
        public DbSet<V_GW_FieldImportantAttach> V_GW_FieldImportantAttach { get; set; }
        public DbSet<V_HR_AttendAssessWithExceptionType> V_HR_AttendAssessWithExceptionType { get; set; }
        public DbSet<T_HR_AttendAssess> T_HR_AttendAssess { get; set; }
        public DbSet<T_HR_Certificate> T_HR_Certificate { get; set; }
        public DbSet<T_HR_Contract> T_HR_Contract { get; set; }
        public DbSet<T_HR_Recommend> T_HR_Recommend { get; set; }
        public DbSet<V_HR_Certificate> V_HR_Certificate { get; set; }
        public DbSet<V_HR_ContractWithStaffName> V_HR_ContractWithStaffName { get; set; }
        public DbSet<V_HR_IDNameWithDepName> V_HR_IDNameWithDepName { get; set; }
        public DbSet<V_HR_RecommendWithName> V_HR_RecommendWithName { get; set; }
    
        public virtual int P_GM_DetailPurchaseFA(string no)
        {
            var noParameter = no != null ?
                new ObjectParameter("no", no) :
                new ObjectParameter("no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_GM_DetailPurchaseFA", noParameter);
        }
    
        public virtual int P_GM_GetTotal(string no)
        {
            var noParameter = no != null ?
                new ObjectParameter("no", no) :
                new ObjectParameter("no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_GM_GetTotal", noParameter);
        }
    
        public virtual ObjectResult<P_PE_GetRolePermission_Result> P_PE_GetRolePermission(string roleID)
        {
            var roleIDParameter = roleID != null ?
                new ObjectParameter("RoleID", roleID) :
                new ObjectParameter("RoleID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<P_PE_GetRolePermission_Result>("P_PE_GetRolePermission", roleIDParameter);
        }
    
        public virtual int pro_insert_material(string pID, string materialName, string size, string unit, Nullable<decimal> price, string extra, string inputPerson, Nullable<System.DateTime> inputTime)
        {
            var pIDParameter = pID != null ?
                new ObjectParameter("PID", pID) :
                new ObjectParameter("PID", typeof(string));
    
            var materialNameParameter = materialName != null ?
                new ObjectParameter("MaterialName", materialName) :
                new ObjectParameter("MaterialName", typeof(string));
    
            var sizeParameter = size != null ?
                new ObjectParameter("Size", size) :
                new ObjectParameter("Size", typeof(string));
    
            var unitParameter = unit != null ?
                new ObjectParameter("Unit", unit) :
                new ObjectParameter("Unit", typeof(string));
    
            var priceParameter = price.HasValue ?
                new ObjectParameter("Price", price) :
                new ObjectParameter("Price", typeof(decimal));
    
            var extraParameter = extra != null ?
                new ObjectParameter("Extra", extra) :
                new ObjectParameter("Extra", typeof(string));
    
            var inputPersonParameter = inputPerson != null ?
                new ObjectParameter("InputPerson", inputPerson) :
                new ObjectParameter("InputPerson", typeof(string));
    
            var inputTimeParameter = inputTime.HasValue ?
                new ObjectParameter("InputTime", inputTime) :
                new ObjectParameter("InputTime", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pro_insert_material", pIDParameter, materialNameParameter, sizeParameter, unitParameter, priceParameter, extraParameter, inputPersonParameter, inputTimeParameter);
        }
    
        public virtual ObjectResult<protest_Result> protest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<protest_Result>("protest");
        }
    
        public virtual int P_CH_DeleteCheckFlows(string idstr)
        {
            var idstrParameter = idstr != null ?
                new ObjectParameter("idstr", idstr) :
                new ObjectParameter("idstr", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_CH_DeleteCheckFlows", idstrParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> P_CH_ExecuteCheck(string operationID, string checkerID, Nullable<int> state, string description)
        {
            var operationIDParameter = operationID != null ?
                new ObjectParameter("OperationID", operationID) :
                new ObjectParameter("OperationID", typeof(string));
    
            var checkerIDParameter = checkerID != null ?
                new ObjectParameter("CheckerID", checkerID) :
                new ObjectParameter("CheckerID", typeof(string));
    
            var stateParameter = state.HasValue ?
                new ObjectParameter("State", state) :
                new ObjectParameter("State", typeof(int));
    
            var descriptionParameter = description != null ?
                new ObjectParameter("Description", description) :
                new ObjectParameter("Description", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("P_CH_ExecuteCheck", operationIDParameter, checkerIDParameter, stateParameter, descriptionParameter);
        }
    
        public virtual int P_CH_TaskAndOperationExpire()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_CH_TaskAndOperationExpire");
        }
    
        public virtual int P_HR_HireStateChange(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_HireStateChange", opreationlistIDParameter);
        }
    
        public virtual ObjectResult<GetSalaryByYearMonth_Result> GetSalaryByYearMonth(string employeeId, Nullable<int> year, Nullable<int> month)
        {
            var employeeIdParameter = employeeId != null ?
                new ObjectParameter("EmployeeId", employeeId) :
                new ObjectParameter("EmployeeId", typeof(string));
    
            var yearParameter = year.HasValue ?
                new ObjectParameter("Year", year) :
                new ObjectParameter("Year", typeof(int));
    
            var monthParameter = month.HasValue ?
                new ObjectParameter("Month", month) :
                new ObjectParameter("Month", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetSalaryByYearMonth_Result>("GetSalaryByYearMonth", employeeIdParameter, yearParameter, monthParameter);
        }
    
        public virtual int P_GM_AddBackTemp(string no)
        {
            var noParameter = no != null ?
                new ObjectParameter("no", no) :
                new ObjectParameter("no", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_GM_AddBackTemp", noParameter);
        }
    
        public virtual int P_HR_BusinessTrip(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_BusinessTrip", opreationlistIDParameter);
        }
    
        public virtual int P_HR_Vacation(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_Vacation", opreationlistIDParameter);
        }
    
        public virtual int P_HR_StaffSalary(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_StaffSalary", opreationlistIDParameter);
        }
    
        public virtual int P_HR_WorkOverTime(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_WorkOverTime", opreationlistIDParameter);
        }
    
        public virtual ObjectResult<P_GM_MaterialGather_Result> P_GM_MaterialGather(string projectno)
        {
            var projectnoParameter = projectno != null ?
                new ObjectParameter("projectno", projectno) :
                new ObjectParameter("projectno", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<P_GM_MaterialGather_Result>("P_GM_MaterialGather", projectnoParameter);
        }
    
        public virtual ObjectResult<string> P_PE_GetSerialNum()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("P_PE_GetSerialNum");
        }
    
        public virtual int P_HR_DimissionToStaff(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_DimissionToStaff", opreationlistIDParameter);
        }
    
        public virtual int P_HR_OWApply(string opreationlistID)
        {
            var opreationlistIDParameter = opreationlistID != null ?
                new ObjectParameter("opreationlistID", opreationlistID) :
                new ObjectParameter("opreationlistID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_HR_OWApply", opreationlistIDParameter);
        }
    
        public virtual int P_GM_DeletePurchaseOrder(string id, Nullable<bool> back)
        {
            var idParameter = id != null ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(string));
    
            var backParameter = back.HasValue ?
                new ObjectParameter("back", back) :
                new ObjectParameter("back", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("P_GM_DeletePurchaseOrder", idParameter, backParameter);
        }
    }
}
