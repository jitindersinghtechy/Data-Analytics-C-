if (typeof isScriptAlreadyIncluded != 'function') {
    function isScriptAlreadyIncluded(src) {
        var scripts = document.getElementsByTagName("script");
        for (var i = 0; i < scripts.length; i++)
            if (scripts[i].getAttribute('src') == src) return true;
        return false;
    }
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Content/bootstrap.css")) {
    document.write('<link href="http://analyticsv1.azurewebsites.net/Content/bootstrap.css" rel="stylesheet" />');
}

if (!isScriptAlreadyIncluded("https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css")) {
    document.write('<link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" rel="stylesheet" integrity="sha384-wvfXpqpZZVQGK6TAh5PVlGOfQNHSoD2xbE+QkPxCAFlNEevoEH3Sl0sibVcOQVnN" crossorigin="anonymous">');
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Content/main.css")) {
    document.write('<link href="http://analyticsv1.azurewebsites.net/Content/main.css" rel="stylesheet" />');
}

document.write('<style type="text/css">.dropdown-menu li {border-bottom: 1px solid #ccc !important;width: 100% !important;}</style>');
if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Content/admin-responsive.css")) {
    document.write('<link href="http://analyticsv1.azurewebsites.net/Content/admin-responsive.css" rel="stylesheet" />');
}
if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Content/Site.css")) {
    document.write('<link href="http://analyticsv1.azurewebsites.net/Content/Site.css" rel="stylesheet" />');
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Scripts/jquery-1.10.2.min.js")) {
    document.write('<script src="http://analyticsv1.azurewebsites.net/Scripts/jquery-1.10.2.min.js"></script>');
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Scripts/bootstrap.min.js")) {
    document.write('<script src="http://analyticsv1.azurewebsites.net/Scripts/bootstrap.min.js"></script>');
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Scripts/bootstrap-toggle.min.js")) {

    document.write('<script src="http://analyticsv1.azurewebsites.net/Scripts/bootstrap-toggle.min.js"></script>');
}

if (!isScriptAlreadyIncluded("https://cdnjs.cloudflare.com/ajax/libs/lodash.js/2.4.1/lodash.min.js")) {
    document.write('<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/lodash.js/2.4.1/lodash.min.js"></script>');
}

if (!isScriptAlreadyIncluded("https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.6/proj4.js")) {

    document.write('<script src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.6/proj4.js"></script>');
}

if (!isScriptAlreadyIncluded("https://code.highcharts.com/highcharts.js")) {
    document.write('<script src="https://code.highcharts.com/highcharts.js"></script>');
}

if (!isScriptAlreadyIncluded("http://code.highcharts.com/highcharts-more.js")) {
    document.write('<script src="http://code.highcharts.com/highcharts-more.js"></script>');
}

if (!isScriptAlreadyIncluded("https://code.highcharts.com/maps/modules/map.js")) {
    document.write('<script src="https://code.highcharts.com/maps/modules/map.js"></script>');
}

if (!isScriptAlreadyIncluded("http://code.highcharts.com/maps/modules/data.js")) {
    document.write('<script src="http://code.highcharts.com/maps/modules/data.js"></script>');
}

if (!isScriptAlreadyIncluded("http://code.highcharts.com/maps/modules/exporting.js")) {

    document.write('<script src="http://code.highcharts.com/maps/modules/exporting.js"></script>');
}

document.write('<script src="http://www.highcharts.com/samples/data/maps/world.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/in/in-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/vn/vn-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/ro/ro-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/it/it-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/at/at-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/de/de-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/ua/ua-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/tr/tr-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/cn/cn-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/fi/fi-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/se/se-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/br/br-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/cs/ca-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/at/at-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/my/my-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/sg/sg-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/kw/kw-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/us/us-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/au/au-all.js"></script>');
document.write('<script src="https://code.highcharts.com/mapdata/countries/sn/sn-all.js"></script>');

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Scripts/angular.min.js")) {

    document.write('<script src="http://analyticsv1.azurewebsites.net/Scripts/angular.min.js"></script>');
}

if (!isScriptAlreadyIncluded("http://analyticsv1.azurewebsites.net/Scripts/angularjs-dropdown-multiselect.js")) {
    document.write('<script src="http://analyticsv1.azurewebsites.net/Scripts/angularjs-dropdown-multiselect.js"></script>');
}


document.write('<script src="http://analyticsv1.azurewebsites.net/App/Widgets/NavMap/app.js"></script>');
document.write('<script src="http://analyticsv1.azurewebsites.net/App/Widgets/NavMap/navMapService.js"></script>');
document.write('<script src="http://analyticsv1.azurewebsites.net/App/Widgets/NavMap/navMapComponent.js"></script>');

//var myElement = document.getElementById('myFirstWidget');
//var JavaScriptCode = document.createElement("script");
//JavaScriptCode.setAttribute('type', 'text/javascript');
//JavaScriptCode.setAttribute("src", 'data.js');
//document.getElementById('myFirstWidget').appendChild(JavaScriptCode);
 