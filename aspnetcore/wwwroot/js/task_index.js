!function ($) {
    var table;
    $(function () {
        table = $("#datalist").DataTable({
            lengthChange: false,//分页大小调整
            searching: false,
            pageLength: 10,//分页大小
            ordering: false,
            serverSide: true,  //启用服务器端分页
            order: [],  //取消默认排序查询,否则复选框一列会出现小箭头
            ajax: function (data, callback, settings) {
                //封装请求参数
                var param = {};
                param.pagesize = data.length;//页面显示记录条数，在页面显示每页显示多少项的时候
                param.pageindex = (data.start / data.length) + 1;//当前页码
                //ajax请求数据
                $.ajax({
                    type: "Post",
                    url: "/Task/Page",
                    cache: false,  //禁用缓存
                    data: param,
                    dataType: "json",
                    success: function (result) {
                        if (result.status != 1)
                            alert(result.msg || '获取失败');

                        //封装返回数据
                        var returnData = {};
                        returnData.draw = data.draw;//这里直接自行返回了draw计数器,应该由后台返回
                        returnData.recordsTotal = result.page.total;//返回数据全部记录
                        returnData.recordsFiltered = result.page.total;//后台不实现过滤功能，每次查询均视作全部结果
                        returnData.data = result.data;//返回的数据列表
                        callback(returnData);
                    }
                });
            },
            columns: [
                { "data": "Id" },
                { "data": "TaskName" },
                { "data": "TaskParam" },
                { "data": "CronExpressionString" },
                { "data": "ApiUri" },
                { "data": "CreatedTime" },
                { "data": "Remark" },
                {
                    "data": function (rowData, type, set, meta) {
                        var btn = '<a href="javascript:update(' + meta.row + ')">修改</a>';
                        var setstate = rowData.Status == 1 ? 0 : 1;
                        var operate = rowData.Status == 1 ? '禁用' : '启用';
                        btn += '&nbsp;|&nbsp;<a href="javascript:setStatus(' + rowData.Id + ',' + setstate + ')">' + operate + '</a>';
                        btn += '&nbsp;|&nbsp;<a href="javascript:setStatus(' + rowData.Id + ',-1)">删除</a>';
                        return btn;
                    }
                },
            ]
        });
        $('#btnSave').bind('click', saveTask);
    });
    function reload() {
        //table.ajax.reload();//分页重置
        table.ajax.reload(null, false);//分页信息保留
    }
    function update(rowindex) {
        var model = table.row(rowindex).data();
        $('#form-body').attr('data-id', model.Id);
        $('#form-body input').each(function () {
            var propname = $(this).attr('id');
            if (model[propname])
                $(this).val(model[propname])
        })
        $('#div_task').show();
    }
    function saveTask() {
        var model = new Object;
        var validated = true;
        $('#form-body input').each(function () {
            var propname = $(this).attr('id');
            if (propname != 'Remark' && !$(this).val().length) {
                alert('请输入' + $(this).prev().html());
                validated = false;
                return false;
            }
            model[propname] = $(this).val();
        })
        if (!validated) return;
        var action = '/Task/Add';
        var tid = $('#form-body').attr('data-id');
        if (tid && tid.length > 0)
            '/Task/Update';
        $.ajax({
            url: action,
            type: 'post',
            data: model,
            dataType: "json",
            success: function (e) {
                if (e.status == 1) {
                    alert('操作成功');
                    reload();
                } else {
                    alert(e.msg || '更新失败');
                }
            }
        })
    }
    function setStatus(id, state) {
        $.ajax({
            url: '/Task/SetStatus',
            type: 'post',
            data: {
                id: id,
                state: state
            },
            dataType: "json",
            success: function (e) {
                if (e.status == 1) {
                    alert('操作成功');
                    reload();
                } else {
                    alert(e.msg || '操作失败');
                }
            }
        })
    }
}(jQuery);
