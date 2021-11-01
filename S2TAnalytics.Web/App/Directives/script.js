//app.directive('script', function () {
//    return {
//        restrict: 'E',
//        scope: false,
//        link: function (scope, elem, attr) {
//            if (attr.type == 'text/javascript-lazy') {
//                var code = elem.text();
//                var f = new Function(code);
//                f();
//            }
//        }
//    };
//});
(function (ng) {
    'use strict';
    app.directive('script', function () {
        return {
            restrict: 'E',
            scope: false,
            link: function (scope, elem, attr) {
                var angularCorrections = //Document.write
                        function (code) {
                            var parentNode = elem[0].parentNode;
                            if (!parentNode.id) parentNode.id = Date.now() + '_' + Math.floor((Math.random() * 10) + 1); //replace with your own random id generator
                            var re = new RegExp("document.write(ln)?", "g"); //Support for Document.write only 
                            var newCode = code.replace(re, "document.getElementById('" + parentNode.id + "').innerHTML += ");
                            console.log(newCode);
                            return newCode;
                        };
                if (attr.type === 'text/javascript-lazy') {
                    var s = document.createElement("script");
                    s.type = "text/javascript";
                    var src = elem.attr('src');
                    if (src !== undefined) {
                        s.src = src;
                    }
                    else {
                        var code = elem.text();
                        s.text = angularCorrections(code);
                    }
                    document.head.appendChild(s);
                    elem.remove();
                }
            }
        };
    });
}(angular));