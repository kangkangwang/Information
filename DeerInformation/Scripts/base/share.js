var getGridValueArr = function (gridname, mem) {
    var grid_data = Ext.getStore(gridname);
    var count = grid_data.getCount();
    var arr = new Array(count);
    for (var i = 0 ; i < count ; i++) {
        arr[i] = grid_data.getAt(i).data[mem];
    }
    return arr;
};

var Judge = function (gridname, mem1, mem2) {
    var count = Ext.getStore(gridname).getCount();
    var x = true;
    var y = true;
    if (count == 0) {
        Ext.Msg.alert("温馨提示", "您还没选择任何物料!!");
        return false;
    };

    var arr1 = new Array(count);
    arr1 = getGridValueArr(gridname, mem1);
    var arr2 = new Array(count);
    arr2 = getGridValueArr(gridname, mem2);
    for (var i = 0; i < count; i++) {
        if (arr1[i] == null || arr1[i] == "") {
            x = false;
            break;
        }
    };
    for (var i = 0; i < count; i++) {
        if (arr2[i] == null || arr2[i] == "") {
            y = false;
            break;
        }
    };

    if (!x) {
        Ext.Msg.alert("温馨提示", "数量不能为空!!");
        return false;
    };
    if (!y) {
        Ext.Msg.alert("温馨提示", "价格不能为空!!");
        return false;
    };
    if (x && y) {
        parent.App.win.close();
    };

};

var onSuccess = function (grid, data) {
    grid.show();
    grid.getStore().loadData(data);
};