//app.directive('autoFocus', function($timeout) {
//    return {
//        restrict: 'A',
//        scope: {
//            autoFocus: '&'
//        },
//        link: function($scope, $element) {
//            $scope.$watch('autofocusIf', function (shouldFocus) {
//                if (shouldFocus) {
//                    $timeout(function() {
//                        $element[0].focus();
//                    });
//                } else {
//                    $timeout(function() {
//                        $element[0].blur();
//                    });
//                }
//            });
//        }
//    };
//})



//app.directive('focusMe', ['$timeout', '$parse', function ($timeout, $parse) {
//    return {
//        //scope: true,   // optionally create a child scope
//        link: function (scope, element, attrs) {
//            var model = $parse(attrs.focusMe);
//            scope.$watch(model, function (value) {
//                //    console.log('value=', value);
//                if (value === true) {
//                    $timeout(function () {
//                        element[0].focus();
//                    }, 500);
//                    //$timeout(function () {
//                    //    element[0].focus();
//                    //});
//                }
//            });
//            // to address @blesh's comment, set attribute value to 'false'
//            // on blur event:
//            //element.bind('blur', function () {
//            //    console.log('blur');
//            //    scope.$apply(model.assign(scope, false));
//            //});
//        }
//    };
//}]);    