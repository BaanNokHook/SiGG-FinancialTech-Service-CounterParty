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
    public class IssuerController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public IssuerController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        [Route("GetDDLIssuerType")]
        public ResultWithModel GetDDLIssuerType(string issuertypedesc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_issuer_type";
            model.SearchValue = issuertypedesc;
            return _uow.Dropdown.Get(model);
        }

        [HttpGet]
        [Route("GetDDLIssuerGroup")]
        public ResultWithModel GetDDLIssuerGroup(string groupdesc)
        {
            DropdownModel model = new DropdownModel();
            model.ProcedureName = "GM_DDL_List_Proc";
            model.DdltTableList = "GM_issuer_group";
            model.SearchValue = groupdesc;
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

        [HttpPost]
        [Route("GetIssuerlist")]
        public ResultWithModel GetIssuerlist(IssuerModel model) => _uow.CounterPartyGroup.Issuer.Get(model);

        [HttpPost]
        [Route("CreateIssuer")]
        public ResultWithModel CreateIssuer(IssuerModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();
                res = _uow.CounterPartyGroup.Issuer.Add(model);

                if (res.Success && res.Data != null)
                {
                    model = res.Data as IssuerModel;

                    if (model?.Identify != null)
                    {
                        foreach (var item in model.Identify)
                        {
                            item.issuer_id = model.issuer_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.IssuerIdentify.Add(item);

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
                            item.issuer_id = model.issuer_id;
                            item.create_by = model.create_by;

                            res = _uow.CounterPartyGroup.IssuerRating.Add(item);

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
        [Route("UpdateIssuer")]
        public ResultWithModel UpdateIssuer(IssuerModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();
                res = _uow.CounterPartyGroup.Issuer.Update(model);

                if (res.Success)
                {
                    if (model.Identify != null)
                    {
                        model.Identify = model.Identify.Where(x => x.rowstatus != null).OrderBy(x => Globals.GetStatusOrder(x.rowstatus)).ToList();

                        foreach (var item in model.Identify)
                        {
                            item.issuer_id = model.issuer_id;
                            item.create_by = model.create_by;
                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.IssuerIdentify.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.IssuerIdentify.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.IssuerIdentify.Update(item);
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
                            item.issuer_id = model.issuer_id;
                            item.create_by = model.create_by;

                            if (item.rowstatus == "delete")
                            {
                                res = _uow.CounterPartyGroup.IssuerRating.Remove(item);
                            }
                            else if (item.rowstatus == "create")
                            {
                                res = _uow.CounterPartyGroup.IssuerRating.Add(item);
                            }
                            else if (item.rowstatus == "update")
                            {
                                res = _uow.CounterPartyGroup.IssuerRating.Update(item);
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
        [Route("DeleteIssuer")]
        public ResultWithModel DeleteIssuer(IssuerModel model)
        {
            ResultWithModel res;
            try
            {
                _uow.BeginTransaction();

                res = _uow.CounterPartyGroup.IssuerIdentify.Remove(new IssuerIdentifyModel { issuer_id = model.issuer_id, create_by = model.create_by });

                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.IssuerRating.Remove(new IssuerRatingModel { issuer_id = model.issuer_id, create_by = model.create_by });

                if (!res.Success)
                {
                    _uow.Rollback();
                    throw new Exception(res.Message);
                }

                res = _uow.CounterPartyGroup.Issuer.Remove(model);
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
        [Route("GetIssuerIdentifylist")]
        public ResultWithModel GetIssuerIdentifylist(IssuerIdentifyModel model) => _uow.CounterPartyGroup.IssuerIdentify.Get(model);

        [HttpPost]
        [Route("GetIssuerRatinglist")]
        public ResultWithModel GetIssuerRatinglist(IssuerRatingModel model) => _uow.CounterPartyGroup.IssuerRating.Get(model);
    }
}