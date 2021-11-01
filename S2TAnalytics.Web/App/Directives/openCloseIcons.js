app.directive('openCloseIcons', [function () {
    return {
        link: function (scope, elem, attrs) {
            var parent = elem.parent('.floater');
            var btn = parent.find(".floater__btn");
            btn.bind("click", function (e) {
                var click = $(this).parents('.floater');
                if (click.hasClass("is-active")) {
                    console.log(click);
                    $(this).parents('.floater').removeClass('is-active');

                } else {
                    console.log(click);
                    $('.floater').removeClass('is-active');
                    $(this).parents('.floater').toggleClass('is-active');
                    e.stopPropagation();
                }
            });
            $(document).on("click", ".container", function () {
                $('.floater').removeClass('is-active');
            });
        }
    }
}]);
