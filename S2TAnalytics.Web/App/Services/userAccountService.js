'use strict';
app.service('userAccountService', ['$http', function ($http) {
    return {
        saveCard: saveCard,
        getDefaultData: getDefaultData,
        deleteCards: deleteCards,
        activateCard: activateCard,
        saveBillingAddress: saveBillingAddress,
        getUserBillingAddresses: getUserBillingAddresses,
        activateBillingAddress: activateBillingAddress,
        getUserBillingAddressById: getUserBillingAddressById,
        changeSettings: changeSettings,
        changeUserDetails: changeUserDetails,
        changePassword: changePassword,
        generateInvoice: generateInvoice,
        emailInvoice: emailInvoice,
        getSubscriptionPlans:getSubscriptionPlans
    };


    function getSubscriptionPlans() {
        return $http.get(serviceBase + "api/UserAccount/GetSubscriptionPlans").success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function saveCard(card) {
        return $http.post(serviceBase + "api/UserAccount/SaveCard",card).success(saveCardComplete).error(saveCardFailed);
        function saveCardComplete(response) {
            return response;
        }
        function saveCardFailed(err, status) {

        }
    }   
    function getDefaultData() {
        return $http.get(serviceBase + "api/UserAccount/GetDefaultData").success(getDefaultDataComplete).error(getDefaultDataFailed);
        function getDefaultDataComplete(response) {
            return response;
        }
        function getDefaultDataFailed(err, status) {

        }
    }
    function deleteCards(cards) {
        return $http.post(serviceBase + "api/UserAccount/DeleteCards", cards).success(deleteCardComplete).error(deleteCardFailed);
        function deleteCardComplete(response) {
            return response;
        }
        function deleteCardFailed(err, status) {

        }
    }

    function activateCard(cardId, isActive) {
        return $http.post(serviceBase + "api/UserAccount/ActivateCard/" + cardId + "/" + isActive).success(acitvateCardComplete).error(acitvateCardFailed);
        function acitvateCardComplete(response) {
            return response;
        }
        function acitvateCardFailed(err, status) {

        }
    }

    function saveBillingAddress(userBillingInfo) {
        return $http.post(serviceBase + "api/UserAccount/SaveBillingAddress", userBillingInfo).success(saveBillingAddressComplete).error(saveBillingAddressFailed);
        function saveBillingAddressComplete(response) {
            return response;
        }
        function saveBillingAddressFailed(err, status) {

        }
    }

    function getUserBillingAddresses() {
        return $http.get(serviceBase + "api/UserAccount/GetUserBillingAddresses").success(getUserBillingAddressesComplete).error(getUserBillingAddressesFailed);
        function getUserBillingAddressesComplete(response) {
            return response;
        }
        function getUserBillingAddressesFailed(err, status) {

        }
    }

    function activateBillingAddress(addressId, isActive) {
        return $http.post(serviceBase + "api/UserAccount/ActivateBillingAddress/" + addressId + "/" + isActive).success(activateBillingAddressComplete).error(activateBillingAddressFailed);
        function activateBillingAddressComplete(response) {
            return response;
        }
        function activateBillingAddressFailed(err, status) {

        }
    }

    function getUserBillingAddressById(addressId) {
        return $http.get(serviceBase + "api/UserAccount/GetUserBillingAddressById/"+addressId).success(getUserBillingAddressByIdComplete).error(getUserBillingAddressByIdFailed);
        function getUserBillingAddressByIdComplete(response) {
            return response;
        }
        function getUserBillingAddressByIdFailed(err, status) {

        }
    }

    function changeSettings(emailNotificationSettingsId, hasAccess) {
        return $http.post(serviceBase + "api/UserAccount/ChangeSettings/" + emailNotificationSettingsId + "/" + hasAccess).success(changeSettingsComplete).error(changeSettingsFailed);
        function changeSettingsComplete(response) {
            return response;
        }
        function changeSettingsFailed(err, status) {

        }
    }

    function changeUserDetails(user) {
        return $http.post(serviceBase + "api/UserAccount/ChangeUserDetails", user).success(changeUserDetailsComplete).error(changeUserDetailsFailed);
        function changeUserDetailsComplete(response) {
            return response;
        }
        function changeUserDetailsFailed(err, status) {

        }
    }

    function changePassword(email, oldPassword, newPassword) {
        return $http.post(serviceBase + "api/UserAccount/ChangePassword/" + email + "/" + oldPassword+"/"+newPassword).success(changePasswordComplete).error(changePasswordFailed);
        function changePasswordComplete(response) {
            return response;
        }
        function changePasswordFailed(err, status) {

        }
    }

    function generateInvoice(invoiceMonth, invoiceYear) {
       // var data = { date: date };
        return $http.post(serviceBase + "api/UserAccount/GenerateInvoice/" + invoiceMonth + "/" + invoiceYear).success(generateInvoiceComplete).error(generateInvoiceFailed);
        function generateInvoiceComplete(response) {
            return response;
        }
        function generateInvoiceFailed(err, status) {

        }
    }

    function emailInvoice(files,invoiceMonth,invoiceYear) {
        // var data = { date: date };
        return $http.post(serviceBase + "api/UserAccount/EmailInvoice/"+invoiceMonth+"/"+invoiceYear, files).success(emailInvoiceComplete).error(emailInvoiceFailed);
        function emailInvoiceComplete(response) {
            return response;
        }
        function emailInvoiceFailed(err, status) {

        }
    }
}]);
