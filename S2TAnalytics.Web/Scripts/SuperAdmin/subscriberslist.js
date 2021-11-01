$(document).ready(function(){
	$('#table-2').DataTable({
		"pageLength": 5,
		"bLengthChange": false,
		"bFilter": false,
		"bSort" : false,
		"bInfo": false,
        "scrollY": 190,
		"columnDefs": [{
          "targets": 'no-sort',
          "orderable": false,
		}]
	});
	
	$('#table-3').DataTable({
		"pageLength": 5,
		"bLengthChange": false,
		"bFilter": false,
		"bSort" : false,
		"bInfo": false,
		"scrollY": 190,
		"columnDefs": [{
          "targets": 'no-sort',
          "orderable": false,
		}]
	});
});