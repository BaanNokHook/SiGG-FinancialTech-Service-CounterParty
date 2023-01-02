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
    public class CounterPartyController : ControllerBase  
    {
        private readonly IUnitOfWork _uow;  
        public CounterPartyController(IUnitOfWork uow)     
        {
            _uow = uow;
        }

        [HttpPost]  
        [Route("GetCounterPartylist")]   
        public ResultWithModel GetCounterPartylist(CounterPartyModel model) => _uow.CounterPartyGroup.CounterParty.Get(model);     

        [HttpGet]
        [Route("GetCounterPartyByID")]   
        public ResultWithModel GetCounterPartyByID(int counter_party_id) => _uow.CounterPartyGroup.CounterParty.Find(new CounterPartyModel { counter_party_id = counter_party_id });      

        [HttpPost]
        [Route("GetCounterPartyMarginlist")]  
        public ResultWithModel GetCounterPartyMarginlist(CounterPartyMarginModel model) => _uow.CounterPartyGroup.CounterPartyMargin.Get(model);   

        [HttpPost]  
        [Route("GetCounterPayment")]  
        public ResultWithModel GetCounterPartyPayment(CounterPartyPaymentModel model) => _uow.CounterPartyGroup.CounterPartyPayment.Get(model);  

        [HttpPost] 
        [Route("GetCounterPartyIdentifylist")]   
        public ResultWithModel GetCounterPartyIdentifylist(CounterPartyIdentifyModel model) => _uow.CounterPartyGroup.CounterPartyIdentify.Get(model);   

        [HttpPost]
        [Route("GetCounterPartyRatinglist")]    
        public ResultWithModel GetCounterPartyRatinglist(CounterPartyRatingModel model) => _uow.CounterPartyGroup.CounterPartyRating.Get(model);    

        [HttpPost]      
        [Route("GetCounterPartyHaircutList")]   
        public ResultWithModel GetCounterPartyHaircutList(CounterPartyHaircutModel model) => _uow.CounterPartyGroup.CounterPartyHaircut.Get(model);       

        [HttpPost]
        [Route("GetCounterPartyExchangeList")]
        public ResultWithModel GetCounterPartyExchangeList(CounterPartyExchRateModel model) => _uow.CounterPartyGroup.CounterPartyExchRate.Get(model);

        [HttpPost]
        [Route("CreateCounterParty")]
        public ResultWithModel CreateCounterParty(CounterPartyModel model)
        {
            ResultWithModel res;   
            try
            {
                  _uow.BeginTransaction();   
                  res = _uow.CounterPartyGroup.CounterParty.Add(model);

                  if (res.Success && res.Data != null)   
                  {
                    model = res.Data as CounterPartyModel;

                    if(model?.Identify != null)
                    {
                        foreach (var item in model.Identify)
                        {
                            item.counter_party_id = model.counter_party_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.CounterPartyIdentify.Add(item);

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
                            item.counter_party_id = model.counter_party_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.CounterPartyPayment.Add(item);

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }
                    if (model?.Rating != null)
                    {
                        foreach (var item in model.Rating)
                        {
                            item.counter_party_id = model.counter_party_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.CounterPartyRating.Add(item);

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }
                    if (model?.Exchange != null)
                    {
                        foreach (var item in model.Exchange)
                        {
                            item.counter_party_id = model.counter_party_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.CounterPartyExchRate.Add(item);

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }
                    if (model?.Haircut != null)
                    {
                        foreach (var item in model.Haircut)
                        {
                            item.counter_party_id = model.counter_party_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.CounterPartyHaircut.Add(item);

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }
                    if (model?.Margin != null)
                    {
                        model.Margin.counter_party_id = model.counter_party_id;
                        model.Margin.create_by = model.create_by;

                        res = _uow.CounterPartyGroup.CounterPartyMargin.Add(model.Margin);
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
        [Route("UpdateCounterParty")]
        public ResultWithModel UpdateCounterParty(CounterPartyModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();
                res = _uow.CounterPartyGroup.CounterParty.Update(model);

                if (res.Success)
                {
                    if (model.Identify != null)
                    {
                        model.Identify = model.Identify.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Identify)
                        {
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyIdentify.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyIdentify.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyIdentify.Update(item);
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
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyPayment.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyPayment.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyPayment.Update(item);
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
                        model.Margin.create_by = model.create_by;

                        res = _uow.CounterPartyGroup.CounterPartyMargin.Update(model.Margin);
                        if (!res.Success)
                        {
                            _uow.Rollback();
                            throw new Exception(res.Message);
                        }
                    }

                    if (model.Exchange != null)
                    {
                        model.Exchange = model.Exchange.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Exchange)
                        {
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyExchRate.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyExchRate.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyExchRate.Update(item);
                            }

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }

                    if (model.Haircut != null)
                    {
                        model.Haircut = model.Haircut.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Haircut)
                        {
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyHaircut.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyHaircut.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyHaircut.Update(item);
                            }

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
                        }
                    }

                    if (model.Rating != null)
                    {
                        model.Rating = model.Rating.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Rating)
                        {
                            item.create_by = model.create_by;
                            item.counter_party_id = model.counter_party_id;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyRating.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyRating.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.CounterPartyRating.Update(item);
                            }

                            if (!res.Success)
                            {
                                _uow.Rollback();
                                throw new Exception(res.Message);
                            }
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
        [Route("DeleteCounterParty")]
        public ResultWithModel DeleteCounterParty(CounterPartyModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();

                res = _uow.CounterPartyGroup.CounterPartyMargin.Remove(new CounterPartyMarginModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });

                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyIdentify.Remove(new CounterPartyIdentifyModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyPayment.Remove(new CounterPartyPaymentModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyExchRate.Remove(new CounterPartyExchRateModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyHaircut.Remove(new CounterPartyHaircutModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterPartyRating.Remove(new CounterPartyRatingModel
                {
                    counter_party_id = model.counter_party_id,
                    create_by = model.create_by
                });
                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.CounterParty.Remove(model);
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

        [HttpGet]
        [Route("GetDDLCounterPartyType")]
        public ResultWithModel GetDDLCounterPartyType(string counter_party_type_desc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_counter_party_type";
            model.SearchValue = counter_party_type_desc;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLCounterPartyGroup")]
        public ResultWithModel GetDDLCounterPartyGroup(string group_desc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_counter_party_group";
            model.SearchValue = group_desc;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLAccountType")]
        public ResultWithModel GetDDLAccountType(string accountdesc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_account_nosto_vosto";
            model.SearchValue = accountdesc;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLBankList")]
        public ResultWithModel GetDDLBankList(string bankname)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_bank_list";
            model.SearchValue = bankname;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLPaymentMethod")]
        public ResultWithModel GetDDLPaymentMethod(string paymentdesc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_payment_method_cpty";
            model.SearchValue = paymentdesc;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLUniqueID")]
        public ResultWithModel GetDDLUniqueID(string uniqueid)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_2List_Proc";
            model.DdltTableList = "GM_indetify_unique";
            model.SearchValue = uniqueid;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLIdentifyType")]
        public ResultWithModel GetDDLIdentifyType(string identifytype)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_2List_Proc";
            model.DdltTableList = "GM_identify_type";
            model.SearchValue = identifytype;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLFundType")]
        public ResultWithModel GetDDLFundType(string fundTypeName)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_fund_type";
            model.SearchValue = fundTypeName;
            return _uow.Dropdown.Get(model);
        }
    }
}