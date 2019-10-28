

function MarkAll() {
    var tbl = event.srcElement;
    while (tbl.tagName != 'TABLE') { tbl = tbl.parentElement; }
    var ch = tbl.rows[1].cells[0].children[0].checked;
    for (var i = 2; i < tbl.rows.length; i++) {
        var tr = tbl.rows[i];
        tr.cells[0].children[0].checked = ch;
    }
}

function GetNumbers(table) {
    var ids = '';
    for (var i = 1; i < table.rows.length; i++) {
        var tr = table.rows[i];
        if (tr.cells[0].children[0].checked)
            ids += (ids.length > 0 ? "," : "") + tr.cells[1].innerText;
    }
    return ids;
}

function Move() {
    var id1 = GetNumbers(document.all("Equipment1"));
    var id2 = GetNumbers(document.all("Equipment2"));
    cmdasync('cmd', 'Move', 'id1', id1, 'id2', id2);
}

