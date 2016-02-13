angular.module('appFactories').factory('constantsFactory',
    function () {
        var repo = {};
        repo.defaultParentImageUrl = "/content/images/profile-128.png";
        repo.defaultSitterImageUrl = "/content/images/profile-128.png";
        repo.defaultMobilePhoneCode = "+1";
        // Cookies expiry time in hours.
        repo.accountCookieExpiration = 24;
        // Patterns.
        repo.mobilePhoneWithCodePattern = /^\+\d{1,3}\d{10}$/;
        repo.mobilePhonePattern = /^\d{10}$/;
        repo.forgotPasswordCodePattern = /^\d{4}$/;
        repo.emailPattern = /^[a-z0-9!#$%&'*+/=?^_`{|}~.-]+@[a-z0-9-]+(\.[a-z0-9-]+)*$/i;
        // Redirect path for anonymous user.
        repo.redirectAnonymousPath = "/user/login";
        // Toaster options.
        repo.toasterTimeout = {
            success: 10000,
            error: 30000
        };
        return repo;
    }
);