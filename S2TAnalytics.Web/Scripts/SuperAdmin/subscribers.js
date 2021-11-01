$(document).ready(function(){
	$('#table-2').DataTable({
		"pageLength": 1000,
		"bLengthChange": false,
		"bFilter": false,
		"paging": false,
		"bInfo": false,
		"order": [[ 1, "desc" ]],
		"columnDefs": [{
          "targets": 'no-sort',
          "orderable": false,
		}]
	});
});