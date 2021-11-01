using MongoDB.Driver;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace S2TAnalytics.Infrastructure.Services
{
    public class ELTService : IELTService
    {
        public readonly IUnitOfWork _unitOfWork;
        public ELTService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region MT4
        public List<MT4UserRequest> GetUsers()
        {
            var userRequests = _unitOfWork.MT4UserRequestRepository.GetAll().ToList();
            //var userRequestsModel = new MT4UserRequestModel().ToMT4UserRequestModel(userRequests);
            return userRequests;
        }
        public List<MT4UserRequest> GetUsersByOrganizationId(Guid organizationId)
        {
            var userRequests = _unitOfWork.MT4UserRequestRepository.GetAll().Where(x => x.OrganizationId == organizationId).ToList();
            //var userRequestsModel = new MT4UserRequestModel().ToMT4UserRequestModel(userRequests);
            return userRequests;
        }

        public MT4UserRequest GetUsersByLogin(int login, Guid organizationId)
        {
            var userRequest = _unitOfWork.MT4UserRequestRepository.GetAll().Where(x => x.Login == login && x.OrganizationId == organizationId).SingleOrDefault();
            //var userRequestsModel = userRequest == null ? null : new MT4UserRequestModel().ToMT4UserRequestModel(userRequest);
            return userRequest;
        }
        public void InsertUserRequest(List<MT4UserRequest> userRequest)
        {
            //var userRequest = new MT4UserRequestModel().ToMT4UserRequest(userRequestModel);
            _unitOfWork.MT4UserRequestRepository.AddMultiple(userRequest);
            //userRequestModel = new MT4UserRequestModel().ToMT4UserRequestModel(userRequest);
            //return userRequestModel;
            //return userRequest;
        }

        public AccountDetail getAccountDetailByAccountNumber(string accountNumber, Guid organizationId)
        {
            var accountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountNumber == accountNumber && x.OrganizationId == organizationId).FirstOrDefault();
            return accountDetail;
        }

        public DailyEquity getDailyEquitiesByAccountNumber(int accountNumber, Guid organizationId)
        {
            var dailyEquity = _unitOfWork.DailyEquityRepository.GetAll().Where(x => x.AccountNumber == accountNumber && x.OrganizationId == organizationId).FirstOrDefault();
            return dailyEquity;
        }

        public bool UpdateUserRequest(MT4UserRequest userRequest)
        {
            try
            {
                _unitOfWork.MT4UserRequestRepository.Update(userRequest);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool UpdateAccountDetail(string filter, UpdateDefinition<AccountDetail> updateDefinition)
        {
            try
            {
                _unitOfWork.AccountDetailRepository.UpdateOne(filter, updateDefinition);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public bool UpdateAccountDetail(AccountDetail existingAccountDetail)
        {
            try
            {
                _unitOfWork.AccountDetailRepository.Update(existingAccountDetail);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public bool UpdateDailyEquity(string filter, UpdateDefinition<DailyEquity> updateDefinition)
        {
            try
            {
                var a = _unitOfWork.DailyEquityRepository.UpdateOne(filter, updateDefinition);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public object UpdateDailyEquity(DailyEquity existingtDailyEquities)
        {
            try
            {
                _unitOfWork.DailyEquityRepository.Update(existingtDailyEquities);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region Oanda
        public OandaAccountModel GetOandaAccountByAccountId(string accountId)
        {
            var oandaAccount = _unitOfWork.OandaAccountRepository.GetAll().Where(x => x.accountId == accountId).SingleOrDefault();
            var oandaAccountModel = oandaAccount == null ? null : new OandaAccountModel().ToOandaAccountModel(oandaAccount);
            return oandaAccountModel;
        }
        public OandaAccountModel InsertOandaAccount(OandaAccountModel oandaAccountModel)
        {
            var oandaAccount = new OandaAccountModel().ToOandaAccount(oandaAccountModel);
            _unitOfWork.OandaAccountRepository.Add(oandaAccount);
            oandaAccountModel = new OandaAccountModel().ToOandaAccountModel(oandaAccount);
            return oandaAccountModel;
        }

        public bool UpdateOandaAccount(string oandaAccountId, UpdateDefinition<OandaAccount> updateDefinition)
        {
            try
            {
                _unitOfWork.OandaAccountRepository.UpdateOne(oandaAccountId, updateDefinition);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public Datasource getDatasourceById(ObjectId datasourceId)
        {
            return _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == datasourceId).SingleOrDefault();    
        }
        #endregion
    }
}
