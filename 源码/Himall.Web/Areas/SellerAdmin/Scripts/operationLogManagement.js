
$(function () {
    query();
    $("#searchBtn").click(function () { query(); });
    AutoComplete();
})

function query() {
    $("#list").hiMallDatagrid({
        url: './list',
        nowrap: false,
        rownumbers: true,
        NoDataMsg: 'No matching data.',
        border: false,
        fit: true,
        fitColumns: true,
        pagination: true,
        idField: "Id",
        pageSize: 10,
        pageNumber: 1,
        queryParams: { userName: $("#autoTextBox").val(), startDate: $("#inputStartDate").val(), endDate: $("#inputEndDate").val() },
        toolbar: /*"#goods-datagrid-toolbar",*/'',
        operationButtons: "",
        columns:
        [[
            { field: "Id", hidden: true },
            { field: "UserName", title: 'Operator' },
            { field: "PageUrl", title: 'Page', 
				formatter: function (value, row, index) {
					return '<span title="'+value+'" class="overflow-ellipsis" style="width:300px; text-align:left">'+value+'</span>';
				}
			},
            { field: "Description", title: 'Action' },
            { field: "Date", title: 'Operation date' },
        { field: "IPAddress", title: 'IP' }
        ]]
    });
}

function AutoComplete() {
    //autocomplete
    $('#autoTextBox').autocomplete({
        source: function (query, process) {
            var matchCount = this.options.items;//返回结果集最大数量
            $.post("./GetManagers", { "keyWords": $('#autoTextBox').val() }, function (respData) {
                return process(respData);
            });
        },
        formatItem: function (item) {
            return item.value;
        },
        setValue: function (item) {
            return { 'data-value': item.value, 'real-value': item.key };
        }
    });
}