using GM.CommonLibs.Helper;
using GM.DataAccess.UnitOfWork;
using GM.Model.Common;
using GM.Model.CounterParty;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GM.Service.CounterParty.Controllers
{
    [Route("[controller]")]  
    [ApiController]  
    public class CounterPartyFundController : ControllerBase
    {
        private readonly IUnitOfWork _uow;     
        public CounterPartyFundController(IUnitOfWork uow) => _uow = uow;   

        [HttpPost]   
        [Route("CreateCounterPartyFund")] 
        public ResultWithModel CreateCounterPartyFund(CounterPartyFundModel model)    
        {
            ResultWithModel res;   
            try
            {
                _uow.BeginTransaction();  
                res = _uow.CounterPartyGroup.CounterPartyFund.Add(model);   

                if (res.Success && res.Data != null)    
                {   
                     model = res.Data as CounterPartyFundModel;   

                     if (model?.Identify != null)   
                     {
                        foreach (var item in model.Identify)   
                        {
                              item.fund_id = model.fund_id;  
                              item.counter_party_id = model.counter_party_id;   
                              item.create_by = model.create_by;   

                              res = _uow.CounterPartyGroup.CounterPartyFundIdentify.Add(item);  

                              if (!res.Success)   
                              {
                                    _uow.Rollback();        
                                    throw new Exception(res.Message);        
                              }
                        }
                     } 

                     if (model?.Payment != null)
                     { 
                        foreach (var item in model.Payment)  
                        {
                              item.fund_id = model.fund_id;   
                              item.counter_party_id = model.counter_party_id;  
                              item.create_by = model.create_by;   

                              res = _uow.CounterPartyGroup.CounterPartyFundPayment.Add(item);   

                              if (!res.Success)   
                              {
                                    _uow.Rollback();   
                                    throw Exception(res.Message);    
                              }  
                        }
                     }

                     if (model?.Margin != null)   
                     {
                        model.Margin.counter_party_id = model.counter_party_id;  
                        model.Margin.fund_id = model.fund_id;
                        model.Margin.create_by = model.create_by;

                        res = _uow.CounterPartyGroup.CounterPartyFundMargin.Add(model.Margin);
                        if (!res.Success)
                        {
                            _uow.Rollback();
                            throw new Exception(res.Message);
                        }
                    }
                }
                else
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                _uow.Commit();
            }
            catch (Exception ex)
            {
                return new ResultWithModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return res;
        }

        [HttpPost]
        [Route("UpdateCounterPartyFund")]
        public ResultWithModel UpdateCounterPartyFund(CounterPartyFundModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();
                res = _uow.CounterPartyGroup.CounterPartyFund.Update(model);

                if (res.Success)
                {
                    if (model.Identify != null)
                    {
                        model.Identify = model.Identify.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Identify)
                        {
                            item.fund_id = model.fund_id;
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundIdentify.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundIdentify.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundIdentify.Update(item);
                            }

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }

                    if (model.Payment != null)
                    {
                        model.Payment = model.Payment.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Payment)
                        {
                            item.fund_id = model.fund_id;
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundPayment.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundPayment.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyFundPayment.Update(item);
                            }

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }

                    if (model.Margin != null)
                    {
                        model.Margin.counter_party_id = model.counter_party_id;
                        model.Margin.fund_id = model.fund_id;
                        model.Margin.create_by = model.create_by;

                        res = _uow.CounterPartyGroup.CounterPartyFundMargin.Update(model.Margin);
                        if (!res.Success)
                        {
                            _uow.Rollback();
                            throw new Exception(res.Message);
                        }
                    }
                }
                else
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                _uow.Commit();
            }
            catch (Exception ex)
            {
                return new ResultWithModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return res;
        }

        [HttpPost]
        [Route("DeleteCounterPartyFund")]
        public ResultWithModel DeleteCounterPartyFund(CounterPartyFundModel model)
        {

            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();

                res = _uow.CounterPartyGroup.CounterPartyFundMargin.Remove(new CounterPartyFundMarginModel
                {
                    fund_id = model.fund_id,
                    create_by = model.create_by
                });

                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyFundIdentify.Remove(new CounterPartyFundIdentifyModel
                {
                    fund_id = model.fund_id,
                    create_by = model.create_by
                });

                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyFundPayment.Remove(new CounterPartyFundPaymentModel
                {
                    fund_id = model.fund_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyFund.Remove(model);
                if (res.Success) _uow.Commit();
                else
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }
            }
            catch (Exception ex)
            {
                return new ResultWithModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            return res;
        }

        [HttpPost]
        [Route("GetCounterPartyFundlist")]
        public ResultWithModel GetCounterPartyFundlist(CounterPartyFundModel model) => _uow.CounterPartyGroup.CounterPartyFund.Get(model);

        [HttpPost]
        [Route("GetCounterParty")]
        public ResultWithModel GetCounterParty(CounterPartyFundModel model) => _uow.CounterPartyGroup.CounterPartyFund.Find(model);

        [HttpPost]
        [Route("GetCounterPartyFundMarginList")]
        public ResultWithModel GetCounterPartyFundMarginList(CounterPartyFundMarginModel model) => _uow.CounterPartyGroup.CounterPartyFundMargin.Get(model);

        [HttpPost]
        [Route("GetCounterPartyFundPayment")]
        public ResultWithModel GetCounterPartyFundPayment(CounterPartyFundPaymentModel model) => _uow.CounterPartyGroup.CounterPartyFundPayment.Get(model);

        [HttpPost]
        [Route("GetCounterPartyFundIdentifyList")]
        public ResultWithModel GetCounterPartyFundIdentifyList(CounterPartyFundIdentifyModel model) => _uow.CounterPartyGroup.CounterPartyFundIdentify.Get(model);

        [HttpGet]
        [Route("GetDDLCounterParty")]
        public ResultWithModel GetDDLCounterParty(string counterpartyname)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_counterparty_code";
            model.SearchValue = counterpartyname;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLCounterPartyFund")]
        public ResultWithModel GetDDLCounterPartyFund(string counterpartynamefund)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_counter_party_fund";
            model.SearchValue = counterpartynamefund;
            model.Paging = new PagingModel { PageNumber = 1, RecordPerPage = 999 };
            return _uow.Dropdown.Get(model);
        }
    }
}
