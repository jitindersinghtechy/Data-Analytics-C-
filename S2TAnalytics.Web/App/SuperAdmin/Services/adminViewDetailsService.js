'use strict';
app.service('adminViewDetailsService', ['$http', function ($http) {
    return {
        getSubscriberList: getSubscriberList,
        updateSubscriberContactInfo: updateSubscriberContactInfo,
        getInvoiceHistory: getInvoiceHistory,
        getUserRequest: getUserRequest,
        updatePlans: updatePlans,
        updateSubscriberSummary: updateSubscriberSummary,
        updateUserActivation : updateUserActivation ,
        sendOverdueReminder: sendOverdueReminder,
        DownloadInvoice: DownloadInvoice
    };
    function getSubscriberList(userId) {
        return $http.get(serviceBase +"api/AdminSubscriber/GetSubscriberList/" + userId).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function updateUserActivation(user) {
        return $http.post(serviceBase + "api/AdminSubscriber/updateUserActivation", user).success(updateUserActivationComplete).error(updateUserActivationFailed);
        function updateUserActivationComplete(response) {
            return response;
        }
        function updateUserActivationFailed(err, status) {
        }
    }

    function sendOverdueReminder(user) {
        return $http.post(serviceBase + "api/AdminSubscriber/sendOverdueReminder", user).success(sendOverdueReminderComplete).error(sendOverdueReminderFailed);
        function sendOverdueReminderComplete(response) {
            return response;
        }
        function sendOverdueReminderFailed(err, status) {
        }
    }
    function DownloadInvoice(userId,invoiceIds) {
        return $http.post(serviceBase + "api/AdminSubscriber/DownloadInvoice/"+userId, invoiceIds).success(DownloadInvoiceComplete).error(DownloadInvoiceFailed);
        function DownloadInvoiceComplete(response) {
            return response;
        }
        function DownloadInvoiceFailed(err, status) {
        }
    }

    function getInvoiceHistory(userId, pagerecordModel) {
        return $http.post(serviceBase + "api/AdminSubscriber/GetInvoiceHistory/" + userId, pagerecordModel).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }

    function getUserRequest(userId, pagerecordModel) {
        return $http.post(serviceBase + "api/AdminSubscriber/GetUserRequest/" + userId, pagerecordModel).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }    

    function updateSubscriberContactInfo(user) {
        return $http.post(serviceBase + "api/AdminSubscriber/UpdateSubscriberContactInfo", user).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }
    function updatePlans(userId,planId) {
        return $http.post(serviceBase +"api/AdminSubscriber/UpdatePlans/" + userId + "/" + planId).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }
    function updateSubscriberSummary(user, selectedPlanId, monthlyRate) {
        return $http.post(serviceBase + "api/AdminSubscriber/UpdateSubscriberSummary/" + selectedPlanId, user).success(contactComplete).error(contactFailed);
        function contactComplete(response) {
            return response;
        }
        function contactFailed(err, status) {
        }
    }
    
    
}]);
