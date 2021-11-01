$(document).ready(function(){
	
	$('#table-2').DataTable({
		"pageLength": 5,
		"bLengthChange": false,
		"bFilter": false,
		"bInfo": false,
		"order": [[ 1, "desc" ]],
		"columnDefs": [{
          "targets": 'no-sort',
          "orderable": false,
		}]
	});

    /* Main Chart */
    var data1 = [[1, 10], [2, 20], [3, 12], [4, 28], [5, 15], [6, 30], [7, 20], [8, 35], [9, 25], [10, 35]];
    var data2 = [[1, 8], [2, 15], [3, 10], [4, 18], [5, 8], [6, 25], [7, 15], [8, 28], [9, 17], [10, 30]];
    var data3 = [[1, 3], [2, 8], [3, 4], [4, 9], [5, 5], [6, 10], [7, 7], [8, 16], [9, 9], [10, 20]];

    var labels = ["Visits", "Page views", "Sales"];
    var colors = [
        '#20b9ae',
        tinycolor('#20b9ae').darken(4).toString(),
        tinycolor('#20b9ae').darken(8).toString()
    ];

    /* Bar chart */
    var data1 = [];
    for (var i = 0; i <= 6; i += 1)
        data1.push([i, parseInt(Math.random() * 20)]);

    var data2 = [];
    for (var i = 0; i <= 6; i += 1)
        data2.push([i, parseInt(Math.random() * 20)]);

    var data = [{
        label : "Data One",
        data : data1,
        bars : {
            order : 1
        }
        
    }, {
        label : "Data Two",
        data : data2,
        bars : {
            order : 2
        }
    }];

	new Chartist.Line('#line-area', {
	  labels: [1, 2, 3, 4, 5, 6, 7, 8],
	  series: [
		[5, 9, 7, 8, 5, 3, 5, 4]
	  ]
	}, {
	  low: 0,
	  showArea: true,
	  height: "260px"
	});
	
	//Email Queries
	var data = [{
		label: "Open",
		data: 12,
		color: "#CFFFAB",
		
	}, {
		label: "On Hold",
		data: 17,
		color: "#BFF140",
	}, {
		label: "Reopened",
		data: 51,
		color:"#576E13",
	}, {
		label: "Closed",
		data: 20,
		color:"#A2C140",
	}];

	$.plot($("#donut1"), data, {
		series: {
			pie: {
				innerRadius: 0.5,
				show: true
			}
		},
		grid: {
			hoverable: true
		},
		color: null,
		tooltip: true,
		tooltipOpts: {
			content: "%p.0%, %s", // show percentages, rounding to 2 decimal places
			shifts: {
				x: 20,
				y: 0
			},
			defaultTheme: false
		}
	});
	
	//Plan Distribution
	var data = [{
		label: "Basic",
		data: 12,
		color: "#CFFFAB",
		
	}, {
		label: "Advanced",
		data: 17,
		color: "#BFF140",
	}, {
		label: "Ultimate",
		data: 20,
		color:"#A2C140",
	}, {
		label: "Custom",
		data: 25,
		color:"#C6DF77",
	}, {
		label: "Preview",
		data: 26,
		color:"#576E13",
	}];

	$.plot($("#donut"), data, {
		series: {
			pie: {
				innerRadius: 0.5,
				show: true
			}
		},
		grid: {
			hoverable: true
		},
		color: null,
		tooltip: true,
		tooltipOpts: {
			content: "%p.0%, %s", // show percentages, rounding to 2 decimal places
			shifts: {
				x: 20,
				y: 0
			},
			defaultTheme: false
		}
	});

});